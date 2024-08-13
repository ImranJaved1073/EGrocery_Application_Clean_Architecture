namespace Domain
{
    public interface IOrderRepository : IRepository<Orders>
    {
        Task<Orders> GetAsync(string orderno);
        Task UpdateStatusAsync(Orders order);
    }
}