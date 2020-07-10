using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

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

        public ModelCollection()
        {
            var changeTrackerInterface = typeof(T).GetInterface("IChangeTracker", true);
            this.descendantsImplementChangeTracker = changeTrackerInterface != null;
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
                {
                    return Array.Empty<IChangeTracker>();
                }

                // TODO: cache descendants
                var set = new IChangeTracker[this.Count];
                var i = 0;
                foreach (IChangeTracker tracker in this)
                {
                    set[i] = tracker;
                    i++;
                }

                return set;
            }
        }

        void ISupportInitialize.BeginInit() => this.BeginInit();

        void ISupportInitialize.EndInit() => this.EndInit();

        void IModelCollection<T>.SetInitialValues(IEnumerable<T> values)
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

        void IModelCollection<T>.SetInitialValues(Action<IModelCollection<T>> action)
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

        void IChangeTracking.AcceptChanges() => this.AcceptChanges(true);

        void IRevertibleChangeTracking.RejectChanges() => this.RejectChanges(true);

        public IModelCollection<T> AsModel()
        {
            return this;
        }

        protected void AcceptChanges(bool includeDescendants)
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

            this.EndInit();
        }

        protected void RejectChanges(bool includeDescendants)
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

            this.EndInit();
        }

        protected virtual void BeginInit() => this.ignoreChanges = true;

        protected virtual void EndInit() => this.ignoreChanges = false;

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.ignoreChanges)
                return;

            base.OnPropertyChanged(e);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
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