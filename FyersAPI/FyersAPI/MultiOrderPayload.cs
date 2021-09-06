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
    /// Payload to place, modify or cancel multiple orders. Fyers allow upto 10 orders per request
    /// </summary>
    /// <typeparam name="T">T is the order payload. <see cref="PlaceOrderPayload"/>, <see cref="ModifyOrderPayload"/>, <see cref="CancelOrderPayload"/></typeparam>
    [DataContract]
    public class MultiOrderPayload<T> : Payload where T : Payload
    {
        /// <summary>
        /// Gets or sets the order payloads
        /// </summary>
        public T[] Orders { get; set; }
        
        public override string GetJsonString()
        {

            if (this.Orders == null || this.Orders.Length == 0)
                return "[]";
            
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < this.Orders.Length; i++)
            {
                if (this.Orders[i] == null)
                    continue;

                sb.Append(this.Orders[i].GetJsonString());

                if (i < this.Orders.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");

            return sb.ToString();

        }
    }
}
