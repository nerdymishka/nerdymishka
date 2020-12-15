using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NerdyMishka.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class NamingConventionsExtensions
    {
        public static DbContextOptionsBuilder UseNamingConvention(
            this DbContextOptionsBuilder optionsBuilder,
            INameTransform transformer)
        {
            optionsBuilder = optionsBuilder ?? throw new ArgumentNullException(nameof(optionsBuilder));

            var extension = (optionsBuilder.Options.FindExtension<NamingConventionOptionsExtension>()
                             ?? new NamingConventionOptionsExtension())
                .WithNameTransform(transformer);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }
    }
}
