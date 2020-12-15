using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerdyMishka;

namespace Nex.Data.Model.Configuration
{
    public class NexIamConfiguration : IEntityTypeConfiguration<Permission>,
        IEntityTypeConfiguration<PersonalAccessToken>,
        IEntityTypeConfiguration<Role>,
        IEntityTypeConfiguration<Group>,
        IEntityTypeConfiguration<User>,
        IEntityTypeConfiguration<ServiceAccount>,
        IEntityTypeConfiguration<ServiceAccountKey>,
        IEntityTypeConfiguration<Tenant>
    {

        public static ModelBuilder Configure(ModelBuilder builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            var config = new NexIamConfiguration();
            config.Configure(builder.Entity<Permission>());
            config.Configure(builder.Entity<PersonalAccessToken>());
            config.Configure(builder.Entity<Role>());
            config.Configure(builder.Entity<Group>());
            config.Configure(builder.Entity<User>());
            config.Configure(builder.Entity<ServiceAccount>());
            config.Configure(builder.Entity<ServiceAccountKey>());
            config.Configure(builder.Entity<Tenant>());

            return builder;
        }

        /// <summary>
        ///     Configures the entity of type <see cref="Permission"/>.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Id)
                .ValueGeneratedNever();

            builder.Property(o => o.Name)
                .HasMaxLength(256)
                .IsRequired();
        }

        /// <summary>
        ///     Configures the entity of type <see cref="PersonalAccessToken" />.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<PersonalAccessToken> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Value)
                .HasMaxLength(1024)
                .IsRequired();

            builder.Property(o => o.Name)
                .HasMaxLength(128)
                .IsRequired();
        }

        /// <summary>
        ///     Configures the entity of type <see cref="Role"/>.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasMany(o => o.Permissions)
                .WithMany(o => o.Roles)
                .UsingEntity(o => o.ToTable("role_permissions"));
        }

        /// <summary>
        ///     Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasMany(o => o.Permissions)
                .WithMany(o => o.Groups)
                .UsingEntity(o => o.ToTable("group_permissions"));

            builder.HasMany(o => o.Roles)
                .WithMany(o => o.Groups)
                .UsingEntity(o => o.ToTable("group_roles"));

            builder.HasMany(o => o.ServiceAccounts)
                .WithMany(o => o.Groups)
                .UsingEntity(o => o.ToTable("group_service_accounts"));
        }

        /// <summary>
        ///     Configures the entity of type <see cref="User"/>.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Pseudonym)
                .HasMaxLength(128);

            builder.Property(o => o.Email)
                .HasMaxLength(256);

            builder.Property(o => o.EmailHash)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasIndex(o => o.Email);
            builder.HasIndex(o => o.EmailHash);

            builder.HasMany(o => o.OwnedGroups)
                .WithMany(o => o.Owners)
                .UsingEntity(o => o.ToTable("group_owners"));

            builder.HasMany(o => o.Groups)
                .WithMany(o => o.Members)
                .UsingEntity(o => o.ToTable("group_members"));

            builder.HasMany(o => o.Permissions)
                .WithMany(o => o.Users)
                .UsingEntity(o => o.ToTable("user_permissions"));

            builder.HasMany(o => o.Roles)
                .WithMany(o => o.Users)
                .UsingEntity(o => o.ToTable("user_roles"));

        }

        /// <summary>
        ///     Configures the entity of type <see cref="ServiceAccount"/>.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<ServiceAccount> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasIndex(o => o.Name);

            builder.HasMany(o => o.Roles)
                .WithMany(o => o.ServiceAccounts)
                .UsingEntity(o => o.ToTable("service_account_roles"));
        }

        /// <summary>
        ///     Configures the entity of type <see cref="Tenant"/>.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasMany(o => o.Users)
                .WithOne(o => o.Tenant);

            builder.HasMany(o => o.GuestUsers)
                .WithMany(o => o.GuestTenants)
                .UsingEntity(o => o.ToTable("tenant_guest_users"));

            builder.HasMany(o => o.ServiceAccounts)
                .WithMany(o => o.Tenants)
                .UsingEntity(o => o.ToTable("tenant_service_accounts"));

            builder.HasIndex(o => o.Name);
        }

        /// <summary>
        ///     Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the entity type. </param>
        public void Configure(EntityTypeBuilder<ServiceAccountKey> builder)
        {
            Check.ArgNotNull(builder, nameof(builder));

            builder.Property(o => o.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasIndex(o => o.Name);
        }
    }
}
