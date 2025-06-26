using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Interface
{
    public interface IEmailService
    {
        Task SendActivationEmailAsync(string email, string activationLink);
    }
}
