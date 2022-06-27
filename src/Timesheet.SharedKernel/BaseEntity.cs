namespace Timesheet.SharedKernel;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        IsActive = true;
    }

    public DateTime? CreatedDt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedDt { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? DeletedDt { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsActive { get; set; }

    public readonly List<BaseDomainEvent> Events = new();

    public void SetToInActive()
    {
        IsActive = false;
    }

    public void SetToActive()
    {
        IsActive = true;
    }

    public void SetToSoftDelete(string deleteBy, DateTime deleteDt)
    {
        SetToInActive();

        DeletedBy = deleteBy;
        DeletedDt = deleteDt;
    }
}