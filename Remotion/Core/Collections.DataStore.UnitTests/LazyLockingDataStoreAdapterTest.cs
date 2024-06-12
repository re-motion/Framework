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
using Moq;
using NUnit.Framework;
using Remotion.Development.Moq.UnitTesting.Threading;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Wrapper = Remotion.Collections.DataStore.LazyLockingDataStoreAdapter<string, object>.Wrapper;

namespace Remotion.Collections.DataStore.UnitTests
{
  using InnerFactory = Func<IDataStore<string, Lazy<Wrapper>>, Lazy<Wrapper>>;

  [TestFixture]
  public class LazyLockingDataStoreAdapterTest
  {
    private Mock<IDataStore<string, Lazy<Wrapper>>> _innerDataStoreMock;
    private LazyLockingDataStoreAdapter<string, object> _store;

    [SetUp]
    public void SetUp ()
    {
      _innerDataStoreMock = new Mock<IDataStore<string, Lazy<Wrapper>>>(MockBehavior.Strict);
      _store = new LazyLockingDataStoreAdapter<string, object>(_innerDataStoreMock.Object);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((INullObject)_store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey ()
    {
      var result = BooleanObjectMother.GetRandomBoolean();
      _innerDataStoreMock
          .Setup(mock => mock.ContainsKey("test"))
          .Returns(result)
          .Callback((string key) => CheckInnerDataStoreIsProtected())
          .Verifiable();

      var actualResult = _store.ContainsKey("test");

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(result));
    }

    [Test]
    public void Add ()
    {
      var value = new object();
      _innerDataStoreMock
          .Setup(store => store.Add("key", It.Is<Lazy<Wrapper>>(c => c.Value.Value == value)))
          .Callback((string key, Lazy<Wrapper> value) => CheckInnerDataStoreIsProtected())
          .Verifiable();

      _store.Add("key", value);

      _innerDataStoreMock.Verify();
    }

    [Test]
    public void Remove ()
    {
      bool result = BooleanObjectMother.GetRandomBoolean();
      _innerDataStoreMock
          .Setup(mock => mock.Remove("key"))
          .Returns(result)
          .Callback((string key) => CheckInnerDataStoreIsProtected())
          .Verifiable();

      var actualResult = _store.Remove("key");

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(result));
    }

    [Test]
    public void Clear ()
    {
      _innerDataStoreMock
          .Setup(store => store.Clear())
          .Callback(() => CheckInnerDataStoreIsProtected())
          .Verifiable();

      _store.Clear();

      _innerDataStoreMock.Verify();
    }

    [Test]
    public void GetValue ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected(value);

      _innerDataStoreMock
          .Setup(mock => mock["test"])
          .Returns(doubleCheckedLockingContainer)
          .Callback((string key) => CheckInnerDataStoreIsProtected())
          .Verifiable();

      var actualResult = _store["test"];

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(value));
    }

    [Test]
    public void SetValue ()
    {
      var value = new object();
      _innerDataStoreMock
          .SetupSet(store => store["key"] = It.Is<Lazy<Wrapper>>(c => c.Value.Value == value))
          .Callback(() => CheckInnerDataStoreIsProtected())
          .Verifiable();

      _store["key"] = value;

      _innerDataStoreMock.Verify();
    }

    [Test]
    public void GetValueOrDefault_ValueFound ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected(value);

      _innerDataStoreMock
          .Setup(mock => mock.GetValueOrDefault("test"))
          .Returns(doubleCheckedLockingContainer)
          .Callback((string key) => CheckInnerDataStoreIsProtected())
          .Verifiable();

      var actualResult = _store.GetValueOrDefault("test");

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(value));
    }

    [Test]
    public void GetValueOrDefault_NoValueFound ()
    {
      _innerDataStoreMock
          .Setup(mock => mock.GetValueOrDefault("test"))
          .Returns((Lazy<Wrapper>)null)
          .Callback((string key) => CheckInnerDataStoreIsProtected())
          .Verifiable();

      var actualResult = _store.GetValueOrDefault("test");

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(null));
    }

    [Test]
    public void TryGetValue_ValueFound ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected(value);

      _innerDataStoreMock
          .Setup(mock => mock.TryGetValue("key", out doubleCheckedLockingContainer))
          .Returns(true)
          .Callback(
              (OutDelegate)((string key, out Lazy<Wrapper> value) =>
              {
                value = doubleCheckedLockingContainer;
                CheckInnerDataStoreIsProtected();
              }))
          .Verifiable();

      var actualResult = _store.TryGetValue("key", out var result);

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(true));

      Assert.That(result, Is.SameAs(value));
    }

    [Test]
    public void TryGetValue_NoValueFound ()
    {
      Lazy<Wrapper> outResult = null;

      _innerDataStoreMock
          .Setup(mock => mock.TryGetValue("key", out outResult))
          .Returns(false)
          .Callback(
              (OutDelegate)((string key, out Lazy<Wrapper> value) =>
              {
                value = null;
                CheckInnerDataStoreIsProtected();
              }))
          .Verifiable();

      var actualResult = _store.TryGetValue("key", out var result);

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(false));

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_TryGetValueIsFalse ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected(value);
      Lazy<Wrapper> outResult = null;

      _innerDataStoreMock
          .Setup(mock => mock.TryGetValue("key", out outResult))
          .Returns(false)
          .Callback(
              (OutDelegate)((string key, out Lazy<Wrapper> value) =>
              {
                value = null;
                CheckInnerDataStoreIsProtected();
              }))
          .Verifiable();
      _innerDataStoreMock
          .Setup(mock => mock.GetOrCreateValue(
              "key",
              It.Is<Func<string, Lazy<Wrapper>>>(f => f("Test").Value.Value.Equals("Test123"))))
          .Returns(doubleCheckedLockingContainer)
          .Callback((string key, Func<string, Lazy<Wrapper>> value) => CheckInnerDataStoreIsProtected())
          .Verifiable();

      var actualResult = _store.GetOrCreateValue("key", key => key + "123");

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(value));
    }

    [Test]
    public void GetOrCreateValue_TryGetValueIsTrue ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected(value);

      _innerDataStoreMock
          .Setup(
              mock => mock.TryGetValue("key", out doubleCheckedLockingContainer))
          .Returns(true)
          .Callback(
              (OutDelegate)((string key, out Lazy<Wrapper> value) =>
              {
                value = doubleCheckedLockingContainer;
                CheckInnerDataStoreIsProtected();
              }))
          .Verifiable();

      var actualResult = _store.GetOrCreateValue("key", key => key + "123");

      _innerDataStoreMock.Verify();
      Assert.That(actualResult, Is.EqualTo(value));
    }

    [Test]
    public void GetOrCreateValue_TwiceWithNullValue_DoesNotEvalueValueFactoryTwice ()
    {
      var adapter = new LazyLockingDataStoreAdapter<string, object>(new SimpleDataStore<string, Lazy<Wrapper>>());

      bool wasCalled = false;

      var value = adapter.GetOrCreateValue(
          "test",
          s =>
          {
            Assert.That(wasCalled, Is.False);
            wasCalled = true;
            return null;
          });
      Assert.That(value, Is.Null);

      value = adapter.GetOrCreateValue("test", s => { throw new InvalidOperationException("Must not be called."); });
      Assert.That(value, Is.Null);
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_HasNoNestedValue ()
    {
      object expected = new object();

      var store =  new LazyLockingDataStoreAdapter<string, object>(new SimpleDataStore<string, Lazy<Wrapper>>());

      var actualValue = store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => store.TryGetValue(key, out _),
                Throws.InvalidOperationException);
            return expected;
          });

      Assert.That(actualValue, Is.SameAs(expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      object expected = new object();

      var store =  new LazyLockingDataStoreAdapter<string, object>(new SimpleDataStore<string, Lazy<Wrapper>>());

      var actualValue = store.GetOrCreateValue(
          "key1",
          delegate (string key)
          {
            Assert.That(
                () => store.GetOrCreateValue(key, nestedKey => 13),
                Throws.InvalidOperationException);

            return expected;
          });

      Assert.That(actualValue, Is.EqualTo(expected));

      Assert.That(store.TryGetValue("key1", out var actualValue2), Is.True);
      Assert.That(actualValue2, Is.SameAs(expected));
    }

    private void CheckInnerDataStoreIsProtected ()
    {
      var lockingDataStoreDecorator = GetLockingDataStoreDecorator(_store);
      var lockObject = PrivateInvoke.GetNonPublicField(lockingDataStoreDecorator, "_lock");

      LockTestHelper.CheckLockIsHeld(lockObject);
    }

    private void CheckInnerDataStoreIsNotProtected ()
    {
      var lockingDataStoreDecorator = GetLockingDataStoreDecorator(_store);
      var lockObject = PrivateInvoke.GetNonPublicField(lockingDataStoreDecorator, "_lock");

      LockTestHelper.CheckLockIsNotHeld(lockObject);
    }

    private Lazy<Wrapper> CreateContainerThatChecksForNotProtected (object value)
    {
      return new Lazy<Wrapper>(() =>
      {
        CheckInnerDataStoreIsNotProtected();
        return new Wrapper(value);
      });
    }

    private LockingDataStoreDecorator<string, Lazy<Wrapper>> GetLockingDataStoreDecorator (
        LazyLockingDataStoreAdapter<string, object> lazyLockingDataStoreAdapter)
    {
      return (LockingDataStoreDecorator<string, Lazy<Wrapper>>)PrivateInvoke.GetNonPublicField(lazyLockingDataStoreAdapter, "_innerDataStore");
    }

    private delegate void OutDelegate (string name, out Lazy<Wrapper> outParam);
  }
}
