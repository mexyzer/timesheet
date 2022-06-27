using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class UserLogin : BaseEntity
{
    public UserLogin()
    {
        UserLoginId = Guid.NewGuid();
        InEurope = false;
    }

    public Guid UserLoginId { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string? UserAgent { get; set; }
    public string? UserAgentDetail { get; set; }
    public string? IpAddress { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public string? Region { get; set; }
    public bool InEurope { get; set; }
    public string? City { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? TimeZone { get; set; }
    public string? Asn { get; set; }
    public string? AsnOrganization { get; set; }
}