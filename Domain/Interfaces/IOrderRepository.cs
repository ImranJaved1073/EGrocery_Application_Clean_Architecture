namespace Domain
{
    public interface IOrderRepository : IRepository<Orders>
    {
        public Orders Get(string orderno);
        public void UpdateStatus(Orders order);
    }
}
