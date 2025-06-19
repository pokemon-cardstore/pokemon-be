using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ModelView
{
    public class CustomerDtoResponse
    {
        public int? CustomerId { get; set; }
        public string? Email { get; set; }
        public int? Status { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime? DoB { get; set; }
        public string? Avatar { get; set; }
    }
}
