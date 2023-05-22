using Skender.Stock.Indicators;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace TestingConsole.DTOs
{
    public class MarketQuoteDTO : IQuote
    {
        public string Symbol { get; set; }
        public int TimeFrame { get; set; }
        public DateTime Date { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volume { get; set; }
    }
}
