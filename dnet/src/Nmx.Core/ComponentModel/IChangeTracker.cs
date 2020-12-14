using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NerdyMishka.ComponentModel
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
