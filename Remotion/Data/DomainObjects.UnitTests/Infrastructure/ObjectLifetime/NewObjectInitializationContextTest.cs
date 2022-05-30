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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Development.UnitTesting.NUnit;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class NewObjectInitializationContextTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private ClientTransaction _rootTransaction;
    private Mock<IEnlistedDomainObjectManager> _enlistedDomainObjectManagerMock;
    private Mock<IDataManager> _dataManagerMock;

    private NewObjectInitializationContext _context;

    private DomainObject _domainObject;

    public override void SetUp ()
    {
      base.SetUp();

      _objectID = DomainObjectIDs.Order1;
      _enlistedDomainObjectManagerMock = new Mock<IEnlistedDomainObjectManager>(MockBehavior.Strict);
      _dataManagerMock = new Mock<IDataManager>(MockBehavior.Strict);
      _rootTransaction = ClientTransaction.CreateRootTransaction();

      _context = new NewObjectInitializationContext(_objectID, _rootTransaction, _enlistedDomainObjectManagerMock.Object, _dataManagerMock.Object);

      _domainObject = DomainObjectMother.CreateFakeObject(_objectID);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_context.ObjectID, Is.EqualTo(_objectID));
      Assert.That(_context.EnlistedDomainObjectManager, Is.SameAs(_enlistedDomainObjectManagerMock.Object));
      Assert.That(_context.DataManager, Is.SameAs(_dataManagerMock.Object));
      Assert.That(_context.RootTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_context.RegisteredObject, Is.Null);
    }

    [Test]
    public void RegisterObject ()
    {
      var sequence = new MockSequence();
      _enlistedDomainObjectManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.EnlistDomainObject(_domainObject))
          .Verifiable();
      StubEmptyDataContainersCollection(_dataManagerMock);
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()))
          .Callback(
              (DataContainer dataContainer) =>
              {
                Assert.That(dataContainer.ID, Is.EqualTo(_objectID));
                Assert.That(dataContainer.DomainObject, Is.SameAs(_domainObject));
              })
          .Verifiable();

      _context.RegisterObject(_domainObject);

      _enlistedDomainObjectManagerMock.Verify();
      _dataManagerMock.Verify();

      Assert.That(_context.RegisteredObject, Is.SameAs(_domainObject));
    }

    [Test]
    public void RegisterObject_WithError ()
    {
      var objectWithWrongID = DomainObjectMother.CreateFakeObject(DomainObjectIDs.Customer1);
      Assert.That(objectWithWrongID.ID, Is.Not.EqualTo(_objectID));

      Assert.That(
          () => _context.RegisterObject(objectWithWrongID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("The given DomainObject must have ID '" + _objectID + "'.", "domainObject"));

      _dataManagerMock.Verify(mock => mock.RegisterDataContainer(It.IsAny<DataContainer>()), Times.Never());
    }

    private void StubEmptyDataContainersCollection (Mock<IDataManager> dataManagerMock)
    {
      dataManagerMock.Setup(stub => stub.DataContainers).Returns(new Mock<IDataContainerMapReadOnlyView>().Object);
    }
  }
}
