/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FyersAPI
{
    [DataContract]
    public class FundsResponse : ResponseBase
    {

        [DataMember(Name = "fund_limit")]
        public List<FundLimit> fund_limit { get; set; }
    }

    [DataContract]
    public class FundLimit
    {
        /// <summary>
        /// Unique identity for particular fund
        /// </summary>
        [DataMember(Name = "id")]
        public int id { get; set; }
        /// <summary>
        /// Each title represents a heading of the ledger
        /// </summary>
        [DataMember(Name = "title")]
        public string title { get; set; }
        /// <summary>
        /// The amount in the capital ledger for the above-mentioned title
        /// </summary>
        [JsonConverter(typeof(FundLimitConverter))]
        [DataMember(Name = "equityAmount")]
        public double equityAmount { get; set; }
        /// <summary>
        /// The amount in the commodity ledger for the above-mentioned title
        /// </summary>
        [DataMember(Name = "commodityAmount")]
        [JsonConverter(typeof(FundLimitConverter))]
        public double commodityAmount { get; set; }
    }

    public class FundLimitConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float)
            {
                return token.ToObject<double>();
            }

            return 0.0d;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
