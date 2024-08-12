using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void AddOrderDetail(OrderDetail detail)
        {
            _orderDetailRepository.Add(detail);
        }

        public List<OrderDetail> GetAll()
        {
           return _orderDetailRepository.Get();
        }
    }
}
