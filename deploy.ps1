# =====================================================
# QUICKCRATE GUEST CHECKOUT & PRICE ROUNDING DEPLOYMENT
# PowerShell Script for Windows
# =====================================================

param(
    [string]$DbUser = "postgres",
    [string]$DbName = "quickcrate",
    [string]$DbHost = "localhost"
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "QuickCrate Deployment Script" -ForegroundColor Cyan
Write-Host "Guest Checkout + Price Rounding" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$BackupDir = ".\backups"
$MigrationDir = ".\Migrations"

# Create backup directory
if (!(Test-Path $BackupDir)) {
    New-Item -ItemType Directory -Path $BackupDir | Out-Null
}

# =====================================================
# STEP 1: BACKUP DATABASE
# =====================================================
Write-Host "?? Step 1: Backing up database..." -ForegroundColor Yellow
$BackupFile = "$BackupDir\backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql"

try {
    & pg_dump -U $DbUser -h $DbHost -d $DbName -f $BackupFile
    Write-Host "? Backup created: $BackupFile" -ForegroundColor Green
} catch {
    Write-Host "? Backup failed! Aborting." -ForegroundColor Red
    exit 1
}

Write-Host ""

# =====================================================
# STEP 2: RUN GUEST CHECKOUT MIGRATION
# =====================================================
Write-Host "?? Step 2: Running Guest Checkout migration..." -ForegroundColor Yellow

try {
    Get-Content "$MigrationDir\GuestCheckout_Migration.sql" | & psql -U $DbUser -h $DbHost -d $DbName
    Write-Host "? Guest Checkout migration completed" -ForegroundColor Green
} catch {
    Write-Host "? Migration failed! Check errors above." -ForegroundColor Red
    Write-Host "Backup available at: $BackupFile" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# =====================================================
# STEP 3: RUN PRICE ROUNDING MIGRATION
# =====================================================
Write-Host "?? Step 3: Running Price Rounding migration..." -ForegroundColor Yellow

try {
    Get-Content "$MigrationDir\Price_Rounding_Migration.sql" | & psql -U $DbUser -h $DbHost -d $DbName
    Write-Host "? Price Rounding migration completed" -ForegroundColor Green
} catch {
    Write-Host "? Migration failed! Check errors above." -ForegroundColor Red
    Write-Host "Backup available at: $BackupFile" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# =====================================================
# STEP 4: VERIFY MIGRATIONS
# =====================================================
Write-Host "?? Step 4: Verifying migrations..." -ForegroundColor Yellow

# Check GuestCarts table
$guestTablesQuery = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name IN ('GuestCarts', 'GuestCheckouts');"
$guestTablesCount = & psql -U $DbUser -h $DbHost -d $DbName -t -c $guestTablesQuery

if ($guestTablesCount -match "2") {
    Write-Host "? Guest tables created successfully" -ForegroundColor Green
} else {
    Write-Host "? Guest tables not found!" -ForegroundColor Red
    exit 1
}

# Check Orders columns
$orderColumnsQuery = "SELECT COUNT(*) FROM information_schema.columns WHERE table_name = 'Orders' AND column_name IN ('IsGuestOrder', 'GuestCheckoutId');"
$orderColumnsCount = & psql -U $DbUser -h $DbHost -d $DbName -t -c $orderColumnsQuery

if ($orderColumnsCount -match "2") {
    Write-Host "? Order columns added successfully" -ForegroundColor Green
} else {
    Write-Host "? Order columns not found!" -ForegroundColor Red
    exit 1
}

# Check for decimal prices
$decimalPricesQuery = "SELECT COUNT(*) FROM \`"Products\`" WHERE \`"Price\`" != ROUND(\`"Price\`", 0) AND \`"IsDeleted\`" = FALSE;"
$decimalPricesCount = & psql -U $DbUser -h $DbHost -d $DbName -t -c $decimalPricesQuery

if ($decimalPricesCount -match "0") {
    Write-Host "? All prices rounded successfully" -ForegroundColor Green
} else {
    Write-Host "??  Warning: $decimalPricesCount products still have decimal prices" -ForegroundColor Yellow
}

Write-Host ""

# =====================================================
# STEP 5: BUILD APPLICATION
# =====================================================
Write-Host "?? Step 5: Building application..." -ForegroundColor Yellow

try {
    dotnet build
    Write-Host "? Build successful" -ForegroundColor Green
} catch {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""

# =====================================================
# STEP 6: PUBLISH APPLICATION
# =====================================================
Write-Host "?? Step 6: Publishing application..." -ForegroundColor Yellow

try {
    dotnet publish -c Release -o .\publish
    Write-Host "? Publish successful" -ForegroundColor Green
} catch {
    Write-Host "? Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""

# =====================================================
# DEPLOYMENT SUMMARY
# =====================================================
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "? DEPLOYMENT COMPLETE!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Summary:" -ForegroundColor White
Write-Host "  ? Database backup: $BackupFile" -ForegroundColor Green
Write-Host "  ? Guest Checkout migration: Complete" -ForegroundColor Green
Write-Host "  ? Price Rounding migration: Complete" -ForegroundColor Green
Write-Host "  ? Application built: Success" -ForegroundColor Green
Write-Host "  ? Application published: .\publish" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Test endpoints manually"
Write-Host "  2. Monitor logs: Get-Content Logs\app-*.log -Wait"
Write-Host "  3. Deploy to production"
Write-Host ""
Write-Host "API Endpoints Available:" -ForegroundColor Cyan
Write-Host "  - POST /api/guestcart/add"
Write-Host "  - GET  /api/guestcart/{guestId}"
Write-Host "  - POST /api/guestcheckout/initiate"
Write-Host "  - GET  /api/guestcheckout/status/{id}"
Write-Host ""
Write-Host "Documentation:" -ForegroundColor Cyan
Write-Host "  - GUEST_CHECKOUT_QUICK_START.md"
Write-Host "  - PRICE_ROUNDING_SUMMARY.md"
Write-Host "  - MASTER_IMPLEMENTATION_CHECKLIST.md"
Write-Host ""
Write-Host "?? Ready for production!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
