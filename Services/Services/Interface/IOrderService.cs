using Repository.Models;
using Services.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Interface
{
    public interface IOrderService
    {
        Task<int> ValidateItemInCart(List<OrderProductDto> cartItems);
        Task<string> CreateOrder(List<OrderProductDto> cartItems);
        Task<List<OrderDtoResponse>> GetAllOrders();
        Task<List<OrderDtoResponse>> GetOrdersByCustomerId(int CustomerId);
        Task<OrderDtoResponse?> GetOrderById(int id);
        Task<Order?> _getOrderById(int id);
        Task UpdateOrderStatus(Order order);
    }
}
