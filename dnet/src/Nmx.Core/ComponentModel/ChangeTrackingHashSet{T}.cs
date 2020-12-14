using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NerdyMishka.ComponentModel
{
    public class ChangeTrackingHashSet<T>
        : ISet<T>, IReadOnlyCollection<T>, IChangeTrackingCollection<T>
    {
        private readonly HashSet<T> additions;
        private readonly bool descendantsTrackChanges;
        private readonly HashSet<T> removals;
        private bool ignoreChanges;
        private HashSet<T> set;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeTrackingHashSet{T}"/> class
        ///     that is empty and uses the default equality comparer for the set type.
        /// </summary>
        public ChangeTrackingHashSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeTrackingHashSet{T}" /> class
        ///     that is empty and uses the specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">
        ///     The <see cref="IEqualityComparer{T}" /> implementation to use when
        ///     comparing values in the set, or null to use the default <see cref="IEqualityComparer{T}" />
        ///     implementation for the set type.
        /// </param>
        public ChangeTrackingHashSet([NotNull] IEqualityComparer<T> comparer)
        {
            this.set = new HashSet<T>(comparer);
            this.additions = new HashSet<T>(comparer);
            this.removals = new HashSet<T>(comparer);
            this.descendantsTrackChanges = typeof(T).GetInterface("IChangeTracker") != null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeTrackingHashSet{T}" /> class
        ///     that uses the default equality comparer for the set type, contains elements copied
        ///     from the specified collection, and has sufficient capacity to accommodate the
        ///     number of elements copied.
        /// </summary>
        /// <param name="collection"> The collection whose elements are copied to the new set. </param>
        public ChangeTrackingHashSet([NotNull] IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeTrackingHashSet{T}" /> class
        ///     that uses the specified equality comparer for the set type, contains elements
        ///     copied from the specified collection, and has sufficient capacity to accommodate
        ///     the number of elements copied.
        /// </summary>
        /// <param name="collection"> The collection whose elements are copied to the new set. </param>
        /// <param name="comparer">
        ///     The <see cref="IEqualityComparer{T}" /> implementation to use when
        ///     comparing values in the set, or null to use the default <see cref="IEqualityComparer{T}" />
        ///     implementation for the set type.
        /// </param>
        public ChangeTrackingHashSet([NotNull] IEnumerable<T> collection, [NotNull] IEqualityComparer<T> comparer)
        {
            this.set = new HashSet<T>(collection, comparer);
            this.additions = new HashSet<T>(comparer);
            this.removals = new HashSet<T>(comparer);
            this.descendantsTrackChanges = typeof(T).GetInterface("IChangeTracker") != null;
        }

        /// <summary>
        ///     Occurs when a property of this hash set (such as <see cref="Count" />) changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Occurs when the contents of the hash set changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        ///     Occurs when a property of this hash set (such as <see cref="Count" />) is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        ///     Gets the <see cref="IEqualityComparer{T}" /> object that is used to determine equality for the values in the set.
        /// </summary>
        public virtual IEqualityComparer<T> Comparer
            => this.set.Comparer;

        public IReadOnlyCollection<T> Additions => this.additions;

        public IReadOnlyCollection<T> Removals => this.removals;

        public bool IsChanged
        {
            get
            {
                if (this.additions.Count > 0 || this.removals.Count > 0)
                    return true;

                return this.descendantsTrackChanges && this.Descendants.Any(tracker => tracker.IsChanged);
            }
        }

        public IReadOnlyList<IChangeTracker> Descendants
        {
            get
            {
                if (!this.descendantsTrackChanges || this.Count == 0)
                    return Array.Empty<IChangeTracker>();

                return this.set.Cast<IChangeTracker>()
                    .ToArray();
            }
        }

        /// <summary>
        ///     Gets the number of elements that are contained in the hash set.
        /// </summary>
        public virtual int Count
            => this.set.Count;

        /// <summary>
        ///     Gets a value indicating whether the hash set is read-only.
        /// </summary>
        public virtual bool IsReadOnly
            => ((ICollection<T>)this.set).IsReadOnly;

        public void BeginInit()
        {
            this.ignoreChanges = true;
        }

        public void EndInit()
        {
            this.ignoreChanges = false;
        }

        public void AcceptChanges()
        {
            this.AcceptChanges(true);
        }

        public void RejectChanges()
        {
            this.RejectChanges(true);
        }

        public void AcceptChanges(bool includeDescendants)
        {
            this.BeginInit();
            this.additions.Clear();
            this.removals.Clear();

            if (includeDescendants && this.descendantsTrackChanges)
                foreach (var tracker in this.Descendants)
                    tracker.AcceptChanges();

            this.EndInit();
        }

        public void RejectChanges(bool includeDescendants)
        {
            this.BeginInit();
            this.additions.Clear();
            this.removals.Clear();

            if (includeDescendants && this.descendantsTrackChanges)
                foreach (var tracker in this.Descendants)
                    tracker.RejectChanges();

            this.EndInit();
        }

        public void UpdateOriginalValues(bool includeDescendants)
        {
            this.BeginInit();
            this.additions.Clear();
            this.removals.Clear();

            if (includeDescendants && this.descendantsTrackChanges)
                foreach (var tracker in this.Descendants)
                    tracker.UpdateOriginalValues(true);

            this.EndInit();
        }

        public void SetInitialValues(IEnumerable<T> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            this.BeginInit();
            this.additions.Clear();
            this.removals.Clear();
            var copy = new HashSet<T>(values, this.Comparer);
            this.set = copy;

            this.EndInit();
        }

        public void SetInitialValues(Action<IChangeTrackingCollection<T>> action)
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

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        /// <summary>
        ///     Removes all elements from the hash set.
        /// </summary>
        public virtual void Clear()
        {
            if (this.set.Count == 0)
                return;

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            var removed = this.ToList();

            this.set.Clear();

            if (this.ignoreChanges)
                return;

            foreach (var item in this.removals)
            {
                this.additions.Remove(item);
                this.removals.Add(item);
            }

            this.OnCollectionChanged(ObservableHashSetSingletons.EmptyItems, removed);
            this.OnCountPropertyChanged();
        }

        /// <summary>
        ///     Determines whether the hash set object contains the
        ///     specified element.
        /// </summary>
        /// <param name="item">The element to locate in the hash set.</param>
        /// <returns>
        ///     <see langword="true" /> if the hash set contains the specified element; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool Contains(T item)
        {
            return this.set.Contains(item);
        }

        /// <summary>
        ///     Copies the elements of the hash set to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional array that is the destination of the elements copied from
        ///     the hash set. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex"> The zero-based index in array at which copying begins. </param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            this.set.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Removes the specified element from the hash set.
        /// </summary>
        /// <param name="item"> The element to remove. </param>
        /// <returns>
        ///     <see langword="true" /> if the element is successfully found and removed; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool Remove(T item)
        {
            if (!this.set.Contains(item))
                return false;

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            this.set.Remove(item);

            if (this.ignoreChanges)
                return true;

            this.additions.Remove(item);
            this.removals.Add(item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            this.OnCountPropertyChanged();

            return true;
        }

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        ///     Adds the specified element to the hash set.
        /// </summary>
        /// <param name="item"> The element to add to the set. </param>
        /// <returns>
        ///     <see langword="true" /> if the element is added to the hash set; <see langword="false" /> if the element is already
        ///     present.
        /// </returns>
        public virtual bool Add(T item)
        {
            if (this.set.Contains(item))
                return false;

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            this.set.Add(item);

            if (this.ignoreChanges)
                return true;

            this.additions.Add(item);
            this.removals.Remove(item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            this.OnCountPropertyChanged();

            return true;
        }

        /// <summary>
        ///     Modifies the hash set to contain all elements that are present in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        public virtual void UnionWith(IEnumerable<T> other)
        {
            var copy = new HashSet<T>(this.set, this.set.Comparer);

            copy.UnionWith(other);

            if (copy.Count == this.set.Count)
                return;

            var added = copy.Where(i => !this.set.Contains(i)).ToList();

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            this.set = copy;

            if (this.ignoreChanges)
                return;

            foreach (var item in added)
            {
                this.removals.Remove(item);
                this.additions.Add(item);
            }

            this.OnCollectionChanged(added, ObservableHashSetSingletons.EmptyItems);
            this.OnCountPropertyChanged();
        }

        /// <summary>
        ///     Modifies the current hash set to contain only
        ///     elements that are present in that object and in the specified collection.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        public virtual void IntersectWith(IEnumerable<T> other)
        {
            var copy = new HashSet<T>(this.set, this.set.Comparer);
            copy.IntersectWith(other);

            if (copy.Count == this.set.Count)
                return;

            var removed = this.set.Where(i => !copy.Contains(i)).ToList();

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            this.set = copy;

            foreach (var item in removed)
            {
                this.additions.Remove(item);
                this.removals.Add(item);
            }

            this.OnCollectionChanged(ObservableHashSetSingletons.EmptyItems, removed);
            this.OnCountPropertyChanged();
        }

        /// <summary>
        ///     Removes all elements in the specified collection from the hash set.
        /// </summary>
        /// <param name="other"> The collection of items to remove from the current hash set. </param>
        public virtual void ExceptWith(IEnumerable<T> other)
        {
            var copy = new HashSet<T>(this.set, this.set.Comparer);

            copy.ExceptWith(other);

            if (copy.Count == this.set.Count)
                return;

            var removed = this.set.Where(i => !copy.Contains(i)).ToList();

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            this.set = copy;

            if (this.ignoreChanges)
                return;

            foreach (var item in removed)
            {
                this.additions.Remove(item);
                this.removals.Add(item);
            }

            this.OnCollectionChanged(ObservableHashSetSingletons.EmptyItems, removed);
            this.OnCountPropertyChanged();
        }

        /// <summary>
        ///     Modifies the current hash set to contain only elements that are present either in that
        ///     object or in the specified collection, but not both.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        public virtual void SymmetricExceptWith(IEnumerable<T> other)
        {
            var copy = new HashSet<T>(this.set, this.set.Comparer);

            copy.SymmetricExceptWith(other);

            var removed = this.set.Where(i => !copy.Contains(i)).ToList();
            var added = copy.Where(i => !this.set.Contains(i)).ToList();

            if (removed.Count == 0
                && added.Count == 0)
                return;

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            this.set = copy;

            if (this.ignoreChanges)
                return;

            foreach (var item in added)
            {
                this.removals.Remove(item);
                this.additions.Add(item);
            }

            foreach (var item in removed)
            {
                this.additions.Remove(item);
                this.removals.Add(item);
            }

            this.OnCollectionChanged(added, removed);
            this.OnCountPropertyChanged();
        }

        /// <summary>
        ///     Determines whether the hash set is a subset of the specified collection.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        /// <returns>
        ///     <see langword="true" /> if the hash set is a subset of other; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool IsSubsetOf(IEnumerable<T> other)
        {
            return this.set.IsSubsetOf(other);
        }

        /// <summary>
        ///     Determines whether the hash set is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        /// <returns>
        ///     <see langword="true" /> if the hash set is a proper subset of other; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return this.set.IsProperSubsetOf(other);
        }

        /// <summary>
        ///     Determines whether the hash set is a superset of the specified collection.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        /// <returns>
        ///     <see langword="true" /> if the hash set is a superset of other; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool IsSupersetOf(IEnumerable<T> other)
        {
            return this.set.IsSupersetOf(other);
        }

        /// <summary>
        ///     Determines whether the hash set is a proper superset of the specified collection.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        /// <returns>
        ///     <see langword="true" /> if the hash set is a proper superset of other; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return this.set.IsProperSupersetOf(other);
        }

        /// <summary>
        ///     Determines whether the current System.Collections.Generic.HashSet`1 object and a specified collection share common
        ///     elements.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        /// <returns>
        ///     <see langword="true" /> if the hash set and other share at least one common element; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public virtual bool Overlaps(IEnumerable<T> other)
        {
            return this.set.Overlaps(other);
        }

        /// <summary>
        ///     Determines whether the hash set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other"> The collection to compare to the current hash set. </param>
        /// <returns>
        ///     <see langword="true" /> if the hash set is equal to other; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool SetEquals(IEnumerable<T> other)
        {
            return this.set.SetEquals(other);
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the hash set.
        /// </summary>
        /// <returns>
        ///     An enumerator for the hash set.
        /// </returns>
        public virtual HashSet<T>.Enumerator GetEnumerator()
        {
            return this.set.GetEnumerator();
        }

        /// <summary>
        ///     Copies the elements of the hash set to an array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional array that is the destination of the elements copied from
        ///     the hash set. The array must have zero-based indexing.
        /// </param>
        public virtual void CopyTo([NotNull] T[] array)
        {
            this.set.CopyTo(array);
        }

        /// <summary>
        ///     Copies the specified number of elements of the hash set to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional array that is the destination of the elements copied from
        ///     the hash set. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex"> The zero-based index in array at which copying begins. </param>
        /// <param name="count"> The number of elements to copy to array. </param>
        public virtual void CopyTo([NotNull] T[] array, int arrayIndex, int count)
        {
            this.set.CopyTo(array, arrayIndex, count);
        }

        /// <summary>
        ///     Removes all elements that match the conditions defined by the specified predicate
        ///     from the hash set.
        /// </summary>
        /// <param name="match">
        ///     The <see cref="Predicate{T}" /> delegate that defines the conditions of the elements to remove.
        /// </param>
        /// <returns> The number of elements that were removed from the hash set. </returns>
        public virtual int RemoveWhere([NotNull] Predicate<T> match)
        {
            var copy = new HashSet<T>(this.set, this.set.Comparer);

            var removedCount = copy.RemoveWhere(match);

            if (removedCount == 0)
                return 0;

            var removed = this.set.Where(i => !copy.Contains(i)).ToList();

            if (!this.ignoreChanges)
                this.OnCountPropertyChanging();

            this.set = copy;

            if (!this.ignoreChanges)
                return removedCount;

            foreach (var item in removed)
            {
                this.additions.Remove(item);
                this.removals.Add(item);
            }

            this.OnCollectionChanged(ObservableHashSetSingletons.EmptyItems, removed);
            this.OnCountPropertyChanged();

            return removedCount;
        }

        /// <summary>
        ///     Sets the capacity of the hash set to the actual number of elements it contains, rounded up to a nearby,
        ///     implementation-specific value.
        /// </summary>
        public virtual void TrimExcess()
        {
            this.set.TrimExcess();
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="e"> Details of the property that changed. </param>
        protected virtual void OnPropertyChanged([NotNull] PropertyChangedEventArgs e)
        {
            if (this.ignoreChanges)
                return;

            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanging" /> event.
        /// </summary>
        /// <param name="e"> Details of the property that is changing. </param>
        protected virtual void OnPropertyChanging([NotNull] PropertyChangingEventArgs e)
        {
            if (this.ignoreChanges)
                return;

            this.PropertyChanging?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e"> Details of the change. </param>
        protected virtual void OnCollectionChanged([NotNull] NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void OnCountPropertyChanged()
        {
            this.OnPropertyChanged(ObservableHashSetSingletons.CountPropertyChanged);
        }

        private void OnCountPropertyChanging()
        {
            this.OnPropertyChanging(ObservableHashSetSingletons.CountPropertyChanging);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item));
        }

        private void OnCollectionChanged(IList newItems, IList oldItems)
        {
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems));
        }
    }

    internal static class ObservableHashSetSingletons
    {
        public static readonly PropertyChangedEventArgs CountPropertyChanged
            = new PropertyChangedEventArgs("Count");

        public static readonly PropertyChangingEventArgs CountPropertyChanging
            = new PropertyChangingEventArgs("Count");

        public static readonly object[] EmptyItems = Array.Empty<object>();
    }
}