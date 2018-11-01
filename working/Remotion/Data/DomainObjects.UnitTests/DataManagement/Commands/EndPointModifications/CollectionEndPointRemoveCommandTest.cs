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
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointRemoveCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private Order _removedRelatedObject;
    private CollectionEndPointRemoveCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _removedRelatedObject = DomainObjectIDs.Order1.GetObject<Order> (Transaction);

      _command = new CollectionEndPointRemoveCommand (
          CollectionEndPoint, _removedRelatedObject, CollectionDataMock, EndPointProviderStub, TransactionEventSinkMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.SameAs (_removedRelatedObject));
      Assert.That (_command.NewRelatedObject, Is.Null);
      Assert.That (_command.ModifiedCollection, Is.SameAs (CollectionEndPoint.Collection));
      Assert.That (_command.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (Transaction, RelationEndPointID.Definition);
      Dev.Null = 
          new CollectionEndPointRemoveCommand (endPoint, _removedRelatedObject, CollectionDataMock, EndPointProviderStub, TransactionEventSinkMock);
    }

    [Test]
    public void Begin ()
    {
      var counter = new OrderedExpectationCounter ();
      CollectionMockEventReceiver
          .Expect (mock => mock.Removing (_removedRelatedObject))
          .WhenCalledOrdered (counter, mi => Assert.That (ClientTransaction.Current, Is.SameAs (Transaction)));
      TransactionEventSinkMock
          .Expect (mock => mock.RaiseRelationChangingEvent (DomainObject, CollectionEndPoint.Definition, _removedRelatedObject, null))
          .Ordered (counter);

      _command.Begin ();

      CollectionMockEventReceiver.VerifyAllExpectations();
      TransactionEventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void End ()
    {
      var counter = new OrderedExpectationCounter ();
      TransactionEventSinkMock
          .Expect (mock => mock.RaiseRelationChangedEvent (DomainObject, CollectionEndPoint.Definition, _removedRelatedObject, null))
          .Ordered (counter);
      CollectionMockEventReceiver
          .Expect (mock => mock.Removed (_removedRelatedObject))
          .WhenCalledOrdered (counter, mi => Assert.That (ClientTransaction.Current, Is.SameAs (Transaction)));
      
      _command.End ();

      TransactionEventSinkMock.VerifyAllExpectations();
      CollectionMockEventReceiver.VerifyAllExpectations();
    }

    [Test]
    public void Perform ()
    {
      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Expect (mock => mock.Remove (_removedRelatedObject)).Return (true);
      CollectionDataMock.Replay ();

      _command.Perform();

      CollectionDataMock.VerifyAllExpectations ();

      CollectionMockEventReceiver.AssertWasNotCalled (mock => mock.Removing());
      CollectionMockEventReceiver.AssertWasNotCalled (mock => mock.Removed());
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var removedEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_removedRelatedObject.ID, "Customer");
      var removedEndPoint = (IObjectEndPoint) DataManager.GetRelationEndPointWithoutLoading (removedEndPointID);
      Assert.That (removedEndPoint, Is.Not.Null);

      EndPointProviderStub.Stub (stub => stub.GetRelationEndPointWithLazyLoad (removedEndPoint.ID)).Return (removedEndPoint);

      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      // DomainObject.Orders.Remove (_removedRelatedObject)
      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (2));

      // _removedRelatedObject.Customer = null
      Assert.That (steps[0], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setCustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[0]).DecoratedCommand);
      Assert.That (setCustomerCommand.ModifiedEndPoint, Is.SameAs (removedEndPoint));
      Assert.That (setCustomerCommand.OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (setCustomerCommand.NewRelatedObject, Is.Null);

      // DomainObject.Orders.Remove (_removedRelatedObject)
      Assert.That (steps[1], Is.SameAs (_command));
    }
  }
}
