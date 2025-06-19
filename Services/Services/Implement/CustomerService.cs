using AutoMapper;
using Azure.Core;
using Services.ModelView;
using Repository.UnitOfWork.Interface;
using Repository.Models;
using Services.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service.Implement
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateCustomerAccount(CustomerDtoRequest request)
        {
            try
            {
                var customer = _mapper.Map<Customer>(request);
                customer.HashedPassword = await HashPassword(request.Password);
                customer.Status = 1;
                await _unitOfWork.CustomerRepository.InsertAsync(customer);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CustomerDtoResponse>> GetAllCustomers()
        {
            try
            {
                var response = new List<CustomerDtoResponse>();
                var customers = await _unitOfWork.CustomerRepository.GetAsync();
                foreach (var customer in customers)
                {
                    var customerRes = _mapper.Map<CustomerDtoResponse>(customer);
                    response.Add(customerRes);
                }
                return response;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CustomerDtoResponse?> GetCustomerById(int id)
        {
            try
            {
                var customer = (await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == id)).FirstOrDefault();
                if(customer == null)
                {
                    return null;
                } else
                {
                    var response = _mapper.Map<CustomerDtoResponse>(customer);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsDublicatedEmail(string email)
        {
            try
            {
                var customer = (await _unitOfWork.CustomerRepository.GetAsync(c => c.Email == email)).FirstOrDefault();
                if (customer == null)
                {
                    return false;
                }
                else { 
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCustomerInformation(int CustomerId, UpdateCustomerDtoRequest request)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(CustomerId);
                if (customer == null)
                {
                    throw new Exception("No customer match this id");
                }
                else {
                    DateTime parsedDate = DateTime.ParseExact(request.DoB, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    customer.Name = request.Name;
                    customer.Address = request.Address;
                    customer.Phone = request.Phone;
                    customer.DoB = parsedDate;
                    await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCustomerStatus(int CustomerId, int status)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(CustomerId);
                if (customer == null)
                {
                    throw new Exception("No customer match this id");
                }
                else
                {
                    customer.Status = status;
                    await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> HashPassword(string password)
        {
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        stringBuilder.Append(hashBytes[i].ToString("x2"));
                    }

                    return await Task.FromResult(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
