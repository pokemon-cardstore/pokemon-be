using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
        }

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
