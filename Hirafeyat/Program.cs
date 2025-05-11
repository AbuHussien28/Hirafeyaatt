using Hirafeyat.AdminRepository;
using Hirafeyat.AdminServices;
using Hirafeyat.CustomerRepository;
using Hirafeyat.CustomersPaymentsRepo;
using Hirafeyat.CustomersPaymentsSerives;
using Hirafeyat.EmailServices;
using Hirafeyat.Models;
using Hirafeyat.SellerServices;
using Hirafeyat.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var googleAuthSettings = builder.Configuration.GetSection("GoogleAuthentication");
            string clientId = googleAuthSettings["ClientId"];
            string clientSecret = googleAuthSettings["ClientSecret"];
            // Add services to the container.
            builder.Services.AddControllersWithViews()
      .AddJsonOptions(options =>
      {
          options.JsonSerializerOptions.PropertyNamingPolicy = null;
      });
           
            builder.Services.AddDbContext<HirafeyatContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("CS")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<HirafeyatContext>()
                .AddDefaultTokenProviders();

            //regester service
            builder.Services.AddScoped<AdminRepository.IProductRepository, AdminRepository.ProductRepository>();
            builder.Services.AddScoped<AdminServices.IProductService, AdminServices.ProductService>();

            builder.Services.AddScoped<AdminRepository.ICategoryRepository, AdminRepository.CategoryRepository>();
            builder.Services.AddScoped<AdminServices.ICategoryService, AdminServices.CategoryService>();

            builder.Services.AddScoped<SellerServices.IProductRepository, SellerServices.ProductService>();
            builder.Services.AddScoped<SellerServices.ICategoryRepository, SellerServices.CategoryService>();

            builder.Services.AddScoped<SellerServices.IProductRepository, SellerServices.ProductService>();

            builder.Services.AddScoped<SellerServices.IOrderItemsRepository, SellerServices.OrderItemsService>();
            builder.Services.AddScoped<IOrderService,OrderService>();
            builder.Services.AddScoped<SellerServices.IProductRepository, SellerServices.ProductService>();
            builder.Services.AddScoped<SellerServices.ICategoryRepository, SellerServices.CategoryService>();
            builder.Services.AddScoped<IOrderCustomerRepository, OrderCustomerRepository>();
            builder.Services.AddScoped<IOrderCustomerService, OrderCustoemrService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderRepositoryAdmin, OrderRepositoryAdmin>();
            builder.Services.AddScoped<IOrderAdminService, OrderAdminService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.CallbackPath = "/signin-google";
           
            });
            builder.Services.AddSingleton<StripeConfigService>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
   .AddCookie(options =>
   {
       options.LoginPath = "/Account/Login";
       options.AccessDeniedPath = "/Account/AccessDenied";
       options.ExpireTimeSpan = TimeSpan.FromDays(30);
       options.SlidingExpiration = true;
       options.Cookie.HttpOnly = true;
       options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
       options.Cookie.SameSite = SameSiteMode.Lax;
   });
            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
          
            app.UseStaticFiles();
            app.MapStaticAssets();
            app.UseMiddleware<StripeExceptionHandlingMiddleware>();

            app.MapControllerRoute(
                name: "default",
                  //pattern: "{controller=Seller}/{action=Orders}")

                //  pattern: "{controller=Account}/{action=Login}")
                //pattern: "{controller=Role}/{action=NewRole}")
                // pattern: "{controller=User}/{action=Sellers}")
                //pattern: "{controller=AdminOrder}/{action=Index}")
                //pattern: "{controller=User}/{action=Customers}")
                pattern: "{controller=Home}/{action=Index}/{id?}")
                //pattern: "{controller=Home}/{action=Index}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
