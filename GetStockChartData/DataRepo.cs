using Microsoft.EntityFrameworkCore;
using TestingConsole.DBServices;

namespace GetStockChartData
{
    public class DataRepo
    {
        private readonly IDbContextFactory<TradingDBContext> _dbContextFactory;

        public DataRepo(IDbContextFactory<TradingDBContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
    }
}
