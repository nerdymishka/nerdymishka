using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NerdyMishka.Models
{
    public interface IModel<T> :
        IChangeTracker,
        INotifyPropertyChanged,
        IDataErrorInfo
        where T : IModel<T>
    {
        IReadOnlyList<string> ChangedProperies { get; }

        bool IsNew { get; }

        bool IsDeleted { get; }

        void MarkDeleted(bool deleted = true);

        void SetInitialValues(Action<T> action);
    }
}