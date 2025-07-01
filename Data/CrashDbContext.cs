using Microsoft.EntityFrameworkCore;
using Plane_Crash_Visualization.Models;

namespace Plane_Crash_Visualization.Data
{
    public class CrashDbContext : DbContext
    {
        public CrashDbContext(DbContextOptions<CrashDbContext> options) : base(options) { }

        public DbSet<Crash> Crashes { get; set; }
    }
}
