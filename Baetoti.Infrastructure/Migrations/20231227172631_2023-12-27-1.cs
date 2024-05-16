using Microsoft.EntityFrameworkCore.Migrations;

namespace Baetoti.Infrastructure.Migrations
{
    public partial class _202312271 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DISCOUNT",
                schema: "baetoti",
                table: "Transaction",
                newName: "Discount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discount",
                schema: "baetoti",
                table: "Transaction",
                newName: "DISCOUNT");
        }
    }
}
