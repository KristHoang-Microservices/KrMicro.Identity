using KrMicro.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KrMicro.Identity.Infrastructure;

public class KrIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public KrIdentityDbContext()
    {
    }

    public KrIdentityDbContext(DbContextOptions<KrIdentityDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Customer>()
            .HasOne<ApplicationUser>(c => c.UserInformation)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>("UserId")
            .IsRequired(false);

        builder.Entity<Customer>()
            .Navigation(c => c.UserInformation).AutoInclude();
    }
}