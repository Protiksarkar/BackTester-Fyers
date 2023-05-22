using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Xml;
using TestingConsole.DTOs;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TestingConsole.DBServices
{
    public class TradingRepo
    {
        private readonly IDbContextFactory<TradingDBContext> _dbContextFactory;

        public TradingRepo(IDbContextFactory<TradingDBContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public void UpdateInstuments(List<InstrumentDTO> instruments)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                _context.Instrument.AddRange(instruments);
                var result = _context.Instrument.AsEnumerable()
                    .GroupBy(s => s.SymbolTicker)
                    .SelectMany(grp => grp.Skip(1)).ToList();

                _context.Instrument.RemoveRange(result);
                _context.SaveChanges();
            }
        }

        public InstrumentDTO GetInstrument(string symbolTicker, int exchange)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                return _context.Instrument.First(x => x.SymbolTicker == symbolTicker && (int)x.Exchange == exchange);
            }
        }

        public InstrumentDTO GetInstrument(string symbolTicker)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                return _context.Instrument.First(x => x.SymbolTicker == symbolTicker);
            }
        }

        public async Task UpdateQuotesAsync(List<MarketQuoteDTO> quotes)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                var newData = quotes.Where(quote => !_context.MarketQuote.Any(dbQuote => dbQuote.Symbol == quote.Symbol && dbQuote.TimeFrame == quote.TimeFrame && dbQuote.Date == quote.Date));

                _context.MarketQuote.AddRange(newData);

                await _context.SaveChangesAsync();
            }
        }

        public void AddQuotes(List<MarketQuoteDTO> marketQuotes)
        {
            foreach (var quote in marketQuotes)
            {
                using (var _context = _dbContextFactory.CreateDbContext())
                {
                    if (!_context.MarketQuote.Any(x => x.Date == quote.Date && x.TimeFrame == quote.TimeFrame && x.Symbol == quote.Symbol))
                    {
                        _context.MarketQuote.Add(quote);
                    }
                    _context.SaveChanges(true);
                }
            }
        }

        public List<MarketQuoteDTO> GetQuotes(string symbol, DateTime from, DateTime to, int timefm)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                return _context.MarketQuote.Where(x => x.Symbol == symbol && x.Date >= from && x.Date <= to && x.TimeFrame == timefm).OrderBy(x => x.Date).ToList<MarketQuoteDTO>();
            }
        }

        public MarketQuoteDTO GetQuote(DateTime tm, int timefm)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                return _context.MarketQuote.FirstOrDefault(x => x.Date == tm && x.TimeFrame == timefm);
            }
        }
        public OrderDTO SaveOrder(OrderDTO order)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                _context.Order.Add(order);
                _context.SaveChanges();
                return order;
            }
        }

        public OrderDTO UpdateOrder(OrderDTO order)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                var ord = _context.Order.Find(order.Id);
                ord.Symbol = order.Symbol;
                ord.TimeStamp = order.TimeStamp;
                ord.Quantity = order.Quantity;
                ord.Price = order.Price;
                ord.TradeId = order.TradeId;
                _context.Update(ord);
                _context.SaveChanges();
                return ord;
            }
        }

        public OrderDTO GetOrder(int id)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                return _context.Order.Find(id);
            }
        }

        public List<OrderDTO> GetOrders()
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                return _context.Order.ToList<OrderDTO>();
            }
        }

        public List<OrderDTO> GetOrders(DateTime from, DateTime to, string? symbol)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                if (symbol == null)
                    return _context.Order.Where(x => x.TimeStamp >= from && x.TimeStamp <= to).ToList();
                else
                    return _context.Order.Where(x => x.TimeStamp >= from && x.TimeStamp <= to && x.Symbol == symbol).ToList();
            }
        }

        public void DeleteOrders(DateTime from, DateTime to, string? symbol)
        {
            using (var _context = _dbContextFactory.CreateDbContext())
            {
                _context.Order.RemoveRange(_context.Order.Where(x => x.TimeStamp >= from && x.TimeStamp <= to && x.Symbol == symbol));
                _context.SaveChanges();
            }
        }
    }
}

