using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ModelView
{
    public class CustomerDtoRequest
    {
        public string? Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime? DoB { get; set; }
    }
}
