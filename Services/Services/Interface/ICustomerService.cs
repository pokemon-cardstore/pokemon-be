using Services.ModelView;
using Repository.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service.Interface
{
    public interface ICustomerService
    {
        Task<List<CustomerDtoResponse>> GetAllCustomers();

        Task<CustomerDtoResponse?> GetCustomerById(int id);

        Task<bool> IsDublicatedEmail(string email);

        Task CreateCustomerAccount (CustomerDtoRequest request);

        Task UpdateCustomerInformation (int CustomerId, UpdateCustomerDtoRequest request);

        Task UpdateCustomerStatus(int CustomerId, int status);
    }
}
