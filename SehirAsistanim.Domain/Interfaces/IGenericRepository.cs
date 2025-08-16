namespace SehirAsistanim.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class,IEntitiy
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
        Task<bool> Exists(int id);
        IQueryable<T> GetQueryable();
        Task RemoveRange(IEnumerable<T> entities);
    }
}
