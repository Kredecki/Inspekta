using Inspekta.Persistance.Entities;
using Inspekta.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Inspekta.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<Company> Companies { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

        Guid superId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        DateTime utcTime = new(2025, 09, 11, 0, 0, 0, DateTimeKind.Utc);
        Company superCompany = new()
        {
            Id = superId,
            Name = "Inspekta sp. z o. o.",
            CreatedAt = utcTime,
            CreatedBy = superId,
            ModifiedAt = utcTime,
            ModifiedBy = superId
        };

        modelBuilder.Entity<Company>().HasData(superCompany);

        modelBuilder.Entity<User>().HasData(
			new User
			{
				Id = superId,
				Login = "Inspekta",
				PassHash = "1ad8a4c6fa0a84514f1f1fc92b6e16130233a571764db6c5bae8413d1e33957f",
				Salt = "RtK3gvva4x7jPnjb",
				Role = EUserRole.SuperAdministrator,
                CompanyId = superCompany.Id,
                CreatedAt = utcTime,
                CreatedBy = superId,
                ModifiedAt = utcTime,
                ModifiedBy = superId
            });

        modelBuilder.Entity<User>()
           .HasOne(u => u.Company)
           .WithMany(c => c.Users)
           .HasForeignKey(u => u.CompanyId)
           .IsRequired();
    }
}
