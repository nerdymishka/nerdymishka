using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using NerdyMishka;

namespace Nex.Data.Mssql.Migrations
{
    public static class MigrationExtensions
    {
        public static MigrationBuilder CreateRole(this MigrationBuilder builder, string roleName)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Sql($"CREATE ROLE [{roleName}]");

            return builder;
        }

        public static MigrationBuilder DropRole(this MigrationBuilder builder, string roleName)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Sql($"DROP ROLE [{roleName}]");

            return builder;
        }

        public static MigrationBuilder GrantAll(this MigrationBuilder builder, string objectName, string role)
        {
            Check.ArgNotNull(builder, nameof(builder));

            return builder.Grant(objectName, role, "ALL");
        }

        public static MigrationBuilder GrantExec(this MigrationBuilder builder, string objectName, string role)
        {
            Check.ArgNotNull(builder, nameof(builder));

            return builder.Grant(objectName, role, "Execute");
        }

        public static MigrationBuilder Grant(this MigrationBuilder builder, string objectName, string role, params string[] permissions)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Sql($"GRANT {string.Join(",", permissions)} ON {objectName} TO {role}");

            return builder;
        }
    }
}
