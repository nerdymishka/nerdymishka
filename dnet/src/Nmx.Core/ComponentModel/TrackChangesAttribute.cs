using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class TrackChangesAttribute : Attribute
    {
        public TrackChangesAttribute()
        {
        }
    }
}
