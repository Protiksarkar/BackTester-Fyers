using GetStockChartData.Models;
using Microsoft.AspNetCore.Mvc;
using TestingConsole;
using TestingConsole.DTOs;

namespace GetStockChartData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController : ControllerBase
    {
        private CommonService commonSrv;

        public QuoteController(CommonService cmnSrv)
        {
            commonSrv = cmnSrv;
        }
        [HttpGet]
        public List<OHLC> Get()
        {
            //var fromDate = Utility.ParseDateTime(from);
            //var toDate = Utility.ParseDateTime(to);
            return commonSrv.GetQuotes();
        }
    }
}