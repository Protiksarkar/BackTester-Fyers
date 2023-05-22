using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestingConsole;

namespace TestingConsole.DTOs
{
    public class InstrumentDTO
    {
        public string Symbol { get; set; }
        [Key]
        public string SymbolTicker { get; set; }
        public Exchanges Exchange { get; set; }
        public Segments Segment { get; set; }
        public int LotSize { get; set; }
        public DateTime? Expiry { get; set; }
        public float? Strike { get; set; }
        public string? OptionType { get; set; }

    }
}
