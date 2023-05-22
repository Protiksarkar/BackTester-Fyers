using System;
using System.ComponentModel.DataAnnotations;

namespace TestingConsole.DTOs
{
    public class OrderDTO
    {
        [Key]
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Quantity  { get; set; }
        public float Price { get; set; }
        [MaxLength(100)]
        public string? TradeId { get; set; }

        public float? Value1 { get; set; }
        public float? Value2 { get; set; }
        public float? Value3 { get; set; }
        public float? Value4 { get; set; }
        public float? Value5 { get; set; }
        public float? Value6 { get; set; }

    }
}
