using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoesOnContainers.Services.OrderApi.Models;

namespace ShoesOnContainers.Services.OrderApi.Data
{
    public class OrdersContext:DbContext
    {

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public OrdersContext(DbContextOptions options) : base(options)
        {
             
        }
        //public OrdersContext() { }
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    var envs = Environment.GetEnvironmentVariables();
        //    var host = envs["DBHOST"];

        //    options.UseMySql($"server={host};userid=root;pwd=order123;port=3306;database=OrderDb");
        //}
        //public DbSet<PaymentMethod> Payments { get; set; }

        //public DbSet<Buyer> Buyers { get; set; }

        //public DbSet<CardType> CardTypes { get; set; }





    }
}
