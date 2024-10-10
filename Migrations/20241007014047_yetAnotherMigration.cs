using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimart_Api.Migrations
{
    public partial class yetAnotherMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

        


         


      

          

            

          


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
                name: "FK_orderTracking_orders_OrderID",
                table: "orderTracking");

            migrationBuilder.DropForeignKey(
                name: "FK_orderStatus_orderTracking_OrderTrackingTrackingID",
                table: "orderStatus");



            migrationBuilder.DropTable(
                name: "orderTracking");

            migrationBuilder.DropTable(
                name: "orderStatus");
        }
    }
}
