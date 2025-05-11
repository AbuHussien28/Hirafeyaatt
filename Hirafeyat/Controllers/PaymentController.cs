
using Hirafeyat.CustomersPaymentsSerives;
using Hirafeyat.ViewModel.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Hirafeyat.Controllers
{
    [CustomerAuthorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
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
                return RedirectToAction("PaymentFailed", new { orderId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int orderId, string paymentIntentId)
        {
            try
            {
                var result = await paymentService.ProcessPaymentAsync(orderId, paymentIntentId);

                if (!result.Success)
                {
                    return RedirectToAction("PaymentFailed", new
                    {
                        orderId = result.OrderId,
                        error = result.ErrorMessage
                    });
                }

                return RedirectToAction("PaymentSuccess", new { orderId = result.OrderId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("PaymentFailed", new
                {
                    orderId,
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult PaymentSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        [HttpGet]
        public IActionResult PaymentFailed(int orderId, string error = null)
        {
            ViewBag.OrderId = orderId;
            ViewBag.ErrorMessage = error ?? TempData["ErrorMessage"]?.ToString() ??
                "Payment processing failed";
            return View();
        }
    
}
}
