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
    public class EDisPayload : Payload
    {
        /// <summary>
        /// Gets or sets the record list
        /// </summary>
        [DataMember(Name = "recordLst")]
        public List<Recordlst> recordLst { get; set; }

    }

    [DataContract]
    public class Recordlst
    {
        /// <summary>
        /// Gets or sets the isin
        /// </summary>
        [DataMember(Name = "isin_code")]
        public string isin_code { get; set; }
        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        [DataMember(Name = "qty")]
        public int qty { get; set; }
    }

}
