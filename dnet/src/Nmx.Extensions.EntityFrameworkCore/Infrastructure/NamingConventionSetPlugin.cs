using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace NerdyMishka.EntityFrameworkCore.Infrastructure
{
    public class NamingConventionSetPlugin : IConventionSetPlugin
    {
        private readonly IDbContextOptions options;

        public NamingConventionSetPlugin([NotNull] IDbContextOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ConventionSet ModifyConventions(ConventionSet conventionSet)
        {
            conventionSet = conventionSet ?? throw new ArgumentNullException(nameof(conventionSet));

            var extension = this.options.FindExtension<NamingConventionOptionsExtension>();
            var transformer = extension.NameTransform;
            var culture = extension.Culture;
            transformer.Culture = culture ?? CultureInfo.InvariantCulture;

            var convention = new NamingConvention(transformer);

            conventionSet.EntityTypeAddedConventions.Add(convention);
            conventionSet.EntityTypeAnnotationChangedConventions.Add(convention);
            conventionSet.PropertyAddedConventions.Add(convention);
            conventionSet.ForeignKeyOwnershipChangedConventions.Add(convention);
            conventionSet.KeyAddedConventions.Add(convention);
            conventionSet.ForeignKeyAddedConventions.Add(convention);
            conventionSet.IndexAddedConventions.Add(convention);
            conventionSet.EntityTypeBaseTypeChangedConventions.Add(convention);

            return conventionSet;
        }
    }
}
