using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "SalesInvoices",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84baf707-1c53-4f17-bf3a-0e1bf64e4132", "AQAAAAIAAYagAAAAEDCgyYYQZKm9XTPYzt2uYh34uLqO4en2/tGWZfd82RdDcyKrMMD+N0IHwt5Z5HVj/w==", "da13a75d-f485-49db-92e1-103800904b74" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_WarehouseId",
                table: "SalesInvoices",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesInvoices_Warehouses_WarehouseId",
                table: "SalesInvoices",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesInvoices_Warehouses_WarehouseId",
                table: "SalesInvoices");

            migrationBuilder.DropIndex(
                name: "IX_SalesInvoices_WarehouseId",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "SalesInvoices");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1b7734c2-fcd4-477b-be62-51d5ee4adbb2", "AQAAAAIAAYagAAAAELy3TFbDY1ELGFAulEYFO0kMKh+WW/chrL8s/6xqPpavbKnKF1a2LFPacdmHNZ75DQ==", "4cc12aaa-e9b9-47cc-9a76-82ad5939389f" });
        }
    }
}
