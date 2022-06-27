namespace Timesheet.WebApi.Models;

public class KeyDto
{
	public const string String = "string";
	public const string Int = "int32";
	public const string Long = "int64";
	public const string Guid = "guid";

	public KeyDto(string id, string type)
	{
		Id = id;
		Type = type;
	}

	public string Id { get; }
	public string Type { get; }
}

public static class KeyDtoHelper
{
	public static KeyDto Create(string id, string type)
	{
		return new KeyDto(id, type);
	}
}