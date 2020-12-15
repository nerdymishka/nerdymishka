using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NerdyMishka.EntityFrameworkCore.Infrastructure;
using Nex.Data.Mssql.Model;

namespace Nex.Data.Mssql.Model
{
    public class NexMssqlDbContextFactory : IDesignTimeDbContextFactory<NexMssqlDbContext>
    {
        /// <summary>Creates a new instance of a derived context.</summary>
        /// <param name="args"> Arguments provided by the design-time service. </param>
        /// <returns> An instance of <typeparamref name="TContext" />. </returns>
        public NexMssqlDbContext CreateDbContext(string[] args)
        {
            var location = typeof(NexMssqlDbContext).Assembly.Location;
            var directory = Path.GetDirectoryName(location);
            if (directory is null)
                throw new NullReferenceException("assembly location returned a null path");

            while (directory.Contains("\\bin"))
            {
                directory = Path.Combine(directory, "..");
                directory = Path.GetFullPath(directory);
            }

            var possibilities = new string[]
            {
                "NMX_ENVIRONMENT",
                "ASPNETCORE_ENVIRONMENT",
                "DOTNET_ENVIRONMENT",
                "DOTNETCORE_ENVIRONMENT",
            };

            var env = "Development";

            foreach (var possibility in possibilities)
            {
                var tmp = Environment.GetEnvironmentVariable(possibility);
                if (tmp is not null)
                {
                    env = tmp;
                    break;
                }
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{env}.json", true)
                .Build();

            var options = new DbContextOptionsBuilder();
            var connectionString = Environment.GetEnvironmentVariable("NMX_MSSQL_CONNECTION_STRING");
            connectionString ??= config.GetSection("nexus:db:connectionString")?.Value;
            connectionString ??= config.GetConnectionString("Nexus");
            connectionString ??= "";
            connectionString ??= "Server=(localdb)\\v11.0;Integrated Security=true;AttachDbFileName=.\\Nex_Test.mdf";

            options.UseNamingConvention(new PluralSnakeCaseNameTransform());
            options.UseSqlServer(connectionString)
                .EnableDetailedErrors();

            return new NexMssqlDbContext(options.Options);
        }
    }
}
