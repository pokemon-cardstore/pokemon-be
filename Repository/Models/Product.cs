using System;
using System.Collections.Generic;

namespace Repository.Models
{
    public partial class Product
    {
        public Product()
        {
            CartItems = new HashSet<CartItem>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
        }

        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
