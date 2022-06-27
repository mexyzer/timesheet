using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Infrastructure.Data;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Timesheet.Infrastructure;

public static class StartupSetup
{
    public static void AddDefaultDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(_ =>
            new QueryFactory(new SqlConnection(connectionString), new SqlServerCompiler()));
    }
}