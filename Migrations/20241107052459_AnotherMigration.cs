using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimart_Api.Migrations
{
    public partial class AnotherMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    CountyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountyCode = table.Column<int>(type: "int", nullable: false),
                    CountyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.CountyId);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    FeatureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeatureOptions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.FeatureID);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.PaymentID);
                });

            migrationBuilder.CreateTable(
                name: "ResponseStatus",
                columns: table => new
                {
                    ResponseStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResponseCode = table.Column<bool>(type: "bit", nullable: false),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseStatus", x => x.ResponseStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    ResponseCode = table.Column<int>(type: "int", nullable: false),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "t_Categories",
                columns: table => new
                {
                    categoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__t_Catego__23CAF1F8AFEA0E02", x => x.categoryID);
                });

            migrationBuilder.CreateTable(
                name: "t_dashboarditems",
                columns: table => new
                {
                    DashBoardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_dashboarditems_DashBoardID", x => x.DashBoardID);
                });

            migrationBuilder.CreateTable(
                name: "t_features",
                columns: table => new
                {
                    productID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MainCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScreenSize = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RefreshRate = table.Column<int>(type: "int", nullable: true),
                    ResponseTime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdaptiveSync = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    connectiveTechnology = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpecialFeatures = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScreenSurface = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MountingType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageBrightness = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemWeight = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayResolution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WarrantyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_features_productID", x => x.productID);
                });

            migrationBuilder.CreateTable(
                name: "t_helpsettings",
                columns: table => new
                {
                    ROWID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_helpsettings_ROWID", x => x.ROWID);
                });

            migrationBuilder.CreateTable(
                name: "t_image",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ImageUrl = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_image_ImageID", x => x.ImageID);
                });

            migrationBuilder.CreateTable(
                name: "t_RefreshToken",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    RefreshToken = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "t_Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    isLoggedIn = table.Column<bool>(type: "bit", nullable: true),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    isAdmin = table.Column<bool>(type: "bit", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    PasswordChangesOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    FailedAttempts = table.Column<int>(type: "int", nullable: true),
                    RoleID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__t_Users__1788CCAC83A2EDF9", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UsrRegStatuses",
                columns: table => new
                {
                    ResponseCode = table.Column<int>(type: "int", nullable: false),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Towns",
                columns: table => new
                {
                    TownId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TownName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Towns", x => x.TownId);
                    table.ForeignKey(
                        name: "FK_Towns_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "CountyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "paymentDetails",
                columns: table => new
                {
                    PaymentMethodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentID = table.Column<int>(type: "int", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentDetails", x => x.PaymentMethodID);
                    table.ForeignKey(
                        name: "FK_paymentDetails_payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "payments",
                        principalColumn: "PaymentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserInfoId);
                    table.ForeignKey(
                        name: "FK_Users_ResponseStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ResponseStatus",
                        principalColumn: "ResponseStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_Products",
                columns: table => new
                {
                    ProductID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    productName = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,0)", nullable: true),
                    stockQuantity = table.Column<int>(type: "int", nullable: false),
                    categoryID = table.Column<int>(type: "int", nullable: true),
                    RowID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InStock = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    SearchKeyWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyFeatures = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Box = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SubCategoryID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__t_Produc__B40CC6ED6CED5B85", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK__t_Product__categ__18EBB532",
                        column: x => x.categoryID,
                        principalTable: "t_Categories",
                        principalColumn: "categoryID");
                });

            

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    County = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraInformation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isDefault = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_Addresses_t_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "t_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_Carts",
                columns: table => new
                {
                    cartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<int>(type: "int", nullable: true),
                    cartName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__t_Carts__415B03D8762A75C5", x => x.cartID);
                    table.ForeignKey(
                        name: "FK__t_Carts__userID__2739D489",
                        column: x => x.userID,
                        principalTable: "t_Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "t_Orders",
                columns: table => new
                {
                    orderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<int>(type: "int", nullable: true),
                    orderDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    totalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__t_Orders__0809337DBD766844", x => x.orderID);
                    table.ForeignKey(
                        name: "FK__t_Orders__userID__1BC821DD",
                        column: x => x.userID,
                        principalTable: "t_Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "DeliveryStations",
                columns: table => new
                {
                    DeliveryStationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryStationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TownId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryStations", x => x.DeliveryStationId);
                    table.ForeignKey(
                        name: "FK_DeliveryStations_Towns_TownId",
                        column: x => x.TownId,
                        principalTable: "Towns",
                        principalColumn: "TownId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    OrderID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodID = table.Column<int>(type: "int", nullable: false),
                    PaymentConfirmation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalOrderAmount = table.Column<double>(type: "float", nullable: false),
                    TotalPaymentAmount = table.Column<double>(type: "float", nullable: false),
                    TotalDeliveryFees = table.Column<double>(type: "float", nullable: false),
                    TotalTax = table.Column<double>(type: "float", nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_orders_paymentDetails_PaymentMethodID",
                        column: x => x.PaymentMethodID,
                        principalTable: "paymentDetails",
                        principalColumn: "PaymentMethodID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_t_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "t_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_Reviews",
                columns: table => new
                {
                    reviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    userID = table.Column<int>(type: "int", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    review_date = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__t_Review__2ECD6E2451F94E4F", x => x.reviewID);
                    table.ForeignKey(
                        name: "FK__t_Reviews__prod",
                        column: x => x.productID,
                        principalTable: "t_Products",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK__t_Reviews__userI__245D67DE",
                        column: x => x.userID,
                        principalTable: "t_Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "SubCategoryFeatures",
                columns: table => new
                {
                    SubCategoryFeatureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    FeatureID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategoryFeatures", x => x.SubCategoryFeatureID);
                    table.ForeignKey(
                        name: "FK_SubCategoryFeatures_Features_FeatureID",
                        column: x => x.FeatureID,
                        principalTable: "Features",
                        principalColumn: "FeatureID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubCategoryFeatures_t_subcategoryid_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "t_subcategoryid",
                        principalColumn: "SubCategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    cartItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cartID = table.Column<int>(type: "int", nullable: true),
                    productID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.cartItemID);
                    table.ForeignKey(
                        name: "FK__CartItems__cartI__2A164134",
                        column: x => x.cartID,
                        principalTable: "t_Carts",
                        principalColumn: "cartID");
                    table.ForeignKey(
                        name: "FK__CartItems__prod",
                        column: x => x.productID,
                        principalTable: "t_Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "t_OrderItems",
                columns: table => new
                {
                    orderItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderID = table.Column<int>(type: "int", nullable: true),
                    productID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__t_OrderI__3724BD72056978F8", x => x.orderItemID);
                    table.ForeignKey(
                        name: "FK__t_OrderIt__order__1EA48E88",
                        column: x => x.orderID,
                        principalTable: "t_Orders",
                        principalColumn: "orderID");
                    table.ForeignKey(
                        name: "FK__t_OrderItems__prod",
                        column: x => x.productID,
                        principalTable: "t_Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "OrderProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductID = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProducts_orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProducts_t_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "t_Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orderStatus",
                columns: table => new
                {
                    Status = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderTrackingTrackingID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderStatus", x => x.Status);
                });

            migrationBuilder.CreateTable(
                name: "orderTracking",
                columns: table => new
                {
                    TrackingID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrackingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    Carrier = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderTracking", x => x.TrackingID);
                    table.ForeignKey(
                        name: "FK_orderTracking_orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orderTracking_orderStatus_CurrentStatus",
                        column: x => x.CurrentStatus,
                        principalTable: "orderStatus",
                        principalColumn: "Status",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orderTracking_orderStatus_PreviousStatus",
                        column: x => x.PreviousStatus,
                        principalTable: "orderStatus",
                        principalColumn: "Status",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserID",
                table: "Addresses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_cartID",
                table: "CartItems",
                column: "cartID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_productID",
                table: "CartItems",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryStations_TownId",
                table: "DeliveryStations",
                column: "TownId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_OrderID",
                table: "OrderProducts",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductID",
                table: "OrderProducts",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_orders_PaymentMethodID",
                table: "orders",
                column: "PaymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_orders_UserID",
                table: "orders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_orderStatus_OrderTrackingTrackingID",
                table: "orderStatus",
                column: "OrderTrackingTrackingID");

            migrationBuilder.CreateIndex(
                name: "IX_orderTracking_CurrentStatus",
                table: "orderTracking",
                column: "CurrentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_orderTracking_OrderID",
                table: "orderTracking",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_orderTracking_PreviousStatus",
                table: "orderTracking",
                column: "PreviousStatus");

            migrationBuilder.CreateIndex(
                name: "IX_paymentDetails_PaymentID",
                table: "paymentDetails",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryFeatures_FeatureID",
                table: "SubCategoryFeatures",
                column: "FeatureID");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryFeatures_SubCategoryId",
                table: "SubCategoryFeatures",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Carts_userID",
                table: "t_Carts",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_t_OrderItems_orderID",
                table: "t_OrderItems",
                column: "orderID");

            migrationBuilder.CreateIndex(
                name: "IX_t_OrderItems_productID",
                table: "t_OrderItems",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_t_Orders_userID",
                table: "t_Orders",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_t_Products_categoryID",
                table: "t_Products",
                column: "categoryID");

            migrationBuilder.CreateIndex(
                name: "IX_t_Reviews_productID",
                table: "t_Reviews",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_t_Reviews_userID",
                table: "t_Reviews",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_t_subcategoryid_CategoryID",
                table: "t_subcategoryid",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Towns_CountyId",
                table: "Towns",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StatusId",
                table: "Users",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_orderStatus_orderTracking_OrderTrackingTrackingID",
                table: "orderStatus",
                column: "OrderTrackingTrackingID",
                principalTable: "orderTracking",
                principalColumn: "TrackingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_t_Users_UserID",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orderTracking_orders_OrderID",
                table: "orderTracking");

            migrationBuilder.DropForeignKey(
                name: "FK_orderStatus_orderTracking_OrderTrackingTrackingID",
                table: "orderStatus");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "DeliveryStations");

            migrationBuilder.DropTable(
                name: "OrderProducts");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "SubCategoryFeatures");

            migrationBuilder.DropTable(
                name: "t_dashboarditems");

            migrationBuilder.DropTable(
                name: "t_features");

            migrationBuilder.DropTable(
                name: "t_helpsettings");

            migrationBuilder.DropTable(
                name: "t_image");

            migrationBuilder.DropTable(
                name: "t_OrderItems");

            migrationBuilder.DropTable(
                name: "t_RefreshToken");

            migrationBuilder.DropTable(
                name: "t_Reviews");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UsrRegStatuses");

            migrationBuilder.DropTable(
                name: "t_Carts");

            migrationBuilder.DropTable(
                name: "Towns");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "t_subcategoryid");

            migrationBuilder.DropTable(
                name: "t_Orders");

            migrationBuilder.DropTable(
                name: "t_Products");

            migrationBuilder.DropTable(
                name: "ResponseStatus");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "t_Categories");

            migrationBuilder.DropTable(
                name: "t_Users");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "paymentDetails");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "orderTracking");

            migrationBuilder.DropTable(
                name: "orderStatus");
        }
    }
}
