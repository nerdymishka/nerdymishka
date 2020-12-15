using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NerdyMishka.EntityFrameworkCore.Infrastructure;

namespace NerdyMishka.EntityFrameworkCore
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    public static class NamingConventionsServiceCollectionExtensions
    {
        /// <summary>
        /// <para>
        /// Adds the services required for applying naming conventions in Entity Framework Core.
        /// You use this method when using dependency injection in your application, such as with ASP.NET.
        /// For more information on setting up dependency injection, see http://go.microsoft.com/fwlink/?LinkId=526890.
        /// </para>
        /// <para>
        /// You only need to use this functionality when you want Entity Framework to resolve the services it uses
        /// from an external dependency injection container. If you are not using an external
        /// dependency injection container, Entity Framework will take care of creating the services it requires.
        /// </para>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>
        /// The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddEntityFrameworkNamingConventions(
            [NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

            new EntityFrameworkServicesBuilder(serviceCollection)
                .TryAdd<IConventionSetPlugin, NamingConventionSetPlugin>();

            return serviceCollection;
        }
    }
}
