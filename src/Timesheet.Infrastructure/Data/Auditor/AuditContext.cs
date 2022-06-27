using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Timesheet.Infrastructure.Data.Auditor;

public class AuditContext : DbContext
{
    public AuditContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Audit> AuditLogs => Set<Audit>();

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
        string? userId = null)
    {
        //logs all changes made within dbcontext
        var auditEntries = OnBeforeSaveChanges(userId);

        var result = await base.SaveChangesAsync(cancellationToken);

        //logs all changes made within dbcontext for temporary properties recursively
        await OnAfterSaveChanges(auditEntries);

        return result;
    }

    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
    {
        if (!auditEntries.Any())
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }

            AuditLogs.Add(auditEntry.ToAudit());
        }

        return SaveChangesAsync();
    }

    private List<AuditEntry> OnBeforeSaveChanges(string? userId)
    {
        ChangeTracker.DetectChanges();

        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Audit || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            auditEntries.Add(MapAuditEntry(entry, userId));
        }

        foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
        {
            AuditLogs.Add(auditEntry.ToAudit());
        }

        return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }

    private AuditEntry MapAuditEntry(EntityEntry entry, string? userId)
    {
        var auditEntry = new AuditEntry(entry);
        auditEntry.TableName = entry.Entity.GetType().Name;
        auditEntry.UserId = userId;

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary)
            {
                auditEntry.TemporaryProperties.Add(property);
                continue;
            }

            string propertyName = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey())
            {
                auditEntry.KeyValues[propertyName] = property.CurrentValue;
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.AuditType = AuditType.Create;
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    auditEntry.AuditType = AuditType.Delete;
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                    break;

                case EntityState.Modified:
                    if (property.IsModified)
                    {
                        auditEntry.ChangedColumns.Add(propertyName);
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                    }

                    break;
            }
        }

        return auditEntry;
    }
}