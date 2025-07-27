using Gonscheg.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gonscheg.Infrastructure.Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
    : base(options)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
}