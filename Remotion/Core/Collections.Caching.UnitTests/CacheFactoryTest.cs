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

//

using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Collections.Caching.UnitTests
{
  [TestFixture]
  public class CacheFactoryTest
  {
    private StringComparer _comparer;

    [SetUp]
    public void SetUp ()
    {
      _comparer = StringComparer.InvariantCultureIgnoreCase;
    }

    [Test]
    public void Create ()
    {
      var result = CacheFactory.Create<string, int>();

      Assert.That(result, Is.TypeOf(typeof(Cache<string, int>)));
    }

    [Test]
    public void Create_CacheInvalidationTokenOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.Create();
      var result = CacheFactory.Create<string, int>(cacheInvalidationToken);

      Assert.That(result, Is.TypeOf(typeof(InvalidationTokenBasedCacheDecorator<string, int>)));
      Assert.That(((InvalidationTokenBasedCacheDecorator<string, int>)result).InvalidationToken, Is.SameAs(cacheInvalidationToken));
      var innerCache = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache, Is.TypeOf(typeof(Cache<string, int>)));
    }

    [Test]
    public void Create_IEqualityComparerOverload ()
    {
      var result = CacheFactory.Create<string, int>(_comparer);

      Assert.That(result, Is.TypeOf(typeof(Cache<string, int>)));
      Assert.That(((Cache<string, int>)result).Comparer, Is.SameAs(_comparer));
    }

    [Test]
    public void Create_CacheInvalidationTokenOverload_IEqualityComparerOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.Create();
      var result = CacheFactory.Create<string, int>(cacheInvalidationToken, _comparer);

      Assert.That(result, Is.TypeOf(typeof(InvalidationTokenBasedCacheDecorator<string, int>)));
      Assert.That(((InvalidationTokenBasedCacheDecorator<string, int>)result).InvalidationToken, Is.SameAs(cacheInvalidationToken));
      var innerCache = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache, Is.TypeOf(typeof(Cache<string, int>)));
      Assert.That(((Cache<string, int>)innerCache).Comparer, Is.SameAs(_comparer));
    }

    [Test]
    public void CreateWithSynchronization ()
    {
      var result = CacheFactory.CreateWithSynchronization<string, int>();

      Assert.That(result, Is.TypeOf(typeof(ConcurrentCache<string, int>)));
    }

    [Test]
    public void CreateWithSynchronization_CacheInvalidationTokenOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.CreatWithLocking();
      var result = CacheFactory.CreateWithSynchronization<string, int>(cacheInvalidationToken);

      Assert.That(result, Is.TypeOf(typeof(InvalidationTokenBasedCacheDecorator<string, int>)));
      Assert.That(((InvalidationTokenBasedCacheDecorator<string, int>)result).InvalidationToken, Is.SameAs(cacheInvalidationToken));
      var innerCache = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache, Is.TypeOf(typeof(ConcurrentCache<string, int>)));
    }

    [Test]
    public void CreateWithSynchronization_IEqualityComparerOverload ()
    {
      var result = CacheFactory.CreateWithSynchronization<string, int>(_comparer);

      Assert.That(result, Is.TypeOf(typeof(ConcurrentCache<string, int>)));

      result.GetOrCreateValue("a", key => 1);

      Assert.That(result.GetOrCreateValue("a", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
      Assert.That(result.GetOrCreateValue("A", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
    }

    [Test]
    public void CreateWithSynchronization_CacheInvalidationTokenOverload_IEqualityComparerOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.CreatWithLocking();
      var result = CacheFactory.CreateWithSynchronization<string, int>(cacheInvalidationToken, _comparer);

      Assert.That(result, Is.TypeOf(typeof(InvalidationTokenBasedCacheDecorator<string, int>)));
      Assert.That(((InvalidationTokenBasedCacheDecorator<string, int>)result).InvalidationToken, Is.SameAs(cacheInvalidationToken));
      var innerCache = (ICache<string, int>)PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache, Is.TypeOf(typeof(ConcurrentCache<string, int>)));

      innerCache.GetOrCreateValue("a", key => 1);

      Assert.That(innerCache.GetOrCreateValue("a", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
      Assert.That(innerCache.GetOrCreateValue("A", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
    }

#pragma warning disable 618
    [Test]
    public void CreateWithLocking ()
    {
      var result = CacheFactory.CreateWithLocking<string, int>();

      Assert.That(result, Is.TypeOf(typeof(LockingCacheDecorator<string, int>)));
      var innerCache = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache, Is.TypeOf(typeof(Cache<string, int>)));
    }

    [Test]
    public void CreateWithLocking_CacheInvalidationTokenOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.CreatWithLocking();
      var result = CacheFactory.CreateWithLocking<string, int>(cacheInvalidationToken);

      Assert.That(result, Is.TypeOf(typeof(LockingCacheDecorator<string, int>)));
      var innerCache1 = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache1, Is.TypeOf(typeof(InvalidationTokenBasedCacheDecorator<string, int>)));
      Assert.That(((InvalidationTokenBasedCacheDecorator<string, int>)innerCache1).InvalidationToken, Is.SameAs(cacheInvalidationToken));
      var innerCache2 = PrivateInvoke.GetNonPublicField(innerCache1, "_innerCache");
      Assert.That(innerCache2, Is.TypeOf(typeof(Cache<string, int>)));
    }

    [Test]
    public void CreateWithLocking_IEqualityComparerOverload ()
    {
      var result = CacheFactory.CreateWithLocking<string, int>(_comparer);

      Assert.That(result, Is.TypeOf(typeof(LockingCacheDecorator<string, int>)));
      var innerCache = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache, Is.TypeOf(typeof(Cache<string, int>)));
      Assert.That(((Cache<string, int>)innerCache).Comparer, Is.SameAs(_comparer));
    }

    [Test]
    public void CreateWithLocking_CacheInvalidationTokenOverload_IEqualityComparerOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.CreatWithLocking();
      var result = CacheFactory.CreateWithLocking<string, int>(cacheInvalidationToken, _comparer);

      Assert.That(result, Is.TypeOf(typeof(LockingCacheDecorator<string, int>)));
      var innerCache1 = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(innerCache1, Is.TypeOf(typeof(InvalidationTokenBasedCacheDecorator<string, int>)));
      Assert.That(((InvalidationTokenBasedCacheDecorator<string, int>)innerCache1).InvalidationToken, Is.SameAs(cacheInvalidationToken));
      var innerCache2 = PrivateInvoke.GetNonPublicField(innerCache1, "_innerCache");
      Assert.That(innerCache2, Is.TypeOf(typeof(Cache<string, int>)));
      Assert.That(((Cache<string, int>)innerCache2).Comparer, Is.SameAs(_comparer));
    }

    [Test]
    public void CreateWithLazyLocking ()
    {
      var result = CacheFactory.CreateWithLazyLocking<string, object>();

      Assert.That(result, Is.TypeOf(typeof(LazyLockingCachingAdapter<string, object>)));
      var innerCache1 = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(
          innerCache1,
          Is.TypeOf(typeof(LockingCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      var innerCache2 = PrivateInvoke.GetNonPublicField(innerCache1, "_innerCache");
      Assert.That(
          innerCache2,
          Is.TypeOf(typeof(Cache<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
    }

    [Test]
    public void CreateWithLazyLocking_CacheInvalidationTokenOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.CreatWithLocking();
      var result = CacheFactory.CreateWithLazyLocking<string, object>(cacheInvalidationToken);

      Assert.That(result, Is.TypeOf(typeof(LazyLockingCachingAdapter<string, object>)));
      var innerCache1 = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(
          innerCache1,
          Is.TypeOf(typeof(LockingCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      var innerCache2 = PrivateInvoke.GetNonPublicField(innerCache1, "_innerCache");
      Assert.That(
          innerCache2,
          Is.TypeOf(
              typeof(InvalidationTokenBasedCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      Assert.That(
          ((InvalidationTokenBasedCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)innerCache2).InvalidationToken,
          Is.SameAs(cacheInvalidationToken));
      var innerCache3 = PrivateInvoke.GetNonPublicField(innerCache2, "_innerCache");
      Assert.That(
          innerCache3,
          Is.TypeOf(typeof(Cache<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
    }

    [Test]
    public void CreateWithLazyLocking_IEqualityComparerOverload ()
    {
      var result = CacheFactory.CreateWithLazyLocking<string, object>(_comparer);

      Assert.That(result, Is.TypeOf(typeof(LazyLockingCachingAdapter<string, object>)));
      var innerCache1 = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(
          innerCache1,
          Is.TypeOf(typeof(LockingCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      var innerCache2 = PrivateInvoke.GetNonPublicField(innerCache1, "_innerCache");
      Assert.That(
          innerCache2,
          Is.TypeOf(typeof(Cache<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      Assert.That(((Cache<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)innerCache2).Comparer, Is.SameAs(_comparer));
    }

    [Test]
    public void CreateWithLazyLocking_CacheInvalidationTokenOverload_IEqualityComparerOverload ()
    {
      var cacheInvalidationToken = InvalidationToken.CreatWithLocking();
      var result = CacheFactory.CreateWithLazyLocking<string, object>(cacheInvalidationToken, _comparer);

      Assert.That(result, Is.TypeOf(typeof(LazyLockingCachingAdapter<string, object>)));
      var innerCache1 = PrivateInvoke.GetNonPublicField(result, "_innerCache");
      Assert.That(
          innerCache1,
          Is.TypeOf(typeof(LockingCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      var innerCache2 = PrivateInvoke.GetNonPublicField(innerCache1, "_innerCache");
      Assert.That(
          innerCache2,
          Is.TypeOf(
              typeof(InvalidationTokenBasedCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      Assert.That(
          ((InvalidationTokenBasedCacheDecorator<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)innerCache2).InvalidationToken,
          Is.SameAs(cacheInvalidationToken));
      var innerCache3 = PrivateInvoke.GetNonPublicField(innerCache2, "_innerCache");
      Assert.That(
          innerCache3,
          Is.TypeOf(typeof(Cache<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      Assert.That(((Cache<string, Lazy<LazyLockingCachingAdapter<string, object>.Wrapper>>)innerCache3).Comparer, Is.SameAs(_comparer));
    }
#pragma warning restore 618
  }
}
