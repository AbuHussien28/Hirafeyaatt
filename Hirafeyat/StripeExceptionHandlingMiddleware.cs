using Stripe;

namespace Hirafeyat
{
    public class StripeExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public StripeExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (StripeException ex)
            {
                context.Response.Redirect($"/Payment/PaymentFailed?error={Uri.EscapeDataString(ex.Message)}");
            }
        }
    }
}
