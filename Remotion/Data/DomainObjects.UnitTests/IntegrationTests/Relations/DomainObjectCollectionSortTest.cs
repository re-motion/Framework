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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class DomainObjectCollectionSortTest : ClientTransactionBaseTest
  {
    private Customer _owningCustomer;
    private Order _itemA;
    private Order _itemB;
    private Comparison<IDomainObject> _reversingComparison;

    public override void SetUp ()
    {
      base.SetUp();

      _owningCustomer = DomainObjectIDs.Customer1.GetObject<Customer>();

      Assert.That(_owningCustomer.Orders.Count, Is.EqualTo(2));
      _itemA = _owningCustomer.Orders[0];
      _itemB = _owningCustomer.Orders[1];

      var weights = new Dictionary<IDomainObject, int> { { _itemA, 2 }, { _itemB, 1 } };
      _reversingComparison = (one, two) => weights[one].CompareTo(weights[two]);
    }

    [Test]
    public void Sort ()
    {
      Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemA, _itemB }));
      Assert.That(_itemA.Customer, Is.SameAs(_owningCustomer));
      Assert.That(_itemB.Customer, Is.SameAs(_owningCustomer));

      _owningCustomer.Orders.Sort(_reversingComparison);

      Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemB, _itemA }));
      Assert.That(_itemA.Customer, Is.SameAs(_owningCustomer));
      Assert.That(_itemB.Customer, Is.SameAs(_owningCustomer));
    }

    [Test]
    public void Sort_AlreadySorted_StateAndHasChangedRemainUnchanged ()
    {
      Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemA, _itemB }));
      Assert.That(_owningCustomer.State.IsUnchanged, Is.True);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasChanged, Is.False);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasBeenTouched, Is.False);
      Assert.That(_itemA.State.IsUnchanged, Is.True);
      Assert.That(_itemB.State.IsUnchanged, Is.True);

      Comparison<IDomainObject> nonReversingComparison = (one, two) => _reversingComparison(two, one);
      _owningCustomer.Orders.Sort(nonReversingComparison);

      Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemA, _itemB }));
      Assert.That(_owningCustomer.State.IsUnchanged, Is.True);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasChanged, Is.False);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasBeenTouched, Is.True);
      Assert.That(_itemA.State.IsUnchanged, Is.True);
      Assert.That(_itemB.State.IsUnchanged, Is.True);
    }

    [Test]
    public void Sort_NotSortedYet_StateAndHasChangedRemainUnchanged_InRootTransaction ()
    {
      Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemA, _itemB }));
      Assert.That(_owningCustomer.State.IsUnchanged, Is.True);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasChanged, Is.False);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasBeenTouched, Is.False);
      Assert.That(_itemA.State.IsUnchanged, Is.True);
      Assert.That(_itemB.State.IsUnchanged, Is.True);

      _owningCustomer.Orders.Sort(_reversingComparison);

      Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemB, _itemA }));
      Assert.That(_owningCustomer.State.IsUnchanged, Is.True);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasChanged, Is.False);
      Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasBeenTouched, Is.True);
      Assert.That(_itemA.State.IsUnchanged, Is.True);
      Assert.That(_itemB.State.IsUnchanged, Is.True);
    }

    [Test]
    public void Sort_NotSortedYet_StateAndHasChangedGoToChanged_InSubTransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemA, _itemB }));
        Assert.That(_owningCustomer.State.IsUnchanged, Is.True);
        Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasChanged, Is.False);
        Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasBeenTouched, Is.False);
        Assert.That(_itemA.State.IsUnchanged, Is.True);
        Assert.That(_itemB.State.IsUnchanged, Is.True);

        _owningCustomer.Orders.Sort(_reversingComparison);

        Assert.That(_owningCustomer.Orders, Is.EqualTo(new[] { _itemB, _itemA }));
        Assert.That(_owningCustomer.State.IsChanged, Is.True);
        Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasChanged, Is.True);
        Assert.That(_owningCustomer.Properties[typeof(Customer), "Orders"].HasBeenTouched, Is.True);
        Assert.That(_itemA.State.IsUnchanged, Is.True);
        Assert.That(_itemB.State.IsUnchanged, Is.True);
      }
    }

    [Test]
    public void Sort_TriggersOnReplaceDataEvent_ButNoRelationChangeEvents ()
    {
      var eventReceiverMock = new Mock<OrderCollection.ICollectionEventReceiver>(MockBehavior.Strict);
      eventReceiverMock.Setup(mock => mock.OnReplaceData()).Verifiable();

      var orderCollection = _owningCustomer.Orders;
      orderCollection.SetEventReceiver(eventReceiverMock.Object);

      var relationEndPointID = RelationEndPointID.Resolve(_owningCustomer, c => c.Orders);

      var eventListenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);
      try
      {
        eventListenerMock
            .Setup(mock => mock.VirtualRelationEndPointStateUpdated(TestableClientTransaction, relationEndPointID, null))
            .Verifiable();

        orderCollection.Sort(_reversingComparison);

        eventReceiverMock.Verify();
        eventListenerMock.Verify();
      }
      finally
      {
        TestableClientTransaction.RemoveListener(eventListenerMock.Object);
      }
    }
  }
}
