using Skender.Stock.Indicators;
using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace TestingConsole.DTOs
{
    public class MarketQuoteDTO : Quote
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Symbol { get; set; }
        //in seconds
        [Required]
        public int TimeFrame { get; set; }
    }
}
