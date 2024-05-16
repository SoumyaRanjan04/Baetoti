using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baetoti.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20240214 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SMSTemplate",
                schema: "baetoti",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SMSType = table.Column<int>(type: "int", nullable: false),
                    SMSText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSTemplate", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SMSTemplate",
                schema: "baetoti");
        }
    }
}
