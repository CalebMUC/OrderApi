-- =============================================================================
-- GUEST CHECKOUT SYSTEM - NEON POSTGRESQL MIGRATION SCRIPT
-- =============================================================================
-- Project  : QuickCrate / Minimart API
-- Branch   : guest_checkout
-- Target DB: Neon PostgreSQL
-- Version  : 1.0
-- Date     : 2025
-- =============================================================================
-- INSTRUCTIONS:
--   1. Open your Neon project dashboard ? SQL Editor
--   2. Paste and run each SECTION individually (or run all at once)
--   3. Run the VERIFICATION section at the end to confirm success
-- =============================================================================


-- =============================================================================
-- SECTION 0: PRE-FLIGHT CHECKS
-- Confirm required tables exist before running migration
-- =============================================================================

DO $$
BEGIN
    -- Check Products table
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Products') THEN
        RAISE EXCEPTION 'ABORT: "Products" table does not exist. Ensure base migration has been applied first.';
    END IF;

    -- Check Orders table
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Orders') THEN
        RAISE EXCEPTION 'ABORT: "Orders" table does not exist. Ensure base migration has been applied first.';
    END IF;

    -- Check Counties table
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Counties') THEN
        RAISE EXCEPTION 'ABORT: "Counties" table does not exist. Ensure base migration has been applied first.';
    END IF;

    -- Check Towns table
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Towns') THEN
        RAISE EXCEPTION 'ABORT: "Towns" table does not exist. Ensure base migration has been applied first.';
    END IF;

    -- Check DeliveryStations table
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'DeliveryStations') THEN
        RAISE EXCEPTION 'ABORT: "DeliveryStations" table does not exist. Ensure base migration has been applied first.';
    END IF;

    RAISE NOTICE 'Pre-flight checks passed. Proceeding with migration...';
END $$;


-- =============================================================================
-- SECTION 1: CREATE GuestCarts TABLE
-- Maps to: Models/GuestCart.cs
-- =============================================================================

CREATE TABLE IF NOT EXISTS "GuestCarts" (
    -- Primary Key: int SERIAL matches [DatabaseGenerated(DatabaseGeneratedOption.Identity)] on int
    "GuestCartId"  SERIAL          PRIMARY KEY,

    -- Anonymous guest identifier (device fingerprint, UUID, session ID)
    "GuestId"      VARCHAR(100)    NOT NULL,

    -- Foreign key to Products.ProductId (UUID)
    "ProductId"    UUID            NOT NULL,

    -- Quantity: 1 to 9999 matching [Range(1, 9999)]
    "Quantity"     INTEGER         NOT NULL
                                   CONSTRAINT "CHK_GuestCarts_Quantity"
                                   CHECK ("Quantity" >= 1 AND "Quantity" <= 9999),

    -- Timestamps: all TIMESTAMP WITH TIME ZONE to match HasColumnType("timestamp with time zone")
    "CreatedAt"    TIMESTAMP WITH TIME ZONE  NOT NULL  DEFAULT NOW(),
    "UpdatedAt"    TIMESTAMP WITH TIME ZONE  NOT NULL  DEFAULT NOW(),

    -- Auto-expiry: carts expire after 7 days (GuestCartService sets ExpiresAt = DateTime.UtcNow.AddDays(7))
    "ExpiresAt"    TIMESTAMP WITH TIME ZONE  NOT NULL  DEFAULT (NOW() + INTERVAL '7 days'),

    -- Foreign Key constraint
    CONSTRAINT "FK_GuestCarts_Products"
        FOREIGN KEY ("ProductId")
        REFERENCES "Products"("ProductId")
        ON DELETE CASCADE   -- matches OnDelete(DeleteBehavior.Cascade) in DbContext
);

-- Indexes matching entity.HasIndex(...) calls in ConfigureGuestCheckoutEntities
CREATE INDEX IF NOT EXISTS "IX_GuestCarts_GuestId"
    ON "GuestCarts"("GuestId");

CREATE INDEX IF NOT EXISTS "IX_GuestCarts_ProductId"
    ON "GuestCarts"("ProductId");

CREATE INDEX IF NOT EXISTS "IX_GuestCarts_ExpiresAt"
    ON "GuestCarts"("ExpiresAt");

-- Composite index: used by GuestCartService.AddToCartAsync() lookup
-- (gc.GuestId == dto.GuestId && gc.ProductId == dto.ProductId)
CREATE INDEX IF NOT EXISTS "IX_GuestCarts_GuestId_ProductId"
    ON "GuestCarts"("GuestId", "ProductId");

COMMENT ON TABLE  "GuestCarts"             IS 'Server-side cart for guest users. Expires after 7 days of inactivity.';
COMMENT ON COLUMN "GuestCarts"."GuestId"   IS 'Anonymous client identifier (UUID, device fingerprint, or session ID).';
COMMENT ON COLUMN "GuestCarts"."ExpiresAt" IS 'Automatically extended on each cart update. Cleaned up by GuestCartCleanupService.';


-- =============================================================================
-- SECTION 2: CREATE GuestCheckouts TABLE
-- Maps to: Models/GuestCheckout.cs
-- =============================================================================

CREATE TABLE IF NOT EXISTS "GuestCheckouts" (
    -- Primary Key: UUID matching Guid GuestCheckoutId with Guid.NewGuid() default
    "GuestCheckoutId"       UUID            PRIMARY KEY  DEFAULT gen_random_uuid(),

    -- Guest identifier (same GuestId used in GuestCarts)
    "GuestId"               VARCHAR(100)    NOT NULL,

    -- Delivery contact details
    "FullName"              VARCHAR(200)    NOT NULL,

    -- Kenyan phone: 07XXXXXXXX or 01XXXXXXXX (10 digits)
    -- Matches [RegularExpression(@"^0[17]\d{8}$")] on PhoneNumber property
    "PhoneNumber"           VARCHAR(15)     NOT NULL
                                            CONSTRAINT "CHK_GuestCheckouts_PhoneNumber"
                                            CHECK ("PhoneNumber" ~ '^0[17]\d{8}$'),

    -- Optional email for receipt
    "Email"                 VARCHAR(255),

    -- Full text delivery address (TEXT type, no length limit)
    "DeliveryAddress"       TEXT            NOT NULL,

    -- Location FKs (all nullable - matching int? in C# model)
    "CountyId"              INTEGER,
    "TownId"                INTEGER,
    "DeliveryStationId"     INTEGER,

    -- Amount fields: DECIMAL(18,2) matching HasColumnType("decimal(18,2)")
    "DeliveryFee"           DECIMAL(18,2)   NOT NULL  DEFAULT 0,
    "SubtotalAmount"        DECIMAL(18,2)   NOT NULL  DEFAULT 0,
    "TotalAmount"           DECIMAL(18,2)   NOT NULL  DEFAULT 0,

    -- Status: Pending ? Completed | Abandoned
    -- Matches GuestCheckoutService logic
    "Status"                VARCHAR(50)     NOT NULL  DEFAULT 'Pending'
                                            CONSTRAINT "CHK_GuestCheckouts_Status"
                                            CHECK ("Status" IN ('Pending', 'Completed', 'Abandoned')),

    -- M-Pesa fields populated by GuestCheckoutService.InitiateCheckoutAsync()
    "CheckoutRequestId"     VARCHAR(100),
    "MerchantRequestId"     VARCHAR(100),

    -- Populated by GuestCheckoutService.ProcessPaymentConfirmationAsync()
    "MpesaReceiptNumber"    VARCHAR(100),

    -- Created after successful payment, links to Orders.OrderID (varchar(50))
    "OrderId"               VARCHAR(50),

    -- Timestamps
    "CreatedAt"             TIMESTAMP WITH TIME ZONE  NOT NULL  DEFAULT NOW(),
    "CompletedAt"           TIMESTAMP WITH TIME ZONE,

    -- Foreign Keys
    CONSTRAINT "FK_GuestCheckouts_Counties"
        FOREIGN KEY ("CountyId")
        REFERENCES "Counties"("CountyId")
        ON DELETE SET NULL,     -- matches OnDelete(DeleteBehavior.SetNull) in DbContext

    CONSTRAINT "FK_GuestCheckouts_Towns"
        FOREIGN KEY ("TownId")
        REFERENCES "Towns"("TownId")
        ON DELETE SET NULL,

    CONSTRAINT "FK_GuestCheckouts_DeliveryStations"
        FOREIGN KEY ("DeliveryStationId")
        REFERENCES "DeliveryStations"("DeliveryStationId")
        ON DELETE SET NULL,

    -- FK to Orders is deferred: Orders.OrderID is populated AFTER order creation
    -- The FK is added after the Orders table columns are extended (see Section 3)
    CONSTRAINT "FK_GuestCheckouts_Orders"
        FOREIGN KEY ("OrderId")
        REFERENCES "Orders"("OrderID")
        ON DELETE SET NULL
);

-- Indexes matching entity.HasIndex(...) in ConfigureGuestCheckoutEntities
CREATE INDEX IF NOT EXISTS "IX_GuestCheckouts_GuestId"
    ON "GuestCheckouts"("GuestId");

CREATE INDEX IF NOT EXISTS "IX_GuestCheckouts_PhoneNumber"
    ON "GuestCheckouts"("PhoneNumber");

CREATE INDEX IF NOT EXISTS "IX_GuestCheckouts_Status"
    ON "GuestCheckouts"("Status");

-- Used by GuestCheckoutService.ProcessPaymentConfirmationAsync() to match M-Pesa callback
CREATE INDEX IF NOT EXISTS "IX_GuestCheckouts_CheckoutRequestId"
    ON "GuestCheckouts"("CheckoutRequestId");

CREATE INDEX IF NOT EXISTS "IX_GuestCheckouts_MerchantRequestId"
    ON "GuestCheckouts"("MerchantRequestId");

CREATE INDEX IF NOT EXISTS "IX_GuestCheckouts_OrderId"
    ON "GuestCheckouts"("OrderId");

CREATE INDEX IF NOT EXISTS "IX_GuestCheckouts_CreatedAt"
    ON "GuestCheckouts"("CreatedAt");

COMMENT ON TABLE  "GuestCheckouts"                      IS 'Stores delivery details and M-Pesa payment state for guest checkouts.';
COMMENT ON COLUMN "GuestCheckouts"."Status"             IS 'Pending = awaiting M-Pesa payment | Completed = order created | Abandoned = expired after 7 days.';
COMMENT ON COLUMN "GuestCheckouts"."CheckoutRequestId"  IS 'M-Pesa STK Push CheckoutRequestID. Used to match payment callbacks.';
COMMENT ON COLUMN "GuestCheckouts"."OrderId"            IS 'Set by ProcessPaymentConfirmationAsync after successful payment. Format: GC-yyyyMMddHHmmss-XXXXXXXX.';


-- =============================================================================
-- SECTION 3: EXTEND Orders TABLE WITH GUEST ORDER SUPPORT
-- Maps to: Models/Order.cs   (IsGuestOrder, GuestCheckoutId properties)
-- =============================================================================

-- Add IsGuestOrder column (bool ? BOOLEAN, default FALSE)
-- Matches: public bool IsGuestOrder { get; set; } = false;
ALTER TABLE "Orders"
    ADD COLUMN IF NOT EXISTS "IsGuestOrder"     BOOLEAN  NOT NULL  DEFAULT FALSE;

-- Add GuestCheckoutId column (Guid? ? UUID nullable)
-- Matches: public Guid? GuestCheckoutId { get; set; }
ALTER TABLE "Orders"
    ADD COLUMN IF NOT EXISTS "GuestCheckoutId"  UUID;

-- Foreign key: Orders ? GuestCheckouts
-- Matches: .HasForeignKey<GuestCheckout>(gco => gco.OrderId) (one-to-one)
-- Note: configured as ON DELETE SET NULL in DbContext
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_name = 'FK_Orders_GuestCheckouts'
          AND table_name = 'Orders'
    ) THEN
        ALTER TABLE "Orders"
            ADD CONSTRAINT "FK_Orders_GuestCheckouts"
            FOREIGN KEY ("GuestCheckoutId")
            REFERENCES "GuestCheckouts"("GuestCheckoutId")
            ON DELETE SET NULL;
    END IF;
END $$;

-- Indexes matching entity.HasIndex in ConfigureGuestCheckoutEntities
CREATE INDEX IF NOT EXISTS "IX_Orders_IsGuestOrder"
    ON "Orders"("IsGuestOrder");

CREATE INDEX IF NOT EXISTS "IX_Orders_GuestCheckoutId"
    ON "Orders"("GuestCheckoutId");

COMMENT ON COLUMN "Orders"."IsGuestOrder"    IS 'TRUE for orders placed without a user account via guest checkout.';
COMMENT ON COLUMN "Orders"."GuestCheckoutId" IS 'Links to the GuestCheckouts record that initiated this order.';


-- =============================================================================
-- SECTION 4: VERIFICATION QUERIES
-- Run these to confirm everything was created correctly
-- =============================================================================

-- 4a. Confirm both new tables exist
SELECT
    tablename,
    schemaname
FROM pg_tables
WHERE tablename IN ('GuestCarts', 'GuestCheckouts')
ORDER BY tablename;
-- Expected: 2 rows

-- 4b. Confirm all GuestCarts columns
SELECT
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'GuestCarts'
ORDER BY ordinal_position;
-- Expected: GuestCartId, GuestId, ProductId, Quantity, CreatedAt, UpdatedAt, ExpiresAt

-- 4c. Confirm all GuestCheckouts columns
SELECT
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'GuestCheckouts'
ORDER BY ordinal_position;
-- Expected: 18 columns

-- 4d. Confirm Orders table was extended with guest columns
SELECT
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'Orders'
  AND column_name IN ('IsGuestOrder', 'GuestCheckoutId')
ORDER BY column_name;
-- Expected: 2 rows

-- 4e. Confirm all indexes were created
SELECT
    tablename,
    indexname
FROM pg_indexes
WHERE tablename IN ('GuestCarts', 'GuestCheckouts', 'Orders')
  AND indexname LIKE '%Guest%'
ORDER BY tablename, indexname;
-- Expected: 11 indexes total

-- 4f. Confirm all foreign key constraints
SELECT
    tc.table_name,
    tc.constraint_name,
    kcu.column_name,
    ccu.table_name  AS referenced_table,
    ccu.column_name AS referenced_column
FROM information_schema.table_constraints       AS tc
JOIN information_schema.key_column_usage        AS kcu
    ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema   = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
    AND ccu.table_schema   = tc.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_name IN ('GuestCarts', 'GuestCheckouts', 'Orders')
  AND tc.constraint_name LIKE '%Guest%'
ORDER BY tc.table_name, tc.constraint_name;
-- Expected: 6 FK constraints

-- 4g. Confirm check constraints (phone + status + quantity)
SELECT
    tc.table_name,
    tc.constraint_name,
    cc.check_clause
FROM information_schema.table_constraints    AS tc
JOIN information_schema.check_constraints    AS cc
    ON tc.constraint_name = cc.constraint_name
WHERE tc.table_name IN ('GuestCarts', 'GuestCheckouts')
  AND tc.constraint_type = 'CHECK'
ORDER BY tc.table_name;
-- Expected: CHK_GuestCarts_Quantity, CHK_GuestCheckouts_PhoneNumber, CHK_GuestCheckouts_Status


-- =============================================================================
-- SECTION 5: OPTIONAL - MONITORING QUERIES
-- Useful after deployment to track guest checkout activity
-- =============================================================================

-- How many active (non-expired) guest cart items?
-- SELECT COUNT(*) AS active_cart_items FROM "GuestCarts" WHERE "ExpiresAt" > NOW();

-- How many pending checkouts?
-- SELECT COUNT(*) AS pending_checkouts FROM "GuestCheckouts" WHERE "Status" = 'Pending';

-- Completed guest orders today?
-- SELECT COUNT(*) AS guest_orders_today
-- FROM "Orders"
-- WHERE "IsGuestOrder" = TRUE
--   AND "OrderDate" >= CURRENT_DATE;

-- Checkout conversion rate?
-- SELECT
--     COUNT(CASE WHEN "Status" = 'Completed' THEN 1 END)                  AS completed,
--     COUNT(*)                                                              AS total,
--     ROUND(
--         COUNT(CASE WHEN "Status" = 'Completed' THEN 1 END) * 100.0
--         / NULLIF(COUNT(*), 0), 2
--     )                                                                     AS conversion_pct
-- FROM "GuestCheckouts";


-- =============================================================================
-- SECTION 6: ROLLBACK SCRIPT (KEEP THIS - USE WITH CAUTION)
-- Run ONLY if you need to undo this migration
-- =============================================================================

/*
-- Step 1: Remove Orders extensions
ALTER TABLE "Orders" DROP CONSTRAINT IF EXISTS "FK_Orders_GuestCheckouts";
DROP INDEX IF EXISTS "IX_Orders_GuestCheckoutId";
DROP INDEX IF EXISTS "IX_Orders_IsGuestOrder";
ALTER TABLE "Orders" DROP COLUMN IF EXISTS "GuestCheckoutId";
ALTER TABLE "Orders" DROP COLUMN IF EXISTS "IsGuestOrder";

-- Step 2: Drop GuestCheckouts (CASCADE removes its indexes and constraints)
DROP TABLE IF EXISTS "GuestCheckouts" CASCADE;

-- Step 3: Drop GuestCarts
DROP TABLE IF EXISTS "GuestCarts" CASCADE;
*/


-- =============================================================================
-- MIGRATION COMPLETE
-- =============================================================================
SELECT 'Guest Checkout Migration applied successfully to Neon PostgreSQL.' AS migration_status;
