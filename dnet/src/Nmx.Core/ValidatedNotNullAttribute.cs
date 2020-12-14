using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
        public ValidatedNotNullAttribute()
        {
        }
    }
}
