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
    /// <a href="https://api-docs.fyers.in/v2/#request-parameters-for-step-1">Token payload</a>
    /// </summary>
    [DataContract]
    public class TokenPayload : Payload
    {
        /// <summary>
        /// This value must always be “authorization_code
        /// </summary>
        [DataMember(Name = "grant_type")]
        public string grant_type { get; set; }
        /// <summary>
        /// SHA-256 of api_id + app_secret
        /// </summary>
        [DataMember(Name = "appIdHash")]
        public string appIdHash { get; set; }
        /// <summary>
        /// This is the auth_code received while login
        /// </summary>
        [DataMember(Name = "code")]
        public string code { get; set; }

    }   
}
