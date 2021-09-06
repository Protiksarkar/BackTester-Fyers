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
    public class PlaceOrderPayload : Payload
    {
        /// <summary>
        /// Gets or sets the symbol. Eg: NSE:SBIN-EQ
        /// </summary>
        [DataMember(Name = "symbol")]
        public string symbol { get; set; }
        /// <summary>
        /// The quantity should be in multiples of lot size for derivatives.
        /// </summary>
        [DataMember(Name = "qty")]
        public int qty { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderType"/>
        /// </summary>
        [DataMember(Name = "type")]
        public int type { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderSide"/>
        /// </summary>
        [DataMember(Name = "side")]
        public int side { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="ProductTypes"/>
        /// </summary>
        [DataMember(Name = "productType")]
        public string productType { get; set; }
        /// <summary>
        /// Provide valid price for Limit and Stoplimit orders, else 0 (zero)
        /// </summary>
        [DataMember(Name = "limitPrice")]
        public decimal limitPrice { get; set; }
        /// <summary>
        /// Provide valid price for Stop and Stoplimit orders, else 0 (zero)
        /// </summary>
        [DataMember(Name = "stopPrice")]
        public decimal stopPrice { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderValidity"/>
        /// </summary>
        [DataMember(Name = "validity")]
        public string validity { get; set; }
        /// <summary>
        /// Allowed only for Equity. Default 0 (zero)
        /// </summary>
        [DataMember(Name = "disclosedQty")]
        public int disclosedQty { get; set; }
        /// <summary>
        /// False => When market is open. True => When placing AMO order
        /// </summary>
        [DataMember(Name = "offlineOrder")]
        public string offlineOrder { get; set; }
        /// <summary>
        /// Provide valid price for CO and BO orders. Default 0 (zero)
        /// </summary>
        [DataMember(Name = "stopLoss")]
        public decimal stopLoss { get; set; }
        /// <summary>
        /// Provide valid price for BO orders. Default 0 (zero)
        /// </summary>
        [DataMember(Name = "takeProfit")]
        public decimal takeProfit { get; set; }
    }
}
