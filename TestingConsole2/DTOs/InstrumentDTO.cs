using System.ComponentModel.DataAnnotations;
using BackTestDemo;

namespace BackTestDemo.DTOs
{
    public class InstrumentDTO
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Fytoken { get; set; }
        [Required]
        public string SymbolName { get; set; }
        [Required]
        public string SymbolTicker { get; set; }
        public Exchanges Exchange { get; set; }
        public Segments Segment { get; set; }
        public int LotSize { get; set; }
        public DateTime? Expiry { get; set; }
        public double? Strike { get; set; }
        public string? OptionType { get; set; }
        
    }
}
