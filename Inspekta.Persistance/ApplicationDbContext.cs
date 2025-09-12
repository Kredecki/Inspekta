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
				PassHash = "ac9628a5c78754cac03869379eaf6f1f261b19e84519a5626d57e23f87b71769",
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
