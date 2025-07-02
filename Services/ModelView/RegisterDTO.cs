using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ModelView
{
    public class RegisterDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PassWord { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
