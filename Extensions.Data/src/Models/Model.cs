using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NerdyMishka.Reflection;

namespace NerdyMishka.Models
{
    public abstract class Model<T> :
        IModel<T>, IDataErrorInfo
        where T : Model<T>, IModel<T>
    {
        private readonly Dictionary<string, object> originalValues = new Dictionary<string, object>();

        private readonly HashSet<string> changedProperies =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private Dictionary<string, string> errors;

        private Dictionary<string, IChangeTracker> descendants;

        private bool ignoreChanges = false;

        private bool isNew = true;

        private bool isDeleted = false;

        public Model()
        {
        }

        public Model(Action<T> setInitialValues)
        {
            this.SetInitialValues(setInitialValues);
        }

        protected event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.PropertyChanged += value;
            }

            remove
            {
                this.PropertyChanged += value;
            }
        }

        bool IChangeTracking.IsChanged
        {
            get
            {
                if (this.changedProperies.Count > 0)
                    return true;

                if (this.descendants == null)
                    return false;

                foreach (var tracker in this.descendants.Values)
                    if (tracker.IsChanged)
                        return true;

                return false;
            }
        }

        bool IModel<T>.IsNew => this.isNew;

        bool IModel<T>.IsDeleted => this.isDeleted;

        IReadOnlyList<IChangeTracker> IChangeTracker.Descendants => this.descendants.Values.ToArray();

        IReadOnlyList<string> IModel<T>.ChangedProperies => this.changedProperies.ToArray();

        string IDataErrorInfo.Error
        {
            get
            {
                if (this.errors == null)
                    return null;

                var sb = new System.Text.StringBuilder();
                foreach (var columnName in this.errors.Keys)
                {
                    sb.AppendLine($"{columnName}: ${this.errors[columnName]}");
                }

                return sb.ToString();
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (this.errors == null)
                    return null;

                if (this.errors.TryGetValue(columnName, out string error))
                    return error;

                return null;
            }
        }

        void IChangeTracking.AcceptChanges() => this.AcceptChanges(true);

        void IRevertibleChangeTracking.RejectChanges()
            => this.RejectChanges(true);

        void IChangeTracker.AcceptChanges(bool includeDescendants)
            => this.AcceptChanges(includeDescendants);

        void IChangeTracker.RejectChanges(bool includeDescendants)
            => this.RejectChanges(includeDescendants);

        void IChangeTracker.UpdateOriginalValues(bool includeDescendants)
            => this.UpdateOriginalValues(includeDescendants);

        void ISupportInitialize.BeginInit() => this.BeginInit();

        void ISupportInitialize.EndInit() => this.EndInit();

        void IModel<T>.MarkDeleted(bool deleted) => this.isDeleted = deleted;

        void IModel<T>.SetInitialValues(Action<T> action)
        {
            this.SetInitialValues(action);
        }

        public IModel<T> AsModel()
        {
            return this;
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

            if (includeDescendants && this.descendants != null)
            {
                foreach (var descendant in this.descendants.Values)
                    descendant.UpdateOriginalValues(includeDescendants);
            }

            if (this.changedProperies.Count > 0)
            {
                ReflectionCache cache = ModelReflectionCache.Default;
                var typeInfo = cache.GetOrAdd(this.GetType());
                var properties = typeInfo.Properties.Where(o => o.CanRead && o.CanWrite);

                foreach (var propInfo in properties)
                {
                    if (this.changedProperies.Contains(propInfo.Name))
                    {
                        this.originalValues.Remove(propInfo.Name);
                        this.originalValues.Add(propInfo.Name, propInfo.GetValue(this));
                    }

                    continue;
                }

                this.changedProperies.Clear();
            }

            this.EndInit();
        }

        protected virtual void SetError(string propertyName, string message)
        {
            if (this.errors == null)
                this.errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(message))
                this.errors.Remove(propertyName);
            else
                this.errors[propertyName] = message;
        }

        protected virtual void ClearErrors()
        {
            if (this.errors != null)
                this.errors.Clear();
        }

        protected virtual void SetValue<TValue>(string name, ref TValue oldValue, TValue newValue)
        {
            if (newValue is IChangeTracker tracker)
            {
                this.descendants = this.descendants ?? new Dictionary<string, IChangeTracker>();
                this.descendants.Remove(name);

                if (!(tracker is null))
                    this.descendants.Add(name, tracker);

                if (!Equals(oldValue, newValue))
                {
                    oldValue = newValue;
                    if (!this.ignoreChanges)
                    {
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                        this.changedProperies.Add(name);
                    }
                    else
                    {
                        this.originalValues[name] = newValue;
                    }
                }

                return;
            }

            if (this.ignoreChanges)
            {
                oldValue = newValue;
                this.originalValues[name] = newValue;
                return;
            }

            if (!Equals(oldValue, newValue))
            {
                oldValue = newValue;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                if (this.originalValues.TryGetValue(name, out object value))
                {
                    if (Equals(value, newValue))
                    {
                        if (this.changedProperies.Contains(name))
                            this.changedProperies.Remove(name);

                        return;
                    }
                }
                else
                {
                    this.originalValues.Add(name, newValue);
                }

                if (!this.changedProperies.Contains(name))
                    this.changedProperies.Add(name);
            }

            return;
        }

        protected virtual void RejectChanges(bool includeDescendants)
        {
            this.BeginInit();

            if (includeDescendants && this.descendants != null)
            {
                foreach (var descendant in this.descendants.Values)
                    descendant.RejectChanges();
            }

            if (this.changedProperies.Count > 0)
            {
                ReflectionCache cache = ModelReflectionCache.Default;
                var typeInfo = cache.GetOrAdd(this.GetType());
                var properties = typeInfo.Properties.Where(o => o.CanRead && o.CanWrite);

                foreach (var propInfo in properties)
                {
                    if (this.changedProperies.Contains(propInfo.Name))
                    {
                        if (this.originalValues.TryGetValue(propInfo.Name, out object value))
                        {
                            if (value == null &&
                                this.descendants != null &&
                                this.descendants.ContainsKey(propInfo.Name))
                            {
                                this.descendants.Remove(propInfo.Name);
                            }

                            propInfo.SetValue(this, value);
                        }
                    }

                    continue;
                }

                this.changedProperies.Clear();
            }

            this.EndInit();
        }

        protected virtual void AcceptChanges(bool includeDescendants)
        {
            this.BeginInit();

            if (includeDescendants && this.descendants != null)
            {
                foreach (var descendant in this.descendants.Values)
                    descendant.AcceptChanges();
            }

            if (this.changedProperies.Count > 0)
            {
                // TODO: determine if type needs to be cached.
                ReflectionCache cache = ModelReflectionCache.Default;
                var typeInfo = cache.GetOrAdd(this.GetType());
                var properties = typeInfo.Properties.Where(o => o.CanRead && o.CanWrite);

                foreach (var propInfo in properties)
                {
                    if (this.changedProperies.Contains(propInfo.Name))
                    {
                        this.originalValues.Remove(propInfo.Name);
                        this.originalValues.Add(propInfo.Name, propInfo.GetValue(this));
                    }

                    continue;
                }

                this.changedProperies.Clear();
            }

            this.isNew = false;
            this.EndInit();
        }

        protected virtual void BeginInit() => this.ignoreChanges = true;

        protected virtual void EndInit() => this.ignoreChanges = false;
    }
}