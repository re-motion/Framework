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
using Moq;
using NUnit.Framework;
using Remotion.Development.Moq.UnitTesting.Threading;
using Remotion.Development.UnitTesting;

namespace Remotion.Collections.Caching.UnitTests
{
  [TestFixture]
  public class LockingCacheDecoratorTest
  {
    private LockingCacheDecorator<string, int> _decorator;

    private LockingDecoratorTestHelper<ICache<string, int>> _helper;

    [SetUp]
    public void SetUp ()
    {
      var innerCacheMock = new Mock<ICache<string, int>>(MockBehavior.Strict);

      _decorator = new LockingCacheDecorator<string, int>(innerCacheMock.Object);

      var lockObject = PrivateInvoke.GetNonPublicField(_decorator, "_lock");
      _helper = new LockingDecoratorTestHelper<ICache<string, int>>(_decorator, lockObject, innerCacheMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((INullObject)_decorator).IsNull, Is.False);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      Func<string, int> valueFactory = _ => 3;
      _helper.ExpectSynchronizedDelegation(
          cache => cache.GetOrCreateValue("hugo", It.Is<Func<string, int>>(_ => _ == valueFactory)),
          cache => cache.GetOrCreateValue("hugo", valueFactory),
          17,
          _ => { });
    }

    [Test]
    public void TryGetValue ()
    {
      int value;
      _helper.ExpectSynchronizedDelegation(cache => cache.TryGetValue("hugo", out value), true);
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_HasNoNestedValue ()
    {
      object expected = new object();

      var cache = new LockingCacheDecorator<string, object>(new Cache<string, object>());

      var actualValue = cache.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => cache.TryGetValue(key, out _),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

            return expected;
          });

      Assert.That(actualValue, Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      object expected = new object();

      var cache = new LockingCacheDecorator<string, object>(new Cache<string, object>());

      var actualValue = cache.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => cache.GetOrCreateValue(key, nestedKey => 13),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

            return expected;
          });

      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(cache.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.SameAs(expected));
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      _helper.ExpectSynchronizedDelegation(
          cache => cache.GetEnumerator(),
          ((IEnumerable<KeyValuePair<string, int>>)new[]
                                                    {
                                                        new KeyValuePair<string, int>("key1", 1),
                                                        new KeyValuePair<string, int>("key2", 2)
                                                    }).GetEnumerator(),
          actual => Assert.That(
              ConvertToSequenceGeneric(actual),
              Is.EquivalentTo(
                  new[]
                  {
                      new KeyValuePair<string, int>("key1", 1),
                      new KeyValuePair<string, int>("key2", 2)
                  })));
    }

    private static IEnumerable<KeyValuePair<string, int>> ConvertToSequenceGeneric (IEnumerator<KeyValuePair<string, int>> actual)
    {
      while (actual.MoveNext())
        yield return actual.Current;
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      _helper.ExpectSynchronizedDelegation(
          cache => cache.GetEnumerator(),
          cache => ((IEnumerable)cache).GetEnumerator(),
          ((IEnumerable<KeyValuePair<string, int>>)new[]
                                                    {
                                                        new KeyValuePair<string, int>("key1", 1),
                                                        new KeyValuePair<string, int>("key2", 2)
                                                    }).GetEnumerator(),
          actual => Assert.That(
              ConvertToSequenceNonGeneric(actual),
              Is.EquivalentTo(
                  new[]
                  {
                      new KeyValuePair<string, int>("key1", 1),
                      new KeyValuePair<string, int>("key2", 2)
                  })));
    }

    private static IEnumerable<object> ConvertToSequenceNonGeneric (IEnumerator actual)
    {
      while (actual.MoveNext())
        yield return actual.Current;
    }

    [Test]
    public void Clear ()
    {
      _helper.ExpectSynchronizedDelegation(store => store.Clear());
    }

    [Test]
    [TestCase(1, 5800)]
    [TestCase(2, 12500)]
    [TestCase(3, 19000)]
    [TestCase(4, 26000)]
    [Explicit]
    public void Performance (int threadCount, int timeTakenExpected)
    {
      var cache = new LockingCacheDecorator<string, object>(new Cache<string, object>());
      var stopwatches = new Stopwatch[threadCount];
      for (int i = 0; i < threadCount; i++)
        stopwatches[i] = new Stopwatch();

      var threadStart = new ParameterizedThreadStart(
          arg =>
          {
            cache.GetOrCreateValue("key", k => "value");
            cache.TryGetValue("key", out var value);

            var stopwatch = (Stopwatch)arg;
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
          });

      GC.Collect();
      GC.WaitForFullGCComplete();
#pragma warning disable RMCORE0001
      var threads = new Thread[threadCount];
      for (int i = 0; i < threadCount; i++)
        threads[i] = new Thread(threadStart);
#pragma warning restore RMCORE0001
      for (int i = 0; i < threadCount; i++)
        threads[i].Start(stopwatches[i]);

      for (int i = 0; i < threadCount; i++)
        threads[i].Join();

      var timeTakenActual = stopwatches.Sum(stopwatch => stopwatch.ElapsedMilliseconds) / stopwatches.Length;

      Console.WriteLine("Time expected: {0}ms (thread count: {1}, release build on Intel Xeon E5-1620 v2 @ 3.70GHz)", timeTakenExpected, threadCount);
      Console.WriteLine("Time taken: {0:D}ms", timeTakenActual);
    }
  }
}
