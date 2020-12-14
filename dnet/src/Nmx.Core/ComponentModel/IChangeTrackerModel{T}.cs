using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NerdyMishka.ComponentModel
{
    public interface IChangeTrackerModel<T> :
        IChangeTracker,
        INotifyPropertyChanged
        where T : IChangeTrackerModel<T>
    {
        IReadOnlyCollection<string> ChangedProperties { get; }

        bool IsNew { get; }

        bool IsDeleted { get; }

        void MarkDeleted(bool deleted = true);

        void SetInitialValues(Action<T> action);
    }
}
