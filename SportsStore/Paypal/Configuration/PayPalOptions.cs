namespace SportsStore.Paypal.Configuration
{
    public class PayPalOptions
    {
        public string PayPalLiveClientId { get; set; }
        public string PayPalLiveClientSecret { get; set; }
        public string PayPalSandboxClientId { get; set; }
        public string PayPalSandboxClientSecret { get; set; }
        public string CurrencyCode { get; set; }
    }
}
