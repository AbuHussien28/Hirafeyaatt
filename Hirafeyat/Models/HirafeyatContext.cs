﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.Models
{
    public class HirafeyatContext:IdentityDbContext<ApplicationUser>
    {
        public HirafeyatContext():base()
        {
            
        }
        public HirafeyatContext(DbContextOptions<HirafeyatContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

    }
}
