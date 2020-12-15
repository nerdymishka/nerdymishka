using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace NerdyMishka.EntityFrameworkCore.Infrastructure
{
    public class NamingConventionOptionsExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo info;
        private INameTransform transformer;
        private CultureInfo culture;

        public NamingConventionOptionsExtension()
        {
        }

        protected NamingConventionOptionsExtension([NotNull] NamingConventionOptionsExtension copyFrom)
        {
            this.transformer = copyFrom.transformer;
            this.culture = copyFrom.culture;
        }

        public virtual DbContextOptionsExtensionInfo Info => this.info ??= new ExtensionInfo(this);

        internal virtual INameTransform NameTransform => this.transformer;

        internal virtual CultureInfo Culture => this.culture;

        protected virtual NamingConventionOptionsExtension Clone() => new(this);

        public virtual NamingConventionOptionsExtension WithNameTransform(INameTransform transformer, CultureInfo culture = null)
        {
            var clone = this.Clone();
            clone.transformer = transformer;
            clone.culture = culture ?? CultureInfo.InvariantCulture;
            return clone;
        }

        public void Validate(IDbContextOptions options) { }

        public void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkNamingConventions();

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private string logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    if (this.logFragment is not null)
                        return this.logFragment;

                    var builder = new StringBuilder();

                    builder.Append(this.Extension.transformer.Name);

                    if (this.Extension.culture is null)
                    {
                        builder
                            .Append(" (culture=")
                            .Append(this.Extension.culture)
                            .Append(")");
                    }

                    this.logFragment = builder.ToString();

                    return this.logFragment;
                }
            }

            private new NamingConventionOptionsExtension Extension
                => (NamingConventionOptionsExtension)base.Extension;

            public override long GetServiceProviderHashCode()
            {
                var hashCode = Extension.transformer.GetHashCode();
                hashCode = (hashCode * 3) ^ (Extension.culture?.GetHashCode() ?? 0);
                return hashCode;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["Naming:Usetransformer"]
                    = this.Extension.transformer.GetHashCode().ToString(CultureInfo.InvariantCulture);
                if (this.Extension.culture != null)
                {
                    debugInfo["Naming:Culture"]
                        = this.Extension.culture.GetHashCode().ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }
}
