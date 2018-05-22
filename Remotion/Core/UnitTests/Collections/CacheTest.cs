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
  public class CacheTest
  {
    private ICache<string, object> _cache;
    
    [SetUp]
    public void SetUp ()
    {
      _cache = new Cache<string, object> ();
    }

    [Test]
    public void Initialize_WithCustomEqualityComparer ()
    {
      var comparer = StringComparer.InvariantCultureIgnoreCase;
      var cache = new Cache<string, int?> (comparer);
      cache.Add ("a", 1);

      Assert.That (cache.GetOrCreateValue ("a", delegate { throw new InvalidOperationException (); }), Is.EqualTo (1));
      Assert.That (cache.GetOrCreateValue ("A", delegate { throw new InvalidOperationException (); }), Is.EqualTo (1));
      Assert.That (cache.Comparer, Is.SameAs (comparer));
    }

    [Test]
    public void TryGet_WithResultNotInCache ()
    {
      object actual;
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.False);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      object expected = new object ();
      bool delegateCalled = false;
      Assert.That (
          _cache.GetOrCreateValue (
              "key1",
              delegate (string key)
              {
                Assert.That (key, Is.EqualTo ("key1"));
                delegateCalled = true;
                return expected;
              }),
          Is.SameAs (expected));
      Assert.That (delegateCalled, Is.True);
    }

    [Test]
    public void GetOrCreateValue_WithException ()
    {
      var exception = new Exception();
      Assert.That (
          () => _cache.GetOrCreateValue ("key1", key => { throw exception; }),
          Throws.Exception.SameAs (exception));
    }

    [Test]
    public void GetOrCreateValue_WithException_DoesNotCacheException ()
    {
      var exception1 = new Exception();
      Assert.That (
          () => _cache.GetOrCreateValue ("key1", key => { throw exception1; }),
          Throws.Exception.SameAs (exception1));

      var exception2 = new Exception();
      Assert.That (
          () => _cache.GetOrCreateValue ("key1", key => { throw exception2; }),
          Throws.Exception.SameAs (exception2));
    }

    [Test]
    public void GetOrCreateValue_WithException_TryGetValue_HasNoValue ()
    {
      var exception = new Exception();
      Assert.That (
          () => _cache.GetOrCreateValue ("key1", key => { throw exception; }),
          Throws.Exception.SameAs (exception));

      object actual;
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.False);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithException_GetOrCreateValue_InsertsSecondValue ()
    {
      var exception = new Exception();
      Assert.That (
          () => _cache.GetOrCreateValue ("key1", key => { throw exception; }),
          Throws.Exception.SameAs (exception));

      object expected = new object ();
      object actual = _cache.GetOrCreateValue ("key1", key => expected);
      Assert.That (actual, Is.SameAs (expected));
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
      object expected = new object();
      Func<string, object> valueFactory = key => expected;
      Assert.That (_cache.GetOrCreateValue ("key1", valueFactory), Is.SameAs (expected));
      return new WeakReference (valueFactory);
    }

    [Test]
    public void GetOrCreateValue_TryGetValue ()
    {
      object expected = new object ();

      _cache.GetOrCreateValue ("key1", delegate (string key) { return expected; });
      object actual;
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.True);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void Add_GetOrCreateValue ()
    {
      object expected = new object ();

      Add ("key1", expected);
      Assert.That (
          _cache.GetOrCreateValue ("key1", delegate (string key) { throw new InvalidOperationException ("The valueFactory must not be invoked."); }),
          Is.SameAs (expected));
    }

    [Test]
    public void Add_TryGetValue ()
    {
      object expected = new object();

      Add ("key1", expected);
      object actual;
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.True);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void Add_TryGetValue_Clear_TryGetValue ()
    {
      object expected = new object ();

      Add ("key1", expected);
      object actual;
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.True);
      Assert.That (actual, Is.SameAs (expected));
      _cache.Clear ();
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.False);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void Add_Null ()
    {
      Add ("key1", null);
      object actual;
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.True);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void GetEnumerator_Generic()
    {
      object expected1 = new object();
      object expected2 = new object();
      _cache.GetOrCreateValue ("key1", delegate { return expected1; });
      _cache.GetOrCreateValue ("key2", delegate { return expected2; });

      Assert.That (
          _cache.ToArray(),
          Is.EquivalentTo (
              new[]
              {
                  new KeyValuePair<string, object> ("key1", expected1),
                  new KeyValuePair<string, object> ("key2", expected2)
              }
              ));
    }

    [Test]
    public void GetEnumerator_NonGeneric()
    {
      object expected1 = new object();
      object expected2 = new object();
      _cache.GetOrCreateValue ("key1", delegate { return expected1; });
      _cache.GetOrCreateValue ("key2", delegate { return expected2; });

      Assert.That (
          _cache.ToNonGenericEnumerable(),
          Is.EquivalentTo (
              new[]
              {
                  new KeyValuePair<string, object> ("key1", expected1),
                  new KeyValuePair<string, object> ("key2", expected2)
              }
              ));
    }

    [Test]
    public void GetIsNull()
    {
      Assert.That (_cache.IsNull, Is.False);
    }

    [Test]
    public void SerializeEmptyCache ()
    {
      ICache<string, object> deserializedCache = Serializer.SerializeAndDeserialize (_cache);
      Assert.That (deserializedCache, Is.Not.Null);

      object result;
      Assert.That (deserializedCache.TryGetValue ("bla", out result), Is.False);
      deserializedCache.GetOrCreateValue ("bla", delegate { return "foo"; });
      Assert.That (deserializedCache.TryGetValue ("bla", out result), Is.True);

      Assert.That (result, Is.EqualTo ("foo"));

      Assert.That (_cache.TryGetValue ("bla", out result), Is.False);
    }

    [Test]
    public void SerializeNonEmptyCache ()
    {
      object result;

      _cache.GetOrCreateValue ("bla", delegate { return "foo"; });
      Assert.That (_cache.TryGetValue ("bla", out result), Is.True);

      ICache<string, object> deserializedCache = Serializer.SerializeAndDeserialize (_cache);
      Assert.That (deserializedCache, Is.Not.Null);

      Assert.That (deserializedCache.TryGetValue ("bla", out result), Is.True);
      Assert.That (result, Is.EqualTo ("foo"));

      deserializedCache.GetOrCreateValue ("whatever", delegate { return "fred"; });
      Assert.That (deserializedCache.TryGetValue ("whatever", out result), Is.True);
      Assert.That (_cache.TryGetValue ("whatever", out result), Is.False);
    }

    private void Add (string key, object value)
    {
      ((Cache<string, object>) _cache).Add (key, value);
    }
  }
}
