using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace BackTestDemo.DTOs
{
    public class MarketQuoteDTO
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Required]
        public string Exchange { get; set; }
        [Required]
        public double Open { get; set; }
        [Required]
        public double High { get; set; }
        [Required]
        public double Low { get; set; }
        [Required]
        public double Close { get; set; }
        public long? Volume { get; set; }
        //in seconds
        [Required]
        public int TimeFrame { get; set; }
    }
}
