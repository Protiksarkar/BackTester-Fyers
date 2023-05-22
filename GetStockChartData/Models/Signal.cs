using System.ComponentModel.DataAnnotations;

namespace GetStockChartData.Models
{
    public class Signal
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public int TradeId { get; set; }
    }
}
