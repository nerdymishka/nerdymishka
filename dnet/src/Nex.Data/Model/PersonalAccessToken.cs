using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nex.Data.Model
{
    public class PersonalAccessToken
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public HashSet<Role> Roles { get; set; }

        public User User { get; set; }
    }
}
