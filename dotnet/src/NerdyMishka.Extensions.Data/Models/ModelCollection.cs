using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace NerdyMishka.Models
{
    public class ModelCollection<T> :
        ObservableCollection<T>,
        IModelCollection<T>
    {
        private readonly List<T> additions = new List<T>();

        private readonly List<T> removals = new List<T>();

        private readonly bool descendantsImplementChangeTracker;

        private bool ignoreChanges = false;

        private IChangeTracker[] cache;

        public ModelCollection()
        {
            var changeTrackerInterface = typeof(T).GetInterface("IChangeTracker", true);
            this.descendantsImplementChangeTracker = changeTrackerInterface != null;
        }

        public ModelCollection(IEnumerable<T> values)
            : this()
        {
            this.SetInitialValues(values);
        }

        public ModelCollection(Action<ModelCollection<T>> values)
            : this()
        {
            this.SetInitialValues(values);
        }

        IReadOnlyList<T> IModelCollection<T>.Additions => this.additions;

        IReadOnlyList<T> IModelCollection<T>.Removals => this.removals;

        bool IChangeTracking.IsChanged
        {
            get
            {
                if (this.additions.Count > 0)
                    return true;

                if (this.removals.Count > 0)
                    return true;

                if (this.descendantsImplementChangeTracker)
                {
                    foreach (IChangeTracker tracker in this)
                    {
                        if (tracker.IsChanged)
                            return true;
                    }
                }

                return false;
            }
        }

        IReadOnlyList<IChangeTracker> IChangeTracker.Descendants
        {
            get
            {
                if (!this.descendantsImplementChangeTracker)
                    return Array.Empty<IChangeTracker>();

                if (this.cache != null)
                    return this.cache;

                var set = new IChangeTracker[this.Count];
                var i = 0;
                foreach (IChangeTracker tracker in this)
                {
                    set[i] = tracker;
                    i++;
                }

                this.cache = set;
                return this.cache;
            }
        }

        void ISupportInitialize.BeginInit() => this.BeginInit();

        void ISupportInitialize.EndInit() => this.EndInit();

        void IModelCollection<T>.SetInitialValues(IEnumerable<T> values)
            => this.SetInitialValues(values);

        void IModelCollection<T>.SetInitialValues(Action<IModelCollection<T>> action)
            => this.SetInitialValues(action);

        void IChangeTracking.AcceptChanges() => this.AcceptChanges(true);

        void IRevertibleChangeTracking.RejectChanges()
            => this.RejectChanges(true);

        void IChangeTracker.AcceptChanges(bool includeDescendants)
            => this.AcceptChanges(includeDescendants);

        void IChangeTracker.RejectChanges(bool includeDescendants)
            => this.RejectChanges(includeDescendants);

        void IChangeTracker.UpdateOriginalValues(bool includeDescendants)
            => this.UpdateOriginalValues(includeDescendants);

        public IModelCollection<T> AsModel()
        {
            return this;
        }

        protected void SetInitialValues(IEnumerable<T> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            this.BeginInit();
            if (this.Count > 0)
                this.Clear();

            this.additions.Clear();
            this.removals.Clear();
            foreach (var item in values)
                this.Add(item);

            this.EndInit();
        }

        protected void SetInitialValues(Action<ModelCollection<T>> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            this.BeginInit();
            if (this.Count > 0)
                this.Clear();

            this.additions.Clear();
            this.removals.Clear();
            action(this);

            this.EndInit();
        }

        protected virtual void AcceptChanges(bool includeDescendants)
        {
            this.BeginInit();
            this.additions.Clear();
            this.removals.Clear();

            if (includeDescendants && this.descendantsImplementChangeTracker)
            {
                foreach (IChangeTracker tracker in this)
                {
                    tracker.AcceptChanges();
                }
            }

            this.cache = null;
            this.EndInit();
        }

        protected virtual void RejectChanges(bool includeDescendants)
        {
            this.BeginInit();

            foreach (var item in this.additions)
                this.Remove(item);

            foreach (var item in this.removals)
                this.Add(item);

            this.additions.Clear();
            this.removals.Clear();

            if (includeDescendants && this.descendantsImplementChangeTracker)
            {
                foreach (IChangeTracker item in this)
                    item.RejectChanges();
            }

            this.cache = null;
            this.EndInit();
        }

        protected virtual void BeginInit() => this.ignoreChanges = true;

        protected virtual void EndInit() => this.ignoreChanges = false;

        protected void UpdateOriginalValues(bool includeDescendants)
        {
            this.BeginInit();
            this.additions.Clear();
            this.removals.Clear();

            if (includeDescendants && this.descendantsImplementChangeTracker)
            {
                foreach (IChangeTracker descendant in this)
                    descendant.UpdateOriginalValues(includeDescendants);
            }

            this.cache = null;

            this.EndInit();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.ignoreChanges)
                return;

            base.OnPropertyChanged(e);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.cache = null;
            if (this.ignoreChanges)
                return;

            if (e is null)
            {
                base.OnCollectionChanged(e);
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                    {
                        if (this.removals.Contains(item))
                        {
                            this.removals.Remove(item);
                            continue;
                        }

                        this.additions.Add(item);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        if (this.additions.Contains(item))
                        {
                            this.additions.Remove(item);
                            continue;
                        }

                        this.removals.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:

                    foreach (T item in e.OldItems)
                    {
                        if (this.additions.Contains(item))
                            this.additions.Remove(item);
                        else if (!this.removals.Contains(item))
                            this.removals.Add(item);
                    }

                    foreach (T item in e.NewItems)
                    {
                        if (!this.additions.Contains(item))
                            this.additions.Add(item);

                        if (this.removals.Contains(item))
                            this.removals.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (T item in e.OldItems)
                    {
                        if (this.additions.Contains(item))
                            this.additions.Remove(item);

                        if (this.removals.Contains(item))
                            this.removals.Add(item);
                    }

                    foreach (T item in e.NewItems)
                    {
                        if (!this.additions.Contains(item))
                            this.additions.Add(item);

                        if (this.removals.Contains(item))
                            this.removals.Remove(item);
                    }

                    break;
            }

            base.OnCollectionChanged(e);
        }
    }
}