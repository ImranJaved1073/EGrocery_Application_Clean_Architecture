namespace Domain
{
    public interface IRepository<TEntity>
    {
        //TEntity FindById(int id);
        public void Add(TEntity entity);
        public void Update(TEntity entity);

        public void Delete(int id);
        public TEntity Get(int id);

        public List<TEntity> Get();

        public List<TEntity> Search(string search);
    }

}
