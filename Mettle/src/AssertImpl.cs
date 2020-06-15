﻿using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace Mettle
{
    public class AssertImpl : IAssert
    {
        private static IAssert s_assert = new AssertImpl();

        public static IAssert Current => s_assert;

        public IAssert Ok(bool condition, string message = null)
        {
            Assert.True(condition, message);
            return this;
        }

        public IAssert True(bool condition, string message = null)
        {
            Assert.True(condition, message);
            return this;
        }

        public IAssert False(bool condition, string message = null)
        {
            Assert.False(condition, message);
            return this;
        }

        public IAssert Null(object @object)
        {
            Assert.Null(@object);
            return this;
        }

        public IAssert NotNull(object @object)
        {
            Assert.NotNull(@object);
            return this;
        }

        public IAssert IsType<T>(object @object)
        {
            Assert.IsType<T>(@object);
            return this;
        }

        public IAssert IsType(Type expectedType, object @object)
        {
            Assert.IsType(expectedType, @object);
            return this;
        }

        public IAssert IsNotType<T>(object @object)
        {
            Assert.IsNotType<T>(@object);
            return this;
        }

        public IAssert IsNotType(Type expectedType, object @object)
        {
            Assert.IsNotType(expectedType, @object);
            return this;
        }

        public IAssert IsAssignableFrom<T>(object @object)
        {
            Assert.IsAssignableFrom<T>(@object);
            return this;
        }

        public IAssert IsAssignableFrom(Type expectedType, object @object)
        {
            Assert.IsAssignableFrom(expectedType, @object);
            return this;
        }

        public IAssert Throws<T>(Action action)
            where T : Exception
        {
            Assert.Throws<T>(action);
            return this;
        }

        public IAssert Throws<T>(Func<object> func)
            where T : Exception
        {
            Assert.Throws<T>(func);
            return this;
        }

        public IAssert Throws(Type expectedType, Action action)
        {
            Assert.Throws(expectedType, action);
            return this;
        }

        public IAssert Throws(Type expectedType, Func<object> func)
        {
            Assert.Throws(expectedType, func);
            return this;
        }

        public IAssert ThrowsAny<T>(Action action)
            where T : Exception
        {
            Assert.ThrowsAny<T>(action);
            return this;
        }

        public IAssert ThrowsAny<T>(Func<object> func)
            where T : System.Exception
        {
            Assert.ThrowsAny<T>(func);
            return this;
        }

        public IAssert Collection<T>(IEnumerable<T> collection, Action<T>[] inspectors)
        {
            Assert.Collection(collection, inspectors);
            return this;
        }

        public IAssert Contains<T>(T expected, IEnumerable<T> collection)
        {
            Assert.Contains(expected, collection);
            return this;
        }

        public IAssert Contains<T>(IEnumerable<T> collection, Predicate<T> filter)
        {
            Assert.Contains(collection, filter);
            return this;
        }

        public IAssert Contains<TKey, TValue>(TKey expected, IDictionary<TKey, TValue> collection)
        {
            Assert.Contains(expected, collection);
            return this;
        }

        public IAssert Contains(string expected, string value)
        {
            Assert.Contains(expected, value);
            return this;
        }

        public IAssert DoesNotContain<T>(T expected, IEnumerable<T> collection)
        {
            Assert.DoesNotContain(expected, collection);
            return this;
        }

        public IAssert DoesNotContain<T>(IEnumerable<T> collection, Predicate<T> filter)
        {
            Assert.DoesNotContain(collection, filter);
            return this;
        }

        public IAssert DoesNotContain<TKey, TValue>(TKey expected, IDictionary<TKey, TValue> collection)
        {
            Assert.DoesNotContain(expected, collection);
            return this;
        }

        public IAssert DoesNotContain(string expected, string value)
        {
            Assert.DoesNotContain(expected, value);
            return this;
        }

        public IAssert Equal<T>(T expected, T actual)
        {
            Assert.Equal(expected, actual);
            return this;
        }

        public IAssert Equal(double expected, double actual, int precision)
        {
            Assert.Equal(expected, actual, precision);
            return this;
        }

        public IAssert Equal<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            Assert.Equal(expected, actual, comparer);
            return this;
        }

        public IAssert Equal(decimal expected, decimal actual, int precision)
        {
            Assert.Equal(expected, actual, precision);
            return this;
        }

        public IAssert Equal(DateTime expected, DateTime actual, TimeSpan precision)
        {
            Assert.Equal(expected, actual, precision);
            return this;
        }

        public IAssert Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer)
        {
            Assert.Equal(expected, actual, comparer);
            return this;
        }

        public IAssert Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.Equal(expected, actual);
            return this;
        }

        public IAssert Equal(
            string expected,
            string actual,
            bool ignoreCase = false,
            bool ignoreLineEndingDifferences = false,
            bool ignoreWhiteSpaceDifferences = false)
        {
            Assert.Equal(expected, actual, ignoreCase, ignoreLineEndingDifferences, ignoreWhiteSpaceDifferences);
            return this;
        }

        public IAssert Equal(string expected, string actual)
        {
            Assert.Equal(expected, actual);
            return this;
        }

        public IAssert NotEmpty(IEnumerable collection)
        {
            Assert.NotEmpty(collection);
            return this;
        }

        public IAssert NotEqual<T>(T expected, T actual)
        {
            Assert.NotEqual(expected, actual);
            return this;
        }

        public IAssert NotEqual(decimal expected, decimal actual, int precision)
        {
            Assert.NotEqual(expected, actual, precision);
            return this;
        }

        public IAssert NotEqual(double expected, double actual, int precision)
        {
            Assert.NotEqual(expected, actual, precision);
            return this;
        }

        public IAssert NotEqual<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            Assert.NotEqual(expected, actual, comparer);
            return this;
        }

        public IAssert NotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.NotEqual(expected, actual);
            return this;
        }

        public IAssert NotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer)
        {
            Assert.NotEqual(expected, actual, comparer);
            return this;
        }

        public IAssert NotSame(object expected, object actual)
        {
            Assert.NotSame(expected, actual);
            return this;
        }

        public IAssert Same(object expected, object actual)
        {
            Assert.Same(expected, actual);
            return this;
        }
    }
}
