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
    public class BackTestService
    {
        private TradingRepo repo;

        public BackTestService(TradingRepo tdRepo)
        {
            repo = tdRepo;
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

        public OrderDTO PlaceEntryOrder(string symbol, DateTime time,decimal price, int quantity,
            float? value1 = null, float? value2 = null, float? value3 = null, float? value4 = null, float? value5 = null, float? value6 = null)
        {
            OrderDTO order = new OrderDTO
            {
                Symbol = symbol,
                TimeStamp = time,
                Quantity = quantity,
                Price = Convert.ToSingle(price),
                Value1 = value1,
                Value2 = value2,
                Value3 = value3,
                Value4 = value4,
                Value5 = value5,
                Value6 = value6,
            };

            if (string.IsNullOrEmpty(order.TradeId))
            {
                var ord = repo.SaveOrder(order);
                return UpdateOrder(ord);
            }
            else
                return repo.SaveOrder(order);
        }

        public bool PlaceExitOrder(OrderDTO placedOrder, DateTime date, decimal price)
        {
            bool result = false;
            if (placedOrder != null)
            {
                OrderDTO order = new OrderDTO();
                order.Symbol = placedOrder.Symbol;
                order.Id = 0;
                order.TimeStamp = date;
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
