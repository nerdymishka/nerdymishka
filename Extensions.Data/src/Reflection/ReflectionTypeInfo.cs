using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    [SuppressMessage("", "CA1819:", Justification = "By Design.")]
    public class ReflectionTypeInfo : ReflectionMember,
        IReflectionTypeInfo
    {
        private readonly IReflectionFactory factory;

        private Type[] interfaces;

        private Type clrType;

        private TypeInfo typeInfo;

        private List<IReflectionProperty> properties;

        private List<IReflectionMethod> methods;

        private List<IReflectionConstructor> constructors;

        private List<IReflectionField> fields;

        private IReflectionCache cache;

        private BindingFlags lastPropertyBindingFlags = BindingFlags.Default;

        private BindingFlags lastMethodBindingFlags = BindingFlags.Default;

        private BindingFlags lastFieldsBindingFlags = BindingFlags.Default;

        public ReflectionTypeInfo(Type type, IReflectionCache cache)
        {
            this.clrType = type ?? throw new ArgumentNullException(nameof(type));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.factory = cache.Factory ?? ReflectionFactory.Default;
        }

        public override Type ClrType => this.clrType;

        public override string Name => this.clrType.Name;

        public TypeInfo TypeInfo
        {
            get
            {
                if (this.typeInfo != null)
                    return this.typeInfo;

                this.typeInfo = this.clrType.GetTypeInfo();
                return this.typeInfo;
            }
        }

        public bool IsGenericType => this.clrType.IsGenericType;

        public bool IsGenericTypeDefintion => this.clrType.IsGenericTypeDefinition;

        public Type[] GenericTypeArguments => this.clrType.GenericTypeArguments;

        public Type[] GenericTypeParameters => this.TypeInfo.GenericTypeParameters;

        public IReadOnlyList<Type> Interfaces
        {
            get
            {
                if (this.interfaces == null)
                    this.LoadInterfaces();

                return this.interfaces;
            }
        }

        public IReadOnlyList<IReflectionField> Fields
        {
            get
            {
                if (this.properties == null)
                    this.LoadProperties();

                return this.fields;
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

        public IReflectionProperty GetProperty(string name)
        {
            var property = this.Properties.SingleOrDefault(
                o => o.Name == name);

            if (property is null)
            {
                var propInfo = this.ClrType.GetProperty(name);
                if (propInfo != null)
                {
                    property = this.factory.CreateProperty(propInfo, this);
                    this.properties.Add(property);
                }
            }

            return property;
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
            if (this.lastMethodBindingFlags == flags)
                return this;
            var methods = this.clrType.GetMethods(flags);
            var set = new List<IReflectionMethod>();
            foreach (var method in methods)
                set.Add(this.factory.CreateMethod(method, null, this));

            this.methods = set;
            this.lastMethodBindingFlags = flags;
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
            if (this.lastPropertyBindingFlags == flags)
                return this;

            var properties = this.clrType.GetProperties(flags);
            var set = new List<IReflectionProperty>();
            foreach (var prop in properties)
                set.Add(this.factory.CreateProperty(prop, this));

            this.properties = set;
            this.lastPropertyBindingFlags = flags;
            return this;
        }

        public IReflectionTypeInfo LoadFields(bool includeStatic = false, bool includeInherit = false)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            if (includeStatic)
                flags |= BindingFlags.Static;

            if (!includeInherit)
                flags |= BindingFlags.DeclaredOnly;

            return this.LoadFields(flags);
        }

        public IReflectionTypeInfo LoadFields(BindingFlags flags)
        {
            if (this.lastFieldsBindingFlags == flags)
                return this;

            var fields = this.clrType.GetFields(flags);
            var set = new List<IReflectionField>();
            foreach (var field in fields)
                set.Add(this.factory.CreateField(field, this));

            this.fields = set;
            this.lastFieldsBindingFlags = flags;
            return this;
        }
    }
}