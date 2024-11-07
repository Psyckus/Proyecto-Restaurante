using System.Collections.Generic;
using System.Threading.Tasks;
using PayPal.Api;  // Asegúrate de tener el SDK de PayPal instalado

namespace MVW_Proyecto_Mesas_Comida.Services
{
    public class PayPalService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode;

        public PayPalService(string clientId, string clientSecret, string mode = "sandbox") // `mode` opcional
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _mode = mode;
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", _mode },  // Usa el modo de la configuración
                { "clientId", _clientId },
                { "clientSecret", _clientSecret }
            };

            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            return new APIContext(accessToken);
        }

        public Payment CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
        {
            var apiContext = GetAPIContext();

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount
                        {
                            currency = currency,
                            total = amount.ToString("F2")  // Total debe ser con dos decimales
                        },
                        description = "Compra en MVW Proyecto Mesas Comida"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };

            return payment.Create(apiContext);
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetAPIContext();
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new Payment { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}
