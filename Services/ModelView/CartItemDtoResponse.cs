using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ModelView
{
    public class CartItemDtoResponse
    {
        public int ItemId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public ProductDtoResponse ProductVIew { get; set; } = new ProductDtoResponse();
    }
}
