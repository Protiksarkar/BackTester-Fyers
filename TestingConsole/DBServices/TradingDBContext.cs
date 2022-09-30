using TestingConsole.DTOs;
using Microsoft.EntityFrameworkCore;

namespace TestingConsole.DBServices
{
    public class TradingDBContext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        //{
        //    optionsBuilder.UseSqlServer(@"Server=PSARKAR02\SQLEXPRESS;Database=TradingDB;Trusted_Connection=True;");
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=PSARKAR02\SQLEXPRESS;Database=TradingDB;Trusted_Connection=True;");
        }

        public DbSet<MarketQuoteDTO> MarketQuote { get; set; }
        public DbSet<OrderDTO> Order { get; set; }
        public DbSet<InstrumentDTO> Instrument { get; set; }
    }
}
