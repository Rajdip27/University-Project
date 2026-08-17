using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaidAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Purchases",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8e39b684-2b69-48c9-bb84-36f0d4ca4bc1", "AQAAAAIAAYagAAAAENHhEnADF8sKw7dchW/7IIDa5qOXTy5rap8mX/Can0Vl04MdBaXgGYM1oDTBM0hc3g==", "1853afbf-8821-4ddc-bf44-eb2e35655f10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Purchases");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84baf707-1c53-4f17-bf3a-0e1bf64e4132", "AQAAAAIAAYagAAAAEDCgyYYQZKm9XTPYzt2uYh34uLqO4en2/tGWZfd82RdDcyKrMMD+N0IHwt5Z5HVj/w==", "da13a75d-f485-49db-92e1-103800904b74" });
        }
    }
}
