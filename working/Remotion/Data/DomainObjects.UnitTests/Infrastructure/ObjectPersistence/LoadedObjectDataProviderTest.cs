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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataProviderTest : StandardMappingTest
  {
    private ILoadedDataContainerProvider _loadedDataContainerProviderMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;

    private LoadedObjectDataProvider _dataProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedDataContainerProviderMock = MockRepository.GenerateStrictMock<ILoadedDataContainerProvider> ();
      _invalidDomainObjectManagerMock = MockRepository.GenerateStrictMock<IInvalidDomainObjectManager>();
      _dataProvider = new LoadedObjectDataProvider (_loadedDataContainerProviderMock, _invalidDomainObjectManagerMock);
    }

    [Test]
    public void GetLoadedObject_Known ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<Order> (dataContainer.ID));
      DataContainerTestHelper.SetClientTransaction (dataContainer, ClientTransaction.CreateRootTransaction());

      _invalidDomainObjectManagerMock
          .Stub (mock => mock.IsInvalid (DomainObjectIDs.Order1))
          .Return (false);
      _invalidDomainObjectManagerMock.Replay ();

      _loadedDataContainerProviderMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (DomainObjectIDs.Order1))
          .Return (dataContainer);
      _loadedDataContainerProviderMock.Replay ();

      var loadedObject = _dataProvider.GetLoadedObject (DomainObjectIDs.Order1);

      _loadedDataContainerProviderMock.VerifyAllExpectations();
      Assert.That (
          loadedObject, 
          Is.TypeOf<AlreadyExistingLoadedObjectData> ()
            .With.Property ((AlreadyExistingLoadedObjectData obj) => obj.ExistingDataContainer).SameAs (dataContainer));
    }

    [Test]
    public void GetLoadedObject_Unknown ()
    {
      _invalidDomainObjectManagerMock
        .Stub (mock => mock.IsInvalid (DomainObjectIDs.Order1))
        .Return (false);
      _invalidDomainObjectManagerMock.Replay ();

      _loadedDataContainerProviderMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (DomainObjectIDs.Order1))
          .Return (null);
      _loadedDataContainerProviderMock.Replay ();

      var loadedObject = _dataProvider.GetLoadedObject (DomainObjectIDs.Order1);

      _loadedDataContainerProviderMock.VerifyAllExpectations ();
      Assert.That (loadedObject, Is.Null);
    }

    [Test]
    public void GetLoadedObject_Invalid ()
    {
      var invalidObjectReference = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);

      _invalidDomainObjectManagerMock
          .Expect (mock => mock.IsInvalid (DomainObjectIDs.Order1))
          .Return (true);
      _invalidDomainObjectManagerMock
          .Expect (mock => mock.GetInvalidObjectReference (DomainObjectIDs.Order1))
          .Return (invalidObjectReference);
      _invalidDomainObjectManagerMock.Replay();
      _loadedDataContainerProviderMock.Replay();

      var loadedObject = _dataProvider.GetLoadedObject (DomainObjectIDs.Order1);

      _loadedDataContainerProviderMock.AssertWasNotCalled (mock => mock.GetDataContainerWithoutLoading (Arg<ObjectID>.Is.Anything));
      _invalidDomainObjectManagerMock.VerifyAllExpectations();

      Assert.That (
          loadedObject, 
          Is.TypeOf<InvalidLoadedObjectData>().With.Property ((InvalidLoadedObjectData obj) => obj.InvalidObjectReference).SameAs (invalidObjectReference));
    }

    [Test]
    public void Serializable ()
    {
      var provider = new LoadedObjectDataProvider (
        new SerializableLoadedDataContainerProviderFake(),
        new SerializableInvalidDomainObjectManagerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (provider);

      Assert.That (deserializedInstance.LoadedDataContainerProvider, Is.Not.Null);
      Assert.That (deserializedInstance.InvalidDomainObjectManager, Is.Not.Null);
    }
  }
}