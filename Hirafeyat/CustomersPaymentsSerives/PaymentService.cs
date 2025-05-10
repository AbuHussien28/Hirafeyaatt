using Hirafeyat.CustomersPaymentsRepo;
using Hirafeyat.Models;
using Hirafeyat.Services;
using Hirafeyat.ViewModel.Payments;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Hirafeyat.CustomersPaymentsSerives
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IEmailSender emailSender;

        public PaymentService(IPaymentRepository paymentRepository, IEmailSender emailSender)
        {
            this.paymentRepository = paymentRepository;
            this.emailSender = emailSender;
        }
        public async Task<PaymentViewModel> GetByIdAsync(int id)
        {
            var order = await paymentRepository.GetOrderWithCustomerAsync(id);

            if (order == null)
                throw new Exception("Order not found");
            if (order.Order.Customer == null)
                throw new Exception("User information missing");

            var totalPrice = order.Product.Price * order.Quantity;
            return new PaymentViewModel
            {
                OrderId = order.Id,
                CustomerEmail = order.Order.Customer.Email,
                Amount = totalPrice
            };
        }

        public async Task HandleStripePaymentAsync(PaymentViewModel paymentViewModel)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(paymentViewModel.Amount * 100),
                Currency = "usd",
                ReceiptEmail = paymentViewModel.CustomerEmail,
                Metadata = new Dictionary<string, string>
        {
            { "OrderId", paymentViewModel.OrderId.ToString() }
        }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            var payment = new Payment
            {
                StripePaymentIntentId = intent.Id,
                Amount = paymentViewModel.Amount,
                PaymentDate = DateTime.UtcNow,
                OrderId = paymentViewModel.OrderId,
                Status = PaymentStatus.Paid
            };

            await paymentRepository.AddAsync(payment);
            await paymentRepository.UpdateOrderAndPaymentStatusAsync(
                paymentViewModel.OrderId,
                OrderStatus.Processing,
                PaymentStatus.Paid);

            await emailSender.SendEmailAsync(
                paymentViewModel.CustomerEmail,
                "Payment Confirmation",
                $"Thank you! Your order #{paymentViewModel.OrderId} was successful. <br> Amount: {paymentViewModel.Amount:C}  <br> Date: {DateTime.UtcNow:yyyy-MM-dd}"
            );
        }
        public async Task<PaymentResult> ProcessPaymentAsync(int orderId, string paymentIntentId)
        {
            try
            {
                var paymentService = new PaymentIntentService();
                var intent = await paymentService.GetAsync(paymentIntentId);

                if (intent.Status != "succeeded")
                {
                    return new PaymentResult
                    {
                        Success = false,
                        ErrorMessage = "Payment not completed",
                        OrderId = orderId
                    };
                }
                await paymentRepository.UpdateOrderAndPaymentStatusAsync(
                    orderId,
                    OrderStatus.Processing,
                    PaymentStatus.Paid,
                    paymentIntentId);
                var order = await paymentRepository.GetOrderWithCustomerAsync(orderId);
                if (order?.Order?.Customer != null)
                {
                    await emailSender.SendEmailAsync(
                        order.Order.Customer.Email,
                        "Payment Confirmation",
                        $"Thank you for your payment for order #{orderId}"
                    );
                }

                return new PaymentResult
                {
                    Success = true,
                    OrderId = orderId
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    OrderId = orderId
                };
            }
        }

    }


}
