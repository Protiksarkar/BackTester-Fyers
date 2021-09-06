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
    public class MarketStatus
    {

        public const string OPEN = "OPEN";
        public const string PREOPEN = "PREOPEN";
        public const string CLOSE = "CLOSE";

        /// <summary>
        /// The exchange in which the position is taken. <see cref="Exchanges"/>
        /// </summary>
        [DataMember(Name = "exchange")]
        public int exchange { get; set; }
        /// <summary>
        /// The segment in which the position is taken. <see cref="Segment"/>
        /// </summary>
        [DataMember(Name = "segment")]
        public int segment { get; set; }
        /// <summary>
        /// NL => Normal Market. MS => Morning Session. ES => Evening Session
        /// </summary>
        [DataMember(Name = "market_type")]
        public string market_type { get; set; }
        /// <summary>
        /// Gets or sets the status. Possible values => CLOSE, OPEN
        /// </summary>
        [DataMember(Name = "status")]
        public string status { get; set; }
    }
}
