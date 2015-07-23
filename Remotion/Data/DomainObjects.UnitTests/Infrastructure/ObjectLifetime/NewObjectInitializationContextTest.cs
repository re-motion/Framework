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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class NewObjectInitializationContextTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private ClientTransaction _rootTransaction;
    private IEnlistedDomainObjectManager _enlistedDomainObjectManagerMock;
    private IDataManager _dataManagerMock;

    private NewObjectInitializationContext _context;

    private DomainObject _domainObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectID = DomainObjectIDs.Order1;
      _enlistedDomainObjectManagerMock = MockRepository.GenerateStrictMock<IEnlistedDomainObjectManager> ();
      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager> ();
      _rootTransaction = ClientTransaction.CreateRootTransaction();

      _context = new NewObjectInitializationContext (_objectID, _rootTransaction, _enlistedDomainObjectManagerMock, _dataManagerMock);

      _domainObject = DomainObjectMother.CreateFakeObject (_objectID);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_context.ObjectID, Is.EqualTo (_objectID));
      Assert.That (_context.EnlistedDomainObjectManager, Is.SameAs (_enlistedDomainObjectManagerMock));
      Assert.That (_context.DataManager, Is.SameAs (_dataManagerMock));
      Assert.That (_context.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_context.RegisteredObject, Is.Null);
    }

    [Test]
    public void RegisterObject ()
    {
      var counter = new OrderedExpectationCounter();
      _enlistedDomainObjectManagerMock
          .Expect (mock => mock.EnlistDomainObject (_domainObject))
          .Ordered (counter);
      StubEmptyDataContainersCollection (_dataManagerMock);
      _dataManagerMock
          .Expect (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything))
          .WhenCalled (mi =>
          {
            var dc = (DataContainer) mi.Arguments[0];
            Assert.That (dc.ID, Is.EqualTo (_objectID));
            Assert.That (dc.DomainObject, Is.SameAs (_domainObject));
          })
          .Ordered (counter);

      _context.RegisterObject (_domainObject);

      _enlistedDomainObjectManagerMock.VerifyAllExpectations();
      _dataManagerMock.VerifyAllExpectations();

      Assert.That (_context.RegisteredObject, Is.SameAs (_domainObject));
    }
    
    [Test]
    public void RegisterObject_WithError ()
    {
      var objectWithWrongID = DomainObjectMother.CreateFakeObject (DomainObjectIDs.Customer1);
      Assert.That (objectWithWrongID.ID, Is.Not.EqualTo (_objectID));

      Assert.That (
          () => _context.RegisterObject (objectWithWrongID),
          Throws.ArgumentException.With.Message.EqualTo ("The given DomainObject must have ID '" + _objectID + "'.\r\nParameter name: domainObject"));

      _dataManagerMock.AssertWasNotCalled (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything));
    }

    private void StubEmptyDataContainersCollection (IDataManager dataManagerMock)
    {
      dataManagerMock.Stub (stub => stub.DataContainers).Return (MockRepository.GenerateStub<IDataContainerMapReadOnlyView> ());
    }
  }
}