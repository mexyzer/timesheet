using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.Users;
using Xunit;

namespace Timesheet.UnitTests.Endpoints.Users;

public class LoginTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", null)]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("", "   ")]
    [InlineData("test", "qwerty@123")]
    public void LoginRequestValidatorReturnFalse(string username, string password)
    {
        var request = new LoginRequest {Username = username, Password = password};

        var validator = new LoginRequestValidator();

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("vendy@timesheet.com", "Qwerty@123")]
    [InlineData("dev@timesheet.com", " @123aax")]
    [InlineData("aa@aa.com", " @123aax")]
    [InlineData("a@a.com", " @123aax")]
    public void LoginRequestValidatorShouldReturnTrue(string username, string password)
    {
        var request = new LoginRequest {Username = username, Password = password};

        var validator = new LoginRequestValidator();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", null)]
    [InlineData("", "")]
    [InlineData("  ", "")]
    [InlineData("", "   ")]
    [InlineData("test", "qwerty@123")]
    public async Task LoginShouldReturnBadRequestWhenValidationIsFailed(string username, string password)
    {
        var request = new LoginRequest {Username = username, Password = password};

        var login = new Login(new UserServiceBuilder().Build().Object,
            new JwtServiceBuilder().Build(),
            new InterfaceDateTimeServiceBuilder().Build(),
            new InterfaceApplicationDbContextBuilder().Build().Object);

        var result = await login.HandleAsync(request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task LoginShouldReturnBadRequestWhenUsernameNotFound()
    {
        var request = new LoginRequest {Username = "test@mail.com", Password = "Qwerty@1234"};

        var login = new Login(new UserServiceBuilder().Build().Object,
            new JwtServiceBuilder().Build(),
            new InterfaceDateTimeServiceBuilder().Build(),
            new InterfaceApplicationDbContextBuilder().Build().Object);

        var result = await login.HandleAsync(request, CancellationToken.None);

        var expectedTypResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ErrorResponse>(expectedTypResult.Value);
    }

    [Fact]
    public async Task LoginShouldReturnBadRequestWhenInvalidPassword()
    {
        var request = new LoginRequest {Username = "test@mail.com", Password = "Qwerty@1234"};

        var login = new Login(
            new UserServiceBuilder()
                .SetupGetUserByUsername(request.Username!, new UserBuilder().Username(request.Username).Build())
                .Build().Object,
            new JwtServiceBuilder().Build(),
            new InterfaceDateTimeServiceBuilder().Build(),
            new InterfaceApplicationDbContextBuilder().Build().Object);

        var result = await login.HandleAsync(request, CancellationToken.None);

        var expectedTypResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<ErrorResponse>(expectedTypResult.Value);
    }

    [Fact]
    public async Task LoginHandleCorrectFlow()
    {
        var request = new LoginRequest {Username = "test@mail.com", Password = "Qwerty@1234"};

        var fakeUser = new UserBuilder().Username(request.Username).Password(request.Password).Build();

        var fakeUserService = new UserServiceBuilder()
            .SetupGetUserByUsername(request.Username!, fakeUser)
            .SetupCheckPassword(fakeUser.UserId, request.Password, true)
            .Build();

        var fakeApplicationDbContext = new InterfaceApplicationDbContextBuilder().Build();

        var login = new Login(
            fakeUserService.Object,
            new JwtServiceBuilder().Build(),
            new InterfaceDateTimeServiceBuilder().Build(),
            fakeApplicationDbContext.Object);

        var result = await login.HandleAsync(request, CancellationToken.None);

        var expectedTypResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<LoginResponse>(expectedTypResult.Value);

        fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        fakeUser.Events.Should().NotBeNullOrEmpty();
        fakeUser.UserTokens.Should().NotBeNullOrEmpty();
        fakeUser.UserLogins.Should().NotBeNullOrEmpty();
    }
}