using System.ComponentModel.DataAnnotations;

namespace BackTestDemo.DTOs
{
    public class OrderDTO
    {
        [Key]
        public int Id { get; set; }
        public DateTime Entry { get; set; }
        public decimal Amount { get; set; }
        public string Symbol { get; set; }
        public int Quantity  { get; set; }
        public double Price { get; set; }
    }
}
