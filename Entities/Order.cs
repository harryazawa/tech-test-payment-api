using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tech_test_payment_api.Entities
{

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int SellerId { get; set; }
        public string Items { get; set; }
        public string AvailableStatus { get; set; }
        public string OrderStatus { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}