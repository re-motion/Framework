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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class VirtualEndPointStateUpdateListenerTest : StandardMappingTest
  {
    private Mock<IClientTransactionEventSink> _eventSinkWithWock;
    private RelationEndPointID _endPointID;

    private VirtualEndPointStateUpdateListener _stateUpdateListener;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _eventSinkWithWock = new Mock<IClientTransactionEventSink>();
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");

      _stateUpdateListener = new VirtualEndPointStateUpdateListener(_eventSinkWithWock.Object);
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_Null ()
    {
      _stateUpdateListener.VirtualEndPointStateUpdated(_endPointID, null);

      _eventSinkWithWock.Verify(mock => mock.RaiseVirtualRelationEndPointStateUpdatedEvent(_endPointID, null), Times.AtLeastOnce());
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_True ()
    {
      _stateUpdateListener.VirtualEndPointStateUpdated(_endPointID, true);

      _eventSinkWithWock.Verify(mock => mock.RaiseVirtualRelationEndPointStateUpdatedEvent(_endPointID, true), Times.AtLeastOnce());
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_False ()
    {
      _stateUpdateListener.VirtualEndPointStateUpdated(_endPointID, false);

      _eventSinkWithWock.Verify(mock => mock.RaiseVirtualRelationEndPointStateUpdatedEvent(_endPointID, false), Times.AtLeastOnce());
    }
  }
}
