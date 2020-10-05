using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public abstract class ReflectionMember :
        IReflectionMember
    {
        private IReadOnlyList<Attribute> attributes;

        private Dictionary<string, object> annotations;

        public abstract string Name { get; }

        public abstract Type ClrType { get; }

        public IReadOnlyList<Attribute> Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.LoadAttributes(true);
                }

                return this.attributes;
            }
        }

        public TValue GetAnnotation<TValue>(string name)
        {
            if (this.annotations == null)
                return default;

            if (this.annotations.TryGetValue(name, out object value))
                return (TValue)value;

            return default;
        }

        public virtual void SetAnnotation<TValue>(string name, TValue value)
        {
            if (this.annotations == null)
                this.annotations = new Dictionary<string, object>();

            this.annotations[name] = value;
        }

        public virtual void LoadAttributes(bool inherit = true)
        {
            this.SetAttributes(
                CustomAttributeExtensions
                    .GetCustomAttributes(this.ClrType, inherit));
        }

        public virtual TAttribute FindAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return (TAttribute)this.Attributes
                .FirstOrDefault(o => o is TAttribute);
        }

        public virtual IReadOnlyList<TAttribute> FindAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return this.Attributes.Where(o => o is TAttribute)
                .Cast<TAttribute>()
                .ToList();
        }

        protected void SetAttributes(IEnumerable<Attribute> range)
        {
            this.attributes = new List<Attribute>(range);
        }
    }
}