using Microsoft.EntityFrameworkCore;
using redis.WebAPi.Model;
using redis.WebAPi.Repository.AppDbContext;
using redis.WebAPi.Service.Benchmark;


namespace redis.WebAPi.Service
{
    public class OperationSQL
    {
        private readonly BenchmarkContent _context;
        private readonly ILogger<OperationSQL> _logger;

        // The constructor relies on injection of BenchmarkDbContext and Logger
        public OperationSQL(BenchmarkContent context, ILogger<OperationSQL> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Update benchmark status
        public async Task<bool> UpdateCacheStatus(string cachekName, SQLDataBaseEnum s, int newStatus)
        {
            try
            {
                BenchmarkRequestModel result = null;
                var property = _context.GetType().GetProperty(s.ToString());

                if (property != null)
                {
                    // ��ȡ DbSet
                    var dbSet = property.GetValue(_context) as IQueryable<BenchmarkRequestModel>;  // ������Ҫ��������

                    if (dbSet != null)
                    {
                        // ʹ�� LINQ ��ѯ DbSet
                        result = await dbSet.FirstOrDefaultAsync(b => b.Name == cachekName);
                        if (result != null)
                        {
                            result.Status = newStatus;
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Successfully updated the status of cache '{cachekName}' to {newStatus}.");
                            return true;
                        }

                    }
                }
                _logger.LogError("No found result.");
                return false;            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the cachek status.");
                return false;
            }
        }

       public async Task<bool> DeletData(string cachekName, SQLDataBaseEnum s, int newStatus)
        {
            try
            {
                BenchmarkRequestModel result = null;
                var property = _context.GetType().GetProperty(s.ToString());

                if (property != null)
                {
                    // ��ȡ DbSet
                    var dbSet = property.GetValue(_context) as IQueryable<BenchmarkRequestModel>;  // ������Ҫ��������

                    if (dbSet != null)
                    {
                        // ʹ�� LINQ ��ѯ DbSet
                        result = await dbSet.FirstOrDefaultAsync(b => b.Name == cachekName);
                        if (result != null)
                        {
                            _context.Remove(result);  // ɾ��ʵ�����
                            await _context.SaveChangesAsync();  // �ύ���ݿ����
                            return true;
                        }

                    }
                }
                _logger.LogError("No found result.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the cachek status.");
                return false;
            }
        }
    }
}
