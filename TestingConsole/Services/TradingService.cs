using AutoMapper;
using FyersAPI;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TestingConsole.DBServices;
using TestingConsole.DTOs;
using TestingConsole.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TestingConsole.Services
{
    public class TradingService
    {
        private TradingRepo repo;
        private Fyers fyers;
        private IMapper mapper;

        public TradingService(TradingRepo tdRepo, Fyers fyrs, IMapper mpr)
        {
            repo = tdRepo;
            fyers = fyrs;
            mapper = mpr;
        }

        public string PlaceOrder()
        {
            var response = fyers.PlaceOrderAsync(new PlaceOrderPayload()
            {
                disclosedQty = 0,
                limitPrice = 100,
                offlineOrder = "False",
                productType = ProductTypes.MARGIN,
                qty = 1,
                takeProfit = 0.0m,
                side = OrderSide.Buy,
                stopLoss = 0.0m,
                stopPrice = 0.0m,
                symbol = "NSE:ONGC-EQ",
                type = (int)OrderType.Limit,
                validity = OrderValidity.DAY
            }).Result;

            if (response != null && !string.IsNullOrEmpty(response.id))
                return response.id;
            else
                return null;
        }

        public static async Task LoginAsync(User user, Fyers fyers)
        {
            string state = Guid.NewGuid().ToString();
            var uri = $"https://api.fyers.in/api/v2/generate-authcode?client_id={user.Key}&redirect_uri={user.RedirectUrl}&response_type=code&state={state}";

            Console.WriteLine("Go to Browser and login to {0}\nPaste new url after login", uri);
            uri = Console.ReadLine().Trim();

            if (user == null)
            {
                Console.WriteLine("Login failed. Please make sure you have entered a valid API Key and API Secret");
                return;
            }

            var resp = Fyers.IsValidLogin(uri, user.Key, user.Secret, out TokenPayload payload);

            if (fyers != null && resp)
            {
                var response = await fyers.GenerateTokenAsync(payload);

                if (response != null && !string.IsNullOrEmpty(response.access_token))
                {
                    Console.WriteLine("Successfully logged in");
                }
                else
                {
                    Console.WriteLine("Login failed. Please make sure you have entered a valid API Key and API Secret");
                }
            }
            else
            {
                Console.WriteLine("Login failed. Please make sure you have entered a valid API Key and API Secret");
            }
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
                    
                    if(!result.s.Equals("ok",StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception(result.message);
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


    }
}

