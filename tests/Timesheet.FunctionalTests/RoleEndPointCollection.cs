using Timesheet.WebApi;
using Xunit;

namespace Timesheet.FunctionalTests;

[CollectionDefinition("Role API Collection")]
public class RoleEndPointCollection : ICollectionFixture<CustomWebApplicationFactory<WebMarker>>
{
}