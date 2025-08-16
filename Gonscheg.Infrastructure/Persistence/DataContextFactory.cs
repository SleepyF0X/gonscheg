using Gonscheg.Shared;
using Gonscheg.Shared.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gonscheg.Infrastructure.Persistence;


public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(Constants.ConnectionString);

        return new DataContext(optionsBuilder.Options);
    }
}