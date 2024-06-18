// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Reflection;
using MixinXRef.Reflection.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class FastMemberInvokerCacheTest
  {
    private FastMemberInvokerCache _cache;

    [SetUp]
    public void SetUp()
    {
      _cache = new FastMemberInvokerCache ();
    }

    [Test]
    public void CacheKey_Equals_EqualKeys()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (int), typeof (string) });

      Assert.That (key1, Is.EqualTo (key2));
    }

    [Test]
    public void CacheKey_Equals_NonEqualKeys_DeclaringType ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (int), "Foo", new Type[0], new[] { typeof (int), typeof (string) });

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void CacheKey_Equals_NonEqualKeys_MemberName ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Bar", new Type[0], new[] { typeof (int), typeof (string) });

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void CacheKey_Equals_NonEqualKeys_ArgumentTypes ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (string), typeof (int) });

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void CacheKey_GetHashCode_EqualKeys ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new Type[0], new[] { typeof (int), typeof (string) });

      Assert.That (key1.GetHashCode(), Is.EqualTo (key2.GetHashCode()));
    }

    [Test]
    public void GetFastMethodInvoker()
    {
      var instance = "stringContent";
      var invoker = _cache.GetOrCreateFastMethodInvoker (
          instance.GetType (),
          "IsNullOrEmpty", new Type[0],
          new[] { typeof (string) }, 
          BindingFlags.Public | BindingFlags.Static);

      var output = invoker (null, new object[] { instance });

      Assert.That (output, Is.EqualTo (false));
    }

    [Test]
    public void GetFastMethodInvoker_Twice ()
    {
      var invoker1 = _cache.GetOrCreateFastMethodInvoker (
          typeof (string),
          "IsNullOrEmpty", new Type[0],
          new[] { typeof (string) },
          BindingFlags.Public | BindingFlags.Static);
      var invoker2 = _cache.GetOrCreateFastMethodInvoker (
          typeof (string),
          "IsNullOrEmpty", new Type[0],
          new[] { typeof (string) },
          BindingFlags.Public | BindingFlags.Static);

      Assert.That (invoker2, Is.SameAs (invoker1));
    }
  }
}