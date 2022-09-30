using BackTestDemo.DTOs;

namespace BackTestDemo
{
    public class Utility
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
                data.TimeStamp = quotes.First().TimeStamp;
            }
            return data;
        }

        internal static int GetQuantity(int lotSize, float slPoint, float maxRiskAmt = 2000)
        {
            int quantity = 0;
            quantity = ((int)Math.Floor(maxRiskAmt / Math.Abs(slPoint)));
            quantity = quantity - (quantity % lotSize);

            return quantity;
        }

        internal static bool IsHighBreak(float LTP, float prevHigh)
        {
            return LTP > prevHigh;
        }

        internal static bool IsLowBreak(float LTP, float prevLow)
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
    }
}
