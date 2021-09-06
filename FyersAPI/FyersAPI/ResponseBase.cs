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
    public abstract class ResponseBase
    {
        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [DataMember(Name = "s")]
        public string s { get; set; }
        /// <summary>
        /// Gets or sets the status code. Negative integer to identify the specific error
        /// </summary>
        [DataMember(Name = "code")]
        public int code { get; set; }
        /// <summary>
        /// Error message to identify error
        /// </summary>
        [DataMember(Name = "message")]
        public string message { get; set; }
    }
}
