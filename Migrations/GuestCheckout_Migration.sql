-- =====================================================
-- GUEST CHECKOUT SYSTEM - DATABASE MIGRATION SCRIPT
-- For PostgreSQL
-- =====================================================
-- Purpose: Add guest checkout functionality to existing QuickCrate system
-- Version: 1.0
-- Date: 2024
-- =====================================================

-- =====================================================
-- STEP 1: CREATE GUESTCARTS TABLE
-- =====================================================
CREATE TABLE "GuestCarts" (
    "GuestCartId" SERIAL PRIMARY KEY,
    "GuestId" VARCHAR(100) NOT NULL,
    "ProductId" UUID NOT NULL,
    "Quantity" INTEGER NOT NULL CHECK ("Quantity" >= 1 AND "Quantity" <= 9999),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "ExpiresAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT (NOW() + INTERVAL '7 days'),
    
    -- Foreign Keys
    CONSTRAINT "FK_GuestCarts_Products" FOREIGN KEY ("ProductId") 
        REFERENCES "Products"("ProductId") ON DELETE CASCADE
);

-- Create indexes for GuestCarts
CREATE INDEX "IX_GuestCarts_GuestId" ON "GuestCarts"("GuestId");
CREATE INDEX "IX_GuestCarts_ProductId" ON "GuestCarts"("ProductId");
CREATE INDEX "IX_GuestCarts_ExpiresAt" ON "GuestCarts"("ExpiresAt");
CREATE INDEX "IX_GuestCarts_GuestId_ProductId" ON "GuestCarts"("GuestId", "ProductId");

COMMENT ON TABLE "GuestCarts" IS 'Server-side cart storage for guest users with 7-day expiration';
COMMENT ON COLUMN "GuestCarts"."GuestId" IS 'Anonymous identifier (device fingerprint, session ID, UUID)';
COMMENT ON COLUMN "GuestCarts"."ExpiresAt" IS 'Auto-delete carts after 7 days of inactivity';

-- =====================================================
-- STEP 2: CREATE GUESTCHECKOUTS TABLE
-- =====================================================
CREATE TABLE "GuestCheckouts" (
    "GuestCheckoutId" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "GuestId" VARCHAR(100) NOT NULL,
    "FullName" VARCHAR(200) NOT NULL,
    "PhoneNumber" VARCHAR(15) NOT NULL CHECK ("PhoneNumber" ~ '^0[17]\d{8}$'),
    "Email" VARCHAR(255),
    "DeliveryAddress" TEXT NOT NULL,
    "CountyId" INTEGER,
    "TownId" INTEGER,
    "DeliveryStationId" INTEGER,
    "DeliveryFee" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "SubtotalAmount" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "TotalAmount" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Pending',
    "CheckoutRequestId" VARCHAR(100),
    "MerchantRequestId" VARCHAR(100),
    "MpesaReceiptNumber" VARCHAR(100),
    "OrderId" VARCHAR(50),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    
    -- Foreign Keys
    CONSTRAINT "FK_GuestCheckouts_Counties" FOREIGN KEY ("CountyId") 
        REFERENCES "Counties"("CountyId") ON DELETE SET NULL,
    CONSTRAINT "FK_GuestCheckouts_Towns" FOREIGN KEY ("TownId") 
        REFERENCES "Towns"("TownId") ON DELETE SET NULL,
    CONSTRAINT "FK_GuestCheckouts_DeliveryStations" FOREIGN KEY ("DeliveryStationId") 
        REFERENCES "DeliveryStations"("DeliveryStationId") ON DELETE SET NULL,
    CONSTRAINT "FK_GuestCheckouts_Orders" FOREIGN KEY ("OrderId") 
        REFERENCES "Orders"("OrderID") ON DELETE SET NULL,
        
    -- Constraints
    CONSTRAINT "CHK_GuestCheckouts_Status" CHECK ("Status" IN ('Pending', 'Completed', 'Abandoned'))
);

-- Create indexes for GuestCheckouts
CREATE INDEX "IX_GuestCheckouts_GuestId" ON "GuestCheckouts"("GuestId");
CREATE INDEX "IX_GuestCheckouts_PhoneNumber" ON "GuestCheckouts"("PhoneNumber");
CREATE INDEX "IX_GuestCheckouts_Status" ON "GuestCheckouts"("Status");
CREATE INDEX "IX_GuestCheckouts_CheckoutRequestId" ON "GuestCheckouts"("CheckoutRequestId");
CREATE INDEX "IX_GuestCheckouts_MerchantRequestId" ON "GuestCheckouts"("MerchantRequestId");
CREATE INDEX "IX_GuestCheckouts_OrderId" ON "GuestCheckouts"("OrderId");
CREATE INDEX "IX_GuestCheckouts_CreatedAt" ON "GuestCheckouts"("CreatedAt");

COMMENT ON TABLE "GuestCheckouts" IS 'Stores guest checkout delivery details before payment confirmation';
COMMENT ON COLUMN "GuestCheckouts"."PhoneNumber" IS 'Kenyan phone number (07XXXXXXXX or 01XXXXXXXX format)';
COMMENT ON COLUMN "GuestCheckouts"."Status" IS 'Pending = awaiting payment, Completed = order created, Abandoned = expired/cancelled';

-- =====================================================
-- STEP 3: ADD GUEST ORDER FIELDS TO ORDERS TABLE
-- =====================================================
-- Add IsGuestOrder column
ALTER TABLE "Orders" 
ADD COLUMN "IsGuestOrder" BOOLEAN NOT NULL DEFAULT FALSE;

-- Add GuestCheckoutId column
ALTER TABLE "Orders" 
ADD COLUMN "GuestCheckoutId" UUID;

-- Add foreign key constraint
ALTER TABLE "Orders"
ADD CONSTRAINT "FK_Orders_GuestCheckouts" 
FOREIGN KEY ("GuestCheckoutId") 
REFERENCES "GuestCheckouts"("GuestCheckoutId") ON DELETE SET NULL;

-- Create indexes
CREATE INDEX "IX_Orders_IsGuestOrder" ON "Orders"("IsGuestOrder");
CREATE INDEX "IX_Orders_GuestCheckoutId" ON "Orders"("GuestCheckoutId");

COMMENT ON COLUMN "Orders"."IsGuestOrder" IS 'Indicates if this is a guest order (no user account)';
COMMENT ON COLUMN "Orders"."GuestCheckoutId" IS 'Links to the guest checkout record if applicable';

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================
-- Verify tables were created
SELECT 
    tablename, 
    schemaname 
FROM pg_tables 
WHERE tablename IN ('GuestCarts', 'GuestCheckouts')
ORDER BY tablename;

-- Verify indexes
SELECT 
    schemaname,
    tablename, 
    indexname 
FROM pg_indexes 
WHERE tablename IN ('GuestCarts', 'GuestCheckouts', 'Orders')
    AND indexname LIKE '%Guest%'
ORDER BY tablename, indexname;

-- Verify Orders table modifications
SELECT 
    column_name, 
    data_type, 
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'Orders' 
    AND column_name IN ('IsGuestOrder', 'GuestCheckoutId')
ORDER BY column_name;

-- =====================================================
-- ROLLBACK SCRIPT (USE WITH CAUTION)
-- =====================================================
/*
-- To rollback these changes, run the following:

-- Remove foreign key from Orders
ALTER TABLE "Orders" DROP CONSTRAINT IF EXISTS "FK_Orders_GuestCheckouts";

-- Remove columns from Orders
ALTER TABLE "Orders" DROP COLUMN IF EXISTS "GuestCheckoutId";
ALTER TABLE "Orders" DROP COLUMN IF EXISTS "IsGuestOrder";

-- Drop GuestCheckouts table
DROP TABLE IF EXISTS "GuestCheckouts" CASCADE;

-- Drop GuestCarts table
DROP TABLE IF EXISTS "GuestCarts" CASCADE;
*/

-- =====================================================
-- SAMPLE DATA FOR TESTING (OPTIONAL)
-- =====================================================
/*
-- Test GuestCart entry
INSERT INTO "GuestCarts" ("GuestId", "ProductId", "Quantity")
VALUES (
    'test-guest-123',
    (SELECT "ProductId" FROM "Products" LIMIT 1),
    2
);

-- Verify test data
SELECT * FROM "GuestCarts" WHERE "GuestId" = 'test-guest-123';
*/

-- =====================================================
-- MIGRATION COMPLETE
-- =====================================================
COMMENT ON SCHEMA public IS 'Guest Checkout Migration v1.0 - Applied';
SELECT 'Guest Checkout System Migration Completed Successfully!' AS status;
