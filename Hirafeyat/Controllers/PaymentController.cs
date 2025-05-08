using Hirafeyat.CustomersPaymentsSerives;
using Hirafeyat.ViewModel.Payments;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> Checkout(int orderId)
        {
            try
            {
                var order = await paymentService.GetByIdAsync(orderId);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found";
                    return RedirectToAction("Failure");
                }

                await paymentService.HandleStripePaymentAsync(order);

                TempData["OrderId"] = order.OrderId;
                TempData["Amount"] = order.Amount.ToString();
                TempData["CustomerEmail"] = order.CustomerEmail;

                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Failure");
            }
        }



        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Failure()
        {
            return View();
        }
    }
}
