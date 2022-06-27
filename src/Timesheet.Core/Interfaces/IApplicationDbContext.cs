namespace Timesheet.Core.Interfaces;

public interface IApplicationDbContext : IUserEntity
{
	void DetachAllEntities();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}