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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class SerializationTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
        base.SetUp();
        Assert2.IgnoreIfFeatureSerializationIsDisabled();
    }

    [Test]
    public void Extensions ()
    {
      var extension = new ClientTransactionExtensionWithQueryFiltering();
      ClientTransactionScope.CurrentTransaction.Extensions.Add(extension);

      var deserializedClientTransaction = Serializer.SerializeAndDeserialize(ClientTransactionScope.CurrentTransaction);

      Assert.That(deserializedClientTransaction, Is.Not.Null);
      Assert.That(deserializedClientTransaction.Extensions, Is.Not.Null);
      Assert.That(deserializedClientTransaction.Extensions.Count, Is.EqualTo(ClientTransactionScope.CurrentTransaction.Extensions.Count));
      Assert.That(deserializedClientTransaction.Extensions, Has.Some.TypeOf<ClientTransactionExtensionWithQueryFiltering>());
    }

    [Test]
    public void EventsAfterDeserializationWithRegisteredEvents ()
    {
      Customer newCustomer1 = Customer.NewObject();
      newCustomer1.Name = "NewCustomer1";

      Customer newCustomer2 = Customer.NewObject();
      newCustomer2.Name = "NewCustomer2";

      Official official2 = DomainObjectIDs.Official2.GetObject<Official>();
      Ceo newCeo1 = Ceo.NewObject();
      newCeo1.Name = "NewCEO1";

      Ceo newCeo2 = Ceo.NewObject();
      newCeo2.Name = "NewCEO2";

      Order newOrder1 = Order.NewObject();
      newOrder1.DeliveryDate = new DateTime(2006, 1, 1);

      Order newOrder2 = Order.NewObject();
      newOrder2.DeliveryDate = new DateTime(2006, 2, 2);

      OrderItem newOrderItem1 = OrderItem.NewObject();
      newOrderItem1.Product = "Product1";

      OrderItem newOrderItem2 = OrderItem.NewObject();
      newOrderItem2.Product = "Product2";

      var domainObjects = new DomainObject[]
                          {
                              newCustomer1,
                              newCustomer2,
                              official2,
                              newCeo1,
                              newCeo2,
                              newOrder1,
                              newOrder2,
                              newOrderItem1,
                              newOrderItem2
                          };

      var collections = new DomainObjectCollection[]
                        {
                            newCustomer1.Orders,
                            newCustomer2.Orders,
                            official2.Orders,
                            newOrder1.OrderItems,
                            newOrder2.OrderItems
                        };

      var eventReceiver = new SequenceEventReceiver(domainObjects, collections);

      var deserializedObjects =
          Serializer.SerializeAndDeserialize(new object[] { domainObjects, collections, ClientTransactionScope.CurrentTransaction, eventReceiver });

      Assert.That(deserializedObjects.Length, Is.EqualTo(4));

      var deserializedDomainObjects = (DomainObject[])deserializedObjects[0];
      var deserializedCollections = (DomainObjectCollection[])deserializedObjects[1];
      var deserializedClientTransaction = (ClientTransaction)deserializedObjects[2];

      using (deserializedClientTransaction.EnterDiscardingScope())
      {
        var deserializedEventReceiver = (SequenceEventReceiver)deserializedObjects[3];

        Assert.That(deserializedDomainObjects.Length, Is.EqualTo(9));
        Assert.That(deserializedCollections.Length, Is.EqualTo(5));

        var desNewCustomer1 = (Customer)deserializedDomainObjects[0];
        var desNewCustomer2 = (Customer)deserializedDomainObjects[1];
        var desOfficial2 = (Official)deserializedDomainObjects[2];
        var desNewCeo1 = (Ceo)deserializedDomainObjects[3];
        var desNewCeo2 = (Ceo)deserializedDomainObjects[4];
        var desNewOrder1 = (Order)deserializedDomainObjects[5];
        var desNewOrder2 = (Order)deserializedDomainObjects[6];
        var desNewOrderItem1 = (OrderItem)deserializedDomainObjects[7];
        var desNewOrderItem2 = (OrderItem)deserializedDomainObjects[8];

        //1
        desNewCeo1.Company = desNewCustomer1;
        //2
        desNewCeo2.Company = desNewCustomer1;
        //3
        desNewCeo1.Company = desNewCustomer2;
        //4
        desNewCeo1.Company = null;

        //5
        desNewCustomer1.Orders.Add(desNewOrder1);
        //6
        desNewCustomer1.Orders.Add(desNewOrder2);
        //7
        desNewCustomer1.Orders.Remove(desNewOrder2);

        //8
        desNewOrderItem1.Order = desNewOrder1;
        //9
        desNewOrderItem2.Order = desNewOrder1;
        //10
        desNewOrderItem1.Order = null;
        //11
        desNewOrderItem1.Order = desNewOrder2;

        //12
        desNewOrder1.Official = desOfficial2;

        //13
        OrderTicket desNewOrderTicket1 = OrderTicket.NewObject(desNewOrder1);
        desNewOrderTicket1.FileName = @"C:\temp\order1.tkt";

        var expectedChangeStates = new ChangeState[]
                                   {
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           desNewCustomer1,
                                           "1: 1. Changing event of newCeo from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           null,
                                           desNewCeo1,
                                           "1: 2. Changing event of newCustomer1 from null to newCeo1"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           null,
                                           null,
                                           "1: 3. Changed event of newCustomer1 from null to newCeo1"),
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           null,
                                           "1: 4. Changed event of newCeo from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCeo2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           desNewCustomer1,
                                           "2: 1. Changing event of newCeo2 from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           desNewCeo1,
                                           desNewCeo2,
                                           "2: 2. Changing event of newCustomer1 from newCeo1 to newCeo2"),
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           desNewCustomer1,
                                           null,
                                           "2: 3. Changing event of newCeo1 from newCustomer1 to null"),
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           null,
                                           "2: 4. Changed event of newCeo1 from newCustomer1 to null"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           null,
                                           null,
                                           "2: 5. Changed event of newCustomer1 from newCeo1 to newCeo2"),
                                       new RelationChangeState(
                                           desNewCeo2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           null,
                                           "2: 6. Changed event of newCeo2 from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           desNewCustomer2,
                                           "3: 1. Changing event of newCeo from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCustomer2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           null,
                                           desNewCeo1,
                                           "3: 2. Changing event of newCustomer2 from null to newCeo1"),
                                       new RelationChangeState(
                                           desNewCustomer2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           null,
                                           null,
                                           "3: 3. Changed event of newCustomer2 from null to newCeo1"),
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           null,
                                           "3: 4. Changed event of newCeo from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           desNewCustomer2,
                                           null,
                                           "4: 1. Changing event of newCeo from newCustomer1 to null"),
                                       new RelationChangeState(
                                           desNewCustomer2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           desNewCeo1,
                                           null,
                                           "4: 2. Changing event of newCustomer2 from newCeo1 to null"),
                                       new RelationChangeState(
                                           desNewCustomer2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo",
                                           null,
                                           null,
                                           "4: 3. Changed event of newCustomer2 from newCeo1 to null"),
                                       new RelationChangeState(
                                           desNewCeo1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company",
                                           null,
                                           null,
                                           "4: 4. Changed event of newCeo from newCustomer1 to null"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                           null,
                                           desNewCustomer1,
                                           "5: 1. Changing event of newOrder1 from null to newCustomer1"),
                                       new CollectionChangeState(desNewCustomer1.Orders, desNewOrder1, "5: 2. Adding of newOrder1 to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                           null,
                                           desNewOrder1,
                                           "5: 3. Changing event of newCustomer1 from null to newOrder1"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                           null,
                                           null,
                                           "5: 4. Changed event of newCustomer1 from null to newOrder1"),
                                       new CollectionChangeState(desNewCustomer1.Orders, desNewOrder1, "5: 5. Added of newOrder1 to newCustomer1"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                           null,
                                           null,
                                           "5: 5. Changed event of newOrder1 from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewOrder2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                           null,
                                           desNewCustomer1,
                                           "6: 1. Changing event of newOrder2 from null to newCustomer1"),
                                       new CollectionChangeState(desNewCustomer1.Orders, desNewOrder2, "6: 2. Adding of newOrder2 to newCustomer1"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                           null,
                                           desNewOrder2,
                                           "6: 3. Changing event of newCustomer1 from null to newOrder2"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                           null,
                                           null,
                                           "6: 4. Changed event of newCustomer1 from null to newOrder2"),
                                       new CollectionChangeState(desNewCustomer1.Orders, desNewOrder2, "6: 5. Added of newOrder2 to newCustomer1"),
                                       new RelationChangeState(
                                           desNewOrder2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                           null,
                                           null,
                                           "6: 6. Changed event of newOrder2 from null to newCustomer1"),
                                       new RelationChangeState(
                                           desNewOrder2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                           desNewCustomer1,
                                           null,
                                           "7: 1. Changing event of newOrder2 from newCustomer1 to null"),
                                       new CollectionChangeState(
                                           desNewCustomer1.Orders, desNewOrder2, "7: 2. Removing of newOrder2 from newCustomer1"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                           desNewOrder2,
                                           null,
                                           "7: 3. Changing event of newCustomer1 from newOrder2 to null"),
                                       new RelationChangeState(
                                           desNewCustomer1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                           null,
                                           null,
                                           "7: 4. Changed event of newCustomer1 from newOrder2 to null"),
                                       new CollectionChangeState(
                                           desNewCustomer1.Orders, desNewOrder2, "7: 5. Removed of newOrder2 from newCustomer1"),
                                       new RelationChangeState(
                                           desNewOrder2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                           null,
                                           null,
                                           "7: 6. Changed event of newOrder2 from newCustomer1 to null"),
                                       new RelationChangeState(
                                           desNewOrderItem1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           null,
                                           desNewOrder1,
                                           "8: 1. Changing event of newOrderItem1 from null to newOrder1"),
                                       new CollectionChangeState(
                                           desNewOrder1.OrderItems, desNewOrderItem1, "8: 2. Adding of newOrderItem1 to newOrder1"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           null,
                                           desNewOrderItem1,
                                           "8: 3. Changing event of newOrder1 from null to newOrderItem1"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           null,
                                           null,
                                           "8: 4. Changed event of newOrder1 from null to newOrderItem1"),
                                       new CollectionChangeState(
                                           desNewOrder1.OrderItems, desNewOrderItem1, "8: 5. Added of newOrderItem1 to newOrder1"),
                                       new RelationChangeState(
                                           desNewOrderItem1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           null,
                                           null,
                                           "8: 5. Changed event of newOrderItem1 from null to newOrder1"),
                                       new RelationChangeState(
                                           desNewOrderItem2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           null,
                                           desNewOrder1,
                                           "9: 1. Changing event of newOrderItem2 from null to newOrder1"),
                                       new CollectionChangeState(
                                           desNewOrder1.OrderItems, desNewOrderItem2, "9: 2. Adding of newOrderItem2 to newOrder1"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           null,
                                           desNewOrderItem2,
                                           "9: 3. Changing event of newOrder1 from null to newOrderItem2"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           null,
                                           null,
                                           "9: 4. Changed event of newOrder1 from null to newOrderItem2"),
                                       new CollectionChangeState(
                                           desNewOrder1.OrderItems, desNewOrderItem2, "9: 5. Added of newOrderItem2 to newOrder1"),
                                       new RelationChangeState(
                                           desNewOrderItem2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           null,
                                           null,
                                           "9: 6. Changed event of newOrderItem2 from null to newOrder1"),
                                       new RelationChangeState(
                                           desNewOrderItem1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           desNewOrder1,
                                           null,
                                           "10: 1. Changing event of newOrderItem1 from newOrder1 to null"),
                                       new CollectionChangeState(
                                           desNewOrder1.OrderItems, desNewOrderItem1, "10: 2. Removing of newOrderItem1 from newOrder1"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           desNewOrderItem1,
                                           null,
                                           "10: 3. Changing event of newOrder1 from newOrderItem1 to null"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           null,
                                           null,
                                           "10: 4. Changed event of newOrder1 from newOrderItem1 to null"),
                                       new CollectionChangeState(
                                           desNewOrder1.OrderItems, desNewOrderItem1, "10: 5. Removed of newOrderItem1 from newOrder1"),
                                       new RelationChangeState(
                                           desNewOrderItem1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           null,
                                           null,
                                           "10: 6. Changed event of newOrderItem2 from newOrder1 to null"),
                                       new RelationChangeState(
                                           desNewOrderItem1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           null,
                                           desNewOrder2,
                                           "11: 1. Changing event of newOrderItem1 from null to newOrder2"),
                                       new CollectionChangeState(
                                           desNewOrder2.OrderItems, desNewOrderItem1, "11: 2. Adding of newOrderItem1 to newOrder2"),
                                       new RelationChangeState(
                                           desNewOrder2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           null,
                                           desNewOrderItem1,
                                           "11: 3. Changing event of newOrder2 from null to newOrderItem1"),
                                       new RelationChangeState(
                                           desNewOrder2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems",
                                           null,
                                           null,
                                           "11: 4. Changed event of newOrder2 from null to newOrderItem1"),
                                       new CollectionChangeState(
                                           desNewOrder2.OrderItems, desNewOrderItem1, "11: 5. Adding of newOrderItem1 to newOrder2"),
                                       new RelationChangeState(
                                           desNewOrderItem1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                           null,
                                           null,
                                           "11: 6. Changed event of newOrderItem2 from null to newOrder2"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official",
                                           null,
                                           desOfficial2,
                                           "12: 1. Changing event of newOrder1 from null to official2"),
                                       new CollectionChangeState(desOfficial2.Orders, desNewOrder1, "12: 2. Adding of newOrder1 to official2"),
                                       new RelationChangeState(
                                           desOfficial2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Official.Orders",
                                           null,
                                           desNewOrder1,
                                           "12: 3. Changing event of official2 from null to newOrder1"),
                                       new RelationChangeState(
                                           desOfficial2,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Official.Orders",
                                           null,
                                           null,
                                           "12: 4. Changed event of official2 from null to newOrder1"),
                                       new CollectionChangeState(desOfficial2.Orders, desNewOrder1, "12: 5. Adding of newOrder1 to official2"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official",
                                           null,
                                           null,
                                           "12: 6. Changed event of newOrder1 from null to official2"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                           null,
                                           desNewOrderTicket1,
                                           "13: 1. Changing event of newOrder1 from null to newOrderTicket1"),
                                       new RelationChangeState(
                                           desNewOrder1,
                                           "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                           null,
                                           null,
                                           "13: 2. Changed event of newOrder1 from null to newOrderTicket1")
                                   };

        deserializedEventReceiver.Check(expectedChangeStates);
        deserializedEventReceiver.Unregister();

        eventReceiver = new SequenceEventReceiver(
            new DomainObject[] { desNewCustomer1, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
            new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders });

        //14
        desNewOrderTicket1.Order = desNewOrder2;


        expectedChangeStates = new ChangeState[]
                               {
                                   new RelationChangeState(
                                       desNewOrderTicket1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                                       desNewOrder1,
                                       desNewOrder2,
                                       "14: 1. Changing event of newOrderTicket1 from newOrder1 to newOrder2"),
                                   new RelationChangeState(
                                       desNewOrder1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                       desNewOrderTicket1,
                                       null,
                                       "14: 2. Changing event of newOrder1 from newOrderTicket1 to null"),
                                   new RelationChangeState(
                                       desNewOrder2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                       null,
                                       desNewOrderTicket1,
                                       "14: 3. Changing event of newOrder1 from null to newOrderTicket1"),
                                   new RelationChangeState(
                                       desNewOrder2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                       null,
                                       null,
                                       "14: 4. Changed event of newOrder1 from null to newOrderTicket1"),
                                   new RelationChangeState(
                                       desNewOrder1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                       null,
                                       null,
                                       "14: 5. Changed event of newOrder1 from newOrderTicket1 to null"),
                                   new RelationChangeState(
                                       desNewOrderTicket1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                                       null,
                                       null,
                                       "14: 6. Changed event of newOrderTicket1 from newOrder1 to newOrder2"),
                               };

        eventReceiver.Check(expectedChangeStates);
        eventReceiver.Unregister();

        //15a
        eventReceiver = new SequenceEventReceiver(
            new DomainObject[] { desNewCustomer1, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
            new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders });

        desNewOrder2.Customer = desNewCustomer1;

        expectedChangeStates = new ChangeState[]
                               {
                                   new RelationChangeState(
                                       desNewOrder2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                       null,
                                       desNewCustomer1,
                                       "15a: 1. Changing event of newOrder2 from null to newCustomer1.Orders"),
                                   new CollectionChangeState(desNewCustomer1.Orders, desNewOrder2, "15a: 2. Adding of newOrder2 to newCustomer1"),
                                   new RelationChangeState(
                                       desNewCustomer1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       null,
                                       desNewOrder2,
                                       "15a: 3. Changing event of newCustomer1 from null to newOrder2"),
                                   new RelationChangeState(
                                       desNewCustomer1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       null,
                                       null,
                                       "15a: 4. Changed event of newCustomer2 from null to newOrder2"),
                                   new CollectionChangeState(desNewCustomer1.Orders, desNewOrder2, "15a: 5. Added of newOrder2 to newCustomer1"),
                                   new RelationChangeState(
                                       desNewOrder2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                       null,
                                       null,
                                       "15a: 6. Changed event of newOrder2 from null to newCustomer1.Orders"),
                               };

        eventReceiver.Check(expectedChangeStates);
        eventReceiver.Unregister();

        //15b
        eventReceiver = new SequenceEventReceiver(
            new DomainObject[] { desNewCustomer1, desNewCustomer2, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
            new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders, desNewCustomer2.Orders });

        desNewOrder2.Customer = desNewCustomer2;

        expectedChangeStates = new ChangeState[]
                               {
                                   new RelationChangeState(
                                       desNewOrder2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                       desNewCustomer1,
                                       desNewCustomer2,
                                       "15b: 1. Changing event of newOrder2 from null to newCustomer2.Orders"),
                                   new CollectionChangeState(desNewCustomer2.Orders, desNewOrder2, "15b: 2. Adding of newOrder2 to newCustomer2"),
                                   new RelationChangeState(
                                       desNewCustomer2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       null,
                                       desNewOrder2,
                                       "15b: 3. Changing event of newCustomer2 from null to newOrder2"),
                                   new CollectionChangeState(desNewCustomer1.Orders, desNewOrder2, "15b: 4. Removing of newOrder2 from newCustomer1")
                                   ,
                                   new RelationChangeState(
                                       desNewCustomer1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       desNewOrder2,
                                       null,
                                       "15b: 5. Changing event of newCustomer1 from newOrder2 to null"),
                                   new RelationChangeState(
                                       desNewCustomer1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       null,
                                       null,
                                       "15b: 6. Changed event of newCustomer1 from newOrder2 to null"),
                                   new CollectionChangeState(desNewCustomer1.Orders, desNewOrder2, "15b: 7. Removed of newOrder2 from newCustomer1"),
                                   new RelationChangeState(
                                       desNewCustomer2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       null,
                                       null,
                                       "15b: 8. Changed event of newCustomer2 from null to newOrder2"),
                                   new CollectionChangeState(desNewCustomer2.Orders, desNewOrder2, "15b: 9. Added of newOrder2 to newCustomer2"),
                                   new RelationChangeState(
                                       desNewOrder2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
                                       null,
                                       null,
                                       "15b: 10. Changed event of newOrder2 from null to newCustomer2.Orders"),
                               };

        eventReceiver.Check(expectedChangeStates);
        eventReceiver.Unregister();

        //16
        eventReceiver = new SequenceEventReceiver(
            new DomainObject[] { desNewCustomer1, desNewCustomer2, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
            new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders, desNewCustomer2.Orders });

        var desNewOrder2Items = desNewOrder2.OrderItems;
        desNewOrder2.Delete();

        expectedChangeStates = new ChangeState[]
                               {
                                   new ObjectDeletionState(desNewOrder2, "16: 1. Deleting event of newOrder2"),
                                   new CollectionDeletionState(desNewOrder2Items, "16: 2. Deleting event of newOrder.OrderItems"),
                                   new RelationChangeState(
                                       desNewOrderTicket1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                                       desNewOrder2,
                                       null,
                                       "16: 3. Changing event of newOrderTicket1 from newOrder2 to null"),
                                   new CollectionChangeState(desNewCustomer2.Orders, desNewOrder2, "16: 4. Removing of newOrder2 from newCustomer2"),
                                   new RelationChangeState(
                                       desNewCustomer2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       desNewOrder2,
                                       null,
                                       "16: 5. Changing event of newCustomer2 from newOrder2 to null"),
                                   new RelationChangeState(
                                       desNewOrderItem1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                       desNewOrder2,
                                       null,
                                       "16: 6. Changing event of newOrderItem1 from newOrder2 to null"),
                                   new RelationChangeState(
                                       desNewOrderItem1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order",
                                       null,
                                       null,
                                       "16: 7. Changed event of newOrderItem1 from newOrder2 to null"),
                                   new RelationChangeState(
                                       desNewCustomer2,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
                                       null,
                                       null,
                                       "16: 8. Changed event of newCustomer2 from newOrder2 to null"),
                                   new CollectionChangeState(desNewCustomer2.Orders, desNewOrder2, "16: 9. Removed of newOrder2 from newCustomer2"),
                                   new RelationChangeState(
                                       desNewOrderTicket1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                                       null,
                                       null,
                                       "16: 10. Changed event of newOrderTicket1 from newOrder2 to null"),
                                   new CollectionDeletionState(desNewOrder2Items, "16: 11. Deleted event of newOrder.OrderItems"),
                                   new ObjectDeletionState(desNewOrder2, "16: 12. Deleted event of newOrder2")
                               };

        eventReceiver.Check(expectedChangeStates);
        eventReceiver.Unregister();

        //17
        eventReceiver = new SequenceEventReceiver(
            new DomainObject[] { desNewCustomer1, desNewCustomer2, desNewOrderTicket1, desNewOrder1, desNewOrderItem1 },
            new DomainObjectCollection[] { desNewCustomer1.Orders, desNewCustomer2.Orders });

        desNewOrderTicket1.Order = desNewOrder1;

        expectedChangeStates = new ChangeState[]
                               {
                                   new RelationChangeState(
                                       desNewOrderTicket1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                                       null,
                                       desNewOrder1,
                                       "17: 1. Changing event of newOrderTicket1 from null to newOrder1"),
                                   new RelationChangeState(
                                       desNewOrder1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                       null,
                                       desNewOrderTicket1,
                                       "17: 2. Changing event of newOrder1 from null to newOrderTicket1"),
                                   new RelationChangeState(
                                       desNewOrder1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
                                       null,
                                       null,
                                       "17: 3. Changed event of newOrder1 from null to newOrderTicket1"),
                                   new RelationChangeState(
                                       desNewOrderTicket1,
                                       "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
                                       null,
                                       null,
                                       "17: 4. Changed event of newOrderTicket1 from null to newOrder1"),
                               };

        eventReceiver.Check(expectedChangeStates);
        eventReceiver.Unregister();

        //cleanup for commit
        desNewCustomer2.Delete();
        desNewCeo1.Delete();
        desNewOrderItem1.Delete();

        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    [Test]
    public void BidirectionalRelationsIncludingHierarchyOfObjects ()
    {
      Employee employee1 = DomainObjectIDs.Employee1.GetObject<Employee>();
      Employee employee2 = DomainObjectIDs.Employee2.GetObject<Employee>();
      Employee employee3 = DomainObjectIDs.Employee3.GetObject<Employee>();
      Employee employee4 = DomainObjectIDs.Employee4.GetObject<Employee>();
      Employee employee5 = DomainObjectIDs.Employee5.GetObject<Employee>();
      Employee employee6 = DomainObjectIDs.Employee6.GetObject<Employee>();
      Employee employee7 = DomainObjectIDs.Employee7.GetObject<Employee>();

      DomainObjectCollection employee1Subordinates = employee1.Subordinates;
      Employee employee1Supervisor = employee1.Supervisor;
      Computer employee1Computer = employee1.Computer;
      DomainObjectCollection employee2Subordinates = employee2.Subordinates;
      Employee employee2Supervisor = employee2.Supervisor;
      Computer employee2Computer = employee2.Computer;
      DomainObjectCollection employee3Subordinates = employee3.Subordinates;
      Employee employee3Supervisor = employee3.Supervisor;
      Computer employee3Computer = employee3.Computer;
      DomainObjectCollection employee4Subordinates = employee4.Subordinates;
      Employee employee4Supervisor = employee4.Supervisor;
      Computer employee4Computer = employee4.Computer;
      DomainObjectCollection employee5Subordinates = employee5.Subordinates;
      Employee employee5Supervisor = employee5.Supervisor;
      Computer employee5Computer = employee5.Computer;
      DomainObjectCollection employee6Subordinates = employee6.Subordinates;
      Employee employee6Supervisor = employee6.Supervisor;
      Computer employee6Computer = employee6.Computer;
      DomainObjectCollection employee7Subordinates = employee7.Subordinates;
      Employee employee7Supervisor = employee1.Supervisor;
      Computer employee7Computer = employee7.Computer;

      var employees = new[] { employee1, employee2, employee3, employee4, employee5, employee6, employee7 };

      object[] deserializedItems = Serializer.SerializeAndDeserialize(new object[] { ClientTransactionScope.CurrentTransaction, employees });
      var deserializedTransaction = (ClientTransaction)deserializedItems[0];
      var deserializedEmployees = (Employee[])deserializedItems[1];

      Employee deserializedEmployee1 = deserializedEmployees[0];
      Employee deserializedEmployee2 = deserializedEmployees[1];
      Employee deserializedEmployee3 = deserializedEmployees[2];
      Employee deserializedEmployee4 = deserializedEmployees[3];
      Employee deserializedEmployee5 = deserializedEmployees[4];
      Employee deserializedEmployee6 = deserializedEmployees[5];
      Employee deserializedEmployee7 = deserializedEmployees[6];

      using (deserializedTransaction.EnterDiscardingScope())
      {
        DomainObjectCollection deserializedEmployee1Subordinates = deserializedEmployee1.Subordinates;
        Employee deserializedEmployee1Supervisor = deserializedEmployee1.Supervisor;
        Computer deserializedEmployee1Computer = deserializedEmployee1.Computer;
        DomainObjectCollection deserializedEmployee2Subordinates = deserializedEmployee2.Subordinates;
        Employee deserializedEmployee2Supervisor = deserializedEmployee2.Supervisor;
        Computer deserializedEmployee2Computer = deserializedEmployee2.Computer;
        DomainObjectCollection deserializedEmployee3Subordinates = deserializedEmployee3.Subordinates;
        Employee deserializedEmployee3Supervisor = deserializedEmployee3.Supervisor;
        Computer deserializedEmployee3Computer = deserializedEmployee3.Computer;
        DomainObjectCollection deserializedEmployee4Subordinates = deserializedEmployee4.Subordinates;
        Employee deserializedEmployee4Supervisor = deserializedEmployee4.Supervisor;
        Computer deserializedEmployee4Computer = deserializedEmployee4.Computer;
        DomainObjectCollection deserializedEmployee5Subordinates = deserializedEmployee5.Subordinates;
        Employee deserializedEmployee5Supervisor = deserializedEmployee5.Supervisor;
        Computer deserializedEmployee5Computer = deserializedEmployee5.Computer;
        DomainObjectCollection deserializedEmployee6Subordinates = deserializedEmployee6.Subordinates;
        Employee deserializedEmployee6Supervisor = deserializedEmployee6.Supervisor;
        Computer deserializedEmployee6Computer = deserializedEmployee6.Computer;
        DomainObjectCollection deserializedEmployee7Subordinates = deserializedEmployee7.Subordinates;
        Employee deserializedEmployee7Supervisor = deserializedEmployee1.Supervisor;
        Computer deserializedEmployee7Computer = deserializedEmployee7.Computer;

        Assert.That(deserializedEmployee1Subordinates.Count, Is.EqualTo(employee1Subordinates.Count));
        AreEqual(employee1Supervisor, deserializedEmployee1Supervisor);
        AreEqual(employee1Computer, deserializedEmployee1Computer);
        Assert.That(deserializedEmployee2Subordinates.Count, Is.EqualTo(employee2Subordinates.Count));
        AreEqual(employee2Supervisor, deserializedEmployee2Supervisor);
        AreEqual(employee2Computer, deserializedEmployee2Computer);
        Assert.That(deserializedEmployee3Subordinates.Count, Is.EqualTo(employee3Subordinates.Count));
        AreEqual(employee3Supervisor, deserializedEmployee3Supervisor);
        AreEqual(employee3Computer, deserializedEmployee3Computer);
        Assert.That(deserializedEmployee4Subordinates.Count, Is.EqualTo(employee4Subordinates.Count));
        AreEqual(employee4Supervisor, deserializedEmployee4Supervisor);
        AreEqual(employee4Computer, deserializedEmployee4Computer);
        Assert.That(deserializedEmployee5Subordinates.Count, Is.EqualTo(employee5Subordinates.Count));
        AreEqual(employee5Supervisor, deserializedEmployee5Supervisor);
        AreEqual(employee5Computer, deserializedEmployee5Computer);
        Assert.That(deserializedEmployee6Subordinates.Count, Is.EqualTo(employee6Subordinates.Count));
        AreEqual(employee6Supervisor, deserializedEmployee6Supervisor);
        AreEqual(employee6Computer, deserializedEmployee6Computer);
        Assert.That(deserializedEmployee7Subordinates.Count, Is.EqualTo(employee7Subordinates.Count));
        AreEqual(employee7Supervisor, deserializedEmployee7Supervisor);
        AreEqual(employee7Computer, deserializedEmployee7Computer);
      }
    }

    [Test]
    public void UnidirectionalRelation ()
    {
      Location location1 = DomainObjectIDs.Location1.GetObject<Location>();
      Client location1Client = location1.Client;
      Location location2 = DomainObjectIDs.Location2.GetObject<Location>();
      Client location2Client = location2.Client;
      Location location3 = DomainObjectIDs.Location3.GetObject<Location>();
      Client location3Client = location3.Client;

      var locations = new[] { location1, location2, location3 };

      object[] deserializedItems = Serializer.SerializeAndDeserialize(new object[] { ClientTransactionScope.CurrentTransaction, locations });
      var deserializedTransaction = (ClientTransaction)deserializedItems[0];
      var deserializedLocations = (Location[])deserializedItems[1];

      Location deserializedLocation1 = deserializedLocations[0];
      Location deserializedLocation2 = deserializedLocations[1];
      Location deserializedLocation3 = deserializedLocations[2];

      using (deserializedTransaction.EnterDiscardingScope())
      {
        Assert.That(deserializedLocation1.ID, Is.EqualTo(location1.ID));
        AreEqual(location1Client, deserializedLocation1.Client);
        Assert.That(deserializedLocation2.ID, Is.EqualTo(location2.ID));
        AreEqual(location2Client, deserializedLocation2.Client);
        Assert.That(deserializedLocation3.ID, Is.EqualTo(location3.ID));
        AreEqual(location3Client, deserializedLocation3.Client);
      }
    }

    [Test]
    public void ReplacedDomainObjectCollection ()
    {
      IndustrialSector industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      var oldCompanies = industrialSector.Companies;
      var newCompanies = new ObjectList<Company> { Company.NewObject(), Company.NewObject() };
      industrialSector.Companies = newCompanies;

      var serializationTuple = Tuple.Create(TestableClientTransaction, industrialSector, oldCompanies, newCompanies);
      var deserializedTuple = Serializer.SerializeAndDeserialize(serializationTuple);
      using (deserializedTuple.Item1.EnterDiscardingScope())
      {
        var deserializedIndustrialSector = deserializedTuple.Item2;
        var deserializedOldCompanies = deserializedTuple.Item3;
        var deserializedNewCompanies = deserializedTuple.Item4;
        Assert.That(deserializedIndustrialSector.Companies, Is.SameAs(deserializedNewCompanies));
        ClientTransaction.Current.Rollback();
        Assert.That(deserializedIndustrialSector.Companies, Is.SameAs(deserializedOldCompanies));
      }
    }

    private void AreEqual (DomainObject expected, DomainObject actual)
    {
      if (expected == null && actual == null)
        return;
      if (expected == null || actual == null)
        Assert.Fail("One reference is null.");

      Assert.That(actual.ID, Is.EqualTo(expected.ID));
    }
  }
}
