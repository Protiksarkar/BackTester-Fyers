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
    public class MultiOrderResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the response array
        /// </summary>
        [DataMember(Name = "data")]
        public List<MultiOrder> data { get; set; }
    }

    public class MultiOrder
    {
        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        [DataMember(Name = "statusCode")]
        public int statusCode { get; set; }
        /// <summary>
        /// Gets or sets the body
        /// </summary>
        [DataMember(Name = "body")]
        public OrderResponse body { get; set; }
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [DataMember(Name = "statusDescription")]
        public string statusDescription { get; set; }
    }

}
