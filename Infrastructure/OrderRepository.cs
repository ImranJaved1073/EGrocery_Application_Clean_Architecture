using Dapper;
using Microsoft.Data.SqlClient;
using Domain;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class OrderRepository : GenericRepository<Orders>, IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connString) : base(connString)
        {
            _connectionString = connString;
        }

        public async Task<Orders> GetAsync(string orderno)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Orders WHERE OrderNum = @OrderNum";
                var order = await connection.QueryFirstOrDefaultAsync<Orders>(query, new { OrderNum = orderno });
                return order!;
            }
        }

        public async Task UpdateStatusAsync(Orders order)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string comm = "UPDATE Orders SET Status = @Status WHERE Id = @Id";
                await conn.ExecuteAsync(comm, new { Status = order.Status, Id = order.Id });
            }
        }
    }
}
