using Repository.Models;
using Repository.UnitOfWork.Implement;
using Repository.UnitOfWork.Interface;
using Services.ModelView;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Implement
{
    public class RegisterServices : IRegisterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> Register(RegisterDTO register)
        {
            // Kiểm tra input
            if (string.IsNullOrWhiteSpace(register.UserName))
                return new ResponseDTO("Username cannot be blank.", 400);

            if (string.IsNullOrWhiteSpace(register.Email))
                return new ResponseDTO("Email cannot be blank.", 400);

            if (!register.Email.EndsWith("@gmail.com"))
                return new ResponseDTO("Email must end with @gmail.com.", 400);

            if (string.IsNullOrWhiteSpace(register.PassWord) || register.PassWord.Length < 6)
                return new ResponseDTO("Password must be at least 6 characters.", 400);

            if (register.PassWord != register.ConfirmPassword)
                return new ResponseDTO("Passwords do not match.", 400);

            // Kiểm tra email đã tồn tại chưa
            var existingCustomer = await _unitOfWork.Customer
                .FirstOrDefaultAsync(c => c.Email == register.Email);
            if (existingCustomer != null)
                return new ResponseDTO("Email already registered.", 409);

            // KHÔNG băm mật khẩu, lưu trực tiếp
            var customer = new Customer
            {
                Name = register.UserName,
                Email = register.Email!,
                HashedPassword = register.PassWord,  // Lưu mật khẩu nguyên bản (không khuyến khích trong production)
                Status = 0,
            };

            // Lưu vào database
            _unitOfWork.CustomerRepository.Add(customer);
            await _unitOfWork.SaveChangeAsync();

            return new ResponseDTO("Sign up successfully", 200, true);
        }

    }


}
