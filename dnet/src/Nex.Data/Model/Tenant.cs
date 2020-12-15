using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nex.Data.Model
{
    public class Tenant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid SyncId { get; set; }

        public HashSet<TenantDomain> Domains { get; set; }

        public HashSet<User> Users { get; set; }

        public HashSet<User> GuestUsers { get; set; }

        public HashSet<Role> Roles { get; set; }

        public HashSet<Group> Groups { get; set; }

        public HashSet<ServiceAccount> ServiceAccounts { get; set; }
    }
}
