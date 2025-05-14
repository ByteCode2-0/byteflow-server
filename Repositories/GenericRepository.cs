using byteflow_server.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace byteflow_server.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ByteFlowDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ByteFlowDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var property = typeof(T).GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool?))
            {
               
                return await _dbSet.Where(e => EF.Property<bool?>(e, "IsDeleted") != true).ToListAsync();
            }
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            var property = typeof(T).GetProperty("IsDeleted");
            //if (property != null && property.PropertyType == typeof(bool?))
            //{
            //    return await _dbSet.Where(e => EF.Property<long>(e, "EmployeeId") == id && EF.Property<bool?>(e, "IsDeleted") != true).FirstOrDefaultAsync();
            //}
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            
            var property = typeof(T).GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool?))
            {
               
                property.SetValue(entity, true);
                Update(entity); 
            }
            else
            {
                throw new InvalidOperationException("Soft delete is not supported for this entity.");
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
