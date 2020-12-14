using System;
using System.Collections.Concurrent;

namespace NerdyMishka.Reflection
{
    public class ReflectionCache : IReflectionCache, IDisposable
    {
        private ReflectionFactory factory;

        private bool isDisposed;

        private ConcurrentDictionary<Type, IReflectionTypeInfo> cache =
            new ConcurrentDictionary<Type, IReflectionTypeInfo>();

        public ReflectionCache(int capacity = -1, int concurrencyLevel = 100)
        {
            if (capacity < 1)
                this.cache = new ConcurrentDictionary<Type, IReflectionTypeInfo>();
            else
                this.cache = new ConcurrentDictionary<Type, IReflectionTypeInfo>(concurrencyLevel, capacity);
        }

        ~ReflectionCache()
        {
            this.Dispose(false);
        }

        public static ReflectionCache Global { get; set; } = new ReflectionCache();

        public virtual IReflectionFactory Factory
        {
            get
            {
                this.factory = this.factory ?? new ReflectionFactory(this);
                return this.factory;
            }
        }

        public virtual void Clear()
        {
            this.cache.Clear();
        }

        public virtual bool TryRemove(Type type)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(nameof(ReflectionCache));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return this.cache.TryRemove(type, out IReflectionTypeInfo result);
        }

        public virtual bool TryRemove(IReflectionTypeInfo type)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(nameof(ReflectionCache));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return this.cache.TryRemove(
                type.ClrType,
                out type);
        }

        public virtual IReflectionTypeInfo GetOrAdd(Type type)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(nameof(ReflectionCache));

            if (this.cache.TryGetValue(type, out IReflectionTypeInfo reflectedType))
                return reflectedType;

            reflectedType = this.Factory.CreateType(type);
            this.cache.TryAdd(type, reflectedType);
            return reflectedType;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                this.Clear();
                this.cache = null;
            }

            this.isDisposed = true;
        }
    }
}