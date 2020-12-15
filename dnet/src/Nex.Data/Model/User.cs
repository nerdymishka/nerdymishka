// ***********************************************************************
// Assembly         : Nex.Data
// Author           : MichaelHerndon
// Created          : 12-14-2020
//
// Last Modified By : MichaelHerndon
// Last Modified On : 12-15-2020
// ***********************************************************************
// <copyright file="User.cs" company="Nerdy Mishka">
//     NerdyMishka © 2016 - 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nex.Data.Model
{
    /// <summary>
    /// Class User.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     The <see cref="User"/> class must be kept minimal. Profile information,
    ///     password and other information must be stored in other tables. 
    ///     </para>
    /// </remarks>
    public class User
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the pseudonym. A pseudonym is encouraged
        /// for GDPR purposes. 
        /// </summary>
        /// <value>The pseudonym.</value>
        public string Pseudonym { get; set; }


        /// <summary>
        /// Gets or sets the email. The e-mail can be removed for
        /// GDPR purposes.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the email hash.
        /// </summary>
        /// <value>The email hash.</value>
        public string EmailHash { get; set; }

        /// <summary>
        /// Gets or sets the synchronize identifier.
        /// </summary>
        /// <value>The synchronize identifier.</value>
        public Guid SyncId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        /// <value>The tenant.</value>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public HashSet<Role> Roles { get; set; }

        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        /// <value>The permissions.</value>
        public HashSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Gets or sets the access tokens.
        /// </summary>
        /// <value>The access tokens.</value>
        public HashSet<PersonalAccessToken> AccessTokens { get; set; }

        /// <summary>
        /// Gets or sets the group the user manages.
        /// </summary>
        /// <value>The owned groups.</value>
        public HashSet<Group> OwnedGroups { get; set; }

        /// <summary>
        /// Gets or sets the tenants the user where the user is
        /// a guest.
        /// </summary>
        /// <value>The guest tenants.</value>
        public HashSet<Tenant> GuestTenants { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public HashSet<Group> Groups { get; set; }
    }
}
