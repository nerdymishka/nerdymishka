using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nex.Data.Model
{
    public class ServiceAccount
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid SyncId { get; set; }

        public HashSet<PersonalAccessToken> ApiKeys { get; set; }

        public HashSet<Role> Roles { get; set; }

        public HashSet<Tenant> Tenants { get; set; }

        public HashSet<Group> Groups { get; set; }
    }
}
