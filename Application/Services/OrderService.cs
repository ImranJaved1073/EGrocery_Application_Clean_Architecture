using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Orders>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAsync();
        }

        public async Task<Orders> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetAsync(id);
        }

        public async Task CreateOrderAsync(Orders od)
        {
            await _orderRepository.AddAsync(od);
        }

        public async Task UpdateOrderAsync(Orders od)
        {
            await _orderRepository.UpdateAsync(od);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _orderRepository.DeleteAsync(id);
        }

        public async Task<Orders> GetbyOrderNoAsync(string orderno)
        {
            return await _orderRepository.GetAsync(orderno);
        }

        public async Task UpdateStatusAsync(Orders order)
        {
            await _orderRepository.UpdateStatusAsync(order);
        }
    }
}
