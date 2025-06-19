using Microsoft.EntityFrameworkCore.Storage;
using Repository.Models;
using Repository.Repository.Implement;
using Repository.Repository.Interface;
using Repository.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork.Implement
{
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        private PokemonDbContext context = new PokemonDbContext();
        private GenericRepository<Customer> _customerRepository;
        private GenericRepository<CartItem> _cartItemRepository;
        private GenericRepository<ChatRequest> _chatRequestRepository;
        private GenericRepository<Category> _categoryRepository;
        private GenericRepository<Order> _orderRepository;
        private GenericRepository<OrderDetail> _orderDetailRepository;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<ProductImage> _productImageRepository;
        private GenericRepository<Payment> _paymentRepository;
        private CartRepository _cartRepository;

        public UnitOfWork(PokemonDbContext context)
        {
            this.context = context;
        }
        private bool disposed = false;

        public IGenericRepository<CartItem> CartItemRepository => _cartItemRepository ?? new GenericRepository<CartItem>(context);

        public IGenericRepository<Category> CategoryRepository => _categoryRepository ?? new GenericRepository<Category>(context);

        public IGenericRepository<Customer> CustomerRepository => _customerRepository ?? new GenericRepository<Customer>(context);

        public IGenericRepository<Order> OrderRepository => _orderRepository ?? new GenericRepository<Order>(context);

        public IGenericRepository<OrderDetail> OrderDetailRepository => _orderDetailRepository ?? new GenericRepository<OrderDetail>(context);

        public IGenericRepository<Product> ProductRepository => _productRepository ?? new GenericRepository<Product>(context);

        public IGenericRepository<ProductImage> ProductImageRepository => _productImageRepository ?? new GenericRepository<ProductImage>(context);

        public IGenericRepository<ChatRequest> ChatRequestRepository => _chatRequestRepository ?? new GenericRepository<ChatRequest>(context);

        public IGenericRepository<Payment> PaymentRepository => _paymentRepository ?? new GenericRepository<Payment>(context);

        public CartRepository cartRepository
        {
            get
            {
                if (cartRepository == null)
                {
                    _cartRepository = new CartRepository(context);
                }
                return _cartRepository;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }
    }
}
