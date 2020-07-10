using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionTypeInfo : ReflectionMember,
        IReflectionTypeInfo
    {
        private Type[] interfaces;

        private Type clrType;

        private List<IReflectionProperty> properties;

        private List<IReflectionMethod> methods;

        private List<IReflectionConstructor> constructors;

        private IReflectionCache cache;

        private IReflectionFactory factory;

        public ReflectionTypeInfo(Type type, IReflectionCache cache)
        {
            this.clrType = type ?? throw new ArgumentNullException(nameof(type));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.factory = cache.Factory ?? ReflectionFactory.Default;
        }

        public override Type ClrType => this.clrType;

        public override string Name => this.clrType.Name;

        public IReadOnlyList<Type> Interfaces
        {
            get
            {
                if (this.interfaces == null)
                    this.LoadInterfaces();

                return this.interfaces;
            }
        }

        public IReadOnlyList<IReflectionProperty> Properties
        {
            get
            {
                if (this.properties == null)
                    this.LoadProperties();

                return this.properties;
            }
        }

        public IReadOnlyList<IReflectionMethod> Methods
        {
            get
            {
                if (this.methods == null)
                    this.LoadMethods();

                return this.methods;
            }
        }

        public IReadOnlyList<IReflectionConstructor> Constructors
        {
            get
            {
                if (this.constructors == null)
                    this.LoadConstructors();

                return this.constructors;
            }
        }

        public IReflectionTypeInfo LoadInterfaces()
        {
            this.interfaces = this.clrType.GetInterfaces();
            return this;
        }

        public IReflectionTypeInfo LoadConstructors()
        {
            var constructors = this.clrType.GetConstructors();
            var set = new List<IReflectionConstructor>();
            foreach (var ctor in constructors)
                set.Add(this.factory.CreateConstructor(ctor, null, this));

            this.constructors = set;

            return this;
        }

        public IReflectionTypeInfo LoadMethods(bool includeStatic = false, bool includeInherit = false)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            if (includeStatic)
                flags |= BindingFlags.Static;

            if (!includeInherit)
                flags |= BindingFlags.DeclaredOnly;

            return this.LoadMethods(flags);
        }

        public IReflectionTypeInfo LoadMethods(BindingFlags flags)
        {
            var methods = this.clrType.GetMethods(flags);
            var set = new List<IReflectionMethod>();
            foreach (var method in methods)
                set.Add(this.factory.CreateMethod(method, null, this));

            this.methods = set;
            return this;
        }

        public IReflectionTypeInfo LoadProperties(bool includeStatic = false, bool includeInherit = false)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            if (includeStatic)
                flags |= BindingFlags.Static;

            if (!includeInherit)
                flags |= BindingFlags.DeclaredOnly;

            return this.LoadProperties(flags);
        }

        public IReflectionTypeInfo LoadProperties(BindingFlags flags)
        {
            var properties = this.clrType.GetProperties(flags);
            var set = new List<IReflectionProperty>();
            foreach (var prop in properties)
                set.Add(this.factory.CreateProperty(prop, this));

            this.properties = set;
            return this;
        }
    }
}