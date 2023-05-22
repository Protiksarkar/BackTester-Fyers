using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingConsole.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instrument",
                columns: table => new
                {
                    SymbolTicker = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exchange = table.Column<int>(type: "int", nullable: false),
                    Segment = table.Column<int>(type: "int", nullable: false),
                    LotSize = table.Column<int>(type: "int", nullable: false),
                    Expiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Strike = table.Column<float>(type: "real", nullable: true),
                    OptionType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrument", x => x.SymbolTicker);
                });

            migrationBuilder.CreateTable(
                name: "MarketQuote",
                columns: table => new
                {
                    Symbol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeFrame = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketQuote", x => new { x.Symbol, x.TimeFrame, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    TradeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Value1 = table.Column<float>(type: "real", nullable: true),
                    Value2 = table.Column<float>(type: "real", nullable: true),
                    Value3 = table.Column<float>(type: "real", nullable: true),
                    Value4 = table.Column<float>(type: "real", nullable: true),
                    Value5 = table.Column<float>(type: "real", nullable: true),
                    Value6 = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instrument");

            migrationBuilder.DropTable(
                name: "MarketQuote");

            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
