-- =====================================================
-- PRICE ROUNDING MIGRATION - CONVERT TO WHOLE NUMBERS
-- For PostgreSQL
-- =====================================================
-- Purpose: Round all prices to whole numbers for Kenya market
-- Reason: M-Pesa compatibility + simpler customer experience
-- =====================================================

-- =====================================================
-- BACKUP YOUR DATA FIRST!
-- =====================================================
-- Run this before proceeding:
-- pg_dump -U your_username -d your_database > backup_before_price_rounding.sql

-- =====================================================
-- STEP 1: ANALYZE CURRENT PRICES
-- =====================================================
SELECT 
    'Products' as table_name,
    COUNT(*) as total_records,
    COUNT(CASE WHEN "Price" != ROUND("Price", 0) THEN 1 END) as prices_with_decimals,
    MIN("Price") as min_price,
    MAX("Price") as max_price,
    AVG("Price") as avg_price
FROM "Products"
WHERE "IsDeleted" = FALSE;

SELECT 
    'Sample prices that will change' as info,
    "ProductId",
    "ProductName",
    "Price" as current_price,
    ROUND("Price", 0) as new_price,
    (ROUND("Price", 0) - "Price") as difference
FROM "Products"
WHERE "Price" != ROUND("Price", 0)
AND "IsDeleted" = FALSE
LIMIT 20;

-- =====================================================
-- STEP 2: ROUND PRICES IN PRODUCTS TABLE
-- =====================================================

-- Round product prices
UPDATE "Products"
SET 
    "Price" = ROUND("Price", 0),
    "UpdatedOn" = NOW(),
    "UpdatedBy" = 'SYSTEM_PRICE_ROUNDING_MIGRATION'
WHERE "Price" != ROUND("Price", 0)
AND "IsDeleted" = FALSE;

-- Log update
SELECT 
    'Products prices rounded' as status,
    COUNT(*) as updated_count
FROM "Products"
WHERE "UpdatedBy" = 'SYSTEM_PRICE_ROUNDING_MIGRATION';

-- =====================================================
-- STEP 3: ROUND ORDER AMOUNTS
-- =====================================================

-- Round order totals
UPDATE "Orders"
SET 
    "TotalOrderAmount" = ROUND("TotalOrderAmount", 0),
    "TotalPaymentAmount" = ROUND("TotalPaymentAmount", 0),
    "TotalDeliveryFees" = ROUND("TotalDeliveryFees", 0),
    "TotalTax" = ROUND("TotalTax", 0)
WHERE (
    "TotalOrderAmount" != ROUND("TotalOrderAmount", 0) OR
    "TotalPaymentAmount" != ROUND("TotalPaymentAmount", 0) OR
    "TotalDeliveryFees" != ROUND("TotalDeliveryFees", 0) OR
    "TotalTax" != ROUND("TotalTax", 0)
);

SELECT 
    'Order amounts rounded' as status,
    COUNT(*) as updated_count
FROM "Orders"
WHERE "TotalOrderAmount" = ROUND("TotalOrderAmount", 0);

-- =====================================================
-- STEP 4: ROUND ORDER PRODUCT PRICES
-- =====================================================

UPDATE "OrderProducts"
SET 
    "TotalPrice" = ROUND("TotalPrice", 0),
    "UpdatedOn" = NOW()
WHERE "TotalPrice" != ROUND("TotalPrice", 0);

SELECT 
    'OrderProducts prices rounded' as status,
    COUNT(*) as updated_count
FROM "OrderProducts"
WHERE "TotalPrice" = ROUND("TotalPrice", 0);

-- =====================================================
-- STEP 5: ROUND PAYMENT DETAILS
-- =====================================================

UPDATE "PaymentDetails"
SET "Amount" = ROUND(CAST("Amount" AS NUMERIC), 0)::MONEY
WHERE CAST("Amount" AS NUMERIC) != ROUND(CAST("Amount" AS NUMERIC), 0);

SELECT 
    'PaymentDetails amounts rounded' as status,
    COUNT(*) as updated_count
FROM "PaymentDetails";

-- =====================================================
-- STEP 6: ROUND GUEST CHECKOUT AMOUNTS (IF ALREADY EXISTS)
-- =====================================================

-- Only run if GuestCheckouts table exists
DO $$
BEGIN
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'GuestCheckouts') THEN
        UPDATE "GuestCheckouts"
        SET 
            "DeliveryFee" = ROUND("DeliveryFee", 0),
            "SubtotalAmount" = ROUND("SubtotalAmount", 0),
            "TotalAmount" = ROUND("TotalAmount", 0)
        WHERE (
            "DeliveryFee" != ROUND("DeliveryFee", 0) OR
            "SubtotalAmount" != ROUND("SubtotalAmount", 0) OR
            "TotalAmount" != ROUND("TotalAmount", 0)
        );
        
        RAISE NOTICE 'GuestCheckouts amounts rounded';
    END IF;
END $$;

-- =====================================================
-- STEP 7: ROUND PAYOUT AMOUNTS
-- =====================================================

UPDATE "Payouts"
SET 
    "GrossAmount" = ROUND("GrossAmount", 0),
    "CommissionAmount" = ROUND("CommissionAmount", 0),
    "NetAmount" = ROUND("NetAmount", 0),
    "UpdatedDate" = NOW()
WHERE (
    "GrossAmount" != ROUND("GrossAmount", 0) OR
    "CommissionAmount" != ROUND("CommissionAmount", 0) OR
    "NetAmount" != ROUND("NetAmount", 0)
);

UPDATE "PayoutTransactions"
SET 
    "OrderAmount" = ROUND("OrderAmount", 0),
    "CommissionAmount" = ROUND("CommissionAmount", 0),
    "NetAmount" = ROUND("NetAmount", 0)
WHERE (
    "OrderAmount" != ROUND("OrderAmount", 0) OR
    "CommissionAmount" != ROUND("CommissionAmount", 0) OR
    "NetAmount" != ROUND("NetAmount", 0)
);

SELECT 
    'Payout amounts rounded' as status,
    (SELECT COUNT(*) FROM "Payouts") as payouts_updated,
    (SELECT COUNT(*) FROM "PayoutTransactions") as transactions_updated;

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================

-- Verify no decimals remain in critical tables
SELECT 
    'Products with decimals' as check_name,
    COUNT(*) as count
FROM "Products"
WHERE "Price" != ROUND("Price", 0)
AND "IsDeleted" = FALSE;

SELECT 
    'Orders with decimals' as check_name,
    COUNT(*) as count
FROM "Orders"
WHERE (
    "TotalOrderAmount" != ROUND("TotalOrderAmount", 0) OR
    "TotalPaymentAmount" != ROUND("TotalPaymentAmount", 0)
);

SELECT 
    'OrderProducts with decimals' as check_name,
    COUNT(*) as count
FROM "OrderProducts"
WHERE "TotalPrice" != ROUND("TotalPrice", 0);

-- Sample verification
SELECT 
    'Sample rounded products' as info,
    "ProductId",
    "ProductName",
    "Price"
FROM "Products"
WHERE "IsDeleted" = FALSE
ORDER BY "CreatedOn" DESC
LIMIT 10;

-- =====================================================
-- SUMMARY REPORT
-- =====================================================
SELECT '========================================' as separator;
SELECT 'PRICE ROUNDING MIGRATION COMPLETE' as status;
SELECT '========================================' as separator;

SELECT 
    'Products' as table_name,
    COUNT(*) as total_records,
    MIN("Price") as min_price,
    MAX("Price") as max_price,
    AVG("Price")::NUMERIC(10,0) as avg_price
FROM "Products"
WHERE "IsDeleted" = FALSE

UNION ALL

SELECT 
    'Orders' as table_name,
    COUNT(*) as total_records,
    MIN("TotalPaymentAmount") as min_amount,
    MAX("TotalPaymentAmount") as max_amount,
    AVG("TotalPaymentAmount")::NUMERIC(10,0) as avg_amount
FROM "Orders";

-- =====================================================
-- ROLLBACK SCRIPT (IN CASE OF ISSUES)
-- =====================================================
/*
-- If you need to rollback, restore from backup:
-- psql -U your_username -d your_database < backup_before_price_rounding.sql
*/

SELECT 'Migration Complete! All prices rounded to whole numbers.' as final_message;
