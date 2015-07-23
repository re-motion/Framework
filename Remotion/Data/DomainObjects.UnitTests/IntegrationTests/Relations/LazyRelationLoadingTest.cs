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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class LazyRelationLoadingTest : ClientTransactionBaseTest
  {
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp ();

      _order = DomainObjectIDs.Order1.GetObjectReference<Order>();
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_ReturnsNonloadedReference_ButLoadsObjectCOntainingForeignKey ()
    {
      Assert.That (_order.State, Is.EqualTo (StateType.NotLoadedYet));

      var customer = _order.Customer;

      Assert.That (customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));
      Assert.That (customer.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_ReturnsNonloadedReference_DataIsLoadedOnDemand ()
    {
      Assert.That (_order.Customer.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (_order.Customer.Name, Is.EqualTo ("Kunde 1"));

      Assert.That (_order.Customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_NonloadedReference_CanBeUsedToRegisterEvents ()
    {
      bool propertyChanged = false;
      _order.Customer.PropertyChanged += delegate { propertyChanged = true; };

      Assert.That (_order.Customer.State, Is.EqualTo (StateType.NotLoadedYet));

      _order.Customer.EnsureDataAvailable();

      Assert.That (_order.Customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (propertyChanged, Is.False);

      _order.Customer.Name = "John Doe";

      Assert.That (propertyChanged, Is.True);
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_InvalidType_IsImmediatelyNoticed ()
    {
      var orderWithInvalidCustomer = DomainObjectIDs.InvalidOrder.GetObject<Order>();

      Assert.That (
          () => orderWithInvalidCustomer.Customer,
          Throws.TypeOf<InvalidTypeException>().With.Message.StringStarting (
              "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' was expected to hold an object of type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer', but it returned an object of type "
              + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Company"));
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_NotFoundKeyException_IsTriggeredOnDemand ()
    {
      var id = new ObjectID (typeof (ClassWithInvalidRelation), new Guid ("{AFA9CF46-8E77-4da8-9793-53CAA86A277C}"));
      var objectWithInvalidRelation = (ClassWithInvalidRelation) id.GetObject<TestDomainBase> ();

      Assert.That (objectWithInvalidRelation.ClassWithGuidKey.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (() => objectWithInvalidRelation.ClassWithGuidKey.EnsureDataAvailable(), Throws.TypeOf<ObjectsNotFoundException> ());
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_ReturnsAlreadyLoadedReference_IfAlreadyLoaded ()
    {
      DomainObjectIDs.Customer1.GetObject<Customer>();

      Assert.That (_order.Customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));
      Assert.That (_order.Customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingOriginalRelatedObject_ForeignKeySide_OriginalObjectIsAlsoNotLoadedOnAccess ()
    {
      Assert.That (_order.Properties[typeof (Order), "Customer"].GetOriginalValue<Customer>().ID, Is.EqualTo (DomainObjectIDs.Customer1));
      Assert.That (_order.Properties[typeof (Order), "Customer"].GetOriginalValue<Customer> ().State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void AccessingRelatedObject_VirtualSide_ReturnsLoadedObject_AndAlsoLoadsOriginatingObject ()
    {
      Assert.That (_order.State, Is.EqualTo (StateType.NotLoadedYet));

      var orderTicket = _order.OrderTicket;

      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingOriginalRelatedObject_VirtualSide_ReturnsLoadedObject_AndAlsoLoadsOriginatingObject ()
    {
      Assert.That (_order.State, Is.EqualTo (StateType.NotLoadedYet));

      var orderTicket = _order.Properties[typeof (Order), "OrderTicket"].GetOriginalValue<OrderTicket>();

      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingRelatedCollection_ReturnsCollectionWithIncompleteContents_AndAlsoLoadsOriginatingObject ()
    {
      Assert.That (_order.State, Is.EqualTo (StateType.NotLoadedYet));

      var orderItems = _order.OrderItems;

      Assert.That (orderItems.AssociatedEndPointID, Is.EqualTo (RelationEndPointID.Resolve (_order, o => o.OrderItems)));
      Assert.That (orderItems.IsDataComplete, Is.False);
      Assert.That (_order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingRelatedCollection_ReturnsCollectionWithIncompleteContents_ContentsIsLoadedWhenNeeded ()
    {
      Assert.That (_order.OrderItems.IsDataComplete, Is.False);

      Assert.That (_order.OrderItems.Count, Is.EqualTo (2));

      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
    }

    [Test]
    public void AccessingRelatedCollection_CollectionWithIncompleteContents_CanBeUsedToRegisterEvents ()
    {
      bool itemAdded = false;
      _order.OrderItems.Added += delegate { itemAdded = true; };

      Assert.That (_order.OrderItems.IsDataComplete, Is.False);

      _order.OrderItems.EnsureDataComplete();

      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
      Assert.That (itemAdded, Is.False);

      _order.OrderItems.Add (OrderItem.NewObject());

      Assert.That (itemAdded, Is.True);
    }

    [Test]
    public void AccessingRelatedCollection_ExceptionOnLoading_IsTriggeredOnDemand ()
    {
      var orderWithoutOrderItems = DomainObjectIDs.OrderWithoutOrderItems.GetObject<Order>();

      Assert.That (orderWithoutOrderItems.OrderItems.IsDataComplete, Is.False);

      Assert.That (() => orderWithoutOrderItems.OrderItems.Count, Throws.TypeOf<PersistenceException>());
    }

    [Test]
    public void AccessingRelatedCollection_ReturnsAlreadyLoadedCollection_IfAlreadyLoaded ()
    {
      TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (_order, o => o.OrderItems));

      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
    }

    [Test]
    public void AccessingOriginalRelatedCollection_LoadsContentsForBothOriginalAndCurrentCollection ()
    {
      Assert.That (_order.Properties[typeof (Order), "OrderItems"].GetOriginalValue<ObjectList<OrderItem>>().IsDataComplete, Is.True);
      // Since the data had to be loaded for the original contents, it has also been loaded into the actual collection.
      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
    }

    [Test]
    public void OneToManyIntegrationTest_WritingRelation_UsingForeignKeyProperty ()
    {
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObjectReference<OrderItem>();
      var order2 = DomainObjectIDs.Order2.GetObjectReference<Order>();

      Assert.That (_order.OrderItems.IsDataComplete, Is.False);
      Assert.That (order2.OrderItems.IsDataComplete, Is.False);
      Assert.That (orderItem1.State, Is.EqualTo (StateType.NotLoadedYet));

      orderItem1.Order = order2;

      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
      Assert.That (order2.OrderItems.IsDataComplete, Is.True);
      Assert.That (orderItem1.State, Is.EqualTo (StateType.Changed));
    }
  }
}