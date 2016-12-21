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
  public class NullCacheTest
  {
    private ICache<string, object> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new NullCache<string, object> ();
    }

    [Test]
    public void TryGetValue ()
    {
      object actual;
      Assert.That (_cache.TryGetValue ("anyKey", out actual), Is.False);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      object exptected = new object();
      Assert.That (_cache.GetOrCreateValue ("anyKey", delegate { return exptected; }), Is.SameAs (exptected));
    }

    [Test]
    public void Add_TryGetValue ()
    {
      _cache.GetOrCreateValue ("key1", delegate { return new object(); });
      object actual;
      Assert.That (_cache.TryGetValue ("key1", out actual), Is.False);
      Assert.That (actual, Is.Null);
    }

    [Test]
    public void GetEnumerator_Generic()
    {
      _cache.GetOrCreateValue ("key1", delegate { return new object(); });
      _cache.GetOrCreateValue ("key2", delegate { return new object(); });

      Assert.That (_cache.ToArray(), Is.Empty);
    }

    [Test]
    public void GetEnumerator_NonGeneric()
    {
      _cache.GetOrCreateValue ("key1", delegate { return new object(); });
      _cache.GetOrCreateValue ("key2", delegate { return new object(); });

      Assert.That (_cache.ToNonGenericEnumerable(), Is.Empty);
    }

    [Test]
    public void Clear ()
    {
      _cache.Clear ();
      // Succeed
    }

    [Test]
    public void GetIsNull()
    {
      Assert.That (_cache.IsNull, Is.True);
    }

    [Test]
    public void Serialization ()
    {
      ICache<string, object> deserializedCache = Serializer.SerializeAndDeserialize (_cache);
      Assert.That (deserializedCache is NullCache<string, object>, Is.True);
      Assert.That (deserializedCache, Is.Not.SameAs (_cache));
    }
  }
}
