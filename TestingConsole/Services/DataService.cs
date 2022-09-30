using TestingConsole.DBServices;
using TestingConsole.DTOs;
using FyersAPI;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TestingConsole.Services
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
        public void saveQuotes(List<string> symbols, DateTime from, DateTime to, string resolution)
        {
            List<Task> tasks = new List<Task>();
            foreach (var symbol in symbols)
            {
                if (string.IsNullOrEmpty(symbol))
                    return;

                DateTime toDate = from;
                bool flag = true;

                while (flag)
                {
                    toDate = toDate.AddDays(90);
                    HistoryResponse result;
                    if (toDate < to)
                    {
                        result = fyers.GetHistoricalDataAsync(symbol, resolution, toDate.AddDays(-90), toDate).Result;
                    }
                    else
                    {
                        result = fyers.GetHistoricalDataAsync(symbol, resolution, toDate.AddDays(-90), to).Result;
                        flag = false;
                    }

                    var data = result.candles;
                    List<MarketQuoteDTO> marketQuotes = data.Select(x =>
                    {
                        return new MarketQuoteDTO()
                        {
                            Symbol = symbol,
                            Date = 
                            TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(x[0])).DateTime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                            Open = Convert.ToDecimal(x[1]),
                            High = Convert.ToDecimal(x[2]),
                            Low = Convert.ToDecimal(x[3]),
                            Close = Convert.ToDecimal(x[4]),
                            Volume = Convert.ToDecimal(x[5]),
                            TimeFrame = Utility.ResolutionToTF(resolution)
                            
                    };
                    }).ToList();

                    tasks.Add(repo.UpdateQuotesAsync(marketQuotes));
                }
            }
            Task.WaitAll(tasks.ToArray());
        }
        public List<MarketQuoteDTO> GetQuotes(string symbol, DateTime from, DateTime to, int tmFrame)
        {
            return repo.GetQuotes(symbol, from, to, tmFrame);
        }

        public void SaveInstumentsFromExcel()
        {
            List<string> filePaths = new List<string>();
            filePaths.Add(@"C:\Users\Protik.Sarkar\source\repos\FyersAPI\TestingConsole\Instruments\NSE_CM.csv");
            filePaths.Add(@"C:\Users\Protik.Sarkar\source\repos\FyersAPI\TestingConsole\Instruments\NSE_FO.csv");
            
            foreach(var file in filePaths)
            {
                using (var reader = new StreamReader(file))
                {
                    List<InstrumentDTO> instruments = new List<InstrumentDTO>();
                    InstrumentDTO instr = null;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',').Select(x => x.Trim()).ToArray();

                        instr = new InstrumentDTO
                        {
                            Fytoken = values[0],
                            Symbol = values[1],
                            SymbolTicker = values[9],
                            Exchange = (Exchanges)Convert.ToInt32(values[10]),
                            Segment = (Segments)Convert.ToInt32(values[11]),
                            LotSize = Convert.ToInt32(values[3]),
                            Expiry = !string.IsNullOrEmpty(values[8]) ? DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(values[8])).DateTime : null,
                            Strike = Convert.ToSingle(values[15]),
                            OptionType = values[16],
                        };
                        instruments.Add(instr);
                        if (instruments.Count == 10000)
                        {
                            repo.UpdateInstuments(instruments);
                            instruments = new List<InstrumentDTO>();
                        }
                        else if (reader.EndOfStream)
                        {
                            repo.UpdateInstuments(instruments);
                        }
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
            if (string.IsNullOrEmpty(order.TradeId))
            {
                var ord = repo.SaveOrder(order);
                return UpdateOrder(ord);
            }
            else
                return repo.SaveOrder(order);
        }
        public OrderDTO UpdateOrder(OrderDTO order)
        {
            order.TradeId = order.Id.ToString();
            return repo.UpdateOrder(order);
        }

        public void GetOrder(int id)
        {
            repo.GetOrder(id);
        }
        public List<OrderDTO> GetOrders(List<int> ids)
        {
            return ids.Select(x => repo.GetOrder(x)).ToList();
        }

        private OrderDTO PlaceOrder(string symbol, MarketQuoteDTO quote, int quantity)
        {
            OrderDTO order = new OrderDTO
            {
                Symbol = symbol,
                TimeStamp = quote.Date,
                Quantity = quantity,
                Price = Convert.ToSingle(quote.Close),
            };
            return SaveOrder(order);
        }

        private bool ExitPosition(DateTime date, decimal price, OrderDTO placedOrder)
        {
            bool result = false;
            if (placedOrder != null)
            {
                OrderDTO order = new OrderDTO();
                order.Symbol = placedOrder.Symbol;
                order.Id = 0;
                order.TimeStamp = date.Date;
                order.Quantity = placedOrder.Quantity * -1;
                order.Price = Convert.ToSingle(price);
                order.TradeId = placedOrder.Id.ToString();
                repo.SaveOrder(order);
                result = true;
            }
            return result;
        }
    }
}
