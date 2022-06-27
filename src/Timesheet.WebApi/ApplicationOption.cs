namespace Timesheet.WebApi;

public class ApplicationOption
{
	private const int DefaultExpireDuration = 86400;

	public string? SecretKey { get; set; }

	/// <summary>
	/// In Seconds. Default "86400" (24 hours)
	/// </summary>
	public string? ExpireDuration { get; set; }

	public string? DoMigration { get; set; }

	public bool GetDoMigration()
	{
		try
		{
			if (DoMigration != null)
				return bool.Parse(DoMigration);
		}
		catch
		{
			// ignored
		}

		return false;
	}

	public int GetExpireDuration()
	{
		try
		{
			if (ExpireDuration != null)
				return int.Parse(ExpireDuration);
		}
		catch
		{
			// ignored
		}

		return DefaultExpireDuration;
	}
}