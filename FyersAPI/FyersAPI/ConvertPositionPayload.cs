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
    public class ConvertPositionPayload : Payload
    {
        /// <summary>
        /// Gets or sets the symbol
        /// </summary>
        [DataMember(Name = "symbol")]
        public string symbol { get; set; }
        /// <summary>
        /// Gets or sets te position side. 1 => Open long positions. -1 => Open short positions
        /// </summary>
        [DataMember(Name = "positionSide")]
        public int positionSide { get; set; }
        /// <summary>
        /// Quantity to be converted. Has to be in multiples of lot size for derivatives
        /// </summary>
        [DataMember(Name = "convertQty")]
        public int convertQty { get; set; }
        /// <summary>
        /// Existing productType. (CNC positions cannot be converted)
        /// </summary>
        [DataMember(Name = "convertFrom")]
        public string convertFrom { get; set; }
        /// <summary>
        /// The new product type
        /// </summary>
        [DataMember(Name = "convertTo")]
        public string convertTo { get; set; }
    }
}
