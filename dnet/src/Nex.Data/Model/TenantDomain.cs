using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nex.Data.Model
{
    public class TenantDomain
    {
        public int Id { get; set; }

        public string Domain { get; set; }

        public int TenantId { get; set; }

        public Tenant Tenant { get; set; }
    }
}
