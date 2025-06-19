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

        public async Task<Customer?> AuthenticateCustomer(string email, string hashedPassword)
        {
            try
            {
                var customer = (await _unitOfWork.CustomerRepository.FindAsync(a => a.HashedPassword == hashedPassword && a.Email == email)).FirstOrDefault();
                if (customer == null) {
                    return null;
                } else
                {
                    return customer;
                }
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
                var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], accessClaims, expires: accessExpiration, signingCredentials: credentials);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);
                return accessToken;
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
                var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], accessClaims, expires: accessExpiration, signingCredentials: credentials);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);
                return accessToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
