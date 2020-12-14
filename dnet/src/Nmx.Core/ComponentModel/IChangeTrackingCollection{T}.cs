using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace NerdyMishka.ComponentModel
{
    public interface IChangeTrackingCollection<T> :
        ICollection<T>,
        INotifyCollectionChanged,
        IChangeTracker,
        INotifyPropertyChanged
    {
        IReadOnlyCollection<T> Additions { get; }

        IReadOnlyCollection<T> Removals { get; }

        void SetInitialValues(IEnumerable<T> values);

        void SetInitialValues(Action<IChangeTrackingCollection<T>> action);
    }
}
