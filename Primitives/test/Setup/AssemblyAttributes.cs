[assembly: Mettle.MettleXunitFramework]
[assembly: Mettle.ServiceProviderFactory(typeof(Tests.ServiceProviderFactory))]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1062:Validate arguments of public methods",
    Justification = "ByDesign")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design", "SA1101: Prefs local calls with this", Justification = "ByDesign")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design", "CA1707: Remove underscores", Justification = "ByDesign")]