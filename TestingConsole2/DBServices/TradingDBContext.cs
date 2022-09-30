using BackTestDemo.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BackTestDemo.DBServices
{
    public class TradingDBContext : DbContext
    {
        public TradingDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<MarketQuoteDTO> MarketQuote { get; set; }
        public DbSet<OrderDTO> Order { get; set; }
        public DbSet<InstrumentDTO> Instrument { get; set; }
    }
}
