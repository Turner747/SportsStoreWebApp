using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PayPalCheckoutSdk.Orders;
using SportsStore.Models;
using SportsStore.Paypal.Client;
using SportsStore.Paypal.Configuration;

namespace SportsStore.Paypal.Controllers
{
    public class PayPalPaymentsController : Controller
    {
        private readonly PayPalOptions _options;
        private bool debug = true;

        public PayPalPaymentsController(IOptions<PayPalOptions> config)
        {
            this._options = config.Value;
            @ViewBag.clientId = _options.PayPalSandboxClientId;
        }

        [HttpPost]
        [Route("/paypal/create-paypal-transaction")]
        public async Task<PayPalTransactionResponse> PayPalCreate()
        {
            var request = new OrdersCreateRequest();

            request.Prefer("return=representation");

            request.RequestBody(OrderBuilderSample.BuildRequestBody());

            try
            {
                var response = await PayPalClient.Client(_options).Execute(request);

                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

                if (debug)
                {
                    DebugTransaction(result);
                }

                return new PayPalTransactionResponse { OrderId = result.Id, Status = "OK" };
            }
            catch (Exception e)
            {
                return new PayPalTransactionResponse { OrderId = "", Status = "ERROR" };
            }
        }

        private void DebugTransaction(PayPalCheckoutSdk.Orders.Order orderResult)
        {
            Console.WriteLine($"Status: {orderResult.Status}");
            Console.WriteLine($"Order ID: {orderResult.Id}");
            Console.WriteLine($"Intent: {orderResult.CheckoutPaymentIntent}");
            Console.WriteLine($"Links:");

            foreach (LinkDescription link in orderResult.Links)
            {
                Console.WriteLine($"\t{link.Rel}: {link.Href}\tCall Type: {link.Method}");
            }

            AmountWithBreakdown amount = orderResult.PurchaseUnits[0].AmountWithBreakdown;
            Console.WriteLine($"Total Amount: {amount.CurrencyCode} {amount.Value}");
        }

        [HttpPost]
        [Route("/paypal/capture-paypal-transaction/{orderId}")]
        public async Task<PayPalTransactionResponse> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);

            request.Prefer("return=representation");

            try
            {
                request.RequestBody(new OrderActionRequest());

                var response = await PayPalClient.Client(_options).Execute(request);

                PayPalCheckoutSdk.Orders.Order result = 
                        response.Result<PayPalCheckoutSdk.Orders.Order>();

                if (debug)
                {
                    DebugCapture(result);
                }

                return new PayPalTransactionResponse { OrderId = result.Id, Status = "OK" };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Processing Error: {e.Message}");
            }

            return new PayPalTransactionResponse { OrderId = "", Status = "ERROR" };
        }

        private void DebugCapture(PayPalCheckoutSdk.Orders.Order orderResult)
        {
            Console.WriteLine($"Status: {orderResult.Status}");
            Console.WriteLine($"Order ID: {orderResult.Id}");
            Console.WriteLine($"Intent: {orderResult.CheckoutPaymentIntent}");
            
            Console.WriteLine($"Links:");
            foreach (LinkDescription link in orderResult.Links)
            {
                Console.WriteLine($"\t{link.Rel}: {link.Href}\tCall Type: {link.Method}");
            }

            Console.WriteLine("Capture IDs:");
            foreach (PurchaseUnit purchaseUnit in orderResult.PurchaseUnits)
            {
                foreach (PayPalCheckoutSdk.Orders.Capture capture in purchaseUnit.Payments.Captures)
                {
                    Console.WriteLine($"\t{capture.Id}");
                }
            }

            AmountWithBreakdown amount = orderResult.PurchaseUnits[0].AmountWithBreakdown;
            Console.WriteLine("Buyer:");
            Console.WriteLine($"\tEmail Address: {orderResult.Payer.Email}\n\tName: {orderResult.Payer.Name.FullName}");
        }
    }
}
