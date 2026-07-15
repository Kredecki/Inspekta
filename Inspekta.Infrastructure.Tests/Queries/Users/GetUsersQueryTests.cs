using Inspekta.Infrastructure.Abstractions.Helpers;
using Inspekta.Infrastructure.Exceptions;
using Inspekta.Infrastructure.Queries.Users;
using Inspekta.Persistance.Abstractions.Repositories;
using Inspekta.Persistance.Entities;
using Inspekta.Shared.Enums;
using Moq;
using System.ComponentModel.Design;
using System.Timers;

namespace Inspekta.Infrastructure.Tests.Queries.Users;

public class GetUsersQueryTests
{
	private readonly Mock<IUsersRepository> _usersRepository = new();
	private readonly Mock<ICompaniesRepository> _companiesRepository = new();
	private readonly Mock<IFilterHelper> _filterHelper = new();

	private readonly GetUsersHandler _handler;

	public GetUsersQueryTests()
	{
		_handler = new GetUsersHandler(
			_usersRepository.Object,
			_companiesRepository.Object,
			_filterHelper.Object);

		_filterHelper
			.Setup(x => x.ApplySearchFilter(
				It.IsAny<IEnumerable<User>>(),
				It.IsAny<string>(),
				It.IsAny<Func<User, string>>()))
			.ReturnsAsync((IEnumerable<User> users, string _, Func<User, string> _) => users);

		_filterHelper
			.Setup(x => x.ApplySortFilter(
				It.IsAny<IEnumerable<User>>(),
				It.IsAny<string>(),
				It.IsAny<bool>()))
			.ReturnsAsync((IEnumerable<User> users, string _, bool _) => users);
	}

	[Fact]
	public async Task Handle_ShouldThrow_WhenUsersRepositoryReturnsNull()
	{
		// Arrange
		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync((IEnumerable<User>?)null);

		var query = CreateQuery();

		// Act & Assert
		await Assert.ThrowsAsync<InspektaNullException>(() =>
			_handler.Handle(query, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_ShouldThrow_WhenAdministratorHasNoCompany()
	{
		// Arrange
		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<User>());

		_companiesRepository
			.Setup(x => x.GetCompanyByUserId(
				It.IsAny<Guid>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync((Company?)null);

		var query = CreateQuery(role: EUserRole.Administrator);

		// Act & Assert
		await Assert.ThrowsAsync<InspektaNullException>(() =>
			_handler.Handle(query, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_ShouldReturnOnlyUsersFromCompany_ForAdministrator()
	{
		// Arrange
		var companyId = Guid.NewGuid();

		var users = new List<User>
		{
			new()
			{
				Id = Guid.NewGuid(),
				Login = "user1",
				Role = EUserRole.Administrator,
				CompanyId = companyId,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = "hash1",
				Salt = "salt1"
			},
			new()
			{
				Id = Guid.NewGuid(),
				Login = "user2",
				Role = EUserRole.Administrator,
				CompanyId = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = "hash2",
				Salt = "salt2"
			}
		};

		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		_companiesRepository
			.Setup(x => x.GetCompanyByUserId(
				It.IsAny<Guid>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new Company
			{
				Id = companyId,
				Name = "Test Company",
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt= DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid()
			});

		var query = CreateQuery(role: EUserRole.Administrator);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Single(result.Item1);
		Assert.Equal("user1", result.Item1.First().Login);
		Assert.Equal(1, result.Total);
	}

	[Fact]
	public async Task Handle_ShouldReturnAllUsers_ForNonAdministrator()
	{
		// Arrange
		var users = new List<User>
		{
			new()
			{
				Id = Guid.NewGuid(),
				Login = "user1",
				Role = EUserRole.Inspector,
				CompanyId = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = "hash1",
				Salt = "salt1"
			},
			new()
			{
				Id = Guid.NewGuid(),
				Login = "user2",
				Role = EUserRole.Inspector,
				CompanyId = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = "hash2",
				Salt = "salt2"
			}
		};

		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		var query = CreateQuery(role: EUserRole.Inspector);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Equal(2, result.Item1.Count);
		Assert.Equal(2, result.Total);

		_companiesRepository.Verify(x =>
			x.GetCompanyByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Fact]
	public async Task Handle_ShouldApplyPaging()
	{
		// Arrange
		var users = Enumerable.Range(1, 10)
			.Select(i => new User
			{
				Id = Guid.NewGuid(),
				Login = $"user{i}",
				Role = EUserRole.Inspector,
				CompanyId = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = $"hash{i}",
				Salt = $"salt{i}"
			})
			.ToList();

		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		var query = new GetUsersQuery(
			CurrentPage: 1,
			RecordsPerPage: 3,
			SearchTerm: "",
			SortColumn: "",
			SortDescending: false,
			UserRole: EUserRole.Inspector,
			UserId: Guid.NewGuid());

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Equal(10, result.Total);
		Assert.Equal(3, result.Item1.Count);

		Assert.Equal("user4", result.Item1[0].Login);
		Assert.Equal("user5", result.Item1[1].Login);
		Assert.Equal("user6", result.Item1[2].Login);
	}

	[Fact]
	public async Task Handle_ShouldMapCompanyToCompanyDto()
	{
		// Arrange
		var company = new Company
		{
			Id = Guid.NewGuid(),
			Name = "Test Company",
			NIP = "1234567890",
			Street = "Main Street",
			ZipCode = "00-001",
			Town = "Warsaw",
			Email = "test@test.pl",
			Phone = "123456789",
			CreatedAt = DateTime.Now,
			CreatedBy = Guid.NewGuid(),
			ModifiedAt = DateTime.Now,
			ModifiedBy = Guid.NewGuid()
		};

		var users = new List<User>
		{
			new()
			{
				Id = Guid.NewGuid(),
				Login = "user1",
				Role = EUserRole.Administrator,
				CompanyId = company.Id,
				Company = company,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = "hash1",
				Salt = "salt1"
			}
		};

		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		var query = CreateQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		var dto = Assert.Single(result.Item1);

		Assert.NotNull(dto.Company);
		Assert.Equal(company.Id, dto.Company!.Id);
		Assert.Equal(company.Name, dto.Company.Name);
		Assert.Equal(company.NIP, dto.Company.NIP);
		Assert.Equal(company.Street, dto.Company.Street);
		Assert.Equal(company.ZipCode, dto.Company.ZipCode);
		Assert.Equal(company.Town, dto.Company.Town);
		Assert.Equal(company.Email, dto.Company.Email);
		Assert.Equal(company.Phone, dto.Company.Phone);
	}

	[Fact]
	public async Task Handle_ShouldReturnNullCompanyDto_WhenCompanyIsNull()
	{
		// Arrange
		var users = new List<User>
		{
			new()
			{
				Id = Guid.NewGuid(),
				Login = "user1",
				Role = EUserRole.Administrator,
				CompanyId = Guid.NewGuid(),
				Company = null,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = "hash1",
				Salt = "salt1"
			}
		};

		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		var query = CreateQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		var dto = Assert.Single(result.Item1);
		Assert.Null(dto.Company);
	}

	[Fact]
	public async Task Handle_ShouldCallSearchAndSortHelpers()
	{
		// Arrange
		var users = new List<User>
		{
			new()
			{
				Id = Guid.NewGuid(),
				Login = "user1",
				Role = EUserRole.Administrator,
				CompanyId = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedBy = Guid.NewGuid(),
				ModifiedAt = DateTime.UtcNow,
				ModifiedBy = Guid.NewGuid(),
				PassHash = "hash1",
				Salt = "salt1"
			}
		};

		_usersRepository
			.Setup(x => x.GetUsers(It.IsAny<CancellationToken>()))
			.ReturnsAsync(users);

		var query = new GetUsersQuery(
			CurrentPage: 0,
			RecordsPerPage: 10,
			SearchTerm: "admin",
			SortColumn: "Login",
			SortDescending: true,
			UserRole: EUserRole.Inspector,
			UserId: Guid.NewGuid());

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		_filterHelper.Verify(x => x.ApplySearchFilter(
				It.Is<IEnumerable<User>>(u => u.SequenceEqual(users)),
				"admin",
				It.IsAny<Func<User, string>>()),
			Times.Once);

		_filterHelper.Verify(x => x.ApplySortFilter(
				It.IsAny<IEnumerable<User>>(),
				"Login",
				true),
			Times.Once);
	}

	private static GetUsersQuery CreateQuery(
		EUserRole role = EUserRole.Inspector)
	{
		return new GetUsersQuery(
			CurrentPage: 0,
			RecordsPerPage: 10,
			SearchTerm: "",
			SortColumn: "",
			SortDescending: false,
			UserRole: role,
			UserId: Guid.NewGuid());
	}
}
