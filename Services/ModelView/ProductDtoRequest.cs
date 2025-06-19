using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ModelView
{
    public class ProductDtoRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public List<ProductImageView> Images { get; set; } = new List<ProductImageView>();
    }
}
