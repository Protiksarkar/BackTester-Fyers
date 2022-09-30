using BackTestDemo.DBServices;
using BackTestDemo.DTOs;
using FyersAPI;
using Microsoft.EntityFrameworkCore;

namespace BackTestDemo.Services
{
    public class DataService
    {
        private TradingRepo repo;
        private Fyers fyers;

        public DataService(TradingRepo tdRepo,Fyers fy)
        {
            repo = tdRepo;
            fyers = fy;
        }
        public void saveQuotes(string symbol,DateTime from, DateTime to, string resolution)
        {
            var data = fyers.GetHistoricalDataAsync(symbol,resolution,from,to).Result.candles;
            List<MarketQuoteDTO> marketQuotes = data.Select(x =>
            {
                return new MarketQuoteDTO()
                {
                    TimeStamp = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(x[0])).DateTime,
                    Open = x[1],
                    High = x[2],
                    Low = x[3],
                    Close = x[4],
                    Volume = Convert.ToInt64(x[5])
                };
            }).ToList();

            repo.UpdateQuotes(marketQuotes);

        }
        public List<MarketQuoteDTO> GetQuotes(string symbol, DateTime from, DateTime to, int tmFrame)
        {
            return repo.GetQuotes(symbol, from, to, tmFrame);
        }

        public void SaveInstumentsFromExcel(List<InstrumentDTO> instrument)
        {

            using (var reader = new StreamReader(@"C:\Project\Practice\FyersAlgoTrader\FyersAlgoTrader\NSE_CM_FYERS.csv"))
            {
                List<InstrumentDTO> instruments = new List<InstrumentDTO>();
                InstrumentDTO instr =null;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    instr = new InstrumentDTO
                    {
                        Fytoken = values[0],
                        SymbolName = values[1],
                        SymbolTicker = values[9],
                        Exchange = (Exchanges)Convert.ToInt32(values[10]),
                        Segment = (Segments)Convert.ToInt32(values[11]),
                        LotSize = Convert.ToInt32(values[3]),
                        Expiry = Convert.ToDateTime(values[8]),
                        Strike = Convert.ToDouble(values[14]),
                        OptionType = values[15],
                    };
                    instruments.Add(instr);
                    if(instruments.Count==10000)
                    {
                        repo.UpdateInstuments(instrument);
                        instruments = new List<InstrumentDTO>();
                    }
                }
            }
        }

        public InstrumentDTO GetInstrument(string symbol)
        {
            return repo.GetInstrument(symbol);
        }

        public OrderDTO SaveOrder(OrderDTO order)
        {
            return repo.SaveOrder(order);
        }

        public void GetOrder(int id)
        {
            repo.GetOrder(id);
        }
        public List<OrderDTO> GetOrders(List<int> ids)
        {
            return ids.Select(x => repo.GetOrder(x)).ToList();
        }
    }
}
