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
using Remotion.Collections.DataStore.UnitTests.Utilities;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Collections.DataStore.UnitTests
{
  [TestFixture]
  public class SimpleDataStoreTest
  {
    private SimpleDataStore<string, int?> _store;

    [SetUp]
    public void SetUp ()
    {
      _store = new SimpleDataStore<string, int?>();
      _store.Add("a", 1);
      _store.Add("b", 2);
    }

    [Test]
    public void Initialize_WithCustomEqualityComparer ()
    {
      var comparer = StringComparer.InvariantCultureIgnoreCase;
      var dataStore = new SimpleDataStore<string, int?>(comparer);
      dataStore.Add("a", 1);

      Assert.That(dataStore.ContainsKey("a"));
      Assert.That(dataStore.ContainsKey("A"));
      Assert.That(dataStore.Comparer, Is.SameAs(comparer));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((INullObject)_store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey_True ()
    {
      Assert.That(_store.ContainsKey("a"));
      Assert.That(_store.ContainsKey("b"));
    }

    [Test]
    public void ContainsKey_False ()
    {
      Assert.That(_store.ContainsKey("c"), Is.False);
    }

    [Test]
    public void Add ()
    {
      _store.Add("c", 5);
      Assert.That(_store.ContainsKey("c"));
      Assert.That(_store["c"], Is.EqualTo(5));
    }

    [Test]
    public void Add_Null ()
    {
      _store.Add("d", null);
      Assert.That(_store.ContainsKey("d"));
      Assert.That(_store["d"], Is.Null);
    }

    [Test]
    public void Add_Twice_ThrowsArgumentException ()
    {
      _store.Add("d", 1);
      Assert.That(
          () => _store.Add("d", 2),
          Throws.ArgumentException.And.ArgumentExceptionMessageEqualTo("The store already contains an element with key \'d\'.", "key"));
    }

    [Test]
    public void Remove_True ()
    {
      Assert.That(_store.Remove("a"), Is.True);
      Assert.That(_store.ContainsKey("a"), Is.False);
    }

    [Test]
    public void Remove_False ()
    {
      Assert.That(_store.Remove("d"), Is.False);
    }

    [Test]
    public void Clear ()
    {
      _store.Clear();
      Assert.That(_store.ContainsKey("a"), Is.False);
      Assert.That(_store.ContainsKey("b"), Is.False);
    }

    [Test]
    public void Clear_Twice ()
    {
      _store.Clear();
      _store.Clear();
      Assert.That(_store.ContainsKey("a"), Is.False);
      Assert.That(_store.ContainsKey("b"), Is.False);
    }

    [Test]
    public void GetValue_Succeed ()
    {
      Assert.That(_store["a"], Is.EqualTo(1));
    }

    [Test]
    public void GetValue_Fail ()
    {
      Assert.That(
          () => _store["c"],
          Throws.Exception.TypeOf<KeyNotFoundException>().And.Message.EqualTo("There is no element with key 'c' in the store."));
    }

    [Test]
    public void SetValue_New ()
    {
      _store["c"] = 5;
      Assert.That(_store["c"], Is.EqualTo(5));
    }

    [Test]
    public void SetValue_Overwrite ()
    {
      _store["a"] = 5;
      Assert.That(_store["a"], Is.EqualTo(5));
    }

    [Test]
    public void SetValue_Null ()
    {
      _store["c"] = null;
      Assert.That(_store["c"], Is.Null);
    }

    [Test]
    public void GetValueOrDefault_Value ()
    {
      Assert.That(_store.GetValueOrDefault("a"), Is.EqualTo(1));
    }

    [Test]
    public void GetValueOrDefault_Default ()
    {
      Assert.That(_store.GetValueOrDefault("c"), Is.Null);
    }

    [Test]
    public void TryGetValue_True ()
    {
      int? value;
      Assert.That(_store.TryGetValue("a", out value));
      Assert.That(value, Is.EqualTo(1));
    }

    [Test]
    public void TryGetValue_False ()
    {
      int? value;
      Assert.That(_store.TryGetValue("c", out value), Is.False);
      Assert.That(value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_Get ()
    {
      bool delegateCalled = false;
      Assert.That(_store.GetOrCreateValue("a", delegate { delegateCalled = true; return 7; }), Is.EqualTo(1));
      Assert.That(delegateCalled, Is.False);
    }

    [Test]
    public void GetOrCreateValue_Create ()
    {
      bool delegateCalled = false;
      Assert.That(_store.GetOrCreateValue("c", delegate (string key)
          {
            delegateCalled = true;
            Assert.That(key, Is.EqualTo("c"));
            return 7;
          }),
          Is.EqualTo(7));
      Assert.That(delegateCalled);
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_ThrowsInvalidOperationException ()
    {
      int expected = 13;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _store.TryGetValue(key, out _),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetValueOrDefault_ThrowsInvalidOperationException ()
    {
      int expected = 13;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _store.GetValueOrDefault(key),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      int expected = 15;

      var actualValue = _store.GetOrCreateValue(
              "key1",
              delegate (string key)
              {
                Assert.That(
                    () => _store.GetOrCreateValue(key, nestedKey => 13),
                    Throws.InvalidOperationException.With.Message.StartsWith(
                        "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

                return expected;
              });

      Assert.That(actualValue, Is.EqualTo(expected));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedAdd_ThrowsInvalidOperationException ()
    {
      int expected = 15;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _store.Add(key, 13),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedContainsKey_ThrowsInvalidOperationException ()
    {
      int expected = 13;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _store.ContainsKey(key),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedRemove_ThrowsInvalidOperationException ()
    {
      int expected = 13;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _store.Remove(key),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedClear_RemovesValues ()
    {
      int expected = 13;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate
          {
            _store.Clear();
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.False);
      Assert.That(actualValue2, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithNestedEnumeration_SkipsNewItem ()
    {
      int expected = 15;
      KeyValuePair<string, int?>[] nestedItems = null;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            nestedItems = _store.ToArray();
            return expected;
          });

      Assert.That(actualValue, Is.EqualTo(expected));
      Assert.That(
          nestedItems,
          Is.EquivalentTo(
              new[]
              {
                  new KeyValuePair<string, int?>("a", 1),
                  new KeyValuePair<string, int?>("b", 2)
              }));

      int? actualValue2;
      Assert.That(_store.TryGetValue("key1", out actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithException ()
    {
      var exception = new Exception();
      Assert.That(
          () => _store.GetOrCreateValue("key1", key => throw exception),
          Throws.Exception.SameAs(exception));
    }

    [Test]
    public void GetOrCreateValue_WithException_DoesNotCacheException ()
    {
      var exception1 = new Exception();
      Assert.That(
          () => _store.GetOrCreateValue("key1", key => throw exception1),
          Throws.Exception.SameAs(exception1));

      var exception2 = new Exception();
      Assert.That(
          () => _store.GetOrCreateValue("key1", key => throw exception2),
          Throws.Exception.SameAs(exception2));
    }

    [Test]
    public void GetOrCreateValue_WithException_TryGetValue_HasNoValue ()
    {
      var exception = new Exception();
      Assert.That(
          () => _store.GetOrCreateValue("key1", key => throw exception),
          Throws.Exception.SameAs(exception));

      int? actual;
      Assert.That(_store.TryGetValue("key1", out actual), Is.False);
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithException_GetOrCreateValue_InsertsSecondValue ()
    {
      var exception = new Exception();
      Assert.That(
          () => _store.GetOrCreateValue("key1", key => throw exception),
          Throws.Exception.SameAs(exception));

      int? expected = 14;
      object actual = _store.GetOrCreateValue("key1", key => expected);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_DoesNotKeepFactoryAlive ()
    {
      WeakReference GetValueFactoryReference ()
      {
        var expected = 13;
        Func<string, int?> valueFactory = key => expected;
        Assert.That(_store.GetOrCreateValue("key1", valueFactory),Is.EqualTo(expected));
        return new WeakReference(valueFactory);
      }

      // The valueFactory must be created in a separate method: The x64 JITter in .NET 4.7.2 (DEBUG builds only) keeps the reference alive until the variable is out of scope.
      var valueFactoryReference = GetValueFactoryReference();
      GC.Collect();
      GC.WaitForFullGCComplete();
      Assert.That(valueFactoryReference.IsAlive, Is.False);
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      Assert.That(
          _store.ToArray(),
          Is.EquivalentTo(
              new[]
              {
                  new KeyValuePair<string, int?>("a", 1),
                  new KeyValuePair<string, int?>("b", 2)
              }
              ));
    }

    [Test]
    public void GetEnumerator_Generic_Reset ()
    {
      using (var enumerator = _store.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, int?>("a", 1)));
        enumerator.Reset();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, int?>("a", 1)));
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, int?>("b", 2)));
      }
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      Assert.That(
          _store.ToNonGenericEnumerable(),
          Is.EquivalentTo(
              new[]
              {
                  new KeyValuePair<string, int?>("a", 1),
                  new KeyValuePair<string, int?>("b", 2)
              }
              ));
    }

    [Test]
    [Explicit]
    public void Performance ()
    {
      var store = new SimpleDataStore<string, object>();
      store.GetOrCreateValue("key", k => "value");
      object value;
      store.TryGetValue("key", out value);

      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < 1000; i++)
      {
        var key = i.ToString("D9");
        store.GetOrCreateValue(key, k => k + ": value");
        for (int j = 0; j < 100 * 1000; j++)
        {
          store.TryGetValue(key, out value);
        }
      }

      stopwatch.Stop();
      // Note: on x64, the time taken is 3000ms instead due to use of RyuJIT
      Console.WriteLine("Time expected: 3250ms (release build x86 on Intel Xeon E5-1620 v2 @ 3.70GHz)");
      Console.WriteLine("Time taken: {0:D}ms", stopwatch.ElapsedMilliseconds);
    }
  }
}
