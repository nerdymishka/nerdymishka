using System.Collections.Generic;
using System.ComponentModel;

namespace NerdyMishka.Models
{
    public interface IChangeTracker :
        ISupportInitialize,
        IChangeTracking,
        IRevertibleChangeTracking
    {
        IReadOnlyList<IChangeTracker> Descendants { get; }
    }
}