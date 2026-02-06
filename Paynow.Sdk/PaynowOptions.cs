namespace Paynow.Sdk
{
    public class PaynowOptions
    {
        public string ApiUrl { get; set; } = "https://api.sandbox.paynow.pl";
        public string ApiKey { get; set; }
        public string SignatureKey { get; set; }
    }
}
