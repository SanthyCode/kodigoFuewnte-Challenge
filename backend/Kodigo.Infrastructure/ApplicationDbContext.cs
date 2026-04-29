using Microsoft.EntityFrameworkCore;
using Kodigo.Domain.Entities;

namespace Kodigo.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Separata> Separatas => Set<Separata>();
    public DbSet<SeparataItem> SeparataItems => Set<SeparataItem>();

    public DbSet<Product> Products => Set<Product>();

}