using Microsoft.EntityFrameworkCore;

namespace Triyaj.Infrastructure.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly TriyajDbContext _context;
    private readonly DbSet<T> _dbSet;
    public GenericRepository(TriyajDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
    public void Remove(T entity) => _dbSet.Remove(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
