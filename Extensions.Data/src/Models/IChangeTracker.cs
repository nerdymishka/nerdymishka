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

        void AcceptChanges(bool includeDescendants);

        void RejectChanges(bool includeDescendants);

        void UpdateOriginalValues(bool includeDescendants);
    }
}