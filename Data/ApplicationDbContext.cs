using Hostelfoodsystem.Model;
using Microsoft.EntityFrameworkCore;

namespace Hostelfoodsystem.Data
{
        public class ApplicationDbContext : DbContext
      {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<User> Users => Set<User>();
            public DbSet<Student> Students => Set<Student>();
            public DbSet<MealRecord> MealRecords => Set<MealRecord>();
            public DbSet<Attendance> Attendances => Set<Attendance>();
            public DbSet<PredictionRecord> PredictionRecords => Set<PredictionRecord>();
        }
    }

