using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository.Implement
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        private readonly PokemonDbContext _dbContext;
        private readonly DbSet<CartItem> _cartItems;
        public CartRepository(PokemonDbContext context) : base(context)
        {
            _dbContext = context;
            _cartItems = _dbContext.Set<CartItem>();
        }

        public async Task<int> GetCount()
        {
            return await _dbContext.CartItems.CountAsync();
        }
    }
}
