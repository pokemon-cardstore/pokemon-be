using Microsoft.EntityFrameworkCore.Storage;
using Repository.Models;
using Repository.Repository.Implement;
using Repository.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<CartItem> CartItemRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<ChatRequest> ChatRequestRepository { get; }
        IGenericRepository<Customer> CustomerRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderDetail> OrderDetailRepository { get; }

        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<ProductImage> ProductImageRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        public CartRepository cartRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveAsync();
    }
}
