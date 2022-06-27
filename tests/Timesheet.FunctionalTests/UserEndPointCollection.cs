using Timesheet.WebApi;
using Xunit;

namespace Timesheet.FunctionalTests;

[CollectionDefinition("User API Collection")]
public class UserEndPointCollection : ICollectionFixture<CustomWebApplicationFactory<WebMarker>>
{
}