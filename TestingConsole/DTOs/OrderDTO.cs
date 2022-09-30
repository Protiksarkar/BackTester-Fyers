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
    }
}
