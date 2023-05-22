using AutoMapper;
using GetStockChartData.Models;
using Newtonsoft.Json.Linq;
using Skender.Stock.Indicators;
using TestingConsole.DTOs;

namespace GetStockChartData
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<MarketQuoteDTO, OHLC>()
                .ForMember(x=>x.Time,act=> act.MapFrom(src => (src.Date.Ticks - 621355968000000000) / 10000000));
            CreateMap<OrderDTO, Signal>()
                .ForMember(x => x.Time, act => act.MapFrom(src => (src.TimeStamp.Ticks - 621355968000000000) / 10000000));
            CreateMap<Quote,OHLC>()  
                .ForMember(x => x.Time, act => act.MapFrom(src => (src.Date.Ticks - 621355968000000000) / 10000000));
        }
    }
}
