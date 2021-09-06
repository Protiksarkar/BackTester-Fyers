/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyersAPI
{
    public class Symbol
    {

        public static bool TryParse(string line, out Symbol symbol)
        {
            symbol = null;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            string[] array = line.Split(',');
            if (array.Length < 13)
                return false;

            Symbol tmp = new Symbol();

            if (string.IsNullOrWhiteSpace(array[0]))
                return false;

            tmp.Fytoken = array[0].Trim();
            tmp.SymbolDetails = array[1];
            if (!int.TryParse(array[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int instrumentTypes))
                return false;

            tmp.InstrumentType = instrumentTypes;

            if (!int.TryParse(array[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int lotSize))
                return false;

            tmp.LotSize = lotSize;

            if (!double.TryParse(array[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double tickSize))
                return false;

            tmp.TickSize = tickSize;
            tmp.ISIN = array[5];
            tmp.TradingSession = array[6];
            
            if (DateTime.TryParseExact(array[7], new string[] { "yyyy-MM-dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastUpdateDate))
                tmp.LastUpdateDate = lastUpdateDate;

            if (long.TryParse(array[8], NumberStyles.Any, CultureInfo.InvariantCulture, out long expiry))
                tmp.ExpiryDate = new DateTime(1970, 1, 1).AddSeconds(expiry);

            if (string.IsNullOrWhiteSpace(array[9]))
                return false;

            tmp.SymbolTicker = array[9].Trim();

            if (!int.TryParse(array[10], NumberStyles.Integer, CultureInfo.InvariantCulture, out int exchange))
                return false;

            tmp.Exchange = exchange;

            if (!int.TryParse(array[11], NumberStyles.Integer, CultureInfo.InvariantCulture, out int segment))
                return false;

            tmp.Segment = segment;

            if (!int.TryParse(array[12], NumberStyles.Integer, CultureInfo.InvariantCulture, out int token))
                return false;

            tmp.ScripCode = token;

            symbol = tmp;

            return true;
        }


        /// <summary>
        /// Unique token for each symbol
        /// </summary>
        public string Fytoken { get; set; }
        /// <summary>
        /// Name of the symbol
        /// </summary>
        public string SymbolDetails	 { get; set; }
        /// <summary>
        /// <see cref="Exchanges"/>
        /// </summary>
        public int Exchange { get; set; }
        /// <summary>
        /// Exchange instrument type. <see cref="InstrumentTypes"/>
        /// </summary>
        public int InstrumentType { get; set; }
        /// <summary>
        /// Minimum qty multiplier
        /// </summary>
        public int LotSize { get; set; }
        /// <summary>
        /// Minimum price multiplier
        /// </summary>
        public double TickSize { get; set; }
        /// <summary>
        /// Unique ISIN provided by exchange for each symbol
        /// </summary>
        public string ISIN { get; set; }
        /// <summary>
        /// Trading session provided in IST
        /// </summary>
        public string TradingSession { get; set; }
        /// <summary>
        /// Date of last update
        /// </summary>
        public DateTime LastUpdateDate { get; set; }
        /// <summary>
        /// Date of expiry for a symbol.Applicable only for derivative contracts
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        /// <summary>
        /// Unique string to identify the symbol
        /// </summary>
        public string SymbolTicker { get; set; }
        /// <summary>
        /// Segment of the symbol. 
        /// </summary>
        public int Segment { get; set; }
        /// <summary>
        /// Token of the Exchange
        /// </summary>
        public int ScripCode { get; set; }

    }
}
