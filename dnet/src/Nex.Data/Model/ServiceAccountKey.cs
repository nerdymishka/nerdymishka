using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nex.Data.Model
{
    public class ServiceAccountKey
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
