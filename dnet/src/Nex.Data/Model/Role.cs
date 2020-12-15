// ***********************************************************************
// Assembly         : Nex.Data
// Author           : MichaelHerndon
// Created          : 12-14-2020
//
// Last Modified By : MichaelHerndon
// Last Modified On : 12-15-2020
// ***********************************************************************
// <copyright file="Role.cs" company="Nerdy Mishka">
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
    /// Class Role.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the known role identifier. Known roles are
        /// built-in defaults that cannot be customized unless the id
        /// is for the custom role.
        /// </summary>
        /// <value>The known role identifier.</value>
        public short KnownRoleId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the synchronize identifier.
        /// </summary>
        /// <value>The synchronize identifier.</value>
        public Guid SyncId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        /// <value>The tenant.</value>
        public Tenant Tenant { get; set; }

        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        /// <value>The permissions.</value>
        public HashSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public HashSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>The groups.</value>
        public HashSet<Group> Groups { get; set; }

        /// <summary>
        /// Gets or sets the service accounts.
        /// </summary>
        /// <value>The service accounts.</value>
        public HashSet<ServiceAccount> ServiceAccounts { get; set; }
    }
}
