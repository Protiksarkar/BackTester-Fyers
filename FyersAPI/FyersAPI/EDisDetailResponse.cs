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
    public class EDisDetailResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the edis detail data
        /// </summary>
        [DataMember(Name = "data")]
        public EDisDetail[] data { get; set; }
    }

    [DataContract]
    public class EDisDetail
    {
        /// <summary>
        /// Gets or sets the client id
        /// </summary>
        [DataMember(Name = "clientId")]
        public string clientId { get; set; }
        /// <summary>
        /// Gets or sets the isin
        /// </summary>
        [DataMember(Name = "isin")]
        public string isin { get; set; }
        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        [DataMember(Name = "qty")]
        public float qty { get; set; }
        /// <summary>
        /// Gets or sets the utilized quantity
        /// </summary>
        [DataMember(Name = "qtyUtlize")]
        public float qtyUtlize { get; set; }
        /// <summary>
        /// Gets or sets the entry date
        /// </summary>
        [DataMember(Name = "entryDate")]
        public string entryDate { get; set; }
        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        [DataMember(Name = "startDate")]
        public string startDate { get; set; }
        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        [DataMember(Name = "endDate")]
        public string endDate { get; set; }
        /// <summary>
        /// Gets or sets the no of days
        /// </summary>
        [DataMember(Name = "noOfDays")]
        public int noOfDays { get; set; }
        [DataMember(Name = "source")]
        public string source { get; set; }
        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [DataMember(Name = "status")]
        public string status { get; set; }
        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        [DataMember(Name = "reason")]
        public string reason { get; set; }
        /// <summary>
        /// Gets or sets the internal transaction id
        /// </summary>
        [DataMember(Name = "internalTxnId")]
        public string internalTxnId { get; set; }
        /// <summary>
        /// Gets or setst he DP transaction id
        /// </summary>
        [DataMember(Name = "dpTxnId")]
        public string dpTxnId { get; set; }
        /// <summary>
        /// Gets or sets the error code
        /// </summary>
        [DataMember(Name = "errCode")]
        public string errCode { get; set; }
        /// <summary>
        /// Gets or sets the error count
        /// </summary>
        [DataMember(Name = "errorCount")]
        public string errorCount { get; set; }
        /// <summary>
        /// Gets or sets the transaction id
        /// </summary>
        [DataMember(Name = "transactionId")]
        public string transactionId { get; set; }
    }
}
