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
using NUnit.Framework;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;
using Wrapper = Remotion.Collections.DataStore.LazyLockingDataStoreAdapter<string, object>.Wrapper;

namespace Remotion.Collections.DataStore.UnitTests
{
  using InnerFactory = Func<IDataStore<string, Lazy<Wrapper>>, Lazy<Wrapper>>;

  [TestFixture]
  public class LazyLockingDataStoreAdapterTest
  {
    private IDataStore<string, Lazy<Wrapper>> _innerDataStoreMock;
    private LazyLockingDataStoreAdapter<string, object> _store;

    [SetUp]
    public void SetUp ()
    {
      _innerDataStoreMock = MockRepository.GenerateStrictMock<IDataStore<string, Lazy<Wrapper>>>();
      _store = new LazyLockingDataStoreAdapter<string, object> (_innerDataStoreMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey ()
    {
      var result = BooleanObjectMother.GetRandomBoolean();
      _innerDataStoreMock
          .Expect (mock => mock.ContainsKey ("test"))
          .Return (result)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());

      var actualResult = _store.ContainsKey ("test");

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (result));
    }

    [Test]
    public void Add ()
    {
      var value = new object ();
      _innerDataStoreMock
          .Expect (store => store.Add (Arg.Is ("key"), Arg<Lazy<Wrapper>>.Matches (c => c.Value.Value == value)))
          .WhenCalled (mi => CheckInnerDataStoreIsProtected ());

      _store.Add ("key", value);

      _innerDataStoreMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove ()
    {
      bool result = BooleanObjectMother.GetRandomBoolean();
      _innerDataStoreMock
          .Expect (mock => mock.Remove ("key"))
          .Return (result)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());

      var actualResult = _store.Remove ("key");

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (result));
    }

    [Test]
    public void Clear ()
    {
      _innerDataStoreMock
          .Expect (store => store.Clear())
          .WhenCalled (mi => CheckInnerDataStoreIsProtected ());

      _store.Clear();

      _innerDataStoreMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetValue ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);
      
      _innerDataStoreMock
          .Expect (mock => ((InnerFactory) (store => store["test"])) (mock))
          .Return (doubleCheckedLockingContainer)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());

      var actualResult = _store["test"];

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (value));
    }

    [Test]
    public void SetValue ()
    {
      var value = new object ();
      _innerDataStoreMock
          .Expect (store => store[Arg.Is ("key")] = Arg<Lazy<Wrapper>>.Matches (c => c.Value.Value == value))
          .WhenCalled (mi => CheckInnerDataStoreIsProtected ());

      _store["key"] = value;

      _innerDataStoreMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetValueOrDefault_ValueFound ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (mock => ((InnerFactory) (store => store.GetValueOrDefault ("test"))) (mock))
          .Return (doubleCheckedLockingContainer)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());

      var actualResult = _store.GetValueOrDefault ("test");

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (value));
    }

    [Test]
    public void GetValueOrDefault_NoValueFound ()
    {
      _innerDataStoreMock
          .Expect (mock => ((InnerFactory) (store => store.GetValueOrDefault ("test"))) (mock))
          .Return (null)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());

      var actualResult = _store.GetValueOrDefault ("test");

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (null));
    }

    [Test]
    public void TryGetValue_ValueFound ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>>.Out (doubleCheckedLockingContainer).Dummy))
          .Return (true)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());

      var actualResult = _store.TryGetValue ("key", out var result);

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (true));

      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    public void TryGetValue_NoValueFound ()
    {
      _innerDataStoreMock
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>>.Out (null).Dummy))
          .Return (false)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());

      var actualResult = _store.TryGetValue ("key", out var result);

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (false));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_TryGetValueIsFalse ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>>.Out (null).Dummy))
          .Return (false)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());
      _innerDataStoreMock
          .Expect (mock => ((InnerFactory) (store => store.GetOrCreateValue (
              Arg.Is ("key"),
              Arg<Func<string, Lazy<Wrapper>>>.Matches (f => f ("Test").Value.Value.Equals ("Test123"))))) (mock))
          .Return (doubleCheckedLockingContainer)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected ());

      var actualResult = _store.GetOrCreateValue ("key", key => key + "123");

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (value));
    }

    [Test]
    public void GetOrCreateValue_TryGetValueIsTrue ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (
              mock => mock.TryGetValue (Arg.Is ("key"), out Arg<Lazy<Wrapper>>.Out (doubleCheckedLockingContainer).Dummy))
          .Return (true)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected ());

      var actualResult = _store.GetOrCreateValue ("key", key => key + "123");

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (value));
    }

    [Test]
    public void GetOrCreateValue_TwiceWithNullValue_DoesNotEvalueValueFactoryTwice ()
    {
      var adapter = new LazyLockingDataStoreAdapter<string, object> (new SimpleDataStore<string, Lazy<Wrapper>>());

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
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (
          new LazyLockingDataStoreAdapter<string, object> (
              new SimpleDataStore<string, Lazy<Wrapper>>()));
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_HasNoNestedValue ()
    {
      object expected = new object();

      var store =  new LazyLockingDataStoreAdapter<string, object> (new SimpleDataStore<string, Lazy<Wrapper>>());

      var actualValue = store.GetOrCreateValue (
          "key1",
          delegate (string key)
          {
            Assert.That (
                () => store.TryGetValue (key, out _),
                Throws.InvalidOperationException);
            return expected;
          });

      Assert.That (actualValue, Is.SameAs (expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      object expected = new object();

      var store =  new LazyLockingDataStoreAdapter<string, object> (new SimpleDataStore<string, Lazy<Wrapper>>());

      var actualValue = store.GetOrCreateValue (
          "key1",
          delegate (string key)
          {
            Assert.That (
                () => store.GetOrCreateValue (key, nestedKey => 13),
                Throws.InvalidOperationException);

            return expected;
          });

      Assert.That (actualValue, Is.EqualTo (expected));

      Assert.That (store.TryGetValue ("key1", out var actualValue2), Is.True);
      Assert.That (actualValue2, Is.SameAs (expected));
    }

    private void CheckInnerDataStoreIsProtected ()
    {
      var lockingDataStoreDecorator = GetLockingDataStoreDecorator (_store);
      var lockObject = PrivateInvoke.GetNonPublicField (lockingDataStoreDecorator, "_lock");

      LockTestHelper.CheckLockIsHeld (lockObject);
    }

    private void CheckInnerDataStoreIsNotProtected ()
    {
      var lockingDataStoreDecorator = GetLockingDataStoreDecorator (_store);
      var lockObject = PrivateInvoke.GetNonPublicField (lockingDataStoreDecorator, "_lock");

      LockTestHelper.CheckLockIsNotHeld (lockObject);
    }

    private Lazy<Wrapper> CreateContainerThatChecksForNotProtected (object value)
    {
      return new Lazy<Wrapper> (() =>
      {
        CheckInnerDataStoreIsNotProtected();
        return new Wrapper (value);
      });
    }

    private LockingDataStoreDecorator<string, Lazy<Wrapper>> GetLockingDataStoreDecorator (
        LazyLockingDataStoreAdapter<string, object> lazyLockingDataStoreAdapter)
    {
      return (LockingDataStoreDecorator<string, Lazy<Wrapper>>) 
          PrivateInvoke.GetNonPublicField (lazyLockingDataStoreAdapter, "_innerDataStore");
    }
  }
}