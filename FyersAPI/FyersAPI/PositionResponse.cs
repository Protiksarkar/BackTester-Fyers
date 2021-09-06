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
    public class PositionResponse : ResponseBase
    {
        [DataMember(Name = "netPositions")]
        public List<Netposition> netPositions { get; set; }
        [DataMember(Name = "overall")]
        public Overall overall { get; set; }

    }

    [DataContract]
    public class Overall
    {
        [DataMember(Name = "count_total")]
        public int count_total { get; set; }
        [DataMember(Name = "count_open")]
        public int count_open { get; set; }
        [DataMember(Name = "pl_total")]
        public double pl_total { get; set; }
        [DataMember(Name = "pl_realized")]
        public double pl_realized { get; set; }
        [DataMember(Name = "pl_unrealized")]
        public double pl_unrealized { get; set; }
    }

    [DataContract]
    public class Netposition
    {
        /// <summary>
        /// Gets or sets the net quantity
        /// </summary>
        [DataMember(Name = "netQty")]
        public int netQty { get; set; }
        /// <summary>
        /// Absolute value of net qty
        /// </summary>
        [DataMember(Name = "qty")]
        public int qty { get; set; }
        /// <summary>
        /// Gets or sets the average price
        /// </summary>
        [DataMember(Name = "avgPrice")]
        public double avgPrice { get; set; }
        /// <summary>
        /// Gets or sets the net average price
        /// </summary>
        [DataMember(Name = "netAvg")]
        public double netAvg { get; set; }
        /// <summary>
        /// The side shows whether the position is long / short
        /// </summary>
        [DataMember(Name = "side")]
        public int side { get; set; }
        /// <summary>
        /// The product type of the position
        /// </summary>
        [DataMember(Name = "productType")]
        public string productType { get; set; }
        /// <summary>
        /// The realized p&l of the position
        /// </summary>
        [DataMember(Name = "realized_profit")]
        public double realized_profit { get; set; }
        /// <summary>
        /// The unrealized p&l of the open position
        /// </summary>
        [DataMember(Name = "unrealized_profit")]
        public double unrealized_profit { get; set; }
        /// <summary>
        /// The total p&l of the position
        /// </summary>
        [DataMember(Name = "pl")]
        public double pl { get; set; }
        /// <summary>
        /// Gets or sets the last traded price
        /// </summary>
        [DataMember(Name = "ltp")]
        public double ltp { get; set; }
        /// <summary>
        /// Total buy qty
        /// </summary>
        [DataMember(Name = "buyQty")]
        public int buyQty { get; set; }
        /// <summary>
        /// Average buy price
        /// </summary>
        [DataMember(Name = "buyAvg")]
        public double buyAvg { get; set; }
        /// <summary>
        /// Gets or sets the buy value
        /// </summary>
        [DataMember(Name = "buyVal")]
        public double buyVal { get; set; }
        /// <summary>
        /// Total sell qty
        /// </summary>
        [DataMember(Name = "sellQty")]
        public int sellQty { get; set; }
        /// <summary>
        /// Average sell price
        /// </summary>
        [DataMember(Name = "sellAvg")]
        public double sellAvg { get; set; }
        /// <summary>
        /// Gets or sets the sell value
        /// </summary>
        [DataMember(Name = "sellVal")]
        public double sellVal { get; set; }
        /// <summary>
        /// This is used for sorting of positions
        /// </summary>
        [DataMember(Name = "slNo")]
        public int slNo { get; set; }
        /// <summary>
        /// Fytoken is a unique identifier for every symbol. <a href="https://api-docs.fyers.in/v2/#fytoken">Token</a>
        /// </summary>
        [DataMember(Name = "fyToken")]
        public string fyToken { get; set; }

        [DataMember(Name = "dummy")]
        public string dummy { get; set; }
        /// <summary>
        /// Y => It is cross currency position. N => It is not a cross currency position
        /// </summary>
        [DataMember(Name = "crossCurrency")]
        public string crossCurrency { get; set; }
        /// <summary>
        /// Incase of cross currency position, the rbi reference rate will be required to calculate the p&l
        /// </summary>
        [DataMember(Name = "rbiRefRate")]
        public double rbiRefRate { get; set; }
        /// <summary>
        /// Incase of commodity positions, this multiplier is required for p&l calculation
        /// </summary>
        [DataMember(Name = "qtyMulti_com")]
        public double qtyMulti_com { get; set; }
        /// <summary>
        /// The <see cref="Segment"/> in which the position is taken.
        /// </summary>
        [DataMember(Name = "segment")]
        public int segment { get; set; }
        /// <summary>
        /// Gets or sets the symbol. Eg: NSE:SBIN-EQ
        /// </summary>
        [DataMember(Name = "symbol")]
        public string symbol { get; set; }
        /// <summary>
        /// The unique value for each position
        /// </summary>
        [DataMember(Name = "id")]
        public string id { get; set; }
    }
}
