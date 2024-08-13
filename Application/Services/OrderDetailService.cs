using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderDetailService
    {
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public OrderDetailService(IRepository<OrderDetail> orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task AddOrderDetailAsync(OrderDetail detail)
        {
            await _orderDetailRepository.AddAsync(detail);
        }

        public async Task<List<OrderDetail>> GetAllAsync()
        {
            return await _orderDetailRepository.GetAsync();
        }
    }
}
