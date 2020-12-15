using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nex.Data.Model
{
    public class Group
    {
        public int Id { get; set; }

        public int? TenantId { get; set; }

        public string Name { get; set; }

        public Guid SyncId { get; set; }

        public Tenant Tenant { get; set; }

        public HashSet<ServiceAccount> ServiceAccounts { get; set; }

        public HashSet<Role> Roles { get; set; }

        public HashSet<User> Members { get; set; }

        public HashSet<User> Owners { get; set; }

        public HashSet<Permission> Permissions { get; set; }
    }
}
