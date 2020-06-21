using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NerdyMishka
{
    [SuppressMessage(
        "Microsoft.Design",
        "CA1000: Do not declare static members on generic types",
        Justification = "Symbols are meant to be only have one instance")]
    public class Symbol<T>
        where T : Symbol<T>, new()
    {
        private static SymbolFactory<T> s_factory;

        public static SymbolFactory<T> Factory
        {
            get
            {
                if (s_factory == null)
                {
                    s_factory = new SymbolFactory<T>();
                    RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
                }

                return s_factory;
            }
        }

        public string Name { get; set; }
    }
}