using Hirafeyat.ViewModel.OrderCustomer;
using Stripe;

namespace Hirafeyat.CustomerRepository
{
    public class OrderCustoemrService: IOrderCustomerService
    {
        private readonly IOrderCustomerRepository repo;
        private readonly IConfiguration configuration;

        public OrderCustoemrService(IOrderCustomerRepository _repo, IConfiguration configuration)
        {
            repo = _repo;
            this.configuration = configuration;
            //StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            var stripeSecretKey = configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(stripeSecretKey))
            {
                StripeConfiguration.ApiKey = stripeSecretKey;
            }
            else
            {
                throw new Exception("Stripe Secret Key is not configured");
            }
        }
        public async Task<CheckoutPageViewModel> PrepareCheckoutAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be empty");
            }

            var cartItems = await repo.GetCartItemsByUserIdAsync(userId);
            if (cartItems == null || !cartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            var total = cartItems.Sum(c => c.Product.Price * c.Quantity);
            var publishableKey = configuration["Stripe:PublishableKey"];

            if (string.IsNullOrEmpty(publishableKey))
            {
                throw new Exception("Stripe Publishable Key is not configured");
            }

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(total * 100),
                    Currency = "egp",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);

                return new CheckoutPageViewModel
                {
                    CheckoutForm = new CheckoutViewModel(),
                    CartItems = cartItems,
                    TotalPrice = total,
                    ClientSecret = intent.ClientSecret,
                    StripePublishableKey = publishableKey
                };
            }
            catch (StripeException ex)
            {
                throw new Exception($"Payment processing error: {ex.Message}");
            }
        }

        public async Task<string> ProcessOrderAsync(string userId, CheckoutPageViewModel model, string paymentMethod, string paymentIntentId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be empty");
            }

            // Get cart items
            var cartItems = await repo.GetCartItemsByUserIdAsync(userId);

            // Check if cart items exist
            if (cartItems == null || !cartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            // Create order
            var order = new Order
            {
                CustomerId = userId,
                FullName = model.CheckoutForm.FullName,
                Address = model.CheckoutForm.Address,
                PhoneNumber = model.CheckoutForm.PhoneNumber,
                Email = model.CheckoutForm.Email,
                Status = paymentMethod == "cod" ? OrderStatus.Processing : OrderStatus.Pending,
                OrderItems = new List<OrderItem>() 
            };

            // Add order items
            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            var payment = new Payment
            {
                Order = order,
                Amount = cartItems.Sum(c => c.Product.Price * c.Quantity),
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod,
                Status = paymentMethod == "cod" ? PaymentStatus.Paid : PaymentStatus.Pending
            };

            if (paymentMethod == "stripe" && !string.IsNullOrEmpty(paymentIntentId))
            {
                payment.StripePaymentIntentId = paymentIntentId;
                if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
                {
                    StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
                }

                var paymentIntentService = new PaymentIntentService();
                var intent = await paymentIntentService.GetAsync(paymentIntentId);

                if (intent.Status == "succeeded")
                {
                    payment.Status = PaymentStatus.Paid;
                    order.Status = OrderStatus.Processing;
                }
            }

            await repo.AddOrderAsync(order);
            await repo.AddPaymentAsync(payment);
            repo.RemoveCartItems(cartItems);
            await repo.SaveChangesAsync();

            return order.Id.ToString();
        }

        public async Task<List<OrderWithPaymentViewModel>> GetUserOrdersAsync(string userId)
        {
            var orders = await repo.GetUserOrdersAsync(userId);

            return orders.Select(o => new OrderWithPaymentViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                OrderItems = o.OrderItems.ToList(),
                TotalAmount = o.OrderItems.Sum(i => i.Product.Price * i.Quantity),
                PaymentStatus = o.Payment?.Status ?? PaymentStatus.Pending,
                PaymentMethod = o.Payment?.PaymentMethod
            }).ToList();

        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await repo.GetOrderByIdAsync(orderId, userId);
            if (order == null || order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                return false;

            order.Status = OrderStatus.Cancelled;
            await repo.SaveChangesAsync();
            return true;
        }
    }
}
