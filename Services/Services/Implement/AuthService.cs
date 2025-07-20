using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.UnitOfWork.Interface;
using Repository.Models;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        // ⚠️ Không còn hashedPassword nữa
        public async Task<Customer?> AuthenticateCustomer(string email, string password)
        {
            try
            {
                var customer = (await _unitOfWork.CustomerRepository.FindAsync(
                    a => a.Email == email && a.HashedPassword == password)).FirstOrDefault();

                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GenerateAccessTokenForCustomer(Customer customer)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var accessClaims = new List<Claim>
                {
                    new Claim("CustomerId", customer.CustomerId.ToString()),
                    new Claim("Role", "CUSTOMER")
                };
                var accessExpiration = DateTime.Now.AddMinutes(30);
                var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                    accessClaims, expires: accessExpiration, signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(accessJwt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GenerateAccessTokenForAdmin()
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var accessClaims = new List<Claim>
                {
                    new Claim("Role", "ADMIN")
                };
                var accessExpiration = DateTime.Now.AddMinutes(30);
                var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                    accessClaims, expires: accessExpiration, signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(accessJwt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            try
            {
                return (await _unitOfWork.CustomerRepository.FindAsync(c => c.Email == email)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching customer by email: {ex.Message}");
            }
        }

        public async Task<Customer> CreateCustomerFromGoogle(string email, string name, string avatarUrl)
        {
            try
            {
                var newCustomer = new Customer
                {
                    Email = email,
                    Name = name,
                    Avatar = avatarUrl,
                    HashedPassword = "", // có thể giữ nguyên
                    Status = 1
                };

                await _unitOfWork.CustomerRepository.InsertAsync(newCustomer);
                await _unitOfWork.SaveAsync();

                return newCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating customer from Google: {ex.Message}");
            }
        }
    }
}
