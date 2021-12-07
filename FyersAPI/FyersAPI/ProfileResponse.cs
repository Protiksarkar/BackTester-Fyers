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
    public class ProfileResponse : ResponseBase
    {
        [DataMember(Name = "data")]
        public Profile data { get; set; }
    }


    [DataContract]
    public class Profile
    {
        /// <summary>
        /// Name of the client
        /// </summary>
        [DataMember(Name = "name")]
        public string name { get; set; }
        /// <summary>
        /// URL link to the user’s profile picture, if any.
        /// </summary>
        [DataMember(Name = "image")]
        public string image { get; set; }
        /// <summary>
        /// Display name, if any, provided by the client
        /// </summary>
        [DataMember(Name = "display_name")]
        public string display_name { get; set; }
        /// <summary>
        /// Email address of the client
        /// </summary>
        [DataMember(Name = "email_id")]
        public string email_id { get; set; }
        /// <summary>
        /// PAN of the client
        /// </summary>
        [DataMember(Name = "PAN")]
        public string PAN { get; set; }
        /// <summary>
        /// The client id of the fyers user
        /// </summary>
        [DataMember(Name = "fy_id")]
        public string fy_id { get; set; }
        /// <summary>
        /// Last password changed date
        /// </summary>
        [DataMember(Name = "pwd_change_date")]
        public string pwd_change_date { get; set; }
        /// <summary>
        /// Number of days until the current password expires
        /// </summary>
        [DataMember(Name = "pwd_to_expire")]
        public int? pwd_to_expire { get; set; }
    }
}
