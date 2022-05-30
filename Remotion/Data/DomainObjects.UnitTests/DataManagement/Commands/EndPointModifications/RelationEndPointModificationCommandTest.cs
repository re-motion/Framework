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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.NUnit;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class RelationEndPointModificationCommandTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;
    private ObjectID _objectID;
    private Order _domainObject;
    private IRelationEndPointDefinition _endPointDefinition;

    private Mock<IRelationEndPoint> _endPointMock;
    private OrderTicket _oldRelatedObject;
    private OrderTicket _newRelatedObject;
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    private RelationEndPointModificationCommand _command;
    private Mock<RelationEndPointModificationCommand> _commandPartialMock;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = new TestableClientTransaction();
      _objectID = DomainObjectIDs.Order1;
      _domainObject = _transaction.ExecuteInScope(() => Order.NewObject());
      _endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");

      _endPointMock = new Mock<IRelationEndPoint>();
      _endPointMock.Setup(mock => mock.ClientTransaction).Returns(_transaction);
      _endPointMock.Setup(mock => mock.ObjectID).Returns(_objectID);
      _endPointMock.Setup(mock => mock.Definition).Returns(_endPointDefinition);
      _endPointMock.Setup(mock => mock.GetDomainObject()).Returns(_domainObject);

      _oldRelatedObject = _transaction.ExecuteInScope(() => OrderTicket.NewObject());
      _newRelatedObject = _transaction.ExecuteInScope(() => OrderTicket.NewObject());

      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);

      _command = CreateTestableCommand();
      _commandPartialMock = CreateCommandPartialMock();
    }

    [Test]
    public void Initialize_WithNullEndPoint ()
    {
      var endPointMock = new Mock<IRelationEndPoint>();
      endPointMock.Setup(_ => _.IsNull).Returns(true);

      Assert.That(
          () => new TestableRelationEndPointModificationCommand(endPointMock.Object, _oldRelatedObject, _newRelatedObject, _transactionEventSinkWithMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Modified end point is null, a NullEndPointModificationCommand is needed.",
              "modifiedEndPoint"));
    }

    [Test]
    public void Initialize_WithNullDomainObjectInEndPoint ()
    {
      var endPointMock = new Mock<IRelationEndPoint>();
      endPointMock.Setup(_ => _.GetDomainObject()).Returns((DomainObject)null);

      Assert.That(
          () => new TestableRelationEndPointModificationCommand(endPointMock.Object, _oldRelatedObject, _newRelatedObject, _transactionEventSinkWithMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "DomainObject of modified end point is null, a NullEndPointModificationCommand is needed.",
              "modifiedEndPoint"));
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That(_command.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void Begin ()
    {
      _commandPartialMock
          .Setup(mock => mock.Begin())
          .CallBase()
          .Verifiable();

      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseRelationChangingEvent(_domainObject, _endPointDefinition, _oldRelatedObject, _newRelatedObject))
          .Verifiable();

      _commandPartialMock.Object.Begin();

      _commandPartialMock.Verify();
      _transactionEventSinkWithMock.Verify();
    }

    [Test]
    public void End ()
    {
      _commandPartialMock
          .Setup(mock => mock.End())
          .CallBase()
          .Verifiable();

      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseRelationChangedEvent(_domainObject, _endPointDefinition, _oldRelatedObject, _newRelatedObject))
          .Verifiable();

      _commandPartialMock.Object.End();

      _commandPartialMock.Verify();
      _transactionEventSinkWithMock.Verify();
    }

    private Mock<RelationEndPointModificationCommand> CreateCommandPartialMock ()
    {
      return new Mock<RelationEndPointModificationCommand>(_endPointMock.Object, _oldRelatedObject, _newRelatedObject, _transactionEventSinkWithMock.Object) { CallBase = true };
    }

    private RelationEndPointModificationCommand CreateTestableCommand ()
    {
      return new TestableRelationEndPointModificationCommand(_endPointMock.Object, _oldRelatedObject, _newRelatedObject, _transactionEventSinkWithMock.Object);
    }
  }
}
