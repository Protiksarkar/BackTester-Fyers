using FyersAPI;
using TestingConsole.DBServices;
using TestingConsole.Models;

namespace TestingConsole.Services
{
    public class TradingService
    {
        private TradingRepo repo;
        private Fyers fyers;

        public TradingService(TradingRepo tdRepo, Fyers fyrs)
        {
            repo = tdRepo;
            fyers = fyrs;
        }

        public string PlaceOrder()
        {
            var response = fyers.PlaceOrderAsync(new PlaceOrderPayload()
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
            }).Result;

            if (response != null && !string.IsNullOrEmpty(response.id))
                return response.id;
            else
                return null;
        }

        public static async Task LoginAsync(User user, Fyers fyers)
        {
            string state = Guid.NewGuid().ToString();
            var uri = $"https://api.fyers.in/api/v2/generate-authcode?client_id={user.Key}&redirect_uri={user.RedirectUrl}&response_type=code&state={state}";

            Console.WriteLine("Go to Browser and login to {0}\nPaste new url after login", uri);
            uri = Console.ReadLine().Trim();

            if (user == null)
            {
                Console.WriteLine("Login failed. Please make sure you have entered a valid API Key and API Secret");
                return;
            }

            var resp = Fyers.IsValidLogin(uri, user.Key, user.Secret, out TokenPayload payload);

            if (fyers != null && resp)
            {
                var response = await fyers.GenerateTokenAsync(payload);

                if (response != null && !string.IsNullOrEmpty(response.access_token))
                {
                    Console.WriteLine("Successfully logged in");
                }
                else
                {
                    Console.WriteLine("Login failed. Please make sure you have entered a valid API Key and API Secret");
                }
            }
            else
            {
                Console.WriteLine("Login failed. Please make sure you have entered a valid API Key and API Secret");
            }
        }
    }
}
