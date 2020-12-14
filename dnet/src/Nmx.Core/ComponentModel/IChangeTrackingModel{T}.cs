using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NerdyMishka.ComponentModel
{
    public interface IChangeTrackingModel<T> :
        IChangeTracker,
        INotifyPropertyChanged,
        IDataErrorInfo
        where T : IChangeTrackingModel<T>
    {
        IReadOnlyList<string> ChangedProperties { get; }

        bool IsNew { get; }

        bool IsDeleted { get; }

        void MarkDeleted(bool deleted = true);

        void SetInitialValues(Action<T> action);
    }
}
