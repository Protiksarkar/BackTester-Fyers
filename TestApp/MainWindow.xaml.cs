/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using FyersAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        Fyers fyers = null;
        string orderId = string.Empty;

        public MainWindow()
        {
            this.User = new User();

            Properties.Settings settings = Properties.Settings.Default;
            if (settings != null)
            {

                if (!string.IsNullOrWhiteSpace(settings.ApiKey))
                {
                    IsSaved = true;
                    this.User.Key = settings.ApiKey;
                }

                if (!string.IsNullOrWhiteSpace(settings.Secret))
                {
                    IsSaved = true;
                    this.User.Secret = settings.Secret;
                }

                if (!string.IsNullOrWhiteSpace(settings.RedirectUrl))
                {
                    IsSaved = true;
                    this.User.RedirectUrl = settings.RedirectUrl;
                }
                
            }

            InitializeComponent();
            DataContext = this;     //Chill, its just a test app ;)

            if (!ServicePointManager.Expect100Continue)
                ServicePointManager.Expect100Continue = true;

            if (ServicePointManager.SecurityProtocol != SecurityProtocolType.Tls12)
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }

        }
        
        #region Overridden methods


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Properties.Settings settings = Properties.Settings.Default;
            if (settings != null)
            {
                if (this.IsSaved)
                {
                    settings.ApiKey = this.User.Key;
                    settings.Secret = this.User.Secret;
                    settings.RedirectUrl = this.User.RedirectUrl;
                }
                else
                {
                    settings.ApiKey = settings.Secret = settings.RedirectUrl = string.Empty;
                }

                settings.Save();
            }
        }

        #endregion

        #region Misc

        private void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            this.Dispatcher.InvokeAsync(() =>
            {
                this.Logs.Insert(0, $"{DateTime.Now:HH:mm:ss} : {message}");
            });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Logs the API messages
        /// </summary>
        public ObservableCollection<string> Logs { get; private set; } = new ObservableCollection<string>();

        private User user;
        /// <summary>
        /// User parameters
        /// </summary>
        public User User
        {
            get { return user; }
            set
            {
                user = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Binds with Save settings checkBox
        /// </summary>
        public bool IsSaved { get; set; }



        private int selectedIndex = 0;
        /// <summary>
        /// Binds with the TabControl.SelectedIndex
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set 
            {
                selectedIndex = value;
                NotifyPropertyChanged();
            }
        }


        private Uri uri;

        public Uri Uri
        {
            get { return uri; }
            set 
            {
                uri = value;
                NotifyPropertyChanged();
            }
        }




        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Connect

        private async void OrderSocket_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            await this.fyers.ConnectToOrderSocketAsync();
        }


        private async void DataSocket_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            await this.fyers.ConnectToDataSocketAsync();
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null)
                return;

            await this.fyers.LogoutAsync();
        }

        private async void Initialize_Click(object sender, RoutedEventArgs e)
        {
            User user = this.User;
            if (user == null)
                return;

            if (string.IsNullOrWhiteSpace(user.Key))
            {
                MessageBox.Show("Please enter the API Key");
                return;
            }

            if (string.IsNullOrWhiteSpace(user.Secret))
            {
                MessageBox.Show("Please enter the API Secret");
                return;
            }

            if (string.IsNullOrWhiteSpace(user.RedirectUrl))
            {
                MessageBox.Show("Please enter the redirect url");
                return;
            }

            if (this.fyers != null)
            {
                await this.fyers.LogoutAsync();

                this.fyers.Exception -= OnException;
                this.fyers.Json -= OnJson;

                this.fyers.Opened -= OnSocketOpened;
                this.fyers.Closed -= OnSocketClosed;
                this.fyers.DataReceived -= OnDataReceived;
                this.fyers.MessageReceived -= OnMessageReceived;
                this.fyers.Error -= OnSocketerror;
            }

            this.fyers = new Fyers(user.Key);

            this.fyers.Exception += OnException;
            this.fyers.Json += OnJson;

            this.fyers.Opened += OnSocketOpened;
            this.fyers.Closed += OnSocketClosed;
            this.fyers.DataReceived += OnDataReceived;
            this.fyers.MessageReceived += OnMessageReceived;
            this.fyers.Error += OnSocketerror;

            string state = Guid.NewGuid().ToString();
            this.Uri = new Uri($"https://api.fyers.in/api/v2/generate-authcode?client_id={this.User.Key}&redirect_uri={user.RedirectUrl}&response_type=code&state={state}");
        }

        #endregion

        #region Event methods

        private void OnSocketerror(object sender, SocketErrorEventArgs e)
        {
            Log($"OnSocketerror.{e.MessageType} : {e.Exception?.Message}");
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Log($"OnMessageReceived.{e.MessageType} : {e.Message}");
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.MessageType == MessageType.Data)
            { 

                BinaryQuotes.TryParse(e.Data, out BinaryQuotes[] quotes);
                if (quotes != null)
                {
                    int count = 0;
                    foreach (var quote in quotes)
                    {
                        if (quote.Header == null)
                            continue;

                        Log($"Len={e.Data.Length}, {quote}");

                        //Log($"count={++count}, Len={e.Data.Length}, Token={quote.Header.FyersToken}, Time={quote.Header.TimeStamp:yyyyMMMdd HH:mm:ss}, TrCode={quote.Header.TransactionCode}, TradeStatus={quote.Header.TradeStatus}, PacketLen={quote.Header.PacketLength}, IsL2={quote.Header.HasLevelII}");
                    }
                }
            }
            else
            {
                Log($"OnDataReceived.{e.MessageType} : {e.Data.Length}");

            }
        }

        private async void OnSocketClosed(object sender, FyersEventArgs e)
        {
            Log($"OnSocketClosed : {e.MessageType}");
        }

        private void OnSocketOpened(object sender, FyersEventArgs e)
        {
            Log($"OnSocketOpened : {e.MessageType}");
        }

        private void OnJson(object sender, JsonEventArgs e)
        {
            Log($"Json={e.Message}");
        }

        private void OnException(object sender, FyersAPI.ExceptionEventArgs e)
        {
            Log($"Exception={e.Exception.Message}");
        }

        #endregion

        #region data api calls

        private async void Subscribe_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            await this.fyers.SubscribeLevel1(new string[] { "NSE:SBIN-EQ" }); //10100000003045

        }

        private async void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            await this.fyers.UnsubscribeLevel1(new string[] { "NSE:SBIN-EQ" });
            
        }

        private async void SubscribeL2_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            await this.fyers.SubscribeLevel2(new string[] { "NSE:SBIN-EQ" });
        }

        private async void UnsubscribeL2_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            await this.fyers.UnsubscribeLevel1(new string[] { "NSE:SBIN-EQ" });
        }

        private async void Quote_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GetQuotesAsync(new string[] { "NSE:SBIN-EQ", "NSE:ONGC-EQ" });
        }

        private async void MarketDepth_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GetMarketDepthAsync("NSE:SBIN-EQ");
        }

        private async void History_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            DateTime from = DateTime.Now.AddDays(-360).Date;
            DateTime to = DateTime.Now.Date.AddDays(1).AddTicks(-1);

            var response = await this.fyers.GetHistoricalDataAsync("NSE:SBIN-EQ", "D", from, to);
            //var response = await this.fyers.GetHistoricalDataAsync("NSE:NIFTY21SEPFUT", "D", Globals.ToUnixTime(from), Globals.ToUnixTime(to), contFlag: 1);
            
            if (response == null)
                return;
            //The json event is not triggered for historical data. Thus we log the response message
            Log($"{response.message} count={response.candles?.Length}");

            
        }

        #endregion

        #region Order placement

        private async void Buy_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.PlaceOrderAsync(new PlaceOrderPayload()
            {
                disclosedQty = 0,
                limitPrice = 100,
                offlineOrder = "False",
                productType = ProductTypes.MARGIN,
                qty = 1,
                takeProfit = 0.0m,
                side = OrderSide.Buy,
                stopLoss = 0.0m,
                stopPrice = 0.0m,
                symbol = "NSE:ONGC-EQ",
                type = (int)OrderType.Limit,
                validity = OrderValidity.DAY
            });

            if (response != null && !string.IsNullOrEmpty(response.id))
                this.orderId = response.id;
        }

        private async void Modify_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            if (string.IsNullOrWhiteSpace(orderId))
                return;

            var response = await this.fyers.ModifyOrderAsync(new ModifyPricePayload() //modifying the price only
            {
                id = orderId,
                limitPrice = 102.0m,
                side = OrderSide.Buy,
                stopPrice = 0.0m,
                type = (int)OrderType.Limit
            });
        }

        private async void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            if (string.IsNullOrWhiteSpace(orderId))
                return;

            var response = await this.fyers.CancelOrderAsync(orderId);
        }

        #endregion

        #region Transactional Info

        private async void OrderStatus_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            if (string.IsNullOrEmpty(this.orderId))
                return;

            var response = await this.fyers.GetOrderStatusAsync(this.orderId);

        }

        private async void TradeInfo_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;
            
            var response = await this.fyers.GetTradeBookAsync();
        }

        

        private async void OrderBook_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GetOrderBookAsync();
        }

        

        private async void Position_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GetPositionAsync();
        }

        #endregion

        #region Misc API calls

        private async void Master_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await Fyers.DownloadMasterAsync("NSE_CM");

        }

        #endregion

        #region User

        private async void Margin_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GetFundsAsync();
        }

        private async void Holding_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GetHoldingAsync();
        }

        #endregion

        #region WebBrowser

        

        private void WebView2_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            
            if (e.IsSuccess)
            {
                Microsoft.Web.WebView2.Wpf.WebView2 webView2 = sender as Microsoft.Web.WebView2.Wpf.WebView2;
                var cookieManager = webView2?.CoreWebView2?.CookieManager;

                if (cookieManager != null)
                {
                    webView2.CoreWebView2.CookieManager.DeleteAllCookies();

                    
                }
            } 
        }

        private async void WebView2_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs obj)
        {
            User user = this.User;
            if (user == null)
                return;

            if (obj == null || string.IsNullOrEmpty(obj.Uri))
                return;

            Log(obj.Uri);


            if (obj.Uri.Contains(user.RedirectUrl))
            {
                if (this.fyers != null && Fyers.IsValidLogin(obj.Uri, user.Key, user.Secret, out TokenPayload payload))
                {
                    var response = await this.fyers.GenerateTokenAsync(payload);

                    if (response != null && !string.IsNullOrEmpty(response.access_token))
                    {
                        this.SelectedIndex = 1;
                    }
                }
                else
                {
                    Log("nyet");
                }
            }
            //else if (obj.Uri.Contains("res://ieframe.dll/navcancl.htm"))
            else
            {
                this.Log("Login failed. Please make sure you have entered a valid API Key and API Secret");
            }
        }



        #endregion

        #region EDIS

        private async void GenerateTPin_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GenerateTPinAsync();

        }


        private async void GenerateCdsl_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.PostEDisRequestAsync(new EDisPayload()
            {
                recordLst = new List<Recordlst>()
                 {
                     new Recordlst() { isin_code = "INE494B01023", qty = 50},    //TVS Motors
                     //new Recordlst(){ isin_code = "INE476A01014", qty = 1},     //Canara Bank
                     //new Recordlst(){ isin_code = "INE009A01021", qty = 1}    //Infosys
                 }
            });

            if (response.code == 200 && !string.IsNullOrWhiteSpace(response.data))
            {
                if (edisWb.CoreWebView2 == null)
                {
                    await edisWb.EnsureCoreWebView2Async(null);
                } 
                
                edisWb.NavigateToString(response.data);
            }
        }


        public async void GetEdisDetails_Click(object sender, RoutedEventArgs e)
        {
            if (this.fyers == null || !this.fyers.IsAuthenticated)
                return;

            var response = await this.fyers.GetEdisDetailsAsync();

            if (response != null && response.code == 200 && response.data != null && response.data.Length > 0)
            {
                var response1 = await this.fyers.PostEDisInquiryAsync(new EDisInquiryPayload()
                {
                    transactionId = response.data[0].transactionId
                });

                if (response1 != null)
                {
                    Log($"Msg={response1.message}, success={response1.data?.SUCEESS_CNT}, failure={response1.data?.FAILED_CNT}");
                }
            }
        }


        #endregion

        
    }
}
