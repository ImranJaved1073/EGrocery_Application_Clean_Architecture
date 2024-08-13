using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain;

namespace Infrastructure
{
    public class GenericRepository<TEntity> : IRepository<TEntity>
    {
        private readonly string connectionString;

        public GenericRepository(string c)
        {
            connectionString = c;
        }

        public async Task AddAsync(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.Name != "Id" && p.GetCustomAttribute<NotMappedAttribute>() == null);

            var columnNames = new List<string>();
            var parameterNames = new List<string>();
            var parameters = new DynamicParameters();

            foreach (var prop in properties)
            {
                if (IsComplexType(prop.PropertyType))
                {
                    var complexValue = prop.GetValue(entity);
                    if (complexValue != null)
                    {
                        foreach (var subProp in prop.PropertyType.GetProperties().Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null))
                        {
                            var value = subProp.GetValue(complexValue);
                            if (value != null)
                            {
                                var columnName = subProp.Name;
                                var parameterName = $"@{prop.Name}_{subProp.Name}";
                                columnNames.Add(columnName);
                                parameterNames.Add(parameterName);
                                parameters.Add(parameterName, value);
                            }
                        }
                    }
                }
                else
                {
                    var value = prop.GetValue(entity);
                    if (value != null)
                    {
                        var columnName = prop.Name;
                        var parameterName = $"@{prop.Name}";
                        columnNames.Add(columnName);
                        parameterNames.Add(parameterName);
                        parameters.Add(parameterName, value);
                    }
                }
            }

            var query = $"INSERT INTO {tableName} ({string.Join(",", columnNames)}) VALUES ({string.Join(",", parameterNames)})";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(query, parameters);
            }
        }

        private bool IsComplexType(Type type)
        {
            return type.IsClass && type != typeof(string);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var properties = typeof(TEntity).GetProperties().Where(x => x.GetCustomAttribute<NotMappedAttribute>() == null);
            var propertiesExclude = typeof(TEntity).GetProperties().Where(x => x.Name != primaryKey && x.GetCustomAttribute<NotMappedAttribute>() == null);

            var setClause = string.Join(",", propertiesExclude.Select(a => $"{a.Name}=@{a.Name}"));

            var query = $"UPDATE {tableName} SET {setClause} WHERE {primaryKey}=@{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var comm = new SqlCommand(query, connection);
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(entity) ?? DBNull.Value;
                    comm.Parameters.AddWithValue("@" + prop.Name, value);
                }
                await comm.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var query = $"DELETE FROM {tableName} WHERE {primaryKey}=@{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(query, new { Id = id });
            }
        }

        public virtual async Task<TEntity> GetAsync(int id)
        {
            var tableName = typeof(TEntity).Name;
            var query = $"SELECT * FROM {tableName} WHERE Id = @Id";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var entity = Activator.CreateInstance<TEntity>();
                    var properties = typeof(TEntity).GetProperties()
                                                     .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null);

                    foreach (var prop in properties)
                    {
                        var value = reader[prop.Name];
                        if (value == DBNull.Value)
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(entity, string.Empty);
                            }
                            else
                            {
                                prop.SetValue(entity, Activator.CreateInstance(prop.PropertyType));
                            }
                        }
                        else
                        {
                            prop.SetValue(entity, value);
                        }
                    }
                    return entity;
                }
            }
            return default!;
        }

        public virtual async Task<List<TEntity>> GetAsync()
        {
            var tableName = typeof(TEntity).Name;
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null);

            var columnNames = new List<string>();
            foreach (var prop in properties)
            {
                if (IsComplexType(prop.PropertyType))
                {
                    columnNames.AddRange(prop.PropertyType.GetProperties().Select(p => p.Name));
                }
                else
                {
                    columnNames.Add(prop.Name);
                }
            }

            var query = $"SELECT {string.Join(",", columnNames)} FROM {tableName}";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<dynamic>(query);

                var entities = new List<TEntity>();

                foreach (var row in result)
                {
                    var entity = Activator.CreateInstance<TEntity>();

                    foreach (var prop in properties)
                    {
                        if (IsComplexType(prop.PropertyType))
                        {
                            var complexInstance = Activator.CreateInstance(prop.PropertyType);
                            foreach (var subProp in prop.PropertyType.GetProperties())
                            {
                                var value = ((IDictionary<string, object>)row)[subProp.Name];
                                subProp.SetValue(complexInstance, value == DBNull.Value ? null : value);
                            }
                            prop.SetValue(entity, complexInstance);
                        }
                        else
                        {
                            var value = ((IDictionary<string, object>)row)[prop.Name];
                            prop.SetValue(entity, value == DBNull.Value ? null : value);
                        }
                    }

                    entities.Add(entity);
                }

                return entities;
            }
        }

        public virtual async Task<List<TEntity>> SearchAsync(string search)
        {
            List<TEntity> entities = new List<TEntity>();
            var entityType = typeof(TEntity);
            var properties = entityType.GetProperties();

            var tableName = entityType.Name;

            var whereClause = string.Join(" OR ", properties.Select(x => $"{x.Name} LIKE @search"));
            var query = $"SELECT * FROM {tableName} WHERE {whereClause}";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var entity = Activator.CreateInstance<TEntity>();
                        foreach (var prop in properties)
                        {
                            prop.SetValue(entity, reader[prop.Name]);
                        }
                        entities.Add(entity);
                    }
                }
            }
            return entities;
        }
    }
}
