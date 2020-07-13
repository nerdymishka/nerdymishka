using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NerdyMishka.Api.KeePass
{
    [SuppressMessage("Microsoft.Naming", "CA1710:", Justification = "By Design")]
    public class MoveableList<T> : IList<T>, ICloneable<MoveableList<T>>
    {
        private List<T> list = new List<T>();

        private bool isCloneable = false;

        public MoveableList()
        {
            var type = typeof(T);
            var cloneableInterface = type.GetInterface($"ICloneable{type.FullName}");
            this.isCloneable = cloneableInterface != null;
        }

        public MoveableList(IEnumerable<T> values)
            : this()
        {
            this.list = new List<T>(values);
        }

        public int Count => this.list.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > (this.Count - 1))
                    return default;

                return this.list[index];
            }

            set
            {
                if (index < 0 || index > (this.Count - 1))
                    return;

                this.list[index] = value;
            }
        }

        public void Add(T item)
            => this.list.Add(item);

        public void Clear()
            => this.list.Clear();

        public MoveableList<T> Clone()
        {
            var set = new MoveableList<T>();

            if (this.isCloneable)
            {
                foreach (ICloneable<T> item in this.list)
                {
                    set.Add(item.Clone());
                }

                return set;
            }

            foreach (var item in this.list)
            {
                set.Add(item);
            }

            return set;
        }

        public void MoveTop(int index)
        {
            if (this.Count <= 1)
                return;

            if (index > 0)
            {
                this.ShiftItem(index, 0);
            }
        }

        public void MoveTop(T item)
        {
            if (this.Count <= 1)
                return;

            var index = this.IndexOf(item);
            if (index == -1)
                return;

            this.MoveTop(index);
        }

        public void MoveBottom(int index)
        {
            if (this.Count <= 1)
                return;

            if (index != this.Count - 1)
            {
                this.ShiftItem(index, this.Count - 1);
            }
        }

        public void MoveBottom(T item)
        {
            if (this.Count <= 1)
                return;

            var index = this.IndexOf(item);
            if (index == -1)
                return;

            this.MoveBottom(index);
        }

        public void MoveUp(T item)
        {
            var index = this.IndexOf(item);
            if (index == -1)
                return;

            this.MoveUp(index);
        }

        public void MoveUp(int index)
        {
            if (this.Count <= 1)
                return;

            if (index > 0)
            {
                var to = index + 1;
                this.SwapItem(index, to);
            }
        }

        public void MoveDown(T item)
        {
            var index = this.IndexOf(item);
            if (index == -1)
                return;

            this.MoveDown(index);
        }

        public void MoveDown(int index)
        {
            if (this.Count <= 1)
                return;

            if (index < (this.Count - 1))
            {
                var to = index - 1;
                this.SwapItem(index, to);
            }
        }

        public void ShiftItem(int index, int newIndex)
        {
            if (index == newIndex)
                return;

            var array = new T[this.Count];
            this.list.CopyTo(array);
            var item = array[index];
            if (newIndex < index)
            {
                Array.Copy(array, newIndex, array, newIndex + 1, index - newIndex);
            }
            else
            {
                Array.Copy(array, index + 1, array, index, newIndex - index);
            }

            array[newIndex] = item;
            var old = this.list;
            this.list = new List<T>(array);
            old.Clear();
        }

        public void SwapItem(int i, int j)
        {
            if (this.Count == 0)
                return;

            if (i < 0 || i >= this.Count)
                throw new ArgumentOutOfRangeException(
                    nameof(i),
                    i,
                    $"i must be greater than -1 and less than {this.Count}");

            if (j < 0 || i >= this.Count)
                throw new ArgumentOutOfRangeException(
                    nameof(j),
                    j,
                    $"j must be greater than -1, and less than {this.Count}");

            var left = this[i];
            var right = this[j];
            this[i] = right;
            this[j] = left;
        }

        public void Sort(IComparer<T> comparer)
        {
            this.list.Sort(comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            this.list.Sort(comparison);
        }

        public bool Contains(T item)
            => this.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
            => this.list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator()
            => this.list.GetEnumerator();

        public int IndexOf(T item)
            => this.IndexOf(item);

        public void Insert(int index, T item)
            => this.Insert(index, item);

        public bool Remove(T item)
            => this.list.Remove(item);

        public void RemoveAt(int index)
            => this.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}