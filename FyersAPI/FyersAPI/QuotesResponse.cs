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
    public class QuotesResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the quote
        /// </summary>
        [DataMember(Name = "d")]
        public Quote[] d { get; set; }

    }

    [DataContract]
    public class Quote
    {
        [DataMember(Name ="n")]
        public string n { get; set; }
        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [DataMember(Name = "s")]
        public string s { get; set; }
        /// <summary>
        /// Gets or sets the touchline
        /// </summary>
        [DataMember(Name = "v")]
        public Touchline v { get; set; }
    }

    public class Touchline
    {
        /// <summary>
        /// Change value
        /// </summary>
        [DataMember(Name = "ch")]
        public double ch { get; set; }
        /// <summary>
        /// Percentage of change between the current value and the previous day's market close
        /// </summary>
        [DataMember(Name = "chp")]
        public double chp { get; set; }
        /// <summary>
        /// Last traded price
        /// </summary>
        [DataMember(Name = "lp")]
        public double lp { get; set; }
        /// <summary>
        /// Difference between lowest asking and highest bidding price
        /// </summary>
        [DataMember(Name = "spread")]
        public double spread { get; set; }
        /// <summary>
        /// Asking price for the symbol
        /// </summary>
        [DataMember(Name = "ask")]
        public double ask { get; set; }
        /// <summary>
        /// Bidding price for the symbol
        /// </summary>
        [DataMember(Name = "bid")]
        public double bid { get; set; }
        /// <summary>
        /// Price at market opening time
        /// </summary>
        [DataMember(Name = "open_price")]
        public double open_price { get; set; }
        /// <summary>
        /// Highest price for the day
        /// </summary>
        [DataMember(Name = "high_price")]
        public double high_price { get; set; }
        /// <summary>
        /// Lowest price for the day
        /// </summary>
        [DataMember(Name = "low_price")]
        public double low_price { get; set; }
        /// <summary>
        /// Previous close
        /// </summary>
        [DataMember(Name = "prev_close_price")]
        public double prev_close_price { get; set; }
        /// <summary>
        /// Volume traded
        /// </summary>
        [DataMember(Name = "volume")]
        public long volume { get; set; }
        /// <summary>
        /// Short name for the symbol Eg: “SBIN-EQ”
        /// </summary>
        [DataMember(Name = "short_name")]
        public string short_name { get; set; }
        /// <summary>
        /// Name of the <see cref="Exchanges"/>. Eg: “NSE” or “BSE”
        /// </summary>
        [DataMember(Name = "exchange")]
        public string exchange { get; set; }
        /// <summary>
        /// Description of the symbol
        /// </summary>
        [DataMember(Name = "description")]
        public string description { get; set; }
        /// <summary>
        /// Original name of the symbol name provided by the use
        /// </summary>
        [DataMember(Name = "original_name")]
        public string original_name { get; set; }
        /// <summary>
        /// Symbol name provided by the user
        /// </summary>
        [DataMember(Name = "symbol")]
        public string symbol { get; set; }
        /// <summary>
        /// <a href="https://api-docs.fyers.in/v2/#fytoken">Fyers token</a>
        /// </summary>
        [DataMember(Name = "fyToken")]
        public string fyToken { get; set; }
        /// <summary>
        /// Today’s time
        /// </summary>
        [DataMember(Name = "tt")]
        public long tt { get; set; }

        /// <summary>
        /// Gets the DateTime formatted todays time
        /// </summary>
        [IgnoreDataMember]
        public DateTime TT
        {
            get { return Globals.FromUnixTime(tt); }
        }

        /// <summary>
        /// Current time, open, high, low price and volume with HH:MM timestamp
        /// </summary>
        [DataMember(Name = "cmd")]
        public Candle cmd { get; set; }
    }

    public class Candle
    {
        /// <summary>
        /// Gets or sets the time stamp
        /// </summary>
        [DataMember(Name = "t")]
        public long t { get; set; }
        /// <summary>
        /// Gets or sets the open
        /// </summary>
        [DataMember(Name = "o")]
        public double o { get; set; }
        /// <summary>
        /// Gets or sets the high
        /// </summary>
        [DataMember(Name = "h")]
        public double h { get; set; }
        /// <summary>
        /// Gets or sets the low
        /// </summary>
        [DataMember(Name = "l")]
        public double l { get; set; }
        /// <summary>
        /// Gets or sets the close
        /// </summary>
        [DataMember(Name = "c")]
        public double c { get; set; }
        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [DataMember(Name = "v")]
        public long v { get; set; }
        /// <summary>
        /// Gets or sets the bar time frame
        /// </summary>
        [DataMember(Name = "tf")]
        public string tf { get; set; }
    }

}
