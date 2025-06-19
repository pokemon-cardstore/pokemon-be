using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ModelView
{
    public class OrderDetailDtoRequest
    {
        public int ProductId { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Quantity { get; set; }
    }
}
