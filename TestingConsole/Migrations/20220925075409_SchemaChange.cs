using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingConsole.Migrations
{
    public partial class SchemaChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exchange",
                table: "MarketQuote");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Exchange",
                table: "MarketQuote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
