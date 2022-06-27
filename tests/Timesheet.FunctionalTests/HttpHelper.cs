using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Timesheet.SharedKernel;

namespace Timesheet.FunctionalTests;

public static class HttpHelper
{
	public static HttpRequestMessage CreateRequest(string uri,
		HttpMethod method,
		AuthenticationHeaderValue? authenticationHeaderValue = null,
		Dictionary<string, string>? headers = null,
		HttpContent? content = null)
	{
		var req = new HttpRequestMessage();

		req.RequestUri = new Uri(uri);
		req.Method = method;

		if (headers != null && headers.Any())
			foreach ((string? key, string? value) in headers)
				if (key.IsNotNullOrWhitespace() && value.IsNotNullOrWhitespace())
					req.Headers.Add(key, value);

		if (content != null)
			req.Content = content;

		if (authenticationHeaderValue != null)
			req.Headers.Authorization = authenticationHeaderValue;

		return req;
	}
}