using System;
using Timesheet.Core.Entities;

namespace Timesheet.UnitTests;

public class UserTokenBuilder
{
	private readonly UserToken _userToken;

	public UserTokenBuilder()
	{
		_userToken = new UserToken {ExpiredUtcDt = DateTime.UtcNow.AddDays(7)};
	}

	public UserTokenBuilder UserTokenId(Guid userTokenId)
	{
		_userToken.UserTokenId = userTokenId;
		return this;
	}

	public UserTokenBuilder UserId(Guid userId)
	{
		_userToken.UserId = userId;
		return this;
	}

	public UserTokenBuilder RefreshToken(string refreshToken)
	{
		_userToken.RefreshToken = refreshToken;
		return this;
	}

	public UserTokenBuilder Expired()
	{
		_userToken.ExpiredUtcDt = DateTime.UtcNow.AddDays(-999);
		return this;
	}

	public UserTokenBuilder IsUsed()
	{
		_userToken.IsUsed = true;
		return this;
	}

	public UserToken Build()
	{
		return _userToken;
	}
}