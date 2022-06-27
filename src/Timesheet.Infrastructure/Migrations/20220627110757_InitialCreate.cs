using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AffectedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MstPermission",
                columns: table => new
                {
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MstPermission", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "MstRole",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MstRole", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "MstUser",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedUsername = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    HashedPassword = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MstUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TrxRolePermission",
                columns: table => new
                {
                    RolePermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrxRolePermission", x => x.RolePermissionId);
                    table.ForeignKey(
                        name: "FK_TrxRolePermission_MstPermission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "MstPermission",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrxRolePermission_MstRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "MstRole",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HtrUserLogin",
                columns: table => new
                {
                    UserLoginId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UserAgentDetail = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    InEurope = table.Column<bool>(type: "bit", nullable: false),
                    City = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Asn = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AsnOrganization = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtrUserLogin", x => x.UserLoginId);
                    table.ForeignKey(
                        name: "FK_HtrUserLogin_MstUser_UserId",
                        column: x => x.UserId,
                        principalTable: "MstUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HtrUserPassword",
                columns: table => new
                {
                    UserPasswordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtrUserPassword", x => x.UserPasswordId);
                    table.ForeignKey(
                        name: "FK_HtrUserPassword_MstUser_UserId",
                        column: x => x.UserId,
                        principalTable: "MstUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrxUserRole",
                columns: table => new
                {
                    UserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrxUserRole", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "FK_TrxUserRole_MstRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "MstRole",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrxUserRole_MstUser_UserId",
                        column: x => x.UserId,
                        principalTable: "MstUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrxUserToken",
                columns: table => new
                {
                    UserTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ExpiredUtcDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    LastModifiedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DeletedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrxUserToken", x => x.UserTokenId);
                    table.ForeignKey(
                        name: "FK_TrxUserToken_MstUser_UserId",
                        column: x => x.UserId,
                        principalTable: "MstUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserLogin_CreatedBy",
                table: "HtrUserLogin",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserLogin_CreatedDt",
                table: "HtrUserLogin",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserLogin_LastModifiedBy",
                table: "HtrUserLogin",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserLogin_LastModifiedDt",
                table: "HtrUserLogin",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserLogin_UserId",
                table: "HtrUserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserPassword_CreatedBy",
                table: "HtrUserPassword",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserPassword_CreatedDt",
                table: "HtrUserPassword",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserPassword_LastModifiedBy",
                table: "HtrUserPassword",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserPassword_LastModifiedDt",
                table: "HtrUserPassword",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_HtrUserPassword_UserId",
                table: "HtrUserPassword",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MstPermission_CreatedBy",
                table: "MstPermission",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MstPermission_CreatedDt",
                table: "MstPermission",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_MstPermission_LastModifiedBy",
                table: "MstPermission",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MstPermission_LastModifiedDt",
                table: "MstPermission",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_MstPermission_Name",
                table: "MstPermission",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstRole_CreatedBy",
                table: "MstRole",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MstRole_CreatedDt",
                table: "MstRole",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_MstRole_LastModifiedBy",
                table: "MstRole",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MstRole_LastModifiedDt",
                table: "MstRole",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_MstRole_Name",
                table: "MstRole",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstRole_NormalizedName",
                table: "MstRole",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MstUser_CreatedBy",
                table: "MstUser",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MstUser_CreatedDt",
                table: "MstUser",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_MstUser_LastModifiedBy",
                table: "MstUser",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MstUser_LastModifiedDt",
                table: "MstUser",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_MstUser_NormalizedUsername",
                table: "MstUser",
                column: "NormalizedUsername");

            migrationBuilder.CreateIndex(
                name: "IX_MstUser_Username",
                table: "MstUser",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_TrxRolePermission_CreatedBy",
                table: "TrxRolePermission",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrxRolePermission_CreatedDt",
                table: "TrxRolePermission",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_TrxRolePermission_LastModifiedBy",
                table: "TrxRolePermission",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrxRolePermission_LastModifiedDt",
                table: "TrxRolePermission",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_TrxRolePermission_PermissionId",
                table: "TrxRolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrxRolePermission_RoleId",
                table: "TrxRolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserRole_CreatedBy",
                table: "TrxUserRole",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserRole_CreatedDt",
                table: "TrxUserRole",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserRole_LastModifiedBy",
                table: "TrxUserRole",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserRole_LastModifiedDt",
                table: "TrxUserRole",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserRole_RoleId",
                table: "TrxUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserRole_UserId",
                table: "TrxUserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserToken_CreatedBy",
                table: "TrxUserToken",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserToken_CreatedDt",
                table: "TrxUserToken",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserToken_LastModifiedBy",
                table: "TrxUserToken",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserToken_LastModifiedDt",
                table: "TrxUserToken",
                column: "LastModifiedDt");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserToken_RefreshToken",
                table: "TrxUserToken",
                column: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_TrxUserToken_UserId",
                table: "TrxUserToken",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "HtrUserLogin");

            migrationBuilder.DropTable(
                name: "HtrUserPassword");

            migrationBuilder.DropTable(
                name: "TrxRolePermission");

            migrationBuilder.DropTable(
                name: "TrxUserRole");

            migrationBuilder.DropTable(
                name: "TrxUserToken");

            migrationBuilder.DropTable(
                name: "MstPermission");

            migrationBuilder.DropTable(
                name: "MstRole");

            migrationBuilder.DropTable(
                name: "MstUser");
        }
    }
}
