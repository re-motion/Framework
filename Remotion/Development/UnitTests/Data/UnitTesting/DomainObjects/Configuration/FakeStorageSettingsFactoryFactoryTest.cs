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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Development.Data.UnitTesting.DomainObjects.Configuration;
using Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.Configuration
{
  [TestFixture]
  public class FakeStorageObjectFactoryFactoryTest
  {
    [Test]
    public void Create_WithoutSetUp_ThrowsInvalidOperationException ()
    {
      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();

      Assert.That(
          () => fakeStorageObjectFactoryFactory.Create(typeof(StorageObjectFactoryStub)),
          Throws.InvalidOperationException.With.Message.EqualTo("FakeStorageObjectFactoryFactory.SetUp(...) must be called before performing the current operation."));
    }

    [Test]
    public void Create_WithRequestedTypeMatchingSetUpType_ReturnsValueFromSetUp ()
    {
      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();

      var storageObjectFactoryStub = new StorageObjectFactoryStub();
      fakeStorageObjectFactoryFactory.SetUp(storageObjectFactoryStub);

      Assert.That(fakeStorageObjectFactoryFactory.Create(typeof(StorageObjectFactoryStub)), Is.SameAs(storageObjectFactoryStub));
    }

    [Test]
    public void Create_WithRequestedTypeAsBaseTypeOfSetUpType_ReturnsValueFromSetUp ()
    {
      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();

      var storageObjectFactoryStub = new StorageObjectFactoryStub();
      fakeStorageObjectFactoryFactory.SetUp(storageObjectFactoryStub);

      Assert.That(fakeStorageObjectFactoryFactory.Create(typeof(IRdbmsStorageObjectFactory)), Is.SameAs(storageObjectFactoryStub));
    }

    [Test]
    public void Create_WithRequestedTypeNotAssignableFromsSetUpType_ThrowsInvalidOperationException ()
    {
      var fakeStorageObjectFactoryFactory = new FakeStorageObjectFactoryFactory();

      var storageObjectFactoryStub = new StorageObjectFactoryStub();
      fakeStorageObjectFactoryFactory.SetUp(storageObjectFactoryStub);

      Assert.That(
          () => fakeStorageObjectFactoryFactory.Create(typeof(INonPersistentStorageObjectFactory)),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
              $"This FakeStorageObjectFactoryFactory is set up to return an instance of type 'Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain.StorageObjectFactoryStub', "
              + $"which is not compatible with the requested type 'Remotion.Data.DomainObjects.Persistence.NonPersistent.INonPersistentStorageObjectFactory'."));
    }
  }
}
