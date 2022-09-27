using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_test_payment_api.Entities;

namespace tech_test_payment_api.Context{

    public class DbOrdersContext : DbContext
    {
        public DbOrdersContext(DbContextOptions<DbOrdersContext> options) : base(options)
        { }

        public DbSet<Seller> OrderSellers { get; set; }
        public DbSet<Order> OrdersTable { get; set; }
    }
}