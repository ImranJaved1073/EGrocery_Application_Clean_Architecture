using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public IEnumerable<Orders> GetAllOrders()
        {
           return _orderRepository.Get();
        }

        public Orders GetOrderById(int id)
        {
            return _orderRepository.Get(id);
        }

        public void CreateOrder(Orders od)
        {
            _orderRepository.Add(od);
        }

        public void UpdateOrder(Orders od)
        {
            _orderRepository.Update(od);
        }

        public void DeleteOrder(int id)
        {
            _orderRepository.Delete(id);
        }

        public Orders GetbyOrderNo(string orderno)
        {
            return _orderRepository.Get(orderno);
        }

        public void UpdateStatus(Orders order)
        {
            _orderRepository.UpdateStatus(order);
        }
    }
}
