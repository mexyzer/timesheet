using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Timesheet.Infrastructure.Data.Auditor;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
        _options = new JsonSerializerOptions();
    }

    public EntityEntry Entry { get; }

    private readonly JsonSerializerOptions? _options;

    public string? UserId { get; set; }
    public string? TableName { get; set; }
    public Dictionary<string, object?> KeyValues { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> OldValues { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> NewValues { get; } = new Dictionary<string, object?>();
    public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new List<string>();
    public bool HasTemporaryProperties => TemporaryProperties.Any();

    public Audit ToAudit()
    {
        var audit = new Audit();
        audit.UserId = UserId;
        audit.Type = AuditType.ToString();
        audit.TableName = TableName;
        audit.DateTime = DateTime.UtcNow;
        audit.PrimaryKey = JsonSerializer.Serialize(KeyValues);
        audit.OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues, _options);
        audit.NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues, _options);
        audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns, _options);
        return audit;
    }
}