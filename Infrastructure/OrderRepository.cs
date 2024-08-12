using Dapper;
using Microsoft.Data.SqlClient;
using Domain;
namespace Infrastructure
{
    public class OrderRepository: GenericRepository<Orders>, IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connString) : base(connString)
        {
            _connectionString = connString;
        }

        public Orders Get(string orderno)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Orders WHERE OrderNum = @OrderNum";
                var order = connection.QueryFirstOrDefault<Orders>(query, new { OrderNum = orderno });
                return order!;
            }
        }

        public void UpdateStatus(Orders order)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string comm = $"Update Orders SET Status='{order.Status}' WHERE Id='{order.Id}'";
                conn.Execute(comm, new { OrderNum = order.OrderNum });
            }
        }
    }
}
