using Hirafeyat.CustomersPaymentsRepo;
using Hirafeyat.Models;
using Hirafeyat.Services;
using Hirafeyat.ViewModel.Payments;
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
        public async Task HandleStripePaymentAsync(CheckoutViewModel paymentViewModel)
        {
            //var order = await paymentRepository.GetByIdAsync();
            //if (order == null)
            //    throw new Exception("Order not found");

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
                //CustomerEmail = paymentViewModel.CustomerEmail,
                Amount = paymentViewModel.Amount,
                PaymentDate = DateTime.UtcNow,
                OrderId = paymentViewModel.OrderId,
               Status = PaymentStatus.Paid
            };
            
            await paymentRepository.AddAsync(payment);

            await emailSender.SendEmailAsync(
                paymentViewModel.CustomerEmail,
                "Payment Confirmation",
                $"Thank you! Your order #{paymentViewModel.OrderId} was successful. <br> Amount: {paymentViewModel.Amount:C}  <br> Date: {DateTime.UtcNow:yyyy-MM-dd}"
            );
        }

      public  async Task<CheckoutViewModel> GetByIdAsync(int id)
        {
            var order = await paymentRepository.GetOrderWithCustomerAsync(id);

            if (order == null)
                throw new Exception("Order not found");
            if (order.Customer == null)
                throw new Exception("User information missing");
            if (order.Product == null)
                throw new Exception("Product information missing");

            return new CheckoutViewModel
            {
                OrderId = order.Id,
                CustomerName = order.Customer.FullName,  // Assuming FullName exists in ApplicationUser
                CustomerEmail = order.Customer.Email,
                ProductName = order.Product.Title,
                Address = order.Address,
                Amount = order.Product.Price
            };
        }
    }
}
