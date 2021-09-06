/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FyersAPI
{
    [DataContract]
    public class HoldingResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the holdings
        /// </summary>
        [DataMember(Name = "holdings")]
        public List<Holding> holdings { get; set; }
        /// <summary>
        /// Overall holdings
        /// </summary>
        [DataMember(Name = "overall")]
        public OverallHolding overall { get; set; }
        
    }

    [DataContract]
    public class OverallHolding
    {
        /// <summary>
        /// Total count
        /// </summary>
        [DataMember(Name = "count_total")]
        public int count_total { get; set; }
        /// <summary>
        /// Total investment
        /// </summary>
        [DataMember(Name = "total_investment")]
        public double total_investment { get; set; }
        /// <summary>
        /// Current value
        /// </summary>
        [DataMember(Name = "total_current_value")]
        public double total_current_value { get; set; }
        /// <summary>
        /// Total profit n loss
        /// </summary>
        [DataMember(Name = "total_pl")]
        public double total_pl { get; set; }
        /// <summary>
        /// Profit n loss percentage
        /// </summary>
        [DataMember(Name = "pnl_perc")]
        public double pnl_perc { get; set; }
    }

    [DataContract]
    public class Holding
    {
        /// <summary>
        /// Identify the type of holding. <see cref="HoldingType"/>
        /// </summary>
        [DataMember(Name = "holdingType")]
        public string holdingType { get; set; }
        /// <summary>
        /// The quantity of the symbol which the user has at the beginning of the day
        /// </summary>
        [DataMember(Name = "quantity")]
        public int quantity { get; set; }
        /// <summary>
        /// The original buy price of the holding
        /// </summary>
        [DataMember(Name = "costPrice")]
        public double costPrice { get; set; }
        /// <summary>
        /// The Market value of the current holding
        /// </summary>
        [DataMember(Name = "marketVal")]
        public double marketVal { get; set; }
        /// <summary>
        /// This reflects the quantity - the quantity sold during the day
        /// </summary>
        [DataMember(Name = "remainingQuantity")]
        public int remainingQuantity { get; set; }
        [DataMember(Name = "pl")]
        public double pl { get; set; }
        /// <summary>
        /// LTP is the price from which the next sale of the stocks happens
        /// </summary>
        [DataMember(Name = "ltp")]
        public double ltp { get; set; }
        [DataMember(Name = "id")]
        public int id { get; set; }
        /// <summary>
        /// Fytoken is a unique identifier for every symbol.
        /// </summary>
        [DataMember(Name = "fyToken")]
        public long fyToken { get; set; }
        /// <summary>
        /// The exchange in which order is placed.
        /// </summary>
        [DataMember(Name = "exchange")]
        public int exchange { get; set; }
        /// <summary>
        /// Gets or sets the symbol. Eg: NSE:RCOM-EQ
        /// </summary>
        [DataMember(Name = "symbol")]
        public string symbol { get; set; }

    }
}
