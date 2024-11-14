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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataProviderTest : StandardMappingTest
  {
    private Mock<ILoadedDataContainerProvider> _loadedDataContainerProviderMock;
    private Mock<IInvalidDomainObjectManager> _invalidDomainObjectManagerMock;

    private LoadedObjectDataProvider _dataProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _loadedDataContainerProviderMock = new Mock<ILoadedDataContainerProvider>(MockBehavior.Strict);
      _invalidDomainObjectManagerMock = new Mock<IInvalidDomainObjectManager>(MockBehavior.Strict);
      _dataProvider = new LoadedObjectDataProvider(_loadedDataContainerProviderMock.Object, _invalidDomainObjectManagerMock.Object);
    }

    [Test]
    public void GetLoadedObject_Known ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject(DomainObjectMother.CreateFakeObject<Order>(dataContainer.ID));
      DataContainerTestHelper.SetClientTransaction(dataContainer, ClientTransaction.CreateRootTransaction());

      _invalidDomainObjectManagerMock
          .Setup(mock => mock.IsInvalid(DomainObjectIDs.Order1))
          .Returns(false);

      _loadedDataContainerProviderMock
          .Setup(mock => mock.GetDataContainerWithoutLoading(DomainObjectIDs.Order1))
          .Returns(dataContainer)
          .Verifiable();

      var loadedObject = _dataProvider.GetLoadedObject(DomainObjectIDs.Order1);

      _loadedDataContainerProviderMock.Verify();
      Assert.That(
          loadedObject,
          Is.TypeOf<AlreadyExistingLoadedObjectData>()
            .With.Property((AlreadyExistingLoadedObjectData obj) => obj.ExistingDataContainer).SameAs(dataContainer));
    }

    [Test]
    public void GetLoadedObject_Unknown ()
    {
      _invalidDomainObjectManagerMock
          .Setup(mock => mock.IsInvalid(DomainObjectIDs.Order1))
          .Returns(false);

      _loadedDataContainerProviderMock
          .Setup(mock => mock.GetDataContainerWithoutLoading(DomainObjectIDs.Order1))
          .Returns((DataContainer)null)
          .Verifiable();

      var loadedObject = _dataProvider.GetLoadedObject(DomainObjectIDs.Order1);

      _loadedDataContainerProviderMock.Verify();
      Assert.That(loadedObject, Is.Null);
    }

    [Test]
    public void GetLoadedObject_Invalid ()
    {
      var invalidObjectReference = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);

      _invalidDomainObjectManagerMock
          .Setup(mock => mock.IsInvalid(DomainObjectIDs.Order1))
          .Returns(true)
          .Verifiable();
      _invalidDomainObjectManagerMock
          .Setup(mock => mock.GetInvalidObjectReference(DomainObjectIDs.Order1))
          .Returns(invalidObjectReference)
          .Verifiable();

      var loadedObject = _dataProvider.GetLoadedObject(DomainObjectIDs.Order1);

      _loadedDataContainerProviderMock.Verify(mock => mock.GetDataContainerWithoutLoading(It.IsAny<ObjectID>()), Times.Never());
      _invalidDomainObjectManagerMock.Verify();

      Assert.That(
          loadedObject,
          Is.TypeOf<InvalidLoadedObjectData>().With.Property((InvalidLoadedObjectData obj) => obj.InvalidObjectReference).SameAs(invalidObjectReference));
    }
  }
}
