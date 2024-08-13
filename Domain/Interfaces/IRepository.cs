namespace Domain
{
    public interface IRepository<TEntity>
    {
        // Task<TEntity> FindByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task<TEntity> GetAsync(int id);
        Task<List<TEntity>> GetAsync();
        Task<List<TEntity>> SearchAsync(string search);
    }
}