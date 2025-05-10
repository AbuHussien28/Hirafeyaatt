using Stripe;

namespace Hirafeyat
{
    public class StripeConfigService
    {
        public StripeConfigService(IConfiguration configuration)
        {
            var secretKey = configuration["Stripe:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("Stripe Secret Key is not configured");
            }
            StripeConfiguration.ApiKey = secretKey;
        }
    }
}
