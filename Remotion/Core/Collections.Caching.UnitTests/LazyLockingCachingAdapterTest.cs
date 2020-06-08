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

#pragma warning disable 618
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Wrapper = Remotion.Collections.Caching.LazyLockingCachingAdapter<string, object>.Wrapper;

namespace Remotion.Collections.Caching.UnitTests
{
  using InnerFactory = Func<ICache<string, Lazy<Wrapper>>, Lazy<Wrapper>>;

  [TestFixture]
  public class LazyLockingCachingAdapterTest
  {
    private ICache<string, Lazy<Wrapper>> _innerCacheMock = default!;
    private LazyLockingCachingAdapter<string, object> _cachingAdapter = default!;

    [SetUp]
    public void SetUp ()
    {
      _innerCacheMock = MockRepository.GenerateStrictMock<ICache<string, Lazy<Wrapper>>> ();
      _cachingAdapter = new LazyLockingCachingAdapter<string, object> (_innerCacheMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _cachingAdapter).IsNull, Is.False);
    }

    [Test]
    public void GetOrCreateValue_TryGetValueIsFalse ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerCacheMock
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>?>.Out (null).Dummy))
          .Return (false)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());
      _innerCacheMock
          .Expect (mock => ((InnerFactory) (store => store.GetOrCreateValue (
              Arg.Is ("key"),
              Arg<Func<string, Lazy<Wrapper>>>.Matches (f => f ("Test").Value.Value.Equals ("Test123"))))) (mock))
          .Return (doubleCheckedLockingContainer)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      var actualResult = _cachingAdapter.GetOrCreateValue ("key", key => key + "123");

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (value));
    }

    [Test]
    public void GetOrCreateValue_TryGetValueIsTrue ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerCacheMock
          .Expect (
              mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>?>.Out (doubleCheckedLockingContainer).Dummy))
          .Return (true)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      var actualResult = _cachingAdapter.GetOrCreateValue ("key", key => key + "123");

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (value));
    }

    [Test]
    public void GetOrCreateValue_TwiceWithNullValue_DoesNotEvalueValueFactoryTwice ()
    {
      var adapter = new LazyLockingCachingAdapter<string, object>(new Cache<string, Lazy<Wrapper>>());

      bool wasCalled = false;

      var value = adapter.GetOrCreateValue (
          "test",
          s =>
          {
            Assert.That (wasCalled, Is.False);
            wasCalled = true;
            return null!;
          });
      Assert.That (value, Is.Null);

      value = adapter.GetOrCreateValue ("test", s => { throw new InvalidOperationException ("Must not be called."); });
      Assert.That (value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_HasNoNestedValue ()
    {
      object expected = new object();

      var cache = new LazyLockingCachingAdapter<string, object> (new Cache<string, Lazy<Wrapper>>());

      var actualValue = cache.GetOrCreateValue (
          "key1",
          delegate (string key)
          {
            Assert.That (
                () => cache.TryGetValue (key, out _),
                Throws.InvalidOperationException);

            return expected;
          });

      Assert.That (actualValue, Is.SameAs (expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      object expected = new object();

      var cache = new LazyLockingCachingAdapter<string, object> (new Cache<string, Lazy<Wrapper>>());

      var actualValue = cache.GetOrCreateValue (
          "key1",
          delegate (string key)
          {
            Assert.That (
                () => cache.GetOrCreateValue (key, nestedKey => 13),
                Throws.InvalidOperationException);

            return expected;
          });

      Assert.That (actualValue, Is.EqualTo (expected));

      Assert.That (cache.TryGetValue ("key1", out var actualValue2), Is.True);
      Assert.That (actualValue2, Is.SameAs (expected));
    }

    [Test]
    public void TryGetValue_ValueFound ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerCacheMock
          .Expect (
              mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>?>.Out (doubleCheckedLockingContainer).Dummy))
          .Return (true)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      var actualResult = _cachingAdapter.TryGetValue ("key", out var result);

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (true));

      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    public void TryGetValue_NoValueFound ()
    {
      _innerCacheMock
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>?>.Out (null).Dummy))
          .Return (false)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      var actualResult = _cachingAdapter.TryGetValue ("key", out var result);

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (false));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      var expected1 = new object();
      var expected2 = new object();

      var returnValue = ((IEnumerable<KeyValuePair<string, Lazy<Wrapper>>>)
          new[]
          {
              new KeyValuePair<string, Lazy<Wrapper>> ("key1", CreateContainerThatChecksForNotProtected (expected1)),
              new KeyValuePair<string, Lazy<Wrapper>> ("key2", CreateContainerThatChecksForNotProtected (expected2))
          }).GetEnumerator();

      _innerCacheMock
          .Expect (mock => mock.GetEnumerator())
          .Return (returnValue)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      var items = _cachingAdapter.ToArray();

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (
          items,
          Is.EquivalentTo (
              new[]
              {
                  new KeyValuePair<string, object> ("key1", expected1),
                  new KeyValuePair<string, object> ("key2", expected2)
              }
              ));
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      var expected1 = new object();
      var expected2 = new object();

      var returnValue = ((IEnumerable<KeyValuePair<string, Lazy<Wrapper>>>)
          new[]
          {
              new KeyValuePair<string, Lazy<Wrapper>> ("key1", CreateContainerThatChecksForNotProtected (expected1)),
              new KeyValuePair<string, Lazy<Wrapper>> ("key2", CreateContainerThatChecksForNotProtected (expected2))
          }).GetEnumerator();

      _innerCacheMock
          .Expect (mock => mock.GetEnumerator())
          .Return (returnValue)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      var actual = new List<object>();
      var enumerator = ((IEnumerable) _cachingAdapter).GetEnumerator();
      while (enumerator.MoveNext())
        actual.Add (enumerator.Current);

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (
          actual,
          Is.EquivalentTo (
              new[]
              {
                  new KeyValuePair<string, object> ("key1", expected1),
                  new KeyValuePair<string, object> ("key2", expected2)
              }
              ));
    }

    [Test]
    public void Clear ()
    {
      _innerCacheMock
          .Expect (store => store.Clear ())
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      _cachingAdapter.Clear ();

      _innerCacheMock.VerifyAllExpectations ();
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (
          new LazyLockingCachingAdapter<string, string> (
              new Cache<string, Lazy<LazyLockingCachingAdapter<string, string>.Wrapper>>()));
    }

    [Test]
    [TestCase (1, 6000)]
    [TestCase (2, 13000)]
    [TestCase (3, 21000)]
    [TestCase (4, 28000)]
    [Explicit]
    public void Performance (int threadCount, int timeTakenExpected)
    {
      var cache = new LazyLockingCachingAdapter<string, object> (new Cache<string, Lazy<Wrapper>>());
      var stopwatches = new Stopwatch[threadCount];
      for (int i = 0; i < threadCount; i++)
        stopwatches[i] = new Stopwatch();

      var threadStart = new ParameterizedThreadStart (
          arg =>
          {
            cache.GetOrCreateValue ("key", k => "value");
            cache.TryGetValue ("key", out var value);

            var stopwatch = (Stopwatch) arg;
            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
              var key = i.ToString ("D9");
              cache.GetOrCreateValue (key, k => k + ": value");

              for (int j = 0; j < 100 * 1000; j++)
              {
                cache.TryGetValue (key, out value);
              }
            }

            stopwatch.Stop();
          });

      GC.Collect();
      GC.WaitForFullGCComplete();

      var threads = new Thread[threadCount];
      for (int i = 0; i < threadCount; i++)
        threads[i] = new Thread (threadStart);

      for (int i = 0; i < threadCount; i++)
        threads[i].Start (stopwatches[i]);

      for (int i = 0; i < threadCount; i++)
        threads[i].Join();

      var timeTakenActual = stopwatches.Sum (stopwatch => stopwatch.ElapsedMilliseconds) / stopwatches.Length;

      Console.WriteLine ("Time expected: {0}ms (thread count: {1}, release build on Intel Xeon E5-1620 v2 @ 3.70GHz)", timeTakenExpected, threadCount);
      Console.WriteLine ("Time taken: {0:D}ms", timeTakenActual);
    }

    private void CheckInnerCacheIsProtected ()
    {
      var lockingCacheDecorator = GetLockingCacheDecorator (_cachingAdapter);
      var lockObject = PrivateInvoke.GetNonPublicField (lockingCacheDecorator, "_lock");

      LockTestHelper.CheckLockIsHeld (lockObject);
    }

    private void CheckInnerCacheIsNotProtected ()
    {
      var lockingCacheDecorator = GetLockingCacheDecorator (_cachingAdapter);
      var lockObject = PrivateInvoke.GetNonPublicField (lockingCacheDecorator, "_lock");

      LockTestHelper.CheckLockIsNotHeld (lockObject);
    }

    private Lazy<Wrapper> CreateContainerThatChecksForNotProtected (object value)
    {
      return new Lazy<Wrapper> (() =>
      {
        CheckInnerCacheIsNotProtected ();
        return new Wrapper (value);
      });
    }

    private LockingCacheDecorator<string, Lazy<Wrapper>> GetLockingCacheDecorator (
        LazyLockingCachingAdapter<string, object> lazyLockingCacheAdapter)
    {
      return (LockingCacheDecorator<string, Lazy<Wrapper>>)
          PrivateInvoke.GetNonPublicField (lazyLockingCacheAdapter, "_innerCache");
    }
  }
}