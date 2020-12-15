# Nmx.Extensions.EntityFrameworkCore

Code borrowed from https://github.com/efcore/EFCore.NamingConventions
which is under the Apache License.

The biggest change is the use of a an interface that enables pluralizing
tables by passing an object that implements `INameTransform`.