using BackTestDemo.DTOs;

namespace BackTestDemo.DBServices
{
    public class TradingRepo
    {
        private TradingDBContext _context;

        public TradingRepo(TradingDBContext context)
        {
            _context = context;
        }
        public void UpdateInstuments(List<InstrumentDTO> instruments)
        {

            foreach (var inst in instruments)
            {
                if (_context.Instrument.Any(x => x.Fytoken == inst.Fytoken))
                {
                    _context.Instrument.Update(inst);
                }
                else
                {
                    _context.Instrument.Add(inst);
                }
            }
            _context.SaveChanges();
        }

        public InstrumentDTO GetInstrument(string symbolTicker, int exchange)
        {
            return _context.Instrument.First(x => x.SymbolTicker == symbolTicker && (int)x.Exchange == exchange);
        }

        public InstrumentDTO GetInstrument(string fyToken)
        {
            return _context.Instrument.First(x => x.Fytoken == fyToken);
        }

        public void UpdateQuotes(List<MarketQuoteDTO> marketQuotes)
        {
            foreach (var quote in marketQuotes)
            {
                if (_context.MarketQuote.Any(x => x.TimeStamp == quote.TimeStamp && x.TimeFrame == quote.TimeFrame && x.Symbol == quote.Symbol && x.Exchange == quote.Exchange))
                {
                    _context.MarketQuote.Update(quote);
                }
                else
                {
                    _context.MarketQuote.Add(quote);
                }
            }
            _context.SaveChanges(true);
        }

        public void AddQuotes(List<MarketQuoteDTO> marketQuotes)
        {
            foreach (var quote in marketQuotes)
            {
                if (!_context.MarketQuote.Any(x => x.TimeStamp == quote.TimeStamp && x.TimeFrame == quote.TimeFrame && x.Symbol == quote.Symbol && x.Exchange == quote.Exchange))
                {
                    _context.MarketQuote.Add(quote);
                }
            }
            _context.SaveChanges(true);
        }

        public List<MarketQuoteDTO> GetQuotes(string symbol, DateTime from, DateTime to, int timefm)
        {
            return _context.MarketQuote.Where(x => x.Symbol==symbol && x.TimeStamp >= from && x.TimeStamp <= to && x.TimeFrame == timefm).ToList<MarketQuoteDTO>();
        }

        public MarketQuoteDTO GetQuote(DateTime tm, int timefm)
        {
            return _context.MarketQuote.FirstOrDefault(x => x.TimeStamp == tm && x.TimeFrame == timefm);
        }

        public OrderDTO SaveOrder(OrderDTO order)
        {
            _context.Order.Add(order);
            _context.SaveChanges();
            return order;
        }

        public OrderDTO GetOrder(int id)
        {
            return _context.Order.Find(id);
        }

        public List<OrderDTO> GetOrders(DateTime from, DateTime to, string? symbol)
        {
            if (symbol == null)
                return _context.Order.Where(x => x.Entry >= from && x.Entry <= to).ToList();
            else
                return _context.Order.Where(x => x.Entry >= from && x.Entry <= to && x.Symbol == symbol).ToList();
        }

        public void DeleteOrders(DateTime from, DateTime to, string? symbol)
        {
            _context.Order.RemoveRange(_context.Order.Where(x => x.Entry >= from && x.Entry <= to && x.Symbol == symbol));
            _context.SaveChanges();
        }
    }
}
