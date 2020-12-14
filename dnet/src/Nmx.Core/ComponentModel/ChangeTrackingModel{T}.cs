using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NerdyMishka.ComponentModel
{
    public class ChangeTrackingModel<T> : IChangeTrackerModel<T>
        where T : ChangeTrackingModel<T>, IChangeTrackerModel<T>
    {
        private readonly Dictionary<string, object> originalValues = new Dictionary<string, object>();

        private readonly HashSet<string> changedProperties =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private Dictionary<string, IChangeTracker> descendants;

        private bool ignoreChanges;

        private bool isNew = true;

        private bool isDeleted;

        private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => this.PropertyChanged += value;
            remove => this.PropertyChanged -= value;
        }

        IReadOnlyCollection<string> IChangeTrackerModel<T>.ChangedProperties
        {
            get
            {
                if (this.changedProperties == null || this.changedProperties.Count == 0)
                    return Array.Empty<string>();

                return this.changedProperties;
            }
        }

        bool IChangeTrackerModel<T>.IsNew => this.isNew;

        bool IChangeTrackerModel<T>.IsDeleted => this.isDeleted;

        IReadOnlyList<IChangeTracker> IChangeTracker.Descendants
            => this.Descendants;

        bool IChangeTracking.IsChanged
        {
            get
            {
                if (this.changedProperties.Count > 0)
                    return true;

                if (this.descendants == null || this.descendants.Count == 0)
                    return false;

                foreach (var tracker in this.descendants.Values)
                    if (tracker.IsChanged)
                        return true;

                return false;
            }
        }

        protected IReadOnlyList<IChangeTracker> Descendants
        {
            get
            {
                if (this.descendants == null || this.descendants.Count == 0)
                    return Array.Empty<IChangeTracker>();

                return this.descendants.Values.ToArray();
            }
        }

        void IChangeTracker.AcceptChanges(bool includeDescendants)
            => this.AcceptChanges(includeDescendants);

        void IChangeTracking.AcceptChanges()
            => this.AcceptChanges(true);

        void ISupportInitialize.BeginInit()
            => this.BeginInit();

        void ISupportInitialize.EndInit()
            => this.EndInit();

        void IChangeTrackerModel<T>.MarkDeleted(bool deleted)
            => this.isDeleted = true;

        void IChangeTracker.RejectChanges(bool includeDescendants)
            => this.RejectChanges(includeDescendants);

        void IRevertibleChangeTracking.RejectChanges()
            => this.RejectChanges(true);

        void IChangeTrackerModel<T>.SetInitialValues(Action<T> action)
            => this.SetInitialValues(action);

        void IChangeTracker.UpdateOriginalValues(bool includeDescendants)
            => this.UpdateOriginalValues(includeDescendants);

        protected virtual void BeginInit() => this.ignoreChanges = true;

        protected virtual void EndInit() => this.ignoreChanges = false;

        protected virtual void RejectChanges(bool includeDescendants)
        {
            this.BeginInit();

            if (includeDescendants)
            {
                foreach (var descendant in this.Descendants)
                    descendant.RejectChanges();
            }

            if (this.changedProperties.Count > 0)
            {
                var cache = ModelReflectionCache.Default;
                var typeInfo = cache.GetOrAdd(this.GetType());
                var properties = typeInfo.
                    Properties
                    .Where(o => o.CanRead && o.CanWrite);

                foreach (var propInfo in properties)
                {
                    if (!this.changedProperties.Contains(propInfo.Name))
                        continue;

                    if (!this.originalValues.TryGetValue(propInfo.Name, out var value))
                        continue;

                    if (value == null &&
                        this.descendants != null &&
                        this.descendants.ContainsKey(propInfo.Name))
                    {
                        this.descendants.Remove(propInfo.Name);
                    }

                    propInfo.SetValue(this, value);
                }

                this.changedProperties.Clear();
            }

            this.EndInit();
        }

        protected void SetInitialValues(Action<T> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            this.BeginInit();
            action((T)this);
            this.EndInit();
            this.isNew = false;
        }

        protected void UpdateOriginalValues(bool includeDescendants)
        {
            this.BeginInit();

            if (includeDescendants)
            {
                foreach (var descendant in this.Descendants)
                    descendant.UpdateOriginalValues(true);
            }

            if (this.changedProperties.Count > 0)
            {
                var cache = ModelReflectionCache.Default;
                var typeInfo = cache.GetOrAdd(this.GetType());
                var properties = typeInfo
                    .Properties
                    .Where(o => o.CanRead && o.CanWrite);

                foreach (var propInfo in properties)
                {
                    if (!this.changedProperties.Contains(propInfo.Name))
                        continue;

                    this.originalValues.Remove(propInfo.Name);
                    this.originalValues.Add(propInfo.Name, propInfo.GetValue(this));
                }

                this.changedProperties.Clear();
            }

            this.EndInit();
        }

        protected virtual void AcceptChanges(bool includeDescendants)
        {
            this.BeginInit();

            if (includeDescendants)
            {
                foreach (var descendant in this.Descendants)
                    descendant.AcceptChanges();
            }

            if (this.changedProperties.Count > 0)
            {
                // TODO: determine if type needs to be cached.
                var cache = ModelReflectionCache.Default;
                var typeInfo = cache.GetOrAdd(this.GetType());
                var properties = typeInfo
                    .Properties
                    .Where(o => o.CanRead && o.CanWrite);

                foreach (var propInfo in properties)
                {
                    if (!this.changedProperties.Contains(propInfo.Name))
                       continue;

                    this.originalValues.Remove(propInfo.Name);
                    this.originalValues.Add(propInfo.Name, propInfo.GetValue(this));
                }

                this.changedProperties.Clear();
            }

            this.isNew = false;
            this.EndInit();
        }

        protected virtual void SetValue<TValue>(string name, ref TValue oldValue, TValue newValue)
        {
            if (newValue is IChangeTracker tracker)
            {
                this.descendants ??= new Dictionary<string, IChangeTracker>();
                this.descendants.Remove(name);

                this.descendants.Add(name, tracker);

                if (Equals(oldValue, newValue))
                    return;

                oldValue = newValue;
                if (!this.ignoreChanges)
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                    this.changedProperties.Add(name);
                }
                else
                {
                    this.originalValues[name] = newValue;
                }

                return;
            }

            if (this.ignoreChanges)
            {
                oldValue = newValue;
                this.originalValues[name] = newValue;
                return;
            }

            if (Equals(oldValue, newValue))
                return;

            oldValue = newValue;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (this.originalValues.TryGetValue(name, out object value))
            {
                if (Equals(value, newValue))
                {
                    if (this.changedProperties.Contains(name))
                        this.changedProperties.Remove(name);

                    return;
                }
            }
            else
            {
                this.originalValues.Add(name, newValue);
            }

            if (!this.changedProperties.Contains(name))
                this.changedProperties.Add(name);
        }
    }
}
