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
    public class EDisInquiryResponse : ResponseBase
    {
        [DataMember(Name = "data")]
         public EDisResult data { get; set; }
    }

    [DataContract]
    public class EDisResult
    {
        /// <summary>
        /// Gets or sets the failed quantity
        /// </summary>
        [DataMember(Name = "FAILED_CNT")]
        public int FAILED_CNT { get; set; }
        /// <summary>
        /// Gets or sets the success quantity
        /// </summary>
        [DataMember(Name = "SUCEESS_CNT")]
        public int SUCEESS_CNT { get; set; }
    }
}
