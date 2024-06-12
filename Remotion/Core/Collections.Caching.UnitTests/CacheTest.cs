// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Remotion.Collections.Caching.UnitTests.Utilities;

namespace Remotion.Collections.Caching.UnitTests
{
  [TestFixture]
  public class CacheTest
  {
    private ICache<string, object> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new Cache<string, object>();
    }

    [Test]
    public void Initialize_WithCustomEqualityComparer ()
    {
      var comparer = StringComparer.InvariantCultureIgnoreCase;
      var cache = new Cache<string, int?>(comparer);
      cache.GetOrCreateValue("a", key => 1);

      Assert.That(cache.GetOrCreateValue("a", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
      Assert.That(cache.GetOrCreateValue("A", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
      Assert.That(cache.Comparer, Is.SameAs(comparer));
    }

    [Test]
    public void TryGet_WithResultNotInCache ()
    {
      Assert.That(_cache.TryGetValue("key1", out var actual), Is.False);
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      object expected = new object();
      bool delegateCalled = false;
      Assert.That(
          _cache.GetOrCreateValue(
              "key1",
              delegate (string key)
              {
                Assert.That(key, Is.EqualTo("key1"));
                delegateCalled = true;
                return expected;
              }),
          Is.SameAs(expected));
      Assert.That(delegateCalled, Is.True);
    }

    [Test]
    public void GetOrCreateValue_WithException ()
    {
      var exception = new Exception();
      Assert.That(
          () => _cache.GetOrCreateValue("key1", key => throw exception),
          Throws.Exception.SameAs(exception));
    }

    [Test]
    public void GetOrCreateValue_WithException_DoesNotCacheException ()
    {
      var exception1 = new Exception();
      Assert.That(
          () => _cache.GetOrCreateValue("key1", key => throw exception1),
          Throws.Exception.SameAs(exception1));

      var exception2 = new Exception();
      Assert.That(
          () => _cache.GetOrCreateValue("key1", key => throw exception2),
          Throws.Exception.SameAs(exception2));
    }

    [Test]
    public void GetOrCreateValue_WithException_TryGetValue_HasNoValue ()
    {
      var exception = new Exception();
      Assert.That(
          () => _cache.GetOrCreateValue("key1", key => throw exception),
          Throws.Exception.SameAs(exception));

      Assert.That(_cache.TryGetValue("key1", out var actual), Is.False);
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithException_GetOrCreateValue_InsertsSecondValue ()
    {
      var exception = new Exception();
      Assert.That(
          () => _cache.GetOrCreateValue("key1", key => throw exception),
          Throws.Exception.SameAs(exception));

      object expected = new object();
      object actual = _cache.GetOrCreateValue("key1", key => expected);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_DoesNotKeepFactoryAlive ()
    {
      WeakReference GetValueFactoryReference ()
      {
        object expected = new object();
        Func<string, object> valueFactory = key => expected;
        Assert.That(_cache.GetOrCreateValue("key1", valueFactory), Is.SameAs(expected));
        return new WeakReference(valueFactory);
      }

      // The valueFactory must be created in a separate method: The x64 JITter in .NET 4.7.2 (DEBUG builds only) keeps the reference alive until the variable is out of scope.
      var valueFactoryReference = GetValueFactoryReference();
      GC.Collect();
      GC.WaitForFullGCComplete();
      Assert.That(valueFactoryReference.IsAlive, Is.False);
    }

    [Test]
    public void GetOrCreateValue_TryGetValue ()
    {
      object expected = new object();

      _cache.GetOrCreateValue("key1", delegate (string key) { return expected; });
      Assert.That(_cache.TryGetValue("key1", out var actual), Is.True);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_Twice ()
    {
      object expected = new object();

      _cache.GetOrCreateValue("key1", key => expected);
      Assert.That(
          _cache.GetOrCreateValue("key1", arg => throw new InvalidOperationException("The valueFactory must not be invoked.")),
          Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_TryGetValue_Clear_TryGetValue ()
    {
      object expected = new object();

      _cache.GetOrCreateValue("key1", key => expected);
      Assert.That(_cache.TryGetValue("key1", out var actual), Is.True);
      Assert.That(actual, Is.SameAs(expected));
      _cache.Clear();
      Assert.That(_cache.TryGetValue("key1", out actual), Is.False);
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_ThrowsInvalidOperationException ()
    {
      object expected = new object();

      var actualValue = _cache.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _cache.TryGetValue(key, out _),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(_cache.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      object expected = new object();

      var actualValue = _cache.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _cache.GetOrCreateValue(key, nestedKey => 13),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

            return expected;
          });

      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(_cache.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedClear_RemovesValues ()
    {
      object expected = new object();

      var actualValue = _cache.GetOrCreateValue(
          "key1",
          delegate
          {
            _cache.Clear();
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(_cache.TryGetValue("key1", out var actualValue2), Is.False);
      Assert.That(actualValue2, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithNestedEnumeration_SkipsNewItem ()
    {
      object expected1 = new object();
      object expected2 = new object();
      object expected3 = new object();
      _cache.GetOrCreateValue("a", delegate { return expected1; });
      _cache.GetOrCreateValue("b", delegate { return expected2; });
      KeyValuePair<string, object>[] nestedItems = null;

      var actualValue = _cache.GetOrCreateValue(
          "key1",
          delegate
          {
            nestedItems = _cache.ToArray();
            return expected3;
          });

      Assert.That(actualValue, Is.SameAs(expected3));
      Assert.That(
          nestedItems,
          Is.EquivalentTo(
              new[]
              {
                  new KeyValuePair<string, object>("a", expected1),
                  new KeyValuePair<string, object>("b", expected2)
              }));

      Assert.That(_cache.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.SameAs(expected3));
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      object expected1 = new object();
      object expected2 = new object();
      _cache.GetOrCreateValue("key1", delegate { return expected1; });
      _cache.GetOrCreateValue("key2", delegate { return expected2; });

      Assert.That(
          _cache.ToArray(),
          Is.EquivalentTo(
              new[]
              {
                  new KeyValuePair<string, object>("key1", expected1),
                  new KeyValuePair<string, object>("key2", expected2)
              }
              ));
    }

    [Test]
    public void GetEnumerator_Generic_Reset ()
    {
      object expected1 = new object();
      object expected2 = new object();
      _cache.GetOrCreateValue("key1", delegate { return expected1; });
      _cache.GetOrCreateValue("key2", delegate { return expected2; });

      using (var enumerator = _cache.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, object>("key1", expected1)));
        enumerator.Reset();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, object>("key1", expected1)));
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, object>("key2", expected2)));
      }
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      object expected1 = new object();
      object expected2 = new object();
      _cache.GetOrCreateValue("key1", delegate { return expected1; });
      _cache.GetOrCreateValue("key2", delegate { return expected2; });

      Assert.That(
          _cache.ToNonGenericEnumerable(),
          Is.EquivalentTo(
              new[]
              {
                  new KeyValuePair<string, object>("key1", expected1),
                  new KeyValuePair<string, object>("key2", expected2)
              }
              ));
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That(_cache.IsNull, Is.False);
    }

    [Test]
    [Explicit]
    public void Performance ()
    {
      // Use local variable instead of the field to allow for better comparison with SimpleDataStoreTest.
      // Note that using a field would incur a 10% performance penalty in this particular test setup.
      var cache = new Cache<string, object>();
      cache.GetOrCreateValue("key", k => "value");
      cache.TryGetValue("key", out var value);

      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < 1000; i++)
      {
        var key = i.ToString("D9");
        cache.GetOrCreateValue(key, k => k + ": value");
        for (int j = 0; j < 100 * 1000; j++)
        {
          cache.TryGetValue(key, out value);
        }
      }

      stopwatch.Stop();
      // Note: on x64, the time taken is 3000ms instead due to use of RyuJIT
      Console.WriteLine("Time expected: 3250ms (release build x86, on Intel Xeon E5-1620 v2 @ 3.70GHz)");
      Console.WriteLine("Time taken: {0:D}ms", stopwatch.ElapsedMilliseconds);
    }
  }
}
