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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class RelationEndPointModificationCommandTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;
    private ObjectID _objectID;
    private Order _domainObject;
    private IRelationEndPointDefinition _endPointDefinition;
    
    private IRelationEndPoint _endPointMock;
    private OrderTicket _oldRelatedObject;
    private OrderTicket _newRelatedObject;
    private IClientTransactionEventSink _transactionEventSinkWithMock;

    private RelationEndPointModificationCommand _command;
    private RelationEndPointModificationCommand _commandPartialMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new TestableClientTransaction ();
      _objectID = DomainObjectIDs.Order1;
      _domainObject = _transaction.ExecuteInScope (() => Order.NewObject ());
      _endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      
      _endPointMock = MockRepository.GenerateMock<IRelationEndPoint> ();
      _endPointMock.Stub (mock => mock.ClientTransaction).Return (_transaction);
      _endPointMock.Stub (mock => mock.ObjectID).Return (_objectID);
      _endPointMock.Stub (mock => mock.Definition).Return (_endPointDefinition);
      _endPointMock.Stub (mock => mock.GetDomainObject()).Return (_domainObject);

      _oldRelatedObject = _transaction.ExecuteInScope (() => OrderTicket.NewObject ());
      _newRelatedObject = _transaction.ExecuteInScope (() => OrderTicket.NewObject ());

      _transactionEventSinkWithMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();

      _command = CreateTestableCommand ();
      _commandPartialMock = CreateCommandPartialMock ();
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions (), Is.Empty);
    }

    [Test]
    public void Begin ()
    {
      _commandPartialMock
          .Expect (mock => mock.Begin())
         .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      _commandPartialMock.Replay ();

      _transactionEventSinkWithMock
          .Expect (mock => mock.RaiseRelationChangingEvent (_domainObject, _endPointDefinition, _oldRelatedObject, _newRelatedObject));
      _transactionEventSinkWithMock.Replay();

      _commandPartialMock.Begin ();

      _commandPartialMock.VerifyAllExpectations ();
      _transactionEventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void End ()
    {
      _commandPartialMock
          .Expect (mock => mock.End())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      _commandPartialMock.Replay ();

      _transactionEventSinkWithMock
          .Expect (mock => mock.RaiseRelationChangedEvent (_domainObject, _endPointDefinition, _oldRelatedObject, _newRelatedObject));
      _transactionEventSinkWithMock.Replay();

      _commandPartialMock.End ();

      _commandPartialMock.VerifyAllExpectations ();
      _transactionEventSinkWithMock.VerifyAllExpectations();
    }

    private RelationEndPointModificationCommand CreateCommandPartialMock ()
    {
      return MockRepository.GeneratePartialMock<RelationEndPointModificationCommand> (_endPointMock, _oldRelatedObject, _newRelatedObject, _transactionEventSinkWithMock);
    }

    private RelationEndPointModificationCommand CreateTestableCommand ()
    {
      return new TestableRelationEndPointModificationCommand (_endPointMock, _oldRelatedObject, _newRelatedObject, _transactionEventSinkWithMock);
    }
  }
}