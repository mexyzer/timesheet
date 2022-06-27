using System.Text.Json;
using System.Text.Json.Serialization;

namespace Timesheet.FunctionalTests;

public static class JsonHelper
{
	public static string Serialize(object obj)
	{
		var opt = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		opt.Converters.Add(new JsonStringEnumConverter());

		return JsonSerializer.Serialize(obj, opt);
	}

	public static T Deserialize<T>(string obj)
	{
		var opt = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		opt.Converters.Add(new JsonStringEnumConverter());

		return JsonSerializer.Deserialize<T>(obj, opt)!;
	}
}