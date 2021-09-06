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
    public abstract class ModifyOrderPayloadBase : Payload
    {
        /// <summary>
        /// Gets or sets the order id
        /// </summary>
        [DataMember(Name = "id")]
        public string id { get; set; }
        /// <summary>
        /// Gets or sets the order <see cref="OrderSide"/>
        /// </summary>
        [DataMember(Name = "side")]
        public int side { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="OrderType"/>
        /// </summary>
        [DataMember(Name = "type")]
        public int type { get; set; }

    }
}
