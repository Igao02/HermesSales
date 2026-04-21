using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HermesSales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateLogProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogProduct_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LogProduct_ProductImages_ProductImageId",
                        column: x => x.ProductImageId,
                        principalTable: "ProductImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LogProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogProduct_ApplicationUserId",
                table: "LogProduct",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogProduct_ProductId",
                table: "LogProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LogProduct_ProductImageId",
                table: "LogProduct",
                column: "ProductImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogProduct");
        }
    }
}
