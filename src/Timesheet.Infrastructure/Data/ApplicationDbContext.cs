using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.Infrastructure.Data.Auditor;
using Timesheet.SharedKernel;

namespace Timesheet.Infrastructure.Data;

public class ApplicationDbContext : AuditContext, IApplicationDbContext
{
	private readonly ICurrentUserService? _currentUserService;
	private readonly IDateTime? _dateTime;
	private readonly IMediator? _mediator;

	public ApplicationDbContext(DbContextOptions options,
		ICurrentUserService? currentUserService,
		IDateTime? dateTime,
		IMediator? mediator) : base(options)
	{
		_currentUserService = currentUserService;
		_dateTime = dateTime;
		_mediator = mediator;
	}

	public DbSet<User> Users => Set<User>();
	public DbSet<UserLogin> UserLogins => Set<UserLogin>();
	public DbSet<UserRole> UserRoles => Set<UserRole>();
	public DbSet<UserPassword> UserPasswords => Set<UserPassword>();
	public DbSet<UserToken> UserTokens => Set<UserToken>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<Permission> Permissions => Set<Permission>();
	public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(builder);
	}

	public void DetachAllEntities()
	{
		var changedEntriesCopy = ChangeTracker.Entries()
			.Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
			.ToList();

		foreach (var entry in changedEntriesCopy)
			entry.State = EntityState.Detached;
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		foreach (var entry in ChangeTracker.Entries<BaseEntity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedBy = _currentUserService?.UserId;
					entry.Entity.CreatedDt ??= _dateTime?.ScopedUtcNow;
					break;

				case EntityState.Modified:
					entry.Entity.LastModifiedBy = _currentUserService?.UserId;
					entry.Entity.LastModifiedDt = _dateTime?.ScopedUtcNow;
					break;
				case EntityState.Detached:
					break;
				case EntityState.Unchanged:
					break;
				case EntityState.Deleted:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		int result = await base.SaveChangesAsync(cancellationToken, _currentUserService?.UserId).ConfigureAwait(false);

		// ignore events if no dispatcher provided
		if (_mediator == null) return result;

		// dispatch events only if save was successful
		var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
			.Select(e => e.Entity)
			.Where(e => e.Events.Any())
			.ToArray();

		foreach (var entity in entitiesWithEvents)
		{
			var events = entity.Events.ToArray();
			entity.Events.Clear();
			foreach (var domainEvent in events)
			{
				await _mediator.Publish(domainEvent, CancellationToken.None).ConfigureAwait(false);
			}
		}

		return result;
	}
}