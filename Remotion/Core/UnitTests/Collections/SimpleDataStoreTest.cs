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
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class SimpleDataStoreTest
  {
    private SimpleDataStore<string, int?> _store;
      
    [SetUp]
    public void SetUp ()
    {
      _store = new SimpleDataStore<string, int?>();
      _store.Add ("a", 1);
      _store.Add ("b", 2);
    }

    [Test]
    public void Initialize_WithCustomEqualityComparer ()
    {
      var dataStore = new SimpleDataStore<string, int?> (StringComparer.InvariantCultureIgnoreCase);
      dataStore.Add ("a", 1);

      Assert.That (dataStore.ContainsKey ("a"));
      Assert.That (dataStore.ContainsKey ("A"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey_True ()
    {
      Assert.That (_store.ContainsKey ("a"));
      Assert.That (_store.ContainsKey ("b"));
    }

    [Test]
    public void ContainsKey_False ()
    {
      Assert.That (_store.ContainsKey ("c"), Is.False);
    }

    [Test]
    public void Add ()
    {
      _store.Add ("c", 5);
      Assert.That (_store.ContainsKey ("c"));
      Assert.That (_store["c"], Is.EqualTo (5));
    }

    [Test]
    public void Add_Null ()
    {
      _store.Add ("d", null);
      Assert.That (_store.ContainsKey ("d"));
      Assert.That (_store["d"], Is.Null);
    }

    [Test]
    public void Add_Twice ()
    {
      _store.Add ("d", 1);
      Assert.That (
          () => _store.Add ("d", 2),
          Throws.ArgumentException.And.Message.EqualTo (
              "The store already contains an element with key \'d\'. (Old value: \'1\', new value: \'2\')\r\nParameter name: key"));
    }

    [Test]
    public void Remove_True ()
    {
      Assert.That (_store.Remove ("a"), Is.True);
      Assert.That (_store.ContainsKey ("a"), Is.False);
    }

    [Test]
    public void Remove_False ()
    {
      Assert.That (_store.Remove ("d"), Is.False);
    }

    [Test]
    public void Clear ()
    {
      _store.Clear();
      Assert.That (_store.ContainsKey ("a"), Is.False);
      Assert.That (_store.ContainsKey ("b"), Is.False);
    }

    [Test]
    public void Clear_Twice ()
    {
      _store.Clear ();
      _store.Clear ();
      Assert.That (_store.ContainsKey ("a"), Is.False);
      Assert.That (_store.ContainsKey ("b"), Is.False);
    }

    [Test]
    public void GetValue_Succeed ()
    {
      Assert.That (_store["a"], Is.EqualTo (1));
    }

    [Test]
    public void GetValue_Fail ()
    {
      Assert.That (
          () => _store["c"],
          Throws.Exception.TypeOf<KeyNotFoundException>().And.Message.EqualTo ("There is no element with key 'c' in the store."));
    }

    [Test]
    public void SetValue_New ()
    {
      _store["c"] = 5;
      Assert.That (_store["c"], Is.EqualTo (5));
    }

    [Test]
    public void SetValue_Overwrite ()
    {
      _store["a"] = 5;
      Assert.That (_store["a"], Is.EqualTo (5));
    }

    [Test]
    public void SetValue_Null ()
    {
      _store["c"] = null;
      Assert.That (_store["c"], Is.Null);
    }

    [Test]
    public void GetValueOrDefault_Value ()
    {
      Assert.That (_store.GetValueOrDefault ("a"), Is.EqualTo (1));
    }

    [Test]
    public void GetValueOrDefault_Default ()
    {
      Assert.That (_store.GetValueOrDefault ("c"), Is.Null);
    }

    [Test]
    public void TryGetValue_True ()
    {
      int? value;
      Assert.That (_store.TryGetValue ("a", out value));
      Assert.That (value, Is.EqualTo (1));
    }

    [Test]
    public void TryGetValue_False ()
    {
      int? value;
      Assert.That (_store.TryGetValue ("c", out value), Is.False);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_Get ()
    {
      bool delegateCalled = false;
      Assert.That (_store.GetOrCreateValue ("a", delegate { delegateCalled = true; return 7; }), Is.EqualTo (1));
      Assert.That (delegateCalled, Is.False);
    }

    [Test]
    public void GetOrCreateValue_Create ()
    {
      bool delegateCalled = false;
      Assert.That (_store.GetOrCreateValue ("c", delegate (string key)
          {
            delegateCalled = true;
            Assert.That (key, Is.EqualTo ("c"));
            return 7;
          }),
          Is.EqualTo (7));
      Assert.That (delegateCalled);
    }

    [Test]
    public void GetOrCreateValue_WithException ()
    {
      var exception = new Exception();
      Assert.That (
          () => _store.GetOrCreateValue ("key1", key => { throw exception; }),
          Throws.Exception.SameAs (exception));
    }

    [Test]
    public void GetOrCreateValue_WithException_DoesNotCacheException ()
    {
      var exception1 = new Exception();
      Assert.That (
          () => _store.GetOrCreateValue ("key1", key => { throw exception1; }),
          Throws.Exception.SameAs (exception1));

      var exception2 = new Exception();
      Assert.That (
          () => _store.GetOrCreateValue ("key1", key => { throw exception2; }),
          Throws.Exception.SameAs (exception2));
    }

    [Test]
    public void GetOrCreateValue_WithException_TryGetValue_HasNoValue ()
    {
      var exception = new Exception();
      Assert.That (
          () => _store.GetOrCreateValue ("key1", key => { throw exception; }),
          Throws.Exception.SameAs (exception));

      int? actual;
      Assert.That (_store.TryGetValue ("key1", out actual), Is.False);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithException_GetOrCreateValue_InsertsSecondValue ()
    {
      var exception = new Exception();
      Assert.That (
          () => _store.GetOrCreateValue ("key1", key => { throw exception; }),
          Throws.Exception.SameAs (exception));

      int? expected = 14;
      object actual = _store.GetOrCreateValue ("key1", key => expected);
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void GetOrCreateValue_DoesNotKeepFactoryAlive ()
    {
      // The valueFactory must be created in a separate method: The x64 JITter in .NET 4.7.2 (DEBUG builds only) keeps the reference alive until the variable is out of scope.
      var valueFactoryReference = GetOrCreateValue_DoesNotKeepFactoryAlive_GetValueFactoryReference();
      GC.Collect();
      GC.WaitForFullGCComplete();
      Assert.That (valueFactoryReference.IsAlive, Is.False);
    }

    private WeakReference GetOrCreateValue_DoesNotKeepFactoryAlive_GetValueFactoryReference ()
    {
      var expected = 13;
      Func<string, int?> valueFactory = key => expected;
      Assert.That (_store.GetOrCreateValue ("key1", valueFactory),Is.EqualTo (expected));
      return new WeakReference (valueFactory);
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      Assert.That (
          _store.ToArray(),
          Is.EquivalentTo (
              new[]
              {
                  new KeyValuePair<string, int?> ("a", 1),
                  new KeyValuePair<string, int?> ("b", 2)
              }
              ));
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      Assert.That (
          _store.ToNonGenericEnumerable(),
          Is.EquivalentTo (
              new[]
              {
                  new KeyValuePair<string, int?> ("a", 1),
                  new KeyValuePair<string, int?> ("b", 2)
              }
              ));
    }

    [Test]
    public void SerializeEmptyDataStore ()
    {
      IDataStore<string, int?> deserializedDataStore = Serializer.SerializeAndDeserialize (_store);
      Assert.That (deserializedDataStore, Is.Not.Null);

      int? result;
      Assert.That (deserializedDataStore.TryGetValue ("bla", out result), Is.False);
      deserializedDataStore.GetOrCreateValue ("bla", delegate { return 17; });
      Assert.That (deserializedDataStore.TryGetValue ("bla", out result), Is.True);

      Assert.That (result, Is.EqualTo (17));

      Assert.That (_store.TryGetValue ("bla", out result), Is.False);
    }

    [Test]
    public void SerializeNonEmptyDataStore ()
    {
      int? result;

      _store.GetOrCreateValue ("bla", delegate { return 19; });
      Assert.That (_store.TryGetValue ("bla", out result), Is.True);

      IDataStore<string, int?> deserializedCache = Serializer.SerializeAndDeserialize (_store);
      Assert.That (deserializedCache, Is.Not.Null);

      Assert.That (deserializedCache.TryGetValue ("bla", out result), Is.True);
      Assert.That (result, Is.EqualTo (19));

      deserializedCache.GetOrCreateValue ("whatever", delegate { return 23; });
      Assert.That (deserializedCache.TryGetValue ("whatever", out result), Is.True);
      Assert.That (_store.TryGetValue ("whatever", out result), Is.False);
    }
  }
}
