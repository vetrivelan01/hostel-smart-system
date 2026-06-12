using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hostelfoodsystem.Data
{
    public class Design_Time_DbContext_Factory
    {

        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

                optionsBuilder.UseSqlServer(
                    "Server=.\\SQLEXPRESS;Database=HostelFoodSystemDB;Trusted_Connection=True;TrustServerCertificate=True;");

                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
    }
}

