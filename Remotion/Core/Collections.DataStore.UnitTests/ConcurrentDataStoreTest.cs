using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Collections.DataStore.UnitTests.Utilities;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Collections.DataStore.UnitTests
{
  [TestFixture]
  public class ConcurrentDataStoreTest
  {
    private ConcurrentDataStore<string, object> _store;

    [SetUp]
    public void SetUp ()
    {
      _store = new ConcurrentDataStore<string, object>();
      _store.Add("a", "1");
      _store.Add("b", "2");
    }

    [Test]
    public void Initialize_WithCustomEqualityComparer ()
    {
      var comparer = StringComparer.InvariantCultureIgnoreCase;
      var dataStore = new ConcurrentDataStore<string, object>(comparer);
      dataStore.Add("a", "1");

      Assert.That(dataStore.ContainsKey("a"));
      Assert.That(dataStore.ContainsKey("A"));
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
      _store.Add("c", "5");
      Assert.That(_store.ContainsKey("c"));
      Assert.That(_store["c"], Is.EqualTo("5"));
    }

    [Test]
    public void Add_Null ()
    {
      _store.Add("d", null);
      Assert.That(_store.ContainsKey("d"));
      Assert.That(_store["d"], Is.Null);
    }

    [Test]
    public void Add_Twice ()
    {
      _store.Add("d", "1");
      Assert.That(
          () => _store.Add("d", "2"),
          Throws.ArgumentException.And.ArgumentExceptionMessageEqualTo(
              "The store already contains an element with key \'d\'. (Old value: \'1\', new value: \'2\')", "key"));
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
      Assert.That(_store["a"], Is.EqualTo("1"));
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
      _store["c"] = "5";
      Assert.That(_store["c"], Is.EqualTo("5"));
    }

    [Test]
    public void SetValue_Overwrite ()
    {
      _store["a"] = "5";
      Assert.That(_store["a"], Is.EqualTo("5"));
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
      Assert.That(_store.GetValueOrDefault("a"), Is.EqualTo("1"));
    }

    [Test]
    public void GetValueOrDefault_Default ()
    {
      Assert.That(_store.GetValueOrDefault("c"), Is.Null);
    }

    [Test]
    public void TryGetValue_WithReferenceType_WithResultNotInDataStore ()
    {
      Assert.That(_store.TryGetValue("key1", out var actual), Is.False);
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void TryGetValue_WithValueType_WithResultNotInDataStore ()
    {
      var dataStore = new ConcurrentDataStore<string, int>();
      int actual;
      Assert.That(dataStore.TryGetValue("key1", out actual), Is.False);
      Assert.That(actual, Is.EqualTo(0));
    }

    [Test]
    public void TryGetValue_True ()
    {
      Assert.That(_store.TryGetValue("a", out var value));
      Assert.That(value, Is.EqualTo("1"));
    }

    [Test]
    public void TryGetValue_False ()
    {
      Assert.That(_store.TryGetValue("c", out var value), Is.False);
      Assert.That(value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_Get ()
    {
      bool delegateCalled = false;
      Assert.That(
          _store.GetOrCreateValue(
              "a",
              delegate
              {
                delegateCalled = true;
                return "7";
              }),
          Is.EqualTo("1"));
      Assert.That(delegateCalled, Is.False);
    }

    [Test]
    public void GetOrCreateValue_Create ()
    {
      bool delegateCalled = false;
      Assert.That(
          _store.GetOrCreateValue(
              "c",
              delegate (string key)
              {
                delegateCalled = true;
                Assert.That(key, Is.EqualTo("c"));
                return "7";
              }),
          Is.EqualTo("7"));
      Assert.That(delegateCalled);
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_ThrowsInvalidOperationException ()
    {
      object expected = "13";

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

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetValueOrDefault_ThrowsInvalidOperationException ()
    {
      object expected = "13";

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

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      object expected = "15";

      var actualValue = _store.GetOrCreateValue(
              "key1",
              delegate (string key)
              {
                Assert.That(
                    () => _store.GetOrCreateValue(key, nestedKey => "13"),
                    Throws.InvalidOperationException.With.Message.StartsWith(
                        "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

                return expected;
              });

      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedAdd_ThrowsInvalidOperationException ()
    {
      object expected = "15";

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => _store.Add(key, "13"),
                Throws.InvalidOperationException.With.Message.StartsWith(
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedContainsKey_ThrowsInvalidOperationException ()
    {
      object expected = "13";

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

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedRemove_ThrowsInvalidOperationException ()
    {
      object expected = "13";

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

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedClear_RemovesValues ()
    {
      object expected = "13";

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate
          {
            _store.Clear();
            return expected;
          });
      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.False);
      Assert.That(actualValue2, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithNestedEnumeration_SkipsNewItem ()
    {
      object expected = "15";
      KeyValuePair<string, object>[] nestedItems = null;

      var actualValue = _store.GetOrCreateValue(
          "key1",
          delegate
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
                  new KeyValuePair<string, object>("a", "1"),
                  new KeyValuePair<string, object>("b", "2")
              }));

      Assert.That(_store.TryGetValue("key1", out var actualValue2), Is.True);
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

      Assert.That(_store.TryGetValue("key1", out var actual), Is.False);
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithException_GetOrCreateValue_InsertsSecondValue ()
    {
      var exception = new Exception();
      Assert.That(
          () => _store.GetOrCreateValue("key1", key => throw exception),
          Throws.Exception.SameAs(exception));

      object expected = "14";
      object actual = _store.GetOrCreateValue("key1", key => expected);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetOrCreateValue_DoesNotKeepFactoryAlive ()
    {
      WeakReference GetValueFactoryReference ()
      {
        var expected = "13";
        Func<string, object> valueFactory = key => expected;
        Assert.That(_store.GetOrCreateValue("key1", valueFactory), Is.EqualTo(expected));
        return new WeakReference(valueFactory);
      }

      // The valueFactory must be created in a separate method: The x64 JITter in .NET 4.7.2 (DEBUG builds only) keeps the reference alive until the variable is out of scope.
      var valueFactoryReference = GetValueFactoryReference();
      GC.Collect();
      GC.WaitForFullGCComplete();
      Assert.That(valueFactoryReference.IsAlive, Is.False);
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
            resultThread2 = _store.GetOrCreateValue(
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
      var resultThread1 = _store.GetOrCreateValue(
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
            resultThread2 = _store.GetOrCreateValue("key", key => throw new InvalidOperationException());
          });
#pragma warning restore RMCORE0001
      var resultThread1 = _store.GetOrCreateValue(
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
            resultThread2 = _store.GetOrCreateValue("key", key => throw new InvalidOperationException());
          });
#pragma warning restore RMCORE0001
      var resultThread1 = _store.GetOrCreateValue(
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
            _store.TryGetValue("key", out resultThread2);
          });
#pragma warning restore RMCORE0001
      var resultThread1 = _store.GetOrCreateValue(
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
    public void GetEnumerator_Generic ()
    {
      Assert.That(
          _store.ToArray(),
          Is.EquivalentTo(
              new[]
              {
                  new KeyValuePair<string, object>("a", "1"),
                  new KeyValuePair<string, object>("b", "2")
              }
              ));
    }

    [Test]
    public void GetEnumerator_Generic_Reset_ResetsEnumerator ()
    {
      object expected1 = new object();
      _store.GetOrCreateValue("key1", delegate { return expected1; });

      using (var enumerator = _store.GetEnumerator())
      {
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Key, Is.AnyOf("a", "b", "key1"));
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.MoveNext(), Is.False);
        enumerator.Reset();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current.Key, Is.AnyOf("a", "b", "key1"));
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
                  new KeyValuePair<string, object>("a", "1"),
                  new KeyValuePair<string, object>("b", "2")
              }
              ));
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
            _store.GetOrCreateValue("key", k => "value");
            _store.TryGetValue("key", out var value);

            var stopwatch = (Stopwatch)arg;
            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
              var key = i.ToString("D9");
              _store.GetOrCreateValue(key, k => k + ": value");

              for (int j = 0; j < 100 * 1000; j++)
              {
                _store.TryGetValue(key, out value);
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
