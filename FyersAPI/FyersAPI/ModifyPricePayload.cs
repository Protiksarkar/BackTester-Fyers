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
    /// <summary>
    /// Payload for price modification
    /// </summary>
    [DataContract]
    public sealed class ModifyPricePayload : ModifyOrderPayloadBase
    {
        /// <summary>
        /// Gets or sets the limit price. Only incase of Limit/ Stoplimit orders
        /// </summary>
        [DataMember(Name = "limitPrice")]
        public decimal limitPrice { get; set; }
        /// <summary>
        /// Gets or sets the stop price. Only incase of Stop/ Stoplimit orders
        /// </summary>
        [DataMember(Name = "stopPrice")]
        public decimal stopPrice { get; set; }
    }
}
