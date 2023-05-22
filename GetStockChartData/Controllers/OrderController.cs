using GetStockChartData.Models;
using Microsoft.AspNetCore.Mvc;
using TestingConsole.DTOs;

namespace GetStockChartData.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private CommonService commonSrv;

        public OrderController(CommonService cmnSrv)
        {
            commonSrv = cmnSrv;
        }
        [HttpGet]
        public List<Signal> Get()
        {
            return commonSrv.GetOrders();
        }
    }
}