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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointReplaceSameCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private CollectionEndPointReplaceSameCommand _command;
    private Order _replacedRelatedObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _replacedRelatedObject = DomainObjectIDs.Order1.GetObject<Order> (Transaction);

      _command = new CollectionEndPointReplaceSameCommand (CollectionEndPoint, _replacedRelatedObject, TransactionEventSinkMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.SameAs (_replacedRelatedObject));
      Assert.That (_command.NewRelatedObject, Is.SameAs (_replacedRelatedObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (Transaction, RelationEndPointID.Definition);
      Dev.Null = new CollectionEndPointReplaceSameCommand (endPoint, _replacedRelatedObject, TransactionEventSinkMock);
    }

    [Test]
    public void Begin_NoEvents ()
    {
      TransactionEventSinkMock.Replay();

      _command.Begin ();
    }

    [Test]
    public void End_NoEvents ()
    {
      TransactionEventSinkMock.Replay();

      _command.End ();
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Replay ();

      _command.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      CollectionMockEventReceiver.AssertWasNotCalled (mock => mock.Adding());
      CollectionMockEventReceiver.AssertWasNotCalled (mock => mock.Added());
      CollectionMockEventReceiver.AssertWasNotCalled (mock => mock.Removing ());
      CollectionMockEventReceiver.AssertWasNotCalled (mock => mock.Removed());

      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      var relationEndPointID = RelationEndPointID.Create(_replacedRelatedObject.ID, CollectionEndPoint.Definition.GetOppositeEndPointDefinition());
      var oppositeEndPoint = DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);

      var steps = bidirectionalModification.GetNestedCommands ();
      Assert.That (steps.Count, Is.EqualTo (2));

      // customer.Orders.Touch()
      Assert.That (steps[0], Is.SameAs (_command));

      // customer.Orders[index].Touch()
      Assert.That (steps[1], Is.InstanceOf (typeof (RelationEndPointTouchCommand)));
      Assert.That (((RelationEndPointTouchCommand) steps[1]).EndPoint, Is.SameAs (oppositeEndPoint));
    }
  }
}
