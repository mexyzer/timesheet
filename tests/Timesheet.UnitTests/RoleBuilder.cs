using System;
using Timesheet.Core.Entities;

namespace Timesheet.UnitTests;

public class RoleBuilder
{
	private Role _role = new Role();

	public RoleBuilder WithDefaultValues()
	{
		_role = new Role {Name = "Test"};
		_role.NormalizedName = _role.Name.ToUpperInvariant();

		return this;
	}

	public RoleBuilder Id(Guid id)
	{
		_role.RoleId = id;

		return this;
	}

	public RoleBuilder Name(string name)
	{
		_role.Name = name;
		_role.NormalizedName = _role.Name.ToUpperInvariant();

		return this;
	}

	public RoleBuilder AddPermission(string permissionId)
	{
		_role.RolePermissions.Add(new RolePermission {PermissionId = new Guid(permissionId)});
		return this;
	}

	public RoleBuilder AddPermission(Guid permissionId)
	{
		_role.RolePermissions.Add(new RolePermission {PermissionId = permissionId});
		return this;
	}

	public Role Build()
	{
		return _role;
	}
}