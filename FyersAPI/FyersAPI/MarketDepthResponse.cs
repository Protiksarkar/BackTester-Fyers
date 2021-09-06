/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FyersAPI
{
    [DataContract]
    public class MarketDepthResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        [DataMember(Name = "d")]
        public Dictionary<string, MarketDepth> d { get; set; }
    }

    [DataContract]
    public class MarketDepth
    {
        /// <summary>
        /// Total buying quantity
        /// </summary>
        [DataMember(Name = "totalbuyqty")]
        public long totalbuyqty { get; set; }
        /// <summary>
        /// Total selling quantity
        /// </summary>
        [DataMember(Name = "totalsellqty")]
        public long totalsellqty { get; set; }
        /// <summary>
        /// Bidding price along with volume and total number of orders
        /// </summary>
        [DataMember(Name = "bids")]
        public MarketDepthItem[] bids { get; set; }
        /// <summary>
        /// Offer price with volume and total number of orders
        /// </summary>
        [DataMember(Name = "ask")]
        public MarketDepthItem[] ask { get; set; }
        /// <summary>
        /// Price at market opening time
        /// </summary>
        [DataMember(Name = "o")]
        public double o { get; set; }
        /// <summary>
        /// Highest price for the day
        /// </summary>
        [DataMember(Name = "h")]
        public double h { get; set; }
        /// <summary>
        /// Lowest price for the day
        /// </summary>
        [DataMember(Name = "l")]
        public double l { get; set; }
        /// <summary>
        /// Price at the of market closing
        /// </summary>
        [DataMember(Name = "c")]
        public double c { get; set; }
        /// <summary>
        /// Percentage of change between the current value and the previous day's market close
        /// </summary>
        [DataMember(Name = "chp")]
        public double chp { get; set; }
        /// <summary>
        /// Change value
        /// </summary>
        [DataMember(Name = "ch")]
        public double ch { get; set; }
        /// <summary>
        /// Last traded quantity
        /// </summary>
        [DataMember(Name = "ltq")]
        public int ltq { get; set; }
        /// <summary>
        /// Last traded time
        /// </summary>
        [DataMember(Name = "ltt")]
        public long ltt { get; set; }

        /// <summary>
        /// Gets the DateTime formatted last traded time
        /// </summary>
        [IgnoreDataMember]
        public DateTime LTT
        {
            get { return Globals.FromUnixTime(this.ltt); }
        }

        /// <summary>
        /// Last traded price
        /// </summary>
        [DataMember(Name = "ltp")]
        public double ltp { get; set; }
        /// <summary>
        /// Volume traded
        /// </summary>
        [DataMember(Name = "v")]
        public long v { get; set; }
        /// <summary>
        /// Average traded price
        /// </summary>
        [DataMember(Name = "atp")]
        public double atp { get; set; }
        /// <summary>
        /// Lower circuit price`
        /// </summary>
        [DataMember(Name = "lower_ckt")]
        public double lower_ckt { get; set; }
        /// <summary>
        /// upper circuit price
        /// </summary>
        [DataMember(Name = "upper_ckt")]
        public double upper_ckt { get; set; }
        /// <summary>
        /// Expiry date
        /// </summary>
        [DataMember(Name = "expiry")]
        public string expiry { get; set; }
        /// <summary>
        /// Gets or sets the open interest
        /// </summary>
        [DataMember(Name = "oi")]
        public long oi { get; set; }
        /// <summary>
        /// Boolean flag for OI data, true or false
        /// </summary>
        [DataMember(Name = "oiflag")]
        public bool oiflag { get; set; }
        /// <summary>
        /// Previous days open interest
        /// </summary>
        [DataMember(Name = "pdoi")]
        public long pdoi { get; set; }
        /// <summary>
        /// Change in open Interest percentage
        /// </summary>
        [DataMember(Name = "oipercent")]
        public double oipercent { get; set; }
    }

    [DataContract]
    public class MarketDepthItem
    {
        /// <summary>
        /// Gets or sets the price
        /// </summary>
        [DataMember(Name = "price")]
        public double price { get; set; }
        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [DataMember(Name = "volume")]
        public int volume { get; set; }
        /// <summary>
        /// Gets or sets the number of orders
        /// </summary>
        [DataMember(Name = "ord")]
        public int ord { get; set; }

        public override string ToString()
        {
            return $"Price={price}|vol={volume},|ord={ord}";
        }
    }
}
