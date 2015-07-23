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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Wrapper = Remotion.Collections.LazyLockingCachingAdapter<string, object>.Wrapper;

namespace Remotion.UnitTests.Collections
{
  using InnerFactory = Func<ICache<string, DoubleCheckedLockingContainer<Wrapper>>, DoubleCheckedLockingContainer<Wrapper>>;

  [TestFixture]
  public class LazyLockingCachingAdapterTest
  {
    private ICache<string, DoubleCheckedLockingContainer<Wrapper>> _innerCacheMock;
    private LazyLockingCachingAdapter<string, object> _cachingAdapter;

    [SetUp]
    public void SetUp ()
    {
      _innerCacheMock = MockRepository.GenerateStrictMock<ICache<string, DoubleCheckedLockingContainer<Wrapper>>> ();
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
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<DoubleCheckedLockingContainer<Wrapper>>.Out (null).Dummy))
          .Return (false)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());
      _innerCacheMock
          .Expect (mock => ((InnerFactory) (store => store.GetOrCreateValue (
              Arg.Is ("key"),
              Arg<Func<string, DoubleCheckedLockingContainer<Wrapper>>>.Matches (f => f ("Test").Value.Value.Equals ("Test123"))))) (mock))
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
              mock => mock.TryGetValue (Arg.Is ("key"), out Arg<DoubleCheckedLockingContainer<Wrapper>>.Out (doubleCheckedLockingContainer).Dummy))
          .Return (true)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      var actualResult = _cachingAdapter.GetOrCreateValue ("key", key => key + "123");

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (value));
    }

    [Test]
    public void GetOrCreateValue_TwiceWithNullValue_DoesNotEvalueValueFactoryTwice ()
    {
      var adapter = new LazyLockingCachingAdapter<string, object>(new Cache<string, DoubleCheckedLockingContainer<Wrapper>>());

      bool wasCalled = false;

      var value = adapter.GetOrCreateValue (
          "test",
          s =>
          {
            Assert.That (wasCalled, Is.False);
            wasCalled = true;
            return null;
          });
      Assert.That (value, Is.Null);

      value = adapter.GetOrCreateValue ("test", s => { throw new InvalidOperationException ("Must not be called."); });
      Assert.That (value, Is.Null);
    }

    [Test]
    public void TryGetValue_ValueFound ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerCacheMock
          .Expect (
              mock => mock.TryGetValue (Arg.Is ("key"), out Arg<DoubleCheckedLockingContainer<Wrapper>>.Out (doubleCheckedLockingContainer).Dummy))
          .Return (true)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      object result;
      var actualResult = _cachingAdapter.TryGetValue ("key", out result);

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (true));

      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    public void TryGetValue_NoValueFound ()
    {
      _innerCacheMock
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<DoubleCheckedLockingContainer<Wrapper>>.Out (null).Dummy))
          .Return (false)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());

      object result;
      var actualResult = _cachingAdapter.TryGetValue ("key", out result);

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (false));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      var expected1 = new object();
      var expected2 = new object();

      var returnValue = ((IEnumerable<KeyValuePair<string, DoubleCheckedLockingContainer<Wrapper>>>)
          new[]
          {
              new KeyValuePair<string, DoubleCheckedLockingContainer<Wrapper>> ("key1", CreateContainerThatChecksForNotProtected (expected1)),
              new KeyValuePair<string, DoubleCheckedLockingContainer<Wrapper>> ("key2", CreateContainerThatChecksForNotProtected (expected2))
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

      var returnValue = ((IEnumerable<KeyValuePair<string, DoubleCheckedLockingContainer<Wrapper>>>)
          new[]
          {
              new KeyValuePair<string, DoubleCheckedLockingContainer<Wrapper>> ("key1", CreateContainerThatChecksForNotProtected (expected1)),
              new KeyValuePair<string, DoubleCheckedLockingContainer<Wrapper>> ("key2", CreateContainerThatChecksForNotProtected (expected2))
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

    private DoubleCheckedLockingContainer<Wrapper> CreateContainerThatChecksForNotProtected (object value)
    {
      return new DoubleCheckedLockingContainer<Wrapper> (() =>
      {
        CheckInnerCacheIsNotProtected ();
        return new Wrapper (value);
      });
    }

    private LockingCacheDecorator<string, DoubleCheckedLockingContainer<Wrapper>> GetLockingCacheDecorator (
        LazyLockingCachingAdapter<string, object> lazyLockingCacheAdapter)
    {
      return (LockingCacheDecorator<string, DoubleCheckedLockingContainer<Wrapper>>)
          PrivateInvoke.GetNonPublicField (lazyLockingCacheAdapter, "_innerCache");
    }
  }
}