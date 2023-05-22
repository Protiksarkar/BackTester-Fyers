using AutoMapper;
using FyersAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skender.Stock.Indicators;
using System;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using TestingConsole.DBServices;
using TestingConsole.DTOs;
using TestingConsole.Models;
using TestingConsole.Other;
using TestingConsole.Services;
using TestingConsole.Strategy;

namespace TestingConsole
{
    class Program
    {
        private static Fyers fyers;
        public static User user = new User
        {
            Key = "5Q8ZFS3URL-100",
            Secret = "B3SYXZI6UT",
            RedirectUrl = @"https://myapi.fyers.in",
        };
        private static ServiceProvider serviceProvider;
        private static BackTestService dataService;
        private static TradingService tradingService;
        private static IMapper mapper;
        private static int interval;

        static void Main(string[] args)
        {
            //setup our DI
            serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<Fyers>(x => new Fyers(user.Key))
                .AddDbContextFactory<TradingDBContext>()
                .AddSingleton<TradingDBContext>()
                .AddScoped<TradingService>()
                .AddScoped<TradingRepo>()
                .AddScoped<BackTestService>()
                .AddScoped<Strategy2>()
                .AddSingleton<IMapper>(new MapperConfiguration(mc => mc.AddProfile(new MappingProfile())).CreateMapper())
                .BuildServiceProvider();

            mapper = serviceProvider.GetService<IMapper>();
            dataService = serviceProvider.GetRequiredService<BackTestService>();
            tradingService = serviceProvider.GetRequiredService<TradingService>();

            RunConsole();

        }

        private static void RunConsole()
        {
            string command = string.Empty;
            while (!command.Equals("EXIT", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine(">>");
                command = Console.ReadLine().Trim().ToUpper();
                try
                {
                    Interprete(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void Interprete(string command)
        {

            int wordIndex = 0;
            List<string> instructions = command.Split(" -").Select(x => x.Trim()).ToList();
            var paramName = instructions[wordIndex];
            wordIndex++;
            Dictionary<string, string> keyVals = Utility.GetParmas(instructions);
            switch (paramName)
            {
                case "LOGIN":
                    TradingService.LoginAsync(user, serviceProvider.GetRequiredService<Fyers>())?.Wait();
                    break;

                case "UPDATEINSTDB":
                    dataService.SaveInstumentsFromExcel();
                    break;

                case "UPDATEQUOTES":
                    var symbols = keyVals.GetValueOrDefault("symbols").Split(',').Select(x => x.Trim()).ToList();
                    var from = DateTime.Parse(keyVals.GetValueOrDefault("from"));
                    var to = DateTime.Parse(keyVals.GetValueOrDefault("to") ?? DateTime.Now.ToString());
                    var rsl = keyVals.GetValueOrDefault("resolution") ?? "1";

                    tradingService.saveQuotes(symbols, from, to, rsl);
                    break;

                case "TEST":
                    var symbol = keyVals.GetValueOrDefault("symbol");
                    from = DateTime.Parse(keyVals.GetValueOrDefault("from"));
                    to = DateTime.Parse(keyVals.GetValueOrDefault("to") ?? DateTime.Now.ToString());
                    interval = keyVals.GetValueOrDefault("interval") != null ? Convert.ToInt32(keyVals.GetValueOrDefault("interval")) : 120;
                    float capital = Convert.ToSingle(keyVals.GetValueOrDefault("capital") ?? "200000");
                    
                    var strategy1 = serviceProvider.GetRequiredService<Strategy2>();
                    var inst = dataService.GetInstrument(symbol); 
                    List<MarketQuoteDTO> quotes = dataService.GetQuotes(symbol, from, to, 60);

                    strategy1.OnRun(quotes, interval, inst, capital);
                    break;

                default:
                    Console.WriteLine("Invalid Command {0}", instructions[0]);

                    break;

            }
        }

    }
}