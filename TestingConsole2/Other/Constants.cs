/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestDemo
{
    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#exchanges">Exchanges</a>
    /// </summary>
    public enum Exchanges
    {
        NSE = 10,
        MCX = 11,
        BSE = 12
    }

    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#segments">Segments</a>
    /// </summary>
    public enum Segments
    {
        CapitalMarket = 10,
        EquityDerivative = 11,
        CurrencyDerivative = 12,
        CommodityDerivative = 13
    }

    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#instrument-types">Instrument types</a>
    /// </summary>
    public enum InstrumentTypes
    {
        EQ = 0,
        PREFSHARES = 1,
        DEBENTURES = 2,
        WARRANTS = 3,
        MISC = 4,

        INDEX = 10,

        FUTIDX = 11,
        FUTIVX = 12,
        FUTSTK = 13,
        OPTIDX = 14,
        OPTSTK = 15,

        FUTCUR = 16,
        FUTIRT = 17,
        FUTIRC = 18,
        OPTCUR = 19,
        UNDCUR = 20,
        UNDIRC = 21,
        UNDIRT = 22,
        UNDIRD = 23,
        INDEX_CD = 24,
        FUTIRD = 25,

        FUTCOM = 30,
        OPTFUT = 31
    }





    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#product-types">Product types</a>
    /// </summary>
    public static class ProductTypes
    {
        public const string CNC = "CNC";
        public const string INTRADAY = "INTRADAY";
        public const string MARGIN = "MARGIN";
        public const string CO = "CO";
        public const string BO = "BO";
    }

    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#order-types">Order types</a>
    /// </summary>
    public enum OrderType
    {
        Limit = 1,
        Market = 2,
        Stop = 3,
        StopLimit = 4
    }

    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#order-status">Order status</a>
    /// </summary>
    public enum OrderStatus
    {
        Cancelled = 1,
        Traded = 2, //filled
        Reserved = 3,
        Transit = 4,
        Rejected = 5,
        Pending = 6
    }


    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#order-sides">Order side</a>
    /// </summary>
    public static class OrderSide
    {
        public const int Buy = 1;
        public const int Sell = -1;
    }

    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#position-sides">Position sides</a>
    /// </summary>
    public static class Position
    {
        public const int Long = 1;
        public const int Short = -1;
        public const int Flat = 0;
    }

    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#holding-types">Holding types</a>
    /// </summary>
    public static class HoldingType
    {
        public const string T1 = "T1";
        public const string HLD = "HLD";
    }

    /// <summary>
    /// <a href="https://api-docs.fyers.in/v2/#order-sources">Order source</a>
    /// </summary>
    public static class OrderSource
    {
        public const string Mobile = "M";
        public const string Web = "W";
        public const string FyersOne = "R";
        public const string Admin = "A";
        public const string API = "ITS";
    }

    /// <summary>
    /// Order validity
    /// </summary>
    public static class OrderValidity
    {
        /// <summary>
        /// Valid till the end of the day
        /// </summary>
        public const string DAY = "DAY";
        /// <summary>
        /// Immediate or Cancel
        /// </summary>
        public const string IOC = "IOC";
    }

    public enum StrategyLifecycle
    {
        OPEN,
        OPEN_ORDER_PLACED,
        POSITION_OPENED,
        CLOSE_ORDER_PLACED,
        POSITION_CLOSED,
        CLOSED,
    }
    public enum StrategyState
    {
        OPEN,
        RUNNING,
        CLOSED,
    }
    public enum StrategyName
    {
        CANDLE_REVERSAL,
        TAKE_ENTRY,
        ROBOT_CANDLE_REVERSAL
    }

}
