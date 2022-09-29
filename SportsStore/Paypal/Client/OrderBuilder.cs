using System.Collections.Generic;
using PayPalCheckoutSdk.Orders;
using SportsStore.Models;
using Order = SportsStore.Models.Order;

namespace SportsStore.Paypal.Client
{
    public static class OrderBuilder
    {
        private static Cart cart;

        public static OrderRequest Build(Order order)
        {
            //cart = cartService;

            if (order.Lines.Count == 0)
                return null;

            OrderRequest orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    BrandName = "SportStore",
                    LandingPage = "LOGIN",
                    UserAction = "PAY_NOW",
                    ReturnUrl = "http://localhost:5000/Checkout/PaymentCompleted",
                    CancelUrl = "http://localhost:5000/Checkout/PaymentCancelled",
                    ShippingPreference = "SET_PROVIDED_ADDRESS",      // use our shipping (can us PayPals)
                    Locale = "en-AU"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId = "joshua.turner@cqumail.com", // [required] The merchant ID 
                        Description = "Sporting Goods",
                        //CustomId = "my_first_sale",
                        SoftDescriptor = "SportsStore",

                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "AUD",  // may need to pass in 

                            Value = cart.ComputeTotalValue().ToString(),

                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = "AUD",
                                    Value = order.ComputeTotalValue().ToString()
                                }

                                /* Remove unneccesary fields below */
                                /*Discount = new Money
                                {
                                    CurrencyCode = cart.CurrencyCode,
                                    Value = cart.Discount.ToString()
                                },
                                TaxTotal = new Money
                                {
                                    CurrencyCode = cart.CurrencyCode,
                                    Value = cart.TaxTotal.ToString()
                                },
                                Shipping = new Money
                                {
                                    CurrencyCode = cart.CurrencyCode,
                                    Value = cart.Shipping.ToString()
                                }*/
                            }
                        },

                        ShippingDetail = new ShippingDetail
                        {
                            Name = new Name
                            {
                                FullName = order.Name
                            },
                            AddressPortable = new AddressPortable
                            {
                                AddressLine1 = order.AddressLine1,
                                AddressLine2 = order.AddressLine2,
                                AddressLine3 = order.AddressLine3,
                                AdminArea2 = order.City,
                                AdminArea1 = order.State,
                                PostalCode = order.ZipCode,
                                CountryCode = order.Country
                            }
                        },
 
                        Items = new List<Item>()   // added from cart below
                   }
                }
            };

            foreach (var line in order.Lines)
            {
                orderRequest.PurchaseUnits[0]
                            .Items
                            .Add(new Item
                            {
                                Name = line.Product.Name,
                                Description = line.Product.Description,

                                UnitAmount = new Money
                                {
                                    CurrencyCode = "AUD",
                                    Value = line.Product.Price.ToString()
                                },

                                /* Remove unnecessary fields below */

                                /*Tax = new Money
                                {
                                    CurrencyCode = cart.CurrencyCode,
                                    Value = line.Product.Tax.ToString()
                                },*/

                                Quantity = line.Quantity.ToString(),
                                Category = "PHYSICAL_GOODS"
                            });
            }

            return orderRequest;
        }

    }
}
