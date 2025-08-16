using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SehirAsistanim.Domain.Interfaces;

namespace SehirAsistanim.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntitiy
    {
        public readonly DbContext _context;
        public readonly DbSet<T> _dbSet;

       public IQueryable<T> GetQueryable()
    {
        return _context.Set<T>().AsQueryable();
    }

        public GenericRepository(DbContext dbContext)
        {
            _context = dbContext;
            _dbSet = _context.Set<T>();
        }

        #region GetById
        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        #endregion

        #region GetAll
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }
        #endregion

        #region Add
        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Update
        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Delete
        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region Exists
        public async Task<bool> Exists(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        #endregion

        #region RemoveRange
        public async Task RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        #endregion

    }
}
