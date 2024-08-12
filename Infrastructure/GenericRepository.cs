using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Dapper;
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

        public void Add(TEntity entity)
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
                connection.Open();
                connection.Execute(query, parameters);
            }
        }

        private bool IsComplexType(Type type)
        {
            return type.IsClass && type != typeof(string);
        }
        public void Update(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var properties = typeof(TEntity).GetProperties().Where(x => x.GetCustomAttribute<NotMappedAttribute>() == null);
            var propertiesExclude = typeof(TEntity).GetProperties().Where(x => x.Name != primaryKey && x.GetCustomAttribute<NotMappedAttribute>() == null);

            var setClause = string.Join(",", propertiesExclude.Select(a => $"{a.Name}=@{a.Name}"));
            //var primaryKeyProp = properties.FirstOrDefault(x => x.Name == primaryKey);

            var query = $"update {tableName} set {setClause} where {primaryKey}=@{primaryKey} ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var comm = new SqlCommand(query, connection);
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(entity) ?? DBNull.Value;
                    comm.Parameters.AddWithValue("@" + prop.Name, value);
                }
                comm.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var query = $"delete from {tableName} where {primaryKey}=@{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                //connection.Open();
                //var comm = new SqlCommand(query, connection);
                //comm.Parameters.AddWithValue("@" + primaryKey, entity.GetType().GetProperty(primaryKey).GetValue(entity));
                //comm.ExecuteNonQuery();
                connection.Execute(query, new { Id = id });
            }
        }


        virtual public TEntity Get(int id)
        {
            var tablename = typeof(TEntity).Name;

            var query = $"select * from {tablename} where id = @id";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
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



        virtual public List<TEntity> Get()
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
                connection.Open();
                var result = connection.Query<dynamic>(query);

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

        virtual public List<TEntity> Search(string search)
        {
            List<TEntity> entities = new List<TEntity>();
            var entityType = typeof(TEntity);
            var properties = entityType.GetProperties();

            var tableName = entityType.Name;

            var whereClause = string.Join(" OR ", properties.Select(x => $"{x.Name} LIKE @search"));
            var query = $"select * from {tableName} where {whereClause}";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
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
