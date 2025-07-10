using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhantomMask.Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pharmacies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cashBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    openingHours = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cashBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Masks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    stockQuantity = table.Column<int>(type: "int", nullable: false),
                    pharmaciesid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Masks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Masks_Pharmacies_pharmaciesid",
                        column: x => x.pharmaciesid,
                        principalTable: "Pharmacies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseHistory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pharmacyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transactionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    transactionQuantity = table.Column<int>(type: "int", nullable: false),
                    transactionDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usersid = table.Column<int>(type: "int", nullable: false),
                    pharmacyId = table.Column<int>(type: "int", nullable: false),
                    masksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseHistory", x => x.id);
                    table.ForeignKey(
                        name: "FK_PurchaseHistory_Users_usersid",
                        column: x => x.usersid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Masks_pharmaciesid",
                table: "Masks",
                column: "pharmaciesid");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseHistory_usersid",
                table: "PurchaseHistory",
                column: "usersid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Masks");

            migrationBuilder.DropTable(
                name: "PurchaseHistory");

            migrationBuilder.DropTable(
                name: "Pharmacies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
