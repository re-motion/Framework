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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Collections.DataStore.UnitTests
{
  [TestFixture]
  public class ExpiringDataStoreFactoryTest
  {
    private StringComparer _comparer;
    private Mock<IExpirationPolicy<object, DateTime, DateTime>> _expirationPolicy;

    [SetUp]
    public void SetUp ()
    {
      _comparer = StringComparer.InvariantCultureIgnoreCase;
      _expirationPolicy = new Mock<IExpirationPolicy<object, DateTime, DateTime>>();
    }

    [Test]
    public void Create ()
    {
      var result = ExpiringDataStoreFactory.Create(_expirationPolicy.Object, _comparer);

      Assert.That(result, Is.TypeOf(typeof(ExpiringDataStore<string, object, DateTime, DateTime>)));
      var expirationPolicy = PrivateInvoke.GetNonPublicField(result, "_expirationPolicy");
      Assert.That(expirationPolicy, Is.SameAs(_expirationPolicy.Object));
      var innerDataStore = PrivateInvoke.GetNonPublicField(result, "_innerDataStore");
      Assert.That(innerDataStore, Is.TypeOf(typeof(SimpleDataStore<string, Tuple<object, DateTime>>)));
      Assert.That(((SimpleDataStore<string, Tuple<object, DateTime>>)innerDataStore).Comparer, Is.SameAs(_comparer));
    }

#pragma warning disable 618
    [Test]
    public void CreateWithLocking ()
    {
      var result = ExpiringDataStoreFactory.CreateWithLocking(_expirationPolicy.Object, _comparer);

      Assert.That(result, Is.TypeOf(typeof(LockingDataStoreDecorator<string, object>)));
      var innerStore = PrivateInvoke.GetNonPublicField(result, "_innerStore");
      Assert.That(innerStore, Is.TypeOf(typeof(ExpiringDataStore<string, object, DateTime, DateTime>)));
      var expirationPolicy = PrivateInvoke.GetNonPublicField(innerStore, "_expirationPolicy");
      Assert.That(expirationPolicy, Is.SameAs(_expirationPolicy.Object));
      var underlyingDataStore = PrivateInvoke.GetNonPublicField(innerStore, "_innerDataStore");
      Assert.That(underlyingDataStore, Is.TypeOf(typeof(SimpleDataStore<string, Tuple<object, DateTime>>)));
      Assert.That(((SimpleDataStore<string, Tuple<object, DateTime>>)underlyingDataStore).Comparer, Is.SameAs(_comparer));
    }

    [Test]
    public void CreateWithLazyLocking ()
    {
      var policy = new Mock<IExpirationPolicy<Lazy<LazyLockingDataStoreAdapter<string, object>.Wrapper>, DateTime, DateTime>>();

      var result = ExpiringDataStoreFactory.CreateWithLazyLocking(policy.Object,  _comparer);

      Assert.That(result, Is.TypeOf(typeof(LazyLockingDataStoreAdapter<string,  object>)));
      var innerDataStore = PrivateInvoke.GetNonPublicField(result, "_innerDataStore");
      Assert.That(innerDataStore, Is.TypeOf(typeof(LockingDataStoreDecorator<string, Lazy<LazyLockingDataStoreAdapter<string, object>.Wrapper>>)));
      var innerStore = PrivateInvoke.GetNonPublicField(innerDataStore, "_innerStore");
      Assert.That(innerStore, Is.TypeOf(typeof(ExpiringDataStore<string, Lazy<LazyLockingDataStoreAdapter<string, object>.Wrapper>, DateTime, DateTime>)));
      var expirationPolicy = PrivateInvoke.GetNonPublicField(innerStore, "_expirationPolicy");
      Assert.That(expirationPolicy, Is.SameAs(expirationPolicy));
      var innerInnerDataStore = PrivateInvoke.GetNonPublicField(innerStore, "_innerDataStore");
      Assert.That(innerInnerDataStore, Is.TypeOf(typeof(SimpleDataStore<string, Tuple<Lazy<LazyLockingDataStoreAdapter<string, object>.Wrapper>, DateTime>>)));
      Assert.That(((SimpleDataStore<string, Tuple<Lazy<LazyLockingDataStoreAdapter<string, object>.Wrapper>, DateTime>>)innerInnerDataStore).Comparer, Is.SameAs(_comparer));
    }
#pragma warning restore 618
  }
}
