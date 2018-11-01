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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class ObjectReferenceInitializationContextTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private IEnlistedDomainObjectManager _enlistedDomainObjectManagerMock;
    private ClientTransaction _rootTransaction;

    private ObjectReferenceInitializationContext _context;

    private DomainObject _domainObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectID = DomainObjectIDs.Order1;
      _enlistedDomainObjectManagerMock = MockRepository.GenerateStrictMock<IEnlistedDomainObjectManager> ();
      _rootTransaction = ClientTransaction.CreateRootTransaction();

      _context = new ObjectReferenceInitializationContext (_objectID, _rootTransaction, _enlistedDomainObjectManagerMock);

      _domainObject = DomainObjectMother.CreateFakeObject (_objectID);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_context.ObjectID, Is.EqualTo (_objectID));
      Assert.That (_context.EnlistedDomainObjectManager, Is.SameAs (_enlistedDomainObjectManagerMock));
      Assert.That (_context.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_context.RegisteredObject, Is.Null);
    }

    [Test]
    public void Initialization_NonRootTransaction ()
    {
      Assert.That (
          () => new ObjectReferenceInitializationContext (_objectID, _rootTransaction.CreateSubTransaction(), _enlistedDomainObjectManagerMock),
          Throws.ArgumentException.With.Message.EqualTo (
              "The rootTransaction parameter must be passed a root transaction.\r\nParameter name: rootTransaction"));
    }

    [Test]
    public void RegisterObject ()
    {
      _enlistedDomainObjectManagerMock.Expect (mock => mock.EnlistDomainObject (_domainObject));

      _context.RegisterObject (_domainObject);

      _enlistedDomainObjectManagerMock.VerifyAllExpectations();

      Assert.That (_context.RegisteredObject, Is.SameAs (_domainObject));
    }

    [Test]
    public void RegisterObject_Twice ()
    {
      _enlistedDomainObjectManagerMock.Stub (mock => mock.EnlistDomainObject (_domainObject));

      _context.RegisterObject (_domainObject);

      Assert.That (
          () => _context.RegisterObject (_domainObject),
          Throws.InvalidOperationException.With.Message.EqualTo ("Only one object can be registered using this context."));

      _enlistedDomainObjectManagerMock.VerifyAllExpectations ();

      Assert.That (_context.RegisteredObject, Is.SameAs (_domainObject));
    }

    [Test]
    public void RegisterObject_WrongID ()
    {
      var objectWithWrongID = DomainObjectMother.CreateFakeObject (DomainObjectIDs.Customer1);
      Assert.That (objectWithWrongID.ID, Is.Not.EqualTo (_objectID));

      Assert.That (
          () => _context.RegisterObject (objectWithWrongID),
          Throws.ArgumentException.With.Message.EqualTo ("The given DomainObject must have ID '" + _objectID + "'.\r\nParameter name: domainObject"));
    }
  }
}