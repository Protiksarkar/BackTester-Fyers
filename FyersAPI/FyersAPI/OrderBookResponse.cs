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
    public class OrderBookResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the orderbook array
        /// </summary>
        [DataMember(Name = "orderBook")]
        public List<OrderBook> orderBook { get; set; }
    }


    [DataContract]
    public class OrderBook
    {
        /// <summary>
        /// The order time as per DD-MMM-YYYY hh:mm:ss in IST
        /// </summary>
        [DataMember(Name = "orderDateTime")]
        public string orderDateTime { get; set; }
        /// <summary>
        /// The unique order id assigned for each order
        /// </summary>
        [DataMember(Name = "id")]
        public string id { get; set; }
        /// <summary>
        /// The order id provided by the exchange
        /// </summary>
        [DataMember(Name = "exchOrdId")]
        public string exchOrdId { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderSide"/>
        /// </summary>
        [DataMember(Name = "side")]
        public int side { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Segment"/>
        /// </summary>
        [DataMember(Name = "segment")]
        public int segment { get; set; }
        /// <summary>
        /// Exchange <see cref="InstrumentTypes"/>
        /// </summary>
        [DataMember(Name = "instrument")]
        public int instrument { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="ProductTypes"/>
        /// </summary>
        [DataMember(Name = "productType")]
        public string productType { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderStatus"/>
        /// </summary>
        [DataMember(Name = "status")]
        public int status { get; set; }
        /// <summary>
        /// The original order qty
        /// </summary>
        [DataMember(Name = "qty")]
        public int qty { get; set; }
        /// <summary>
        /// The remaining qty
        /// </summary>
        [DataMember(Name = "remainingQuantity")]
        public int remainingQuantity { get; set; }
        /// <summary>
        /// Gets or sets the filled quantity
        /// </summary>
        [DataMember(Name = "filledQty")]
        public int filledQty { get; set; }
        /// <summary>
        /// The limit price for the order
        /// </summary>
        [DataMember(Name = "limitPrice")]
        public double limitPrice { get; set; }
        /// <summary>
        /// The limit price for the order
        /// </summary>
        [DataMember(Name = "stopPrice")]
        public double stopPrice { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderType"/>
        /// </summary>
        [DataMember(Name = "type")]
        public int type { get; set; }
        /// <summary>
        /// Gets or sets the Disclosed quantity
        /// </summary>
        [DataMember(Name = "discloseQty")]
        public int discloseQty { get; set; }
        /// <summary>
        /// Gets or sets the remaining disclosed quantity
        /// </summary>
        [DataMember(Name = "dqQtyRem")]
        public int dqQtyRem { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderValidity"/>
        /// </summary>
        [DataMember(Name = "orderValidity")]
        public string orderValidity { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderSource"/>
        /// </summary>
        [DataMember(Name = "source")]
        public string source { get; set; }
        /// <summary>
        /// This is used to sort the orders based on the time
        /// </summary>
        [DataMember(Name = "slNo")]
        public int slNo { get; set; }
        /// <summary>
        /// Fytoken is a unique identifier for every symbol. <a href="https://api-docs.fyers.in/v2/#fytoken">Token</a>
        /// </summary>
        [DataMember(Name = "fyToken")]
        public string fyToken { get; set; }
        /// <summary>
        /// False => When market is open. True => When placing AMO order
        /// </summary>
        [DataMember(Name = "offlineOrder")]
        public string offlineOrder { get; set; }
        /// <summary>
        /// The error messages are shown here
        /// </summary>
        [DataMember(Name = "message")]
        public string message { get; set; }
        /// <summary>
        /// Gets or sets the order number status
        /// </summary>
        [DataMember(Name = "orderNumStatus")]
        public string orderNumStatus { get; set; }
        /// <summary>
        /// The average traded price for the order
        /// </summary>
        [DataMember(Name = "tradedPrice")]
        public double tradedPrice { get; set; }
        /// <summary>
        /// The exchange in which order is placed. <see cref="Exchanges"/>
        /// </summary>
        [DataMember(Name = "exchange")]
        public int exchange { get; set; }
        /// <summary>
        /// PAN of the client
        /// </summary>
        [DataMember(Name = "pan")]
        public string pan { get; set; }
        /// <summary>
        /// The client id of the fyers user
        /// </summary>
        [DataMember(Name = "clientId")]
        public string clientId { get; set; }
        /// <summary>
        /// The symbol for which order is placed
        /// </summary>
        [DataMember(Name = "symbol")]
        public string symbol { get; set; }
        /// <summary>
        /// Gets or sets the price change
        /// </summary>
        [DataMember(Name = "ch")]
        public double ch { get; set; }
        /// <summary>
        /// Gets or sets the percentage change
        /// </summary>
        [DataMember(Name = "chp")]
        public double chp { get; set; }
        /// <summary>
        /// Gets or sets the last traded price
        /// </summary>
        [DataMember(Name = "lp")]
        public double lp { get; set; }
        /// <summary>
        /// Gets or sets the symbol
        /// </summary>
        [DataMember(Name = "ex_sym")]
        public string ex_sym { get; set; }
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [DataMember(Name = "description")]
        public string description { get; set; }
        /// <summary>
        /// Gets or sets the parent type
        /// </summary>
        [DataMember(Name = "parentType")]
        public int parentType { get; set; }
        /// <summary>
        /// The parent order id will be provided only for applicable orders.Eg: BO Leg 2 & 3 and CO Leg 2
        /// </summary>
        [DataMember(Name = "parentId")]
        public string parentId { get; set; }
    }


}
