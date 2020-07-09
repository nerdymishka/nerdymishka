using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace NerdyMishka.Models
{
    public interface IModelCollection<T> :
        ICollection<T>,
        INotifyCollectionChanged,
        IChangeTracker
    {
        IReadOnlyList<T> Additions { get; }

        IReadOnlyList<T> Removals { get; }

        void SetInitialValues(IEnumerable<T> values);

        void SetInitialValues(Action<IModelCollection<T>> action);
    }
}