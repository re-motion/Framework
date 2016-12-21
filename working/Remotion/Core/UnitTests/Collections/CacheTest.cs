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
using System.Collections;
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
      _cache = CreateCache<string, object> ();
    }

    protected virtual ICache<TKey, TValue> CreateCache<TKey, TValue> ()
    {
      return new Cache<TKey, TValue> ();
    }

    private void Add (string key, object value)
    {
      _cache.GetOrCreateValue (key, delegate { return value; });
    }

    [Test]
    public void Initialize_WithCustomEqualityComparer ()
    {
      var cache = new Cache<string, int?> (StringComparer.InvariantCultureIgnoreCase);
      cache.Add ("a", 1);

      Assert.That (cache.GetOrCreateValue ("a", delegate { throw new InvalidOperationException (); }), Is.EqualTo (1));
      Assert.That (cache.GetOrCreateValue ("A", delegate { throw new InvalidOperationException (); }), Is.EqualTo (1));
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
      Assert.That (_cache.GetOrCreateValue ("key1", delegate (string key) { return expected; }), Is.SameAs (expected));
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
      Assert.That (_cache.GetOrCreateValue ("key1", delegate (string key) { throw new InvalidOperationException ("The valueFactory must not be invoked."); }), Is.SameAs (expected));
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
  }
}
