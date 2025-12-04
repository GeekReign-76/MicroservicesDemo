using Microsoft.EntityFrameworkCore;
using ProcessingService.Models;

namespace ProcessingService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<DataRecord> DataRecords { get; set; }
    }
}
