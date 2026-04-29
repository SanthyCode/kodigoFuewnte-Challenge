using Microsoft.EntityFrameworkCore;
using Kodigo.Domain.Entities;

namespace Kodigo.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Separata> Separatas => Set<Separata>();

}