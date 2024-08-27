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
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Collections.Caching.UnitTests.Utilities;

namespace Remotion.Collections.Caching.UnitTests
{
  [TestFixture]
  public class ConcurrentCacheTest
  {
    private ICache<string, object> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new ConcurrentCache<string, object>();
    }

    [Test]
    public void Initialize_WithCustomEqualityComparer ()
    {
      var comparer = StringComparer.InvariantCultureIgnoreCase;
      var cache = new ConcurrentCache<string, int?>(comparer);
      cache.GetOrCreateValue("a", key => 1);

      Assert.That(cache.GetOrCreateValue("a", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
      Assert.That(cache.GetOrCreateValue("A", delegate { throw new InvalidOperationException(); }), Is.EqualTo(1));
    }

    [Test]
    public void TryGetValue_WithReferenceType_WithResultNotInCache ()
    {
      Assert.That(_cache.TryGetValue("key1", out var actual), Is.False);
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void TryGetValue_WithValueType_WithResultNotInCache ()
    {
      var cache = new ConcurrentCache<string, int>();
      int actual;
      Assert.That(cache.TryGetValue("key1", out actual), Is.False);
      Assert.That(actual, Is.EqualTo(0));
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
    public void GetOrCreateValue_WithNull ()
    {
      bool delegateCalled = false;
      Assert.That(
          _cache.GetOrCreateValue(
              "key1",
              delegate (string key)
              {
                Assert.That(key, Is.EqualTo("key1"));
                delegateCalled = true;
                return null;
              }),
          Is.Null);
      Assert.That(delegateCalled, Is.True);
    }

    [Test]
    public void GetOrCreateValue_WithParallelThreadsInsertingDifferentKeys ()
    {
      var expectedThread1 = new object();
      var expectedThread2 = new object();
      object resultThread2 = null;
      var waitHandleThread1a = new ManualResetEvent(false);
      var waitHandleThread1b = new ManualResetEvent(false);
      var waitHandleThread2 = new ManualResetEvent(false);
#pragma warning disable RMCORE0001
      var thread2 = Task.Run(
          () =>
          {
            resultThread2 = _cache.GetOrCreateValue(
                "key2",
                key =>
                {
                  waitHandleThread1a.Set();
                  Assert.That(waitHandleThread2.WaitOne(TimeSpan.FromSeconds(1)), "Dead lock detected.");
                  return expectedThread2;
                });
            waitHandleThread1b.Set();
          });
#pragma warning restore RMCORE0001
      Assert.That(waitHandleThread1a.WaitOne(TimeSpan.FromSeconds(1)), "Dead lock detected.");
      var resultThread1 = _cache.GetOrCreateValue(
          "key1",
          key =>
          {
            waitHandleThread2.Set();
            Assert.That(waitHandleThread1b.WaitOne(TimeSpan.FromSeconds(1)), "Dead lock detected.");
            return expectedThread1;
          });
      Assert.That(resultThread1, Is.SameAs(expectedThread1));
      Assert.That(thread2.Wait(TimeSpan.FromSeconds(1)), "Dead lock detected.");
      Assert.That(resultThread2, Is.SameAs(expectedThread2));
    }

    [Test]
    public void GetOrCreateValue_WithParallelThreadsInsertingSameKey_SecondFactoryCallWillNotBeExecuted ()
    {
      var expectedThread1 = "T1";
      object resultThread2 = null;
      var waitHandleThread2 = new ManualResetEvent(false);
#pragma warning disable RMCORE0001
      var thread2 = Task.Run(
          () =>
          {
            Assert.That(waitHandleThread2.WaitOne(TimeSpan.FromSeconds(1)), "Dead lock detected.");
            resultThread2 = _cache.GetOrCreateValue("key", key => throw new InvalidOperationException());
          });
#pragma warning restore RMCORE0001
      var resultThread1 = _cache.GetOrCreateValue(
          "key",
          key =>
          {
            waitHandleThread2.Set();
            return expectedThread1;
          });
      Assert.That(resultThread1, Is.EqualTo(expectedThread1));
      Assert.That(thread2.Wait(TimeSpan.FromSeconds(1)), "Dead lock detected.");
      Assert.That(resultThread2, Is.EqualTo(expectedThread1));
    }

    [Test]
    public void GetOrCreateValue_WithParallelThreadsInsertingSameKeyWithNullValue_SecondFactoryCallWillNotBeExecuted ()
    {
      object resultThread2 = null;
      var waitHandleThread2 = new ManualResetEvent(false);
#pragma warning disable RMCORE0001
      var thread2 = Task.Run(
          () =>
          {
            Assert.That(waitHandleThread2.WaitOne(TimeSpan.FromSeconds(1)), "Dead lock detected.");
            resultThread2 = _cache.GetOrCreateValue("key", key => throw new InvalidOperationException());
          });
#pragma warning restore RMCORE0001
      var resultThread1 = _cache.GetOrCreateValue(
          "key",
          key =>
          {
            waitHandleThread2.Set();
            return null;
          });
      Assert.That(resultThread1, Is.EqualTo(null));
      Assert.That(thread2.Wait(TimeSpan.FromSeconds(1)), "Dead lock detected.");
      Assert.That(resultThread2, Is.EqualTo(null));
    }

    [Test]
    public void GetOrCreateValue_TryGetValue ()
    {
      object expected = new object();

      _cache.GetOrCreateValue("key1", key => expected);
      Assert.That(_cache.TryGetValue("key1", out var actual), Is.True);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_TryGetValue_WithParallelThreadsUsingSameKey_TryGetValueDuringFactoryCallWillBlock ()
    {
      var expectedThread1 = "T1";
      object resultThread2 = null;
      var waitHandleThread2 = new ManualResetEvent(false);
#pragma warning disable RMCORE0001
      var thread2 = Task.Run(
          () =>
          {
            Assert.That(waitHandleThread2.WaitOne(TimeSpan.FromSeconds(1)), "Dead lock detected.");
            _cache.TryGetValue("key", out resultThread2);
          });
#pragma warning restore RMCORE0001
      var resultThread1 = _cache.GetOrCreateValue(
          "key",
          key =>
          {
            waitHandleThread2.Set();
            return expectedThread1;
          });
      Assert.That(resultThread1, Is.EqualTo(expectedThread1));
      Assert.That(thread2.Wait(TimeSpan.FromSeconds(1)), "Dead lock detected.");
      Assert.That(resultThread2, Is.EqualTo(expectedThread1));
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
    public void GetOrCreateValue_Twice_SecondCallDoesNotUseFactory ()
    {
      object expected = new object();

      _cache.GetOrCreateValue("key1", key => expected);
      Assert.That(
          _cache.GetOrCreateValue("key1", key => throw new InvalidOperationException("The valueFactory must not be invoked.")),
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
    public void GetEnumerator_Generic_Reset_ResetsEnumerator ()
    {
      object expected1 = new object();
      _cache.GetOrCreateValue("key1", delegate { return expected1; });

      using (var enumerator = _cache.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, object>("key1", expected1)));
        Assert.That(enumerator.MoveNext(), Is.False);
        enumerator.Reset();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(new KeyValuePair<string, object>("key1", expected1)));
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
    [TestCase(1, 4900)]
    [TestCase(2, 5400)]
    [TestCase(3, 5700)]
    [TestCase(4, 6100)]
    [Explicit]
    public void Performance (int threadCount, int timeTakenExpected)
    {
      var stopwatches = new Stopwatch[threadCount];
      for (int i = 0; i < threadCount; i++)
        stopwatches[i] = new Stopwatch();

      var threadStart = new ParameterizedThreadStart(
          arg =>
          {
            _cache.GetOrCreateValue("key", k => "value");
            _cache.TryGetValue("key", out var value);

            var stopwatch = (Stopwatch)arg;
            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
              var key = i.ToString("D9");
              _cache.GetOrCreateValue(key, k => k + ": value");

              for (int j = 0; j < 100 * 1000; j++)
              {
                _cache.TryGetValue(key, out value);
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

      Console.WriteLine(
          "Time expected: {0}ms (thread count: {1}, release build x86 on Intel Xeon E5-1620 v2 @ 3.70GHz)",
          timeTakenExpected,
          threadCount);
      Console.WriteLine("Time taken: {0:D}ms", timeTakenActual);
    }
  }
}
