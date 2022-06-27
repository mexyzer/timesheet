using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Timesheet.Core.Entities;
using Timesheet.WebApi;
using Timesheet.WebApi.EndPoints.Users;
using Xunit;

namespace Timesheet.UnitTests.Endpoints.Users;

public class RefreshTests
{
	[Fact]
	public async Task RefreshShouldReturnNotFoundWhenUserTokenByIdentifierIsNull()
	{
		var request = new RefreshTokenRequest {Token = "ABBCC"};
		string idt = Guid.NewGuid().ToString();
		string userId = Guid.NewGuid().ToString();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupIdt(idt);
		fakeCurrentUserServiceBuilder.SetupUserId(userId);
		var fakeCurrentUSerService = fakeCurrentUserServiceBuilder.Build().Object;

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		fakeApplicationDbContextBuilder.SetupUserToken(new List<UserToken>());
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeJwtServiceBuilder = new JwtServiceBuilder();
		var fakeJwtService = fakeJwtServiceBuilder.Build();

		var fakeDateTimeServiceBuilder = new InterfaceDateTimeServiceBuilder();
		var fakeDateTimeService = fakeDateTimeServiceBuilder.Build();

		var handler = new RefreshToken(fakeCurrentUSerService, fakeJwtService, fakeDateTimeService,
			fakeApplicationDbContext.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Result.Should().BeOfType<NotFoundObjectResult>();

		var resultValue = (ObjectResult)result.Result!;
		resultValue.Value.Should().BeOfType<ErrorResponse>();

		var value = (ErrorResponse)resultValue.Value!;
		value.Message.Should().Be("Data not found");
	}

	[Fact]
	public async Task RefreshShouldReturnBadRequestWhenUserTokenIsUsed()
	{
		var request = new RefreshTokenRequest {Token = "ABBCC"};
		string idt = Guid.NewGuid().ToString();
		string userId = Guid.NewGuid().ToString();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupIdt(idt);
		fakeCurrentUserServiceBuilder.SetupUserId(userId);
		var fakeCurrentUSerService = fakeCurrentUserServiceBuilder.Build().Object;

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var userTokens = new List<UserToken>
		{
			new UserTokenBuilder().UserId(new Guid(userId)).UserTokenId(new Guid(idt)).IsUsed().Build()
		};
		fakeApplicationDbContextBuilder.SetupUserToken(userTokens);
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeJwtServiceBuilder = new JwtServiceBuilder();
		var fakeJwtService = fakeJwtServiceBuilder.Build();

		var fakeDateTimeServiceBuilder = new InterfaceDateTimeServiceBuilder();
		var fakeDateTimeService = fakeDateTimeServiceBuilder.Build();

		var handler = new RefreshToken(fakeCurrentUSerService, fakeJwtService, fakeDateTimeService,
			fakeApplicationDbContext.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Result.Should().BeOfType<BadRequestObjectResult>();

		var resultValue = (ObjectResult)result.Result!;
		resultValue.Value.Should().BeOfType<ErrorResponse>();

		var value = (ErrorResponse)resultValue.Value!;
		value.Message.Should().Be("Token already used");
	}

	[Fact]
	public async Task RefreshShouldReturnBadRequestWhenInvalidRequestToken()
	{
		var request = new RefreshTokenRequest {Token = "ABBCC"};
		string idt = Guid.NewGuid().ToString();
		string userId = Guid.NewGuid().ToString();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupIdt(idt);
		fakeCurrentUserServiceBuilder.SetupUserId(userId);
		var fakeCurrentUSerService = fakeCurrentUserServiceBuilder.Build().Object;

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var userTokens = new List<UserToken>
		{
			new UserTokenBuilder().UserId(new Guid(userId)).UserTokenId(new Guid(idt)).RefreshToken("Hahahihi")
				.Build()
		};
		fakeApplicationDbContextBuilder.SetupUserToken(userTokens);
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeJwtServiceBuilder = new JwtServiceBuilder();
		var fakeJwtService = fakeJwtServiceBuilder.Build();

		var fakeDateTimeServiceBuilder = new InterfaceDateTimeServiceBuilder();
		var fakeDateTimeService = fakeDateTimeServiceBuilder.Build();

		var handler = new RefreshToken(fakeCurrentUSerService, fakeJwtService, fakeDateTimeService,
			fakeApplicationDbContext.Object);

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Result.Should().BeOfType<BadRequestObjectResult>();

		var resultValue = (ObjectResult)result.Result!;
		resultValue.Value.Should().BeOfType<ErrorResponse>();

		var value = (ErrorResponse)resultValue.Value!;
		value.Message.Should().Be("Invalid token");
	}

	[Fact]
	public async Task RefreshShouldReturnBadRequestWhenUserTokenIsExpired()
	{
		var request = new RefreshTokenRequest {Token = "ABBCC"};
		string idt = Guid.NewGuid().ToString();
		string userId = Guid.NewGuid().ToString();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupIdt(idt);
		fakeCurrentUserServiceBuilder.SetupUserId(userId);
		var fakeCurrentUSerService = fakeCurrentUserServiceBuilder.Build().Object;

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();
		var fakeUserToken = new UserTokenBuilder().UserId(new Guid(userId)).UserTokenId(new Guid(idt))
			.RefreshToken(request.Token)
			.Expired().Build();
		var userTokens = new List<UserToken> {fakeUserToken};
		fakeApplicationDbContextBuilder.SetupUserToken(userTokens);
		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeJwtServiceBuilder = new JwtServiceBuilder();
		var fakeJwtService = fakeJwtServiceBuilder.Build();

		var fakeDateTimeServiceBuilder = new InterfaceDateTimeServiceBuilder();
		var fakeDateTimeService = fakeDateTimeServiceBuilder.Build();

		var handler = new RefreshToken(fakeCurrentUSerService, fakeJwtService, fakeDateTimeService,
			fakeApplicationDbContext.Object);

		//before
		fakeUserToken.IsActive.Should().BeTrue();
		fakeUserToken.IsUsed.Should().BeFalse();

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Result.Should().BeOfType<BadRequestObjectResult>();

		var resultValue = (ObjectResult)result.Result!;
		resultValue.Value.Should().BeOfType<ErrorResponse>();

		var value = (ErrorResponse)resultValue.Value!;
		value.Message.Should().Be("Token expired");

		//after
		fakeUserToken.IsActive.Should().BeFalse();
		fakeUserToken.IsUsed.Should().BeTrue();

		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task RefreshTokenCorrectFlow()
	{
		var request = new RefreshTokenRequest {Token = "ABBCC"};
		string idt = Guid.NewGuid().ToString();
		string userId = Guid.NewGuid().ToString();

		var fakeCurrentUserServiceBuilder = new InterfaceCurrentUserServiceBuilder();
		fakeCurrentUserServiceBuilder.SetupIdt(idt);
		fakeCurrentUserServiceBuilder.SetupUserId(userId);
		var fakeCurrentUSerService = fakeCurrentUserServiceBuilder.Build().Object;

		var fakeApplicationDbContextBuilder = new InterfaceApplicationDbContextBuilder();

		var fakeUser = new UserBuilder().WithDefaultValues().Id(new Guid(userId)).Build();
		var fakeUserToken = new UserTokenBuilder().UserId(new Guid(userId)).UserTokenId(new Guid(idt))
			.RefreshToken(request.Token).Build();
		fakeUserToken.User = fakeUser;
		var userTokens = new List<UserToken> {fakeUserToken};
		fakeApplicationDbContextBuilder.SetupUserToken(userTokens);

		var fakeApplicationDbContext = fakeApplicationDbContextBuilder.Build();

		var fakeJwtServiceBuilder = new JwtServiceBuilder();
		var fakeJwtService = fakeJwtServiceBuilder.Build();

		var fakeDateTimeServiceBuilder = new InterfaceDateTimeServiceBuilder();
		var fakeDateTimeService = fakeDateTimeServiceBuilder.Build();

		var handler = new RefreshToken(fakeCurrentUSerService, fakeJwtService, fakeDateTimeService,
			fakeApplicationDbContext.Object);

		//before
		fakeUserToken.IsActive.Should().BeTrue();
		fakeUserToken.IsUsed.Should().BeFalse();

		var result = await handler.HandleAsync(request, CancellationToken.None);

		result.Result.Should().BeOfType<OkObjectResult>();

		var resultValue = (ObjectResult)result.Result!;
		resultValue.Value.Should().BeOfType<LoginResponse>();

		//after
		fakeUserToken.IsActive.Should().BeFalse();
		fakeUserToken.IsUsed.Should().BeTrue();

		//user entity result from user service result should has user-tokens count 1
		fakeUser.UserTokens.Should().NotBeEmpty();
		fakeUser.UserTokens.Should().HaveCount(1);
		fakeUser.UserTokens.First().ExpiredUtcDt.Should().Be(fakeDateTimeService.ScopedUtcNow.AddMonths(1));

		fakeApplicationDbContext.Verify(e => e.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}