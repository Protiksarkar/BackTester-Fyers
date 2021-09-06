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
    public class Postback : ResponseBase
    {
        /// <summary>
        /// Order postback data
        /// </summary>
        [DataMember(Name = "d")]
        public OrderPostback d { get; set; }

        /// <summary>
        /// websocket type
        /// </summary>
        [DataMember(Name = "ws_type")]
        public string ws_type { get; set; }
    }

    [DataContract]
    public class OrderPostback
    {
        /// <summary>
        /// Gets or sets the parent type
        /// </summary>
        [DataMember(Name = "parentType")]
        public int parentType { get; set; }
        /// <summary>
        /// Gets or sets the parent id
        /// </summary>
        [DataMember(Name = "parentId")]
        public string parentId { get; set; }
        /// <summary>
        /// Gets or sets the order date time
        /// </summary>
        [DataMember(Name = "orderDateTime")]
        public long orderDateTime { get; set; }
        /// <summary>
        /// Gets or sets the order id
        /// </summary>
        [DataMember(Name = "id")]
        public string id { get; set; }
        /// <summary>
        /// Gets or sets the exchange order id
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
        public string segment { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="InstrumentTypes"/>
        /// </summary>
        [DataMember(Name = "instrument")]
        public string instrument { get; set; }
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
        /// Gets or sets the order quantity
        /// </summary>
        [DataMember(Name = "qty")]
        public int qty { get; set; }
        /// <summary>
        /// Gets or sets the remaining quantity
        /// </summary>
        [DataMember(Name = "remainingQuantity")]
        public int remainingQuantity { get; set; }
        /// <summary>
        /// Gets or sets the filled quantity
        /// </summary>
        [DataMember(Name = "filledQty")]
        public int filledQty { get; set; }
        /// <summary>
        /// Gets or sets the limit price
        /// </summary>
        [DataMember(Name = "limitPrice")]
        public double limitPrice { get; set; }
        /// <summary>
        /// Gets or sets the stop price
        /// </summary>
        [DataMember(Name = "stopPrice")]
        public double stopPrice { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderType"/>
        /// </summary>
        [DataMember(Name = "type")]
        public int type { get; set; }
        /// <summary>
        /// Gets or sets the disclosed quantity
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
        /// This is used for sorting of positions
        /// </summary>
        [DataMember(Name = "slNo")]
        public int slNo { get; set; }
        /// <summary>
        /// False => When market is open. True => When placing AMO order
        /// </summary>
        [DataMember(Name = "offlineOrder")]
        public bool offlineOrder { get; set; }
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
        /// Fytoken is a unique identifier for every symbol. <a href="https://api-docs.fyers.in/v2/#fytoken">token</a>
        /// </summary>
        [DataMember(Name = "fyToken")]
        public string fyToken { get; set; }
        /// <summary>
        /// Gets or sets the symbol
        /// </summary>
        [DataMember(Name = "symbol")]
        public string symbol { get; set; }
    }


}
