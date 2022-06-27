namespace Timesheet.SharedKernel;

public static class NullExtension
{
	public static bool IsNullOrEmpty(this string? s)
	{
		return string.IsNullOrEmpty(s);
	}

	public static bool IsNullOrWhitespace(this string? s)
	{
		return string.IsNullOrWhiteSpace(s);
	}

	public static bool IsNotNullOrEmpty(this string? s)
	{
		return !IsNullOrEmpty(s);
	}

	public static bool IsNotNullOrWhitespace(this string? s)
	{
		return !string.IsNullOrWhiteSpace(s);
	}
}