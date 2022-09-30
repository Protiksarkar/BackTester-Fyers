using AutoMapper;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingConsole.DTOs;

namespace TestingConsole.Other
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<MarketQuoteDTO, Quote>();
        }
    }
}
