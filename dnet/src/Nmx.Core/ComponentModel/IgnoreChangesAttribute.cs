using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.ComponentModel
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreChangesAttribute : Attribute
    {
        public IgnoreChangesAttribute()
        {
        }
    }
}
