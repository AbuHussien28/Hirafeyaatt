using Hirafeyat.CustomersPaymentsSerives;
using Hirafeyat.ViewModel.Payments;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Hirafeyat.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }
        [HttpGet]
        public IActionResult PaymentPay()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Checkout(int orderId)
        {
            try
            {
                var paymentViewModel = await paymentService.GetByIdAsync(orderId);
                return View(paymentViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("PaymentFailed");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string paymentIntentId)
        {
            var result = await paymentService.ProcessPaymentAsync(orderId, paymentIntentId);

            if (result.Success)
            {
                return RedirectToAction("PaymentSuccess", new { orderId = result.OrderId });
            }

            return RedirectToAction("PaymentFailed", new
            {
                orderId = result.OrderId,
                error = result.ErrorMessage
            });
        }
        [HttpGet]
        public IActionResult PaymentSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
        [HttpGet]
        public IActionResult PaymentFailed(int orderId)
        {
            ViewBag.OrderId = orderId;
            ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString() ?? "Payment processing failed";
            return View();
        }
    }
}
