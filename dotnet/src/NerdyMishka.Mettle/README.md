# NerdyMishka.Mettle

xUnit extensions that provides:

- Dependency injection for test methods.
- An extensible assert object where extensions methods can be applied
  and the object can be injected into the test method.

## Common 

The `Common` folder holds classes that contains slightly 
modified shared code from xUnit.NET.  The functionality
is required to create a xUnit test framework that runs
newer attributes and support the pre-defined attributes
that ship with xUnit.net 