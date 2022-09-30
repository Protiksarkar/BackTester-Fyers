using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TestingConsole.DTOs;

namespace TestingConsole
{
    public static class Utility
    {

        internal static MarketQuoteDTO SampleData(List<MarketQuoteDTO> quotes)
        {
            var data = new MarketQuoteDTO();
            if (quotes.Count > 0)
            {
                data.Open = quotes.First().Open;
                data.Close = quotes.Last().Close;
                data.High = quotes.Max(x => x.High);
                data.Low = quotes.Min(x => x.Low);
                data.Date = quotes.First().Date;
            }
            return data;
        }

        internal static int GetQuantity(int lotSize, float slPoint, float maxRiskAmt = 2000)
        {
            if (lotSize == 0)
                lotSize = 1;
            int quantity = 0;
            quantity = ((int)Math.Floor(maxRiskAmt / Math.Abs(slPoint*lotSize)));
            quantity = quantity - (quantity % lotSize);

            return quantity;
        }

        internal static bool IsHighBreak(decimal LTP, decimal prevHigh)
        {
            return LTP > prevHigh;
        }

        internal static bool IsLowBreak(decimal LTP, decimal prevLow)
        {
            return LTP < prevLow;
        }

        internal static string TfToResolution(int tmFrame)
        {
            switch (tmFrame)
            {
                case 1 * 60: return "1";
                case 2 * 60: return "2";
                case 3 * 60: return "3";
                case 5 * 60: return "5";
                case 10 * 60: return "10";
                case 15 * 60: return "15";
                case 30 * 60: return "30";
                case 60 * 60: return "60";
                case 120 * 60: return "120";
                case 24 * 60 * 60: return "D";
                default: return null;
            }
        }

        internal static int ResolutionToTF(string resol)
        {
            switch (resol)
            {
                case "1": return 1 * 60;
                case "2": return 2 * 60;
                case "3": return 3 * 60;
                case "5": return 5 * 60;
                case "10": return 10 * 60;
                case "15": return 15 * 60;
                case "30": return 30 * 60;
                case "60": return 60 * 60;
                case "120": return 120 * 60;
                case "D": return 24 * 60 * 60;
                default: return 0;
            }
        }

        public static Dictionary<string, string> GetParmas(List<string> instructions)
        {
            Dictionary<string, string> keyVals = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            List<string> tempVal;
            for (int i = 1; i < instructions.Count; i++)
            {
                tempVal = instructions[i].Split('=').Select(x => x.Trim()).ToList();
                keyVals.Add(tempVal[0], tempVal[1]);
            }
            return keyVals;
        }

        public static Quote GetPrevQuote(ref List<Quote> quotes, DateTime timeStamp, int position, int interval)
        {
            return quotes.FirstOrDefault(x =>
            {
                var time = timeStamp.AddSeconds(position * interval);
                if (x.Date <= time)
                {
                    if (x.Date > time.AddSeconds(-1 * interval))
                        return true;
                }
                return false;
            });
        }

        public static T GetPrevValue<T>(ref List<T> resultCollection, DateTime timeStamp, int position, int interval) where T : IResult
        {
            return resultCollection.FirstOrDefault(x =>
            {
                var time = timeStamp.AddSeconds(position * interval);
                if (x.Date <= time)
                {
                    if (x.Date > time.AddSeconds(-1 * interval))
                        return true;
                }
                return false;
            });
        }

        public static int GetPrevQuoteIndex(ref List<Quote> quotes, DateTime timeStamp, int position, int interval)
        {
            return quotes.FindIndex(x =>
            {
                var time = timeStamp.AddSeconds(position * interval);
                if (x.Date <= time)
                {
                    if (x.Date > time.AddSeconds(-1 * interval))
                        return true;
                }
                return false;
            });
        }

        //static MarketQuoteDTO GetLastCandle(ref List<MarketQuoteDTO> candles, DateTime time, int tmFrame)
        //{
        //    candles.Where(time)
        //}
    }
}
