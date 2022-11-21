using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Reserve.Data;

namespace Reserve.API.Configurations
{
    public static class MySQLConfigurations
    {
        public static void AddMySQL(this IServiceCollection services, IConfiguration configured)
        {
            services
                .AddDbContext<DatabaseContext>(options =>
                {
                    options.UseMySql(
                        configured.GetConnectionString("Development"),
                        opts => { opts.ServerVersion(new Version(8, 0, 19), ServerType.MySql); }
                    );
                });
        }
    }
}