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
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class DeleteCommandTest : StandardMappingTest
  {
    private TestableClientTransaction _transaction;
    private Order _order1;
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    private DeleteCommand _deleteOrder1Command;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = new TestableClientTransaction();
      _order1 = (Order)LifetimeService.GetObject(_transaction, DomainObjectIDs.Order1, false);
      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);

      _deleteOrder1Command = new DeleteCommand(_transaction, _order1, _transactionEventSinkWithMock.Object);
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That(_deleteOrder1Command.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void Begin ()
    {
      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectDeletingEvent(_order1)).Verifiable();

      _deleteOrder1Command.Begin();

      _transactionEventSinkWithMock.Verify();
    }

    [Test]
    public void Begin_TriggersEndPointModifications ()
    {
      var endPointCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      endPointCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      var sequence = new VerifiableSequence();
      _transactionEventSinkWithMock.InVerifiableSequence(sequence).Setup(mock => mock.RaiseObjectDeletingEvent(_order1)).Verifiable();
      endPointCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable();

      var compositeCommand = (CompositeCommand)PrivateInvoke.GetNonPublicField(_deleteOrder1Command, "_endPointDeleteCommands");
      var compositeCommandWithMockStep = compositeCommand.CombineWith(endPointCommandMock.Object);
      PrivateInvoke.SetNonPublicField(_deleteOrder1Command, "_endPointDeleteCommands", compositeCommandWithMockStep);

      _deleteOrder1Command.Begin();

      endPointCommandMock.Verify();
      endPointCommandMock.Reset(); // For Discard
      sequence.Verify();
    }

    [Test]
    public void End ()
    {
      _transactionEventSinkWithMock.Setup(mock => mock.RaiseObjectDeletedEvent(_order1)).Verifiable();

      _deleteOrder1Command.End();

      _transactionEventSinkWithMock.Verify();
    }

    [Test]
    public void End_TriggersEndPointModifications ()
    {
      var endPointCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      endPointCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      var sequence = new VerifiableSequence();
      endPointCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _transactionEventSinkWithMock.InVerifiableSequence(sequence).Setup(mock => mock.RaiseObjectDeletedEvent(_order1)).Verifiable();

      var compositeCommand = (CompositeCommand)PrivateInvoke.GetNonPublicField(_deleteOrder1Command, "_endPointDeleteCommands");
      var compositeCommandWithMockStep = compositeCommand.CombineWith(endPointCommandMock.Object);
      PrivateInvoke.SetNonPublicField(_deleteOrder1Command, "_endPointDeleteCommands", compositeCommandWithMockStep);

      _deleteOrder1Command.End();

      endPointCommandMock.Verify();
      endPointCommandMock.Reset(); // For Discard
      sequence.Verify();
    }

    [Test]
    public void Perform_PerformsEndPointDelete ()
    {
      _deleteOrder1Command.Perform();

      _transaction.ExecuteInScope(() => Assert.That(_order1.OrderItems, Is.Empty));
    }

    [Test]
    public void Perform_DeletesExistingDataContainer ()
    {
      _deleteOrder1Command.Perform();

      _transaction.ExecuteInScope(() => Assert.That(_order1.State.IsDeleted, Is.True));
    }

    [Test]
    public void Perform_DiscardsNewDataContainer ()
    {
      var newOrder = _transaction.ExecuteInScope(() => Order.NewObject());
      var deleteNewOrderCommand = new DeleteCommand(_transaction, newOrder, _transactionEventSinkWithMock.Object);

      deleteNewOrderCommand.Perform();

      Assert.That(_transaction.IsInvalid(newOrder.ID), Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var extended = _deleteOrder1Command.ExpandToAllRelatedObjects();

      var commands = extended.GetNestedCommands();
      Assert.That(commands.Count, Is.EqualTo(6)); // self, Official, OrderTicket, Customer, OrderItem1, OrderItem2

      Assert.That(commands, Has.Member(_deleteOrder1Command));

      var officialCommand = GetEndPointModificationCommand(commands, DomainObjectIDs.Official1);
      Assert.That(officialCommand, Is.Not.Null);
      Assert.That(officialCommand.OldRelatedObject, Is.SameAs(_order1));
      Assert.That(officialCommand.NewRelatedObject, Is.Null);

      var orderTicketCommand = GetEndPointModificationCommand(commands, DomainObjectIDs.OrderTicket1);
      Assert.That(orderTicketCommand, Is.Not.Null);
      Assert.That(orderTicketCommand.OldRelatedObject, Is.SameAs(_order1));
      Assert.That(orderTicketCommand.NewRelatedObject, Is.Null);

      var customerCommand = GetEndPointModificationCommand(commands, DomainObjectIDs.Customer1);
      Assert.That(customerCommand, Is.Not.Null);
      Assert.That(customerCommand.OldRelatedObject, Is.SameAs(_order1));

      var orderItem1Command = GetEndPointModificationCommand(commands, DomainObjectIDs.OrderItem1);
      Assert.That(orderItem1Command, Is.Not.Null);
      Assert.That(orderItem1Command.OldRelatedObject, Is.SameAs(_order1));
      Assert.That(orderItem1Command.NewRelatedObject, Is.Null);

      var orderItem2Command = GetEndPointModificationCommand(commands, DomainObjectIDs.OrderItem2);
      Assert.That(orderItem2Command, Is.Not.Null);
      Assert.That(orderItem2Command.OldRelatedObject, Is.SameAs(_order1));
      Assert.That(orderItem2Command.NewRelatedObject, Is.Null);
    }

    [Test]
    public void ExpandToAllRelatedObjects_WithSelfReferencingEndPoint ()
    {
      var folder = (Folder)LifetimeService.NewObject(_transaction, typeof(Folder), ParamList.Empty);
      folder.ParentFolder = folder;

      var deleteFolderCommand = new DeleteCommand(_transaction, folder, _transactionEventSinkWithMock.Object);
      var extended = deleteFolderCommand.ExpandToAllRelatedObjects();

      var commands = extended.GetNestedCommands();
      Assert.That(commands.Count, Is.EqualTo(1)); // self

      Assert.That(commands, Has.Member(deleteFolderCommand));
    }

    [Test]
    public void ExpandToAllRelatedObjects_WithSelfReferencingEndPoint_AndOtherEndPoints ()
    {
      var folder = (Folder)LifetimeService.NewObject(_transaction, typeof(Folder), ParamList.Empty);
      folder.ParentFolder = folder;

      var file = (File)LifetimeService.NewObject(_transaction, typeof(File), ParamList.Empty);
      file.ParentFolder = folder;

      var deleteFolderCommand = new DeleteCommand(_transaction, folder, _transactionEventSinkWithMock.Object);
      var extended = deleteFolderCommand.ExpandToAllRelatedObjects();

      var commands = extended.GetNestedCommands();
      Assert.That(commands.Count, Is.EqualTo(2)); // self, File

      Assert.That(commands, Has.Member(deleteFolderCommand));

      var fileCommand = GetEndPointModificationCommand(commands, file.ID);
      Assert.That(fileCommand, Is.Not.Null);
      Assert.That(fileCommand.DomainObject, Is.SameAs(file));
      Assert.That(fileCommand.OldRelatedObject, Is.SameAs(folder));
      Assert.That(fileCommand.NewRelatedObject, Is.Null);
    }

    private RelationEndPointModificationCommand GetEndPointModificationCommand (IEnumerable<IDataManagementCommand> commands, ObjectID objectID)
    {
      return commands
          .Select(UnwrapCommand)
          .OfType<RelationEndPointModificationCommand>()
          .SingleOrDefault(cmd => cmd.DomainObject.ID == objectID);
    }

    private IDataManagementCommand UnwrapCommand (IDataManagementCommand c)
    {
      if (c is RealObjectEndPointRegistrationCommandDecorator)
        return ((RealObjectEndPointRegistrationCommandDecorator)c).DecoratedCommand;
      else if (c is VirtualEndPointStateUpdatedRaisingCommandDecorator)
        return ((VirtualEndPointStateUpdatedRaisingCommandDecorator)c).DecoratedCommand;
      else
        return c;
    }
  }
}
