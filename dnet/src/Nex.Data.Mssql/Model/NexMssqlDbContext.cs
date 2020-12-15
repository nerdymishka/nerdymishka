using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NerdyMishka;
using Nex.Data.Model;

namespace Nex.Data.Mssql.Model
{
    public class NexMssqlDbContext : NexDbContext
    {
        public NexMssqlDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        ///     <para>
        ///         Override this method to configure the database (and other options) to be used for this context.
        ///         This method is called for each instance of the context that is created.
        ///         The base implementation does nothing.
        ///     </para>
        ///     <para>
        ///         In situations where an instance of <see cref="Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
        ///         to the constructor, you can use <see cref="Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
        ///         the options have already been set, and skip some or all of the logic in
        ///         <see cref="Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder">
        ///     A builder used to create or modify options for this context. Databases (and other extensions)
        ///     typically define extension methods on this object that allow you to configure the context.
        /// </param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Check.ArgNotNull(optionsBuilder, nameof(optionsBuilder));

            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("NMX_MSSQL_CONNECTION_STRING");
                connectionString ??= "Server=(localdb)\\v11.0;Integrated Security=true;AttachDbFileName=.\\Nex_Test.mdf";
                optionsBuilder.UseSqlServer(connectionString)
                    .EnableDetailedErrors();
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}
