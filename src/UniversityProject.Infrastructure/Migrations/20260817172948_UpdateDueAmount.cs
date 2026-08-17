using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDueAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DueAmount",
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
                values: new object[] { "01b2e34c-bea7-4ef5-a5f9-b9236eaed300", "AQAAAAIAAYagAAAAEJBHO4J0zj2t8Z0dLI5i1mPuiTjbxPKq2F8zGoPAxpetl9dndtQ63IIctQYUcy3g9w==", "8a739b82-3d09-4895-b184-e61294a1c885" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueAmount",
                table: "Purchases");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8e39b684-2b69-48c9-bb84-36f0d4ca4bc1", "AQAAAAIAAYagAAAAENHhEnADF8sKw7dchW/7IIDa5qOXTy5rap8mX/Can0Vl04MdBaXgGYM1oDTBM0hc3g==", "1853afbf-8821-4ddc-bf44-eb2e35655f10" });
        }
    }
}
