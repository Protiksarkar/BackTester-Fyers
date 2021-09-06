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
    public class SocketMessageBase : Payload
    {
        //{"T":"SUB_DATA","TLIST":["NSE:NIFTY50-INDEX","NSE:TATAPOWER-EQ"],"SUB_T":1}
        [DataMember(Name = "T")]
        public string T { get; set; }
        
        [DataMember(Name = "SUB_T")]
        public int SUB_T { get; set; }
    }

    public class L1Message : SocketMessageBase
    {
        public L1Message()
        {
            this.T = "SUB_DATA";
        }

        [DataMember(Name = "TLIST")]
        public string[] TLIST { get; set; }
    }

    public class L2Message : SocketMessageBase
    {
        public L2Message()
        {
            this.T = "SUB_L2";
        }

        [DataMember(Name = "L2LIST")]
        public string[] L2LIST { get; set; }
    }

    public class OrderMessage : SocketMessageBase
    {
        public OrderMessage()
        {
            this.T = "SUB_ORD";
        }

        [DataMember(Name = "SLIST")]
        public string[] SLIST { get; set; }
    }


}
