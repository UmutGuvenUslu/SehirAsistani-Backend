namespace SehirAsistanim.Domain.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class, IEntitiy;

        Task CommitAsync();

        ValueTask DisposeAsync();
    }
}
