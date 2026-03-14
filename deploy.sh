#!/bin/bash

# =====================================================
# QUICKCRATE GUEST CHECKOUT & PRICE ROUNDING DEPLOYMENT
# =====================================================

set -e  # Exit on error

echo "========================================="
echo "QuickCrate Deployment Script"
echo "Guest Checkout + Price Rounding"
echo "========================================="
echo ""

# Configuration
DB_USER=${DB_USER:-postgres}
DB_NAME=${DB_NAME:-quickcrate}
BACKUP_DIR="./backups"
MIGRATION_DIR="./Migrations"

# Create backup directory
mkdir -p "$BACKUP_DIR"

# =====================================================
# STEP 1: BACKUP DATABASE
# =====================================================
echo "?? Step 1: Backing up database..."
BACKUP_FILE="$BACKUP_DIR/backup_$(date +%Y%m%d_%H%M%S).sql"
pg_dump -U "$DB_USER" -d "$DB_NAME" > "$BACKUP_FILE"

if [ $? -eq 0 ]; then
    echo "? Backup created: $BACKUP_FILE"
else
    echo "? Backup failed! Aborting."
    exit 1
fi

echo ""

# =====================================================
# STEP 2: RUN GUEST CHECKOUT MIGRATION
# =====================================================
echo "?? Step 2: Running Guest Checkout migration..."
psql -U "$DB_USER" -d "$DB_NAME" < "$MIGRATION_DIR/GuestCheckout_Migration.sql"

if [ $? -eq 0 ]; then
    echo "? Guest Checkout migration completed"
else
    echo "? Migration failed! Rolling back..."
    psql -U "$DB_USER" -d "$DB_NAME" < "$BACKUP_FILE"
    exit 1
fi

echo ""

# =====================================================
# STEP 3: RUN PRICE ROUNDING MIGRATION
# =====================================================
echo "?? Step 3: Running Price Rounding migration..."
psql -U "$DB_USER" -d "$DB_NAME" < "$MIGRATION_DIR/Price_Rounding_Migration.sql"

if [ $? -eq 0 ]; then
    echo "? Price Rounding migration completed"
else
    echo "? Migration failed! Rolling back..."
    psql -U "$DB_USER" -d "$DB_NAME" < "$BACKUP_FILE"
    exit 1
fi

echo ""

# =====================================================
# STEP 4: VERIFY MIGRATIONS
# =====================================================
echo "?? Step 4: Verifying migrations..."

# Check GuestCarts table
GUEST_TABLES=$(psql -U "$DB_USER" -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_name IN ('GuestCarts', 'GuestCheckouts');")

if [ "$GUEST_TABLES" -eq 2 ]; then
    echo "? Guest tables created successfully"
else
    echo "? Guest tables not found!"
    exit 1
fi

# Check Orders columns
ORDER_COLUMNS=$(psql -U "$DB_USER" -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM information_schema.columns WHERE table_name = 'Orders' AND column_name IN ('IsGuestOrder', 'GuestCheckoutId');")

if [ "$ORDER_COLUMNS" -eq 2 ]; then
    echo "? Order columns added successfully"
else
    echo "? Order columns not found!"
    exit 1
fi

# Check for decimal prices
DECIMAL_PRICES=$(psql -U "$DB_USER" -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM \"Products\" WHERE \"Price\" != ROUND(\"Price\", 0) AND \"IsDeleted\" = FALSE;")

if [ "$DECIMAL_PRICES" -eq 0 ]; then
    echo "? All prices rounded successfully"
else
    echo "??  Warning: $DECIMAL_PRICES products still have decimal prices"
fi

echo ""

# =====================================================
# STEP 5: BUILD APPLICATION
# =====================================================
echo "?? Step 5: Building application..."
dotnet build

if [ $? -eq 0 ]; then
    echo "? Build successful"
else
    echo "? Build failed!"
    exit 1
fi

echo ""

# =====================================================
# STEP 6: PUBLISH APPLICATION
# =====================================================
echo "?? Step 6: Publishing application..."
dotnet publish -c Release -o ./publish

if [ $? -eq 0 ]; then
    echo "? Publish successful"
else
    echo "? Publish failed!"
    exit 1
fi

echo ""

# =====================================================
# DEPLOYMENT SUMMARY
# =====================================================
echo "========================================="
echo "? DEPLOYMENT COMPLETE!"
echo "========================================="
echo ""
echo "Summary:"
echo "  ? Database backup: $BACKUP_FILE"
echo "  ? Guest Checkout migration: Complete"
echo "  ? Price Rounding migration: Complete"
echo "  ? Application built: Success"
echo "  ? Application published: ./publish"
echo ""
echo "Next Steps:"
echo "  1. Test endpoints manually"
echo "  2. Monitor logs: tail -f Logs/app-*.log"
echo "  3. Deploy to production"
echo ""
echo "API Endpoints Available:"
echo "  - POST /api/guestcart/add"
echo "  - GET  /api/guestcart/{guestId}"
echo "  - POST /api/guestcheckout/initiate"
echo "  - GET  /api/guestcheckout/status/{id}"
echo ""
echo "Documentation:"
echo "  - GUEST_CHECKOUT_QUICK_START.md"
echo "  - PRICE_ROUNDING_SUMMARY.md"
echo "  - MASTER_IMPLEMENTATION_CHECKLIST.md"
echo ""
echo "?? Ready for production!"
echo "========================================="
