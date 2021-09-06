/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebSocket4Net;

namespace FyersAPI
{
    public sealed class Fyers
    {
        private const string grant_type = "authorization_code";

        private static string[] masterFiles = null;
        /// <summary>
        /// Gets the master file exchange names
        /// </summary>
        public static string[] MasterFiles
        {
            get 
            {
                if (masterFiles == null)
                {
                    masterFiles = new[] { "NSE_CD", "NSE_FO", "NSE_CM", "MCX_COM" };
                }
                return masterFiles; 
            }
        }

        /// <summary>
        /// Parses a json string 
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="str">json string</param>
        /// <returns></returns>
        public static T ParseJson<T>(string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(T);

            //str = str.Replace(@"\", string.Empty).Replace("\"{", "{").Replace("}\"", "}");

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore

                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Downloads the <a href="https://myapi.fyers.in/docs/#tag/Broker-Config/paths/~1Broker%20Config/patch">Master symbol files</a>.
        /// </summary>
        /// <param name="exchange">Exchange for which the master is to be downloaded.</param>
        /// <returns>Returns the response stream on success</returns>
        public static async Task<Stream> DownloadMasterAsync(string exchange)
        {
            if (string.IsNullOrWhiteSpace(exchange))
                return null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://public.fyers.in");
                HttpResponseMessage response = null;
                try
                {
                    response = await httpClient.GetAsync($"/sym_details/{exchange.Trim()}.csv").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw;
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                }
            }

            return null;
        }


        /// <summary>
        /// Validates the url to determine a valid login
        /// </summary>
        /// <param name="requestUrl">request url</param>
        /// <param name="apiKey">Api key</param>
        /// <param name="secret">Api secret</param>
        /// <param name="auth_code">auth_code</param>
        /// <param name="appIdHash">appIdHash</param>
        /// <returns>Returns true if the login is a success. Also parse the requestToken and the checksum</returns>
        public static bool IsValidLogin(string url, string appId, string secret, out TokenPayload tokenPayload)
        {
            tokenPayload = null;
                        
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(secret))
                return false;

            int idx = url.IndexOf('?');
            if (idx < 0)
                return false;

            var token = url.Substring(idx);
            var query = HttpUtility.ParseQueryString(token);
            token = query.Get("auth_code");

            if (string.IsNullOrEmpty(token))
                return false;

            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes($"{appId}:{secret}"));

                foreach (var item in result)
                {
                    sb.Append(item.ToString("x2"));
                }
            }

            var check = sb.ToString();
            if (string.IsNullOrEmpty(check))
                return false;

            tokenPayload = new TokenPayload()
            {
                appIdHash = check,
                code = token,
                grant_type = grant_type
            };
            
            return true;
        }

        //***************************************************************************************************

        HttpClient httpClient = null;
        string accessToken = string.Empty;

        WebSocket dataSocket = null;
        WebSocket orderSocket = null;

        public Fyers(string appId)
        {
            this.AppId = appId;

            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://api.fyers.in");
            this.httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }


        /// <summary>
        /// Generate <a href="https://myapi.fyers.in/docs/#tag/Authentication-and-Login-Flow-User-Apps">Access token</a>
        /// </summary>
        /// <param name="payload"><see cref="TokenPayload"/></param>
        /// <returns><see cref="TokenResponse"/></returns>
        public async Task<TokenResponse> GenerateTokenAsync(TokenPayload payload)
        {
            this.accessToken = string.Empty;

            var response = await Query<TokenResponse>(this.httpClient, HttpMethod.POST, $"/api/v2/validate-authcode", payload: payload, triggerJsonEvent: false).ConfigureAwait(false);

            if (response == null || response.code != 200 || string.IsNullOrWhiteSpace(response.access_token))
                return null;

            this.accessToken = response.access_token;

            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"{this.AppId}:{response.access_token}");

            return response;
        }

       

        /// <summary>
        /// Logs out of the sockets
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogoutAsync()
        {
            this.accessToken = string.Empty;

            WebSocket dataSocket = this.dataSocket;
            if (dataSocket != null)
            {
                //if (dataSocket.State == WebSocketState.Open)
                //{
                //    dataSocket.Close();
                //}

                dataSocket.Opened -= OnDataSocketOpened;
                dataSocket.Closed -= OnDataSocketClosed;
                dataSocket.MessageReceived -= OnDataMessageReceived;
                dataSocket.Error -= OnDataError;
                dataSocket.DataReceived -= OnDataReceived;
                dataSocket.Dispose();
                this.dataSocket = null;
            }

            WebSocket orderSocket = this.orderSocket;

            if (orderSocket != null)
            {
                if (orderSocket.State == WebSocketState.Open)
                {
                    //un-subscribe from the order updates
                    await this.SendAsync(new OrderMessage()
                    {
                        SLIST = new string[] { "orderUpdate" },
                        SUB_T = 0
                    }, MessageType.Order).ConfigureAwait(false);

                    //orderSocket.Close();
                }


                orderSocket.Opened -= OnOrderSocketOpened;
                orderSocket.Closed -= OnOrderSocketClosed;
                orderSocket.MessageReceived -= OnOrderMessageReceived;
                orderSocket.DataReceived -= OnOrderDataReceived;
                orderSocket.Error -= OnOrderError;

                orderSocket.Dispose();
                this.orderSocket = null;
            }

            return true;
        }

        /// <summary>
        /// Gets the user <a href="https://myapi.fyers.in/docs/#tag/User/paths/~1User/post">profile</a>
        /// </summary>
        /// <returns><see cref="ProfileResponse"/></returns>
        public async Task<ProfileResponse> GetProfileAsync()
        {
            return await Query<ProfileResponse>(this.httpClient, HttpMethod.GET, "/api/v2/profile").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/User/paths/~1User/put">account balance</a>
        /// </summary>
        /// <returns><see cref="FundsResponse"/></returns>
        public async Task<FundsResponse> GetFundsAsync()
        {
            return await Query<FundsResponse>(this.httpClient, HttpMethod.GET, "/api/v2/funds").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/User/paths/~1User/get">demat holding</a>
        /// </summary>
        /// <returns><see cref="HoldingResponse"/></returns>
        public async Task<HoldingResponse> GetHoldingAsync()
        {
            return await Query<HoldingResponse>(this.httpClient, HttpMethod.GET, "/api/v2/holdings").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Transaction-Info/paths/~1Transaction%20Info/post">order book</a>
        /// </summary>
        /// <returns><see cref="OrderBookResponse"/></returns>
        public async Task<OrderBookResponse> GetOrderBookAsync()
        {
            return await Query<OrderBookResponse>(this.httpClient, HttpMethod.GET, "/api/v2/orders").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Transaction-Info/paths/~1Transaction%20Info/put">order status</a>
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns><see cref="OrderBookResponse"/></returns>
        public async Task<OrderBookResponse> GetOrderStatusAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return null;

            return await Query<OrderBookResponse>(this.httpClient, HttpMethod.GET, $"/api/v2/orders?id={orderId}").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Transaction-Info/paths/~1Transaction%20Info/patch">positions</a>
        /// </summary>
        /// <returns><see cref="PositionResponse"/></returns>
        public async Task<PositionResponse> GetPositionAsync()
        {
            return await Query<PositionResponse>(this.httpClient, HttpMethod.GET, "/api/v2/positions").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Transaction-Info/paths/~1Transaction%20Info/get">trade book</a>
        /// </summary>
        /// <returns><see cref="TradeResponse"/></returns>
        public async Task<TradeResponse> GetTradeBookAsync()
        {
            return await Query<TradeResponse>(this.httpClient, HttpMethod.GET, "/api/v2/tradebook").ConfigureAwait(false);
        }


        /// <summary>
        /// Place an <a href="https://myapi.fyers.in/docs/#tag/Order-Placement/paths/~1OthePlacement/get">order</a>
        /// </summary>
        /// <param name="payload"><see cref="PlaceOrderPayload">Order payload</see></param>
        /// <returns><see cref="OrderResponse"/></returns>
        public async Task<OrderResponse> PlaceOrderAsync(PlaceOrderPayload payload)
        {
            return await Query<OrderResponse>(this.httpClient, HttpMethod.POST, "/api/v2/orders", payload: payload).ConfigureAwait(false);
        }

        /// <summary>
        /// Places multiple or <a href="https://myapi.fyers.in/docs/#tag/Order-Placement/paths/~1OthePlacement/patch">basket orders</a>. Fyers let you place upto 10 orders per request.
        /// </summary>
        /// <param name="payload"><see cref="MultiOrderPayload{T}">Payload</see> of <see cref="PlaceOrderPayload"/></param>
        /// <returns><see cref="MultiOrderResponse"/></returns>
        public async Task<MultiOrderResponse> PlaceBasketOrderAsync(MultiOrderPayload<PlaceOrderPayload> payload)
        {
            return await Query<MultiOrderResponse>(this.httpClient, HttpMethod.POST, "/api/v2/orders-multi", payload: payload as MultiOrderPayload<PlaceOrderPayload>).ConfigureAwait(false);
        }

        /// <summary>
        /// <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OtherTransactions/post">Modify an order</a>
        /// </summary>
        /// <param name="payload"><see cref="ModifyOrderPayload">Modify order payload</see></param>
        /// <returns><see cref="OrderResponse"/></returns>
        public async Task<OrderResponse> ModifyOrderAsync(ModifyOrderPayloadBase payload)
        {
            return await Query<OrderResponse>(this.httpClient, HttpMethod.PUT, "/api/v2/orders", payload: payload).ConfigureAwait(false);
        }

        /// <summary>
        /// <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OtherTransactions/get">Modify multiple orders</a>. Fyers lets you modify 10 orders per request
        /// </summary>
        /// <param name="payload"><see cref="MultiOrderPayload{T}">Multi order payload</see> of <see cref="ModifyOrderPayload"/></param>
        /// <returns><see cref="MultiOrderResponse"/></returns>
        public async Task<MultiOrderResponse> ModifyBasketOrderAsync(MultiOrderPayload<ModifyOrderPayloadBase> payload)
        {
            return await Query<MultiOrderResponse>(this.httpClient, HttpMethod.PUT, "/api/v2/orders-multi", payload: payload).ConfigureAwait(false);
        }

        /// <summary>
        /// <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OtherTransactions/put">Cancel an order</a>
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns><see cref="OrderResponse"/></returns>
        public async Task<OrderResponse> CancelOrderAsync(string orderId)
        {
            var payload = new CancelOrderPayload()
            {
                id = orderId
            };

            return await Query<OrderResponse>(this.httpClient, HttpMethod.DELETE, "/api/v2/orders", payload: payload).ConfigureAwait(false);
        }

        /// <summary>
        /// <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OtherTransactions/delete">Cancel multiple orders</a>. Fyers lets you modify upto 10 order per request.
        /// </summary>
        /// <param name="payload"><see cref="MultiOrderPayload{T}">Multi order payload</see> of <see cref="CancelOrderPayload"/></param>
        /// <returns><see cref="MultiOrderResponse"/></returns>
        public async Task<MultiOrderResponse> CancelBasketOrderAsync(MultiOrderPayload<CancelOrderPayload> payload)
        {
            return await Query<MultiOrderResponse>(this.httpClient, HttpMethod.DELETE, "/api/v2/orders-multi", payload: payload).ConfigureAwait(false);
        }

        /// <summary>
        /// Exit all <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OtherTransactions/patch">positions</a>
        /// </summary>
        /// <returns><see cref="ExitPositionResponse"/></returns>
        public async Task<ExitPositionResponse> ExitPositionAsync()
        {
            return await Query<ExitPositionResponse>(this.httpClient, HttpMethod.DELETE, "/api/v2/positions").ConfigureAwait(false);
        }

        /// <summary>
        /// Exit <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OtherTransactions/options">position by symbol</a>
        /// </summary>
        /// <param name="payload"><see cref="ExitPositionBySymbolPayload">Payload</see></param>
        /// <returns><see cref="ExitPositionResponse"/></returns>
        public async Task<ExitPositionResponse> ExitPositionAsync(ExitPositionBySymbolPayload payload)
        {
            return await Query<ExitPositionResponse>(this.httpClient, HttpMethod.DELETE, "/api/v2/positions", payload: payload).ConfigureAwait(false);
        }

        /// <summary>
        /// <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OtherTransactions/options">Exit position by symbol and</a> <see cref="ProductTypes"/>
        /// </summary>
        /// <param name="payload"><see cref="ExitPositionByIdPayload">Payload</see></param>
        /// <returns><see cref="ExitPositionResponse"/></returns>
        public async Task<ExitPositionResponse> ExitPositionAsync(ExitPositionByIdPayload payload)
        {
            return await Query<ExitPositionResponse>(this.httpClient, HttpMethod.DELETE, "/api/v2/positions", payload: payload as ExitPositionByIdPayload).ConfigureAwait(false);
        }

        /// <summary>
        /// <a href="https://myapi.fyers.in/docs/#tag/Other-Transactions/paths/~1OthePlacement/post">Convert position</a>
        /// </summary>
        /// <param name="payload"><see cref="ConvertPositionPayload">Payload</see></param>
        /// <returns><see cref="ConvertPositionResponse"/></returns>
        public async Task<ConvertPositionResponse> ConvertPositionAsync(ConvertPositionPayload payload)
        {
            return await Query<ConvertPositionResponse>(this.httpClient, HttpMethod.PUT, "/api/v2/positions", payload: payload).ConfigureAwait(false);
        }


        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Broker-Config/paths/~1Broker%20Config/post">market status</a>
        /// </summary>
        /// <returns><see cref="MarketStatusResponse"/></returns>
        public async Task<MarketStatusResponse> GetMarketStatusAsync()
        {
            return await Query<MarketStatusResponse>(this.httpClient, HttpMethod.GET, "/api/v2/market-status").ConfigureAwait(false);
        }


        #region EDIS

        /// <summary>
        /// Generates the <see href="https://myapi.fyers.in/docs/#tag/EDIS/paths/~1EDIS/post">T-Pin</see>
        /// </summary>
        /// <returns></returns>
        public async Task<EDisResponse> GenerateTPinAsync()
        {
            return await Query<EDisResponse>(this.httpClient, HttpMethod.GET, "/api/v2/tpin").ConfigureAwait(false);
        }

        

        /// <summary>
        /// Gets the <see href="https://myapi.fyers.in/docs/#tag/EDIS/paths/~1EDIS/get">eDis status</see> details 
        /// </summary>
        /// <returns></returns>
        public async Task<EDisDetailResponse> GetEdisDetailsAsync()
        {
            return await Query<EDisDetailResponse>(this.httpClient, HttpMethod.GET, "/api/v2/details").ConfigureAwait(false);
        }

        /// <summary>
        /// Posts the request to CDSL page and lets user confirm the <see cref="https://myapi.fyers.in/docs/#tag/EDIS/paths/~1EDIS/put">authentication process</see>
        /// </summary>
        /// <param name="payload"><see cref="FyersAPI.EDisInquiryPayload"/></param>
        /// <returns></returns>
        public async Task<EDisResponse> PostEDisRequestAsync(EDisPayload payload)
        {
            return await Query<EDisResponse>(this.httpClient, HttpMethod.POST, "/api/v2/index", payload: payload, triggerJsonEvent: false).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see href="https://myapi.fyers.in/docs/#tag/EDIS/paths/~1EDIS/delete">EDis inquiry result</see>
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<EDisInquiryResponse> PostEDisInquiryAsync(EDisInquiryPayload payload)
        {
            return await Query<EDisInquiryResponse>(this.httpClient, HttpMethod.POST, "/api/v2/inquiry", payload: payload).ConfigureAwait(false);
        }

        #endregion


        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Data-Api/paths/~1DataApi/post">historical data</a> date wise
        /// </summary>
        /// <param name="symbol">Symbol for which the data being downloaded, e.g. NSE:SBIN-EQ</param>
        /// <param name="resolution">Data granularity. 'D' or '1D' for daily data. '1', '2', etc for minute data</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns></returns>
        public async Task<HistoryResponse> GetHistoricalDataAsync(string symbol, string resolution, DateTime from, DateTime to, int? contFlag = null)
        {
            return await Query<HistoryResponse>(this.httpClient, HttpMethod.GET, string.Format(CultureInfo.InvariantCulture, "/data-rest/v2/history/?symbol={0}&resolution={1}&date_format=1&range_from={2:yyyy-MM-dd}&range_to={3:yyyy-MM-dd}&cont_flag={4}", symbol, resolution, from, to, contFlag), triggerJsonEvent: false).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Data-Api/paths/~1DataApi/post">historical data</a> epoch date wise
        /// </summary>
        /// <param name="symbol">Symbol for which the data being downloaded, e.g. NSE:SBIN-EQ</param>
        /// <param name="resolution">Data granularity. 'D' or '1D' for daily data. '1', '2', etc for minute data</param>
        /// <param name="from">From epoch date value</param>
        /// <param name="to">To epoch date value</param>
        /// <returns></returns>
        public async Task<HistoryResponse> GetHistoricalDataAsync(string symbol, string resolution, long from, long to, int? contFlag = null)
        {
            return await Query<HistoryResponse>(this.httpClient, HttpMethod.GET, string.Format(CultureInfo.InvariantCulture, "/data-rest/v2/history/?symbol={0}&resolution={1}&date_format=0&range_from={2}&range_to={3}&cont_flag={4}", symbol, resolution, from, to, contFlag), triggerJsonEvent: false).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <a href="https://myapi.fyers.in/docs/#tag/Data-Api/paths/~1DataApi/get">quotes</a> for upto 50 scrips
        /// </summary>
        /// <param name="symbols">Array of symbols</param>
        /// <returns><see cref="QuotesResponse"/></returns>
        public async Task<QuotesResponse> GetQuotesAsync(string[] symbols)
        {
            if (symbols == null || symbols.Length == 0)
                return null;

            string sym = string.Join(",", symbols);

            return await Query<QuotesResponse>(this.httpClient, HttpMethod.GET, $"/data-rest/v2/quotes/?symbols={sym}").ConfigureAwait(false);
        }

        /// <summary>
        /// Get <a href="https://myapi.fyers.in/docs/#tag/Data-Api/paths/~1DataApi/put">market depth</a>
        /// </summary>
        /// <param name="symbol">Symbol for which the data to be fetched. e.g. NSE:SBIN-EQ</param>
        /// <returns><see cref="MarketDepthResponse"/></returns>
        public async Task<MarketDepthResponse> GetMarketDepthAsync(string symbol)
        {
            return await Query<MarketDepthResponse>(this.httpClient, HttpMethod.GET, $"/data-rest/v2/depth/?symbol={symbol}&ohlcv_flag=1").ConfigureAwait(false);
        }

        

        private async Task<T> Query<T>(HttpClient client, HttpMethod httpMethod, string requestUri, Payload payload = null, bool triggerJsonEvent = true)
        {
            if (client == null)
                return default(T);

            /*
            if (payload != null)
            {
                string str = await payload.GetHttpContent().ReadAsStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(str))
                {
                    str = $"{payload.GetType().Name} Payload ={str}";
                    OnJson(typeof(T), str);
                }
            }
            */

            HttpResponseMessage response = null;
            try
            {
                switch (httpMethod)
                {
                    case HttpMethod.GET:
                        response = await client.GetAsync(requestUri).ConfigureAwait(false);
                        break;
                    case HttpMethod.POST:

                        if (payload == null)
                            return default(T);

                        response = await client.PostAsync(requestUri, payload?.GetHttpContent()).ConfigureAwait(false);
                        break;
                    case HttpMethod.PUT:

                        if (payload == null)
                            return default(T);

                        response = await client.PutAsync(requestUri, payload?.GetHttpContent()).ConfigureAwait(false);
                        break;
                    case HttpMethod.DELETE:

                        if (payload == null)
                        {
                            response = await client.DeleteAsync(requestUri).ConfigureAwait(false);
                        }
                        else
                        {
                            response = await client.SendAsync(new HttpRequestMessage(System.Net.Http.HttpMethod.Delete, requestUri)
                            {
                                Content = payload.GetHttpContent()
                            }).ConfigureAwait(false);
                        }

                        break;
                    default:
                        break;
                }


            }
            catch (AggregateException aex)
            {
                if (aex != null && aex.InnerExceptions != null)
                {
                    foreach (var inner in aex.InnerExceptions)
                    {
                        OnException(typeof(T), inner);
                    }
                }
            }
            catch (Exception ex)
            {
                OnException(typeof(T), ex);
            }


            if (response == null)
                return default(T);

            /*
            if (typeof(T) == typeof(HistoryResponse))
            {
                string absoluteUri = response.RequestMessage?.RequestUri?.AbsoluteUri;
                if (!string.IsNullOrWhiteSpace(absoluteUri))
                {
                    OnJson(typeof(T), absoluteUri);
                }
            }
            */

            string txt = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return ParseString<T>(txt, triggerJsonEvent: triggerJsonEvent);
            }
            else
            {
                T errResponse = ParseString<T>(txt, triggerJsonEvent: triggerJsonEvent);
                if (errResponse == null)
                {
                    OnException(typeof(T), new System.Exception(txt));
                }
                else
                {
                    return errResponse;
                }
            }

            return default(T);
        }

        protected T ParseString<T>(string str, bool triggerJsonEvent = true)
        {
            if (string.IsNullOrEmpty(str))
                return default(T);

            if (triggerJsonEvent)
            {
                OnJson(typeof(T), str);
            }

            try
            {
                return ParseJson<T>(str);
            }
            catch (Exception ex)
            {
                if (!triggerJsonEvent)
                {
                    OnJson(typeof(T), str);
                }
                throw;
            }
        }

        protected void OnException(Type type, Exception exception)
        {
            this.Exception?.Invoke(this, new ExceptionEventArgs(type, exception));
        }

        protected void OnJson(Type type, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            this.Json?.Invoke(this, new JsonEventArgs(type, message));
        }

        /// <summary>
        /// Checks if user is logged in and has generated an access token
        /// </summary>
        public bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(this.accessToken); }
        }

        /// <summary>
        /// Checks if data socket is open or not
        /// </summary>
        public bool IsDataSocketConnected
        {
            get { return this.dataSocket?.State == WebSocketState.Open; }
        }

        /// <summary>
        /// Checks if order socket is open or not
        /// </summary>
        public bool IsOrderSocketConnected
        {
            get { return this.orderSocket?.State == WebSocketState.Open; }
        }




        /// <summary>
        /// Gets the exceptions
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Exception;
        /// <summary>
        /// Raised when a json string is parsed
        /// </summary>
        public event EventHandler<JsonEventArgs> Json;

        //socket *********************************************************************************************

        /// <summary>
        /// Connect to data socket
        /// </summary>
        /// <param name="pingInterval"></param>
        /// <returns></returns>
        public async Task<bool> ConnectToDataSocketAsync(int pingInterval = 10)
        {
            if (string.IsNullOrEmpty(this.AppId) || string.IsNullOrEmpty(this.accessToken))
                return false;

            //connect to the data socket
            WebSocket dataSocket = this.dataSocket;
            if (dataSocket != null)
            {
                //if (dataSocket.State == WebSocketState.Open)
                //{
                //    dataSocket.Close();
                //}

                dataSocket.Opened -= OnDataSocketOpened;
                dataSocket.Closed -= OnDataSocketClosed;
                dataSocket.MessageReceived -= OnDataMessageReceived;
                dataSocket.Error -= OnDataError;
                dataSocket.DataReceived -= OnDataReceived;
                dataSocket.Dispose();
                this.dataSocket = null;
            }

            this.dataSocket = new WebSocket($"wss://api.fyers.in/socket/v2/dataSock?access_token={this.AppId}:{this.accessToken}", origin: "https://data.fyers.in");
            //this.dataSocket = new WebSocket($"wss://data.fyers.in/dataSock?access_token={this.AppId}:{this.accessToken}&user-agent=fyers-api", origin: "https://data.fyers.in");

            //this.dataSocket = new WebSocket("wss://data.fyers.in/dataSock", userAgent: "fyers-api")

            this.dataSocket.Opened += OnDataSocketOpened;
            this.dataSocket.Closed += OnDataSocketClosed;
            this.dataSocket.MessageReceived += OnDataMessageReceived;
            this.dataSocket.Error += OnDataError;
            this.dataSocket.DataReceived += OnDataReceived;

            this.dataSocket.AutoSendPingInterval = pingInterval;
            this.dataSocket.EnableAutoSendPing = true;

            this.dataSocket.Open();
            return true;

        }

        /// <summary>
        /// Connect to order socket
        /// </summary>
        /// <param name="pingInterval"></param>
        /// <returns></returns>
        public async Task<bool> ConnectToOrderSocketAsync(int pingInterval = 10)
        {
            if (string.IsNullOrEmpty(this.AppId) || string.IsNullOrEmpty(this.accessToken))
                return false;

            WebSocket orderSocket = this.orderSocket;
            
            if (orderSocket != null)
            {
                //if (orderSocket.State == WebSocketState.Open)
                //{
                //    orderSocket.Close();
                //}

                orderSocket.Opened -= OnOrderSocketOpened;
                orderSocket.Closed -= OnOrderSocketClosed;
                orderSocket.MessageReceived -= OnOrderMessageReceived;
                orderSocket.DataReceived -= OnOrderDataReceived;
                orderSocket.Error -= OnOrderError;

                orderSocket.Dispose();
                this.orderSocket = null;
            }

            this.orderSocket = new WebSocket($"wss://data.fyers.in/orderSock?access_token={this.AppId}:{this.accessToken}&user-agent=fyers-api&type=orderUpdate", origin: "https://data.fyers.in");

            this.orderSocket.Opened += OnOrderSocketOpened;
            this.orderSocket.Closed += OnOrderSocketClosed;
            this.orderSocket.MessageReceived += OnOrderMessageReceived;
            this.orderSocket.DataReceived += OnOrderDataReceived;
            this.orderSocket.Error += OnOrderError;
            
            this.orderSocket.AutoSendPingInterval = pingInterval;
            this.orderSocket.EnableAutoSendPing = true;
            
            this.orderSocket.Open();
            
            return true;
        }

        private void OnOrderError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            this.Error?.Invoke(this, new SocketErrorEventArgs(MessageType.Order, e.Exception));
        }

        private void OnOrderDataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            this.DataReceived?.Invoke(this, new DataReceivedEventArgs(MessageType.Order, e.Data));
        }

        private void OnOrderMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(MessageType.Order, e.Message));
        }

        private void OnOrderSocketClosed(object sender, EventArgs e)
        {
            this.Closed?.Invoke(this, new FyersEventArgs(MessageType.Order));
        }

        private async void OnOrderSocketOpened(object sender, EventArgs e)
        {
            //subscribe to the order updates
            await this.SendAsync(new OrderMessage()
            {
                SLIST = new string[] { "orderUpdate" },
                SUB_T = 1
            }, MessageType.Order).ConfigureAwait(false);

            this.Opened?.Invoke(this, new FyersEventArgs(MessageType.Order));
        }

        //

        private void OnDataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            this.DataReceived?.Invoke(this, new DataReceivedEventArgs(MessageType.Data, e.Data));
        }

        private void OnDataError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            this.Error?.Invoke(this, new SocketErrorEventArgs(MessageType.Data, e.Exception));
        }

        private void OnDataMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(MessageType.Data, e.Message));
        }

        private void OnDataSocketClosed(object sender, EventArgs e)
        {
            this.Closed?.Invoke(this, new FyersEventArgs(MessageType.Data));
        }

        private void OnDataSocketOpened(object sender, EventArgs e)
        {
            this.Opened?.Invoke(this, new FyersEventArgs(MessageType.Data));
        }

        
        /// <summary>
        /// Send messages to the socket
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="payload">Payload</param>
        /// <param name="messageType"><see cref="MessageType"/></param>
        /// <returns></returns>
        private async Task<bool> SendAsync<T>(T payload, MessageType messageType) where T : SocketMessageBase
        {
            
            string json = payload?.GetJsonString();
            if (string.IsNullOrEmpty(json))
                return false;


            WebSocket socket = null;

            switch (messageType)
            {
                case MessageType.Data:
                    socket = this.dataSocket;
                    break;
                case MessageType.Order:
                    socket = this.orderSocket;
                    break;
            }

            if (socket == null || socket.State != WebSocketState.Open)
                return false;

            socket.Send(json);
            return true;
        }

        private async Task<bool> SubscribeLevel1(string[] symbols, int sub_t)
        {
            if (symbols == null || symbols.Length == 0)
                return false;

            return await SendAsync<L1Message>(new L1Message()
            {
                SUB_T = sub_t,
                TLIST = symbols
            }, MessageType.Data).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe Level-I data
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <returns></returns>
        public async Task<bool> SubscribeLevel1(string[] symbols)
        {
            return await SubscribeLevel1(symbols, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Un-subscribe Level-II data
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <returns></returns>
        public async Task<bool> UnsubscribeLevel1(string[] symbols)
        {
            return await SubscribeLevel1(symbols, 0).ConfigureAwait(false);
        }


        private async Task<bool> SubscribeLevel2(string[] symbols, int sub_t)
        {
            if (symbols == null || symbols.Length == 0)
                return false;

            return await SendAsync<L2Message>(new L2Message()
            {
                SUB_T = sub_t,
                L2LIST = symbols
            }, MessageType.Data).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe Level-I data
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <returns></returns>
        public async Task<bool> SubscribeLevel2(string[] symbols)
        {
            return await SubscribeLevel2(symbols, 1).ConfigureAwait(false);
        }

        /// <summary>
        /// Un-subscribe Level-II data
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <returns></returns>
        public async Task<bool> UnsubscribeLevel2(string[] symbols)
        {
            return await SubscribeLevel2(symbols, 0).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the App id
        /// </summary>
        public string AppId { get; private set; }

        /// <summary>
        /// Called when the order socket or data socket is closed
        /// </summary>
        public event EventHandler<FyersEventArgs> Closed;
        /// <summary>
        /// Called when the order socket or data socket is opened
        /// </summary>
        public event EventHandler<FyersEventArgs> Opened;
        /// <summary>
        /// Called when the socket receives data
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        /// <summary>
        /// Called when the socket receives any message
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        /// <summary>
        /// Called when the socket throws an error
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> Error;

    }
}
