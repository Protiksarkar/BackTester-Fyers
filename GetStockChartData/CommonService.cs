using AutoMapper;
using GetStockChartData.Models;
using Skender.Stock.Indicators;
using TestingConsole;
using TestingConsole.DBServices;
using TestingConsole.DTOs;

namespace GetStockChartData
{
    public class CommonService
    {
        private TradingRepo repo;
        private IMapper mapper;

        public CommonService(TradingRepo rp, IMapper mpr)
        {
            repo = rp;
            mapper = mpr;
        }

        public List<OHLC> GetQuotes(int interval = 60*2)
        {
            var orders = repo.GetOrders().OrderBy(x => x.TimeStamp).ToList();
            var quotes = repo.GetQuotes(orders.First().Symbol, orders.Min(x => x.TimeStamp.Date), orders.Max(x => x.TimeStamp.Date.AddDays(1).AddSeconds(-1)), 60);
            if (quotes == null)
                return null;

            var cquotes = quotes.Aggregate(TimeSpan.FromSeconds(interval)).OrderBy(x=>x.Date); 
            var ohlcs = mapper.Map<List<OHLC>>(cquotes);

            return ohlcs;
        }

        public List<Signal> GetOrders()
        {
            var orders = repo.GetOrders().OrderBy(x => x.TimeStamp).ToList();

            return mapper.Map<List<Signal>>(orders);
        }
    }
}
