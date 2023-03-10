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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void LoadingOfSimpleObject ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();

      Assert.That(classWithAllDataTypes.ID.Value, Is.EqualTo(DomainObjectIDs.ClassWithAllDataTypes1.Value), "ID.Value");
      Assert.That(classWithAllDataTypes.ID.ClassID, Is.EqualTo(DomainObjectIDs.ClassWithAllDataTypes1.ClassID), "ID.ClassID");
      Assert.That(
          classWithAllDataTypes.ID.StorageProviderDefinition,
          Is.SameAs(DomainObjectIDs.ClassWithAllDataTypes1.StorageProviderDefinition),
          "ID.StorageProviderDefinition");

      Assert.That(classWithAllDataTypes.BooleanProperty, Is.EqualTo(false), "BooleanProperty");
      Assert.That(classWithAllDataTypes.ByteProperty, Is.EqualTo(85), "ByteProperty");
      Assert.That(classWithAllDataTypes.DateProperty, Is.EqualTo(new DateTime(2005, 1, 1)), "DateProperty");
      Assert.That(classWithAllDataTypes.DateTimeProperty, Is.EqualTo(new DateTime(2005, 1, 1, 17, 0, 0)), "DateTimeProperty");
      Assert.That(classWithAllDataTypes.DecimalProperty, Is.EqualTo(123456.789m), "DecimalProperty");
      Assert.That(classWithAllDataTypes.DoubleProperty, Is.EqualTo(987654.321d), "DoubleProperty");
      Assert.That(classWithAllDataTypes.EnumProperty, Is.EqualTo(ClassWithAllDataTypes.EnumType.Value1), "EnumProperty");
      Assert.That(classWithAllDataTypes.ExtensibleEnumProperty, Is.EqualTo(Color.Values.Red()), "ExtensibleEnumProperty");
      Assert.That(classWithAllDataTypes.FlagsProperty, Is.EqualTo(ClassWithAllDataTypes.FlagsType.Flag2), "FlagsProperty");
      Assert.That(classWithAllDataTypes.GuidProperty, Is.EqualTo(new Guid("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}")), "GuidProperty");
      Assert.That(classWithAllDataTypes.Int16Property, Is.EqualTo(32767), "Int16Property");
      Assert.That(classWithAllDataTypes.Int32Property, Is.EqualTo(2147483647), "Int32Property");
      Assert.That(classWithAllDataTypes.Int64Property, Is.EqualTo(9223372036854775807L), "Int64Property");
      Assert.That(classWithAllDataTypes.SingleProperty, Is.EqualTo(6789.321f), "SingleProperty");
      Assert.That(classWithAllDataTypes.StringProperty, Is.EqualTo("abcdeföäü"), "StringProperty");
      Assert.That(
          classWithAllDataTypes.StringPropertyWithoutMaxLength,
          Is.EqualTo("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"),
          "StringPropertyWithoutMaxLength");
      ResourceManager.IsEqualToImage1(classWithAllDataTypes.BinaryProperty, "BinaryProperty");

      Assert.That(classWithAllDataTypes.NaBooleanProperty, Is.EqualTo(true), "NaBooleanProperty");
      Assert.That(classWithAllDataTypes.NaByteProperty, Is.EqualTo((byte)78), "NaByteProperty");
      Assert.That(classWithAllDataTypes.NaDateProperty, Is.EqualTo(new DateTime(2005, 2, 1)), "NaDateProperty");
      Assert.That(classWithAllDataTypes.NaDateTimeProperty, Is.EqualTo(new DateTime(2005, 2, 1, 5, 0, 0)), "NaDateTimeProperty");
      Assert.That(classWithAllDataTypes.NaDecimalProperty, Is.EqualTo(765.098m), "NaDecimalProperty");
      Assert.That(classWithAllDataTypes.NaDoubleProperty, Is.EqualTo(654321.789d), "NaDoubleProperty");
      Assert.That(classWithAllDataTypes.NaEnumProperty, Is.EqualTo(ClassWithAllDataTypes.EnumType.Value2), "NaEnumProperty");
      Assert.That(classWithAllDataTypes.NaFlagsProperty, Is.EqualTo(ClassWithAllDataTypes.FlagsType.Flag1 | ClassWithAllDataTypes.FlagsType.Flag2), "NaFlagsProperty");
      Assert.That(classWithAllDataTypes.NaGuidProperty, Is.EqualTo(new Guid("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")), "NaGuidProperty");
      Assert.That(classWithAllDataTypes.NaInt16Property, Is.EqualTo((short)12000), "NaInt16Property");
      Assert.That(classWithAllDataTypes.NaInt32Property, Is.EqualTo(-2147483647), "NaInt32Property");
      Assert.That(classWithAllDataTypes.NaInt64Property, Is.EqualTo(3147483647L), "NaInt64Property");
      Assert.That(classWithAllDataTypes.NaSingleProperty, Is.EqualTo(12.456F), "NaSingleProperty");

      Assert.That(classWithAllDataTypes.NaBooleanWithNullValueProperty, Is.Null, "NaBooleanWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaByteWithNullValueProperty, Is.Null, "NaByteWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaDecimalWithNullValueProperty, Is.Null, "NaDecimalWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaDateWithNullValueProperty, Is.Null, "NaDateWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaDateTimeWithNullValueProperty, Is.Null, "NaDateTimeWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaDoubleWithNullValueProperty, Is.Null, "NaDoubleWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaEnumWithNullValueProperty, Is.Null, "NaEnumWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaFlagsWithNullValueProperty, Is.Null, "NaFlagsWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaGuidWithNullValueProperty, Is.Null, "NaGuidWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaInt16WithNullValueProperty, Is.Null, "NaInt16WithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaInt32WithNullValueProperty, Is.Null, "NaInt32WithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaInt64WithNullValueProperty, Is.Null, "NaInt64WithNullValueProperty");
      Assert.That(classWithAllDataTypes.NaSingleWithNullValueProperty, Is.Null, "NaSingleWithNullValueProperty");
      Assert.That(classWithAllDataTypes.StringWithNullValueProperty, Is.Null, "StringWithNullValueProperty");
      Assert.That(classWithAllDataTypes.ExtensibleEnumWithNullValueProperty, Is.Null, "ExtensibleEnumWithNullValueProperty");
      Assert.That(classWithAllDataTypes.NullableBinaryProperty, Is.Null, "NullableBinaryProperty");
    }

    [Test]
    public void SetNullIntoNonNullableValueType ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That(
          () => SetPropertyValue(classWithAllDataTypes.InternalDataContainer, typeof(ClassWithAllDataTypes), "BooleanProperty", null),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty' does not allow null values."));
    }

    [Test]
    public void LoadingOfDerivedObject ()
    {
      Company company = DomainObjectIDs.Partner2.GetObject<Company>();
      Assert.That(company, Is.Not.Null);

      Assert.That(company, Is.InstanceOf(typeof(Partner)));
      var partner = (Partner)company;

      Assert.That(partner.ID, Is.EqualTo(DomainObjectIDs.Partner2), "ID");
      Assert.That(partner.Name, Is.EqualTo("Partner 2"), "Name");

      Assert.That(partner.ContactPerson.ID, Is.EqualTo(DomainObjectIDs.Person2), "ContactPerson");
    }

    [Test]
    public void LoadingOfTwiceDerivedObject ()
    {
      Company company = DomainObjectIDs.Supplier1.GetObject<Company>();
      Assert.That(company, Is.Not.Null);

      Assert.That(company, Is.InstanceOf(typeof(Supplier)));
      var supplier = (Supplier)company;

      Assert.That(supplier.ID, Is.EqualTo(DomainObjectIDs.Supplier1));
      Assert.That(supplier.Name, Is.EqualTo("Lieferant 1"), "Name");
      Assert.That(supplier.ContactPerson.ID, Is.EqualTo(DomainObjectIDs.Person3), "ContactPerson");
      Assert.That(supplier.SupplierQuality, Is.EqualTo(1), "SupplierQuality");
    }

    [Test]
    public void OnLoaded ()
    {
      var id = new ObjectID("ClassWithAllDataTypes", new Guid("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = id.GetObject<ClassWithAllDataTypes>();

      Assert.That(classWithAllDataTypes.OnLoadedCalled, Is.True);
      Assert.That(classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo(1));
      Assert.That(classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo(LoadMode.WholeDomainObjectInitialized));
    }

    [Test]
    public void NoOnLoaded_ByGetObjectReference ()
    {
      var id = new ObjectID("ClassWithAllDataTypes", new Guid("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      var classWithAllDataTypes = id.GetObjectReference<ClassWithAllDataTypes>();
      Assert.That(classWithAllDataTypes.OnLoadedCalled, Is.False);
    }

    [Test]
    public void OnLoaded_OnFirstAccess ()
    {
      var id = new ObjectID("ClassWithAllDataTypes", new Guid("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      var classWithAllDataTypes = id.GetObjectReference<ClassWithAllDataTypes>();
      classWithAllDataTypes.Int32Property = 5;

      Assert.That(classWithAllDataTypes.OnLoadedCalled, Is.True);
      Assert.That(classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo(1));
      Assert.That(classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo(LoadMode.WholeDomainObjectInitialized));
    }

    [Test]
    public void OnLoadedInSubTransaction ()
    {
      var id = new ObjectID("ClassWithAllDataTypes", new Guid("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        ClassWithAllDataTypes classWithAllDataTypes = id.GetObject<ClassWithAllDataTypes>();

        Assert.That(classWithAllDataTypes.OnLoadedCalled, Is.True);
        Assert.That(classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo(2));
        Assert.That(classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo(LoadMode.DataContainerLoadedOnly));
      }
    }

    [Test]
    public void OnLoadedInParentAndSubTransaction ()
    {
      var id = new ObjectID("ClassWithAllDataTypes", new Guid("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      ClassWithAllDataTypes classWithAllDataTypes = id.GetObject<ClassWithAllDataTypes>();
      Assert.That(classWithAllDataTypes.OnLoadedCalled, Is.True);
      Assert.That(classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo(1));
      Assert.That(classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo(LoadMode.WholeDomainObjectInitialized));

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        id.GetObject<ClassWithAllDataTypes>();

        Assert.That(classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo(2));
        Assert.That(classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo(LoadMode.DataContainerLoadedOnly));
      }
    }

    [Test]
    public void OnLoadedWithNewInParentAndGetInSubTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That(classWithAllDataTypes.OnLoadedCalled, Is.False);
      Assert.That(classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo(0));

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Dev.Null = classWithAllDataTypes.Int32Property;

        Assert.That(classWithAllDataTypes.OnLoadedCalled, Is.True);
        Assert.That(classWithAllDataTypes.OnLoadedCallCount, Is.EqualTo(1));
        Assert.That(classWithAllDataTypes.OnLoadedLoadMode, Is.EqualTo(LoadMode.DataContainerLoadedOnly));
      }
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();

      Assert.That(order.OrderTicket, Is.Not.Null);
      Assert.That(order.OrderTicket.ID, Is.EqualTo(DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void GetRelatedObjectByInheritedRelationTwice ()
    {
      Customer customer = DomainObjectIDs.Customer4.GetObject<Customer>();

      Ceo ceoReference1 = customer.Ceo;

      Ceo ceoReference2 = customer.Ceo;

      Assert.That(ceoReference2, Is.SameAs(ceoReference1));
    }

    [Test]
    public void GetDerivedRelatedObject ()
    {
      Ceo ceo = DomainObjectIDs.Ceo10.GetObject<Ceo>();

      Company company = ceo.Company;
      Assert.That(company, Is.Not.Null);

      var distributor = company as Distributor;
      Assert.That(distributor, Is.Not.Null);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      Assert.That(customer.Orders, Is.Not.Null);
      Assert.That(customer.Orders.Count, Is.EqualTo(2));
      Assert.That(customer.Orders[DomainObjectIDs.Order1].ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(customer.Orders[DomainObjectIDs.Order2].ID, Is.EqualTo(DomainObjectIDs.Order2));
    }

    [Test]
    public void GetRelatedObjectsWithDerivation ()
    {
      IndustrialSector industrialSector = DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector>();
      DomainObjectCollection collection = industrialSector.Companies;

      Assert.That(collection.Count, Is.EqualTo(7));
      Assert.That(collection[DomainObjectIDs.Company1].GetPublicDomainObjectType(), Is.EqualTo(typeof(Company)));
      Assert.That(collection[DomainObjectIDs.Company2].GetPublicDomainObjectType(), Is.EqualTo(typeof(Company)));
      Assert.That(collection[DomainObjectIDs.Customer2].GetPublicDomainObjectType(), Is.EqualTo(typeof(Customer)));
      Assert.That(collection[DomainObjectIDs.Customer3].GetPublicDomainObjectType(), Is.EqualTo(typeof(Customer)));
      Assert.That(collection[DomainObjectIDs.Partner2].GetPublicDomainObjectType(), Is.EqualTo(typeof(Partner)));
      Assert.That(collection[DomainObjectIDs.Supplier2].GetPublicDomainObjectType(), Is.EqualTo(typeof(Supplier)));
      Assert.That(collection[DomainObjectIDs.Distributor1].GetPublicDomainObjectType(), Is.EqualTo(typeof(Distributor)));
    }

    [Test]
    public void ChangeTrackingEvents ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      var eventReceiver = new DomainObjectEventReceiver(customer, false);
      customer.Name = "New name";

      Assert.That(eventReceiver.HasChangingEventBeenCalled, Is.EqualTo(true));
      Assert.That(eventReceiver.HasChangedEventBeenCalled, Is.EqualTo(true));
      Assert.That(customer.Name, Is.EqualTo("New name"));
      Assert.That(eventReceiver.ChangingOldValue, Is.EqualTo("Kunde 1"));
      Assert.That(eventReceiver.ChangingNewValue, Is.EqualTo("New name"));
      Assert.That(eventReceiver.ChangedOldValue, Is.EqualTo("Kunde 1"));
      Assert.That(eventReceiver.ChangedNewValue, Is.EqualTo("New name"));
    }

    [Test]
    public void CancelChangeTrackingEvents ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      var eventReceiver = new DomainObjectEventReceiver(customer, true);

      try
      {
        customer.Name = "New name";
        Assert.Fail("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That(eventReceiver.HasChangingEventBeenCalled, Is.EqualTo(true));
        Assert.That(eventReceiver.HasChangedEventBeenCalled, Is.EqualTo(false));
        Assert.That(customer.Name, Is.EqualTo("Kunde 1"));
        Assert.That(eventReceiver.ChangingOldValue, Is.EqualTo("Kunde 1"));
        Assert.That(eventReceiver.ChangingNewValue, Is.EqualTo("New name"));
      }
    }

    [Test]
    public void StateInDifferentTransactions ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      customer.Name = "New name";

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(customer.State.IsNotLoadedYet, Is.True);

        customer.EnsureDataAvailable();

        Assert.That(customer.State.IsUnchanged, Is.True);
        Assert.That(customer.TransactionContext[TestableClientTransaction].State.IsChanged, Is.True);

        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          Assert.That(customer.TransactionContext[TestableClientTransaction].State.IsChanged, Is.True); // must not throw a ClientTransactionDiffersException
        }
      }
    }

    [Test]
    public void InvalidStateType ()
    {
      ClassWithAllDataTypes newObject = ClassWithAllDataTypes.NewObject();
      DataContainer newObjectDataContainer = newObject.InternalDataContainer;
      ClassWithAllDataTypes loadedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      DataContainer loadedObjectDataContainer = newObject.InternalDataContainer;

      Assert.That(newObject.State.IsInvalid, Is.False);
      Assert.That(newObject.State.IsNew, Is.True);
      Assert.That(loadedObject.State.IsInvalid, Is.False);
      Assert.That(loadedObject.State.IsUnchanged, Is.True);

      newObject.Delete();

      Assert.That(newObject.State.IsInvalid, Is.True);
      Assert.That(newObjectDataContainer.State.IsDiscarded, Is.True);

      loadedObject.Delete();
      TestableClientTransaction.Commit();

      Assert.That(loadedObject.State.IsInvalid, Is.True);
      Assert.That(loadedObjectDataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void StateInRootAndSubTransaction ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      customer.Name = "New name";

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(customer.State.IsNotLoadedYet, Is.True);

        customer.EnsureDataAvailable();

        Assert.That(customer.State.IsUnchanged, Is.True);
        Assert.That(customer.TransactionContext[TestableClientTransaction].State.IsChanged, Is.True);

        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          Assert.That(customer.TransactionContext[TestableClientTransaction].State.IsChanged, Is.True); // must not throw a ClientTransactionDiffersException
        }
      }
    }

    [Test]
    public void IsInvalidInTransaction ()
    {
      var discardedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      var nonDiscardedObject = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes>();

      discardedObject.Delete();
      TestableClientTransaction.Commit();

      Assert.That(discardedObject.State.IsInvalid, Is.True);
      Assert.That(nonDiscardedObject.State.IsInvalid, Is.False);

      Assert.That(discardedObject.TransactionContext[TestableClientTransaction].State.IsInvalid, Is.True);
      Assert.That(nonDiscardedObject.TransactionContext[TestableClientTransaction].State.IsInvalid, Is.False);
    }

    [Test]
    public void DoesNotPerformMaxLengthCheck ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      const string tooLongName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901";
      customer.Name = tooLongName;
      Assert.That(customer.Name, Is.EqualTo(tooLongName));
    }

    [Test]
    public void TypeCheck ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      const int invalidName = 123;
      Assert.That(
          () => customer.NamePropertyOfInvalidType = invalidName,
          Throws.InstanceOf<InvalidTypeException>());
    }

    [Test]
    public void TestAllOperations ()
    {
      Order order1 = DomainObjectIDs.Order1.GetObject<Order>();
      Order order3 = DomainObjectIDs.Order3.GetObject<Order>();

      Customer customer1 = order1.Customer;
      Customer customer4 = DomainObjectIDs.Customer4.GetObject<Customer>();

      Order order4 = customer4.Orders[DomainObjectIDs.Order4];
      Dev.Null = customer4.Orders[DomainObjectIDs.Order5];

      OrderTicket orderTicket1 = order1.OrderTicket;
      OrderTicket orderTicket3 = order3.OrderTicket;

      Official official1 = order1.Official;

      var orderItem1 = order1.OrderItems[DomainObjectIDs.OrderItem1];
      var orderItem2 = order1.OrderItems[DomainObjectIDs.OrderItem2];
      Dev.Null = order4.OrderItems[DomainObjectIDs.OrderItem4];

      order1.Delete();
      orderItem1.Delete();
      orderItem2.Delete();

      order4.OrderNumber = 7;

      Order newOrder = Order.NewObject();
      ObjectID newOrderID = newOrder.ID;
      newOrder.DeliveryDate = DateTime.Now;
      newOrder.Official = official1;
      customer1.Orders.Add(newOrder);

      newOrder.OrderTicket = orderTicket1;
      orderTicket1.FileName = @"C:\NewFile.tif";

      OrderItem newOrderItem1 = OrderItem.NewObject();
      newOrderItem1.Product = "Product 1";
      ObjectID newOrderItem1ID = newOrderItem1.ID;

      newOrderItem1.Position = 1;
      newOrder.OrderItems.Add(newOrderItem1);

      OrderItem newOrderItem2 = OrderItem.NewObject();
      newOrderItem2.Product = "Product 2";
      ObjectID newOrderItem2ID = newOrderItem2.ID;
      newOrderItem2.Position = 2;
      order4.OrderItems.Add(newOrderItem2);

      Customer newCustomer = Customer.NewObject();
      newCustomer.Name = "Customer";
      ObjectID newCustomerID = newCustomer.ID;

      Ceo newCeo = Ceo.NewObject();
      newCeo.Name = "CEO";
      ObjectID newCeoID = newCeo.ID;
      newCustomer.Ceo = newCeo;
      order3.Customer = newCustomer;

      orderTicket3.FileName = @"C:\NewFile.gif";

      Order deletedNewOrder = Order.NewObject();
      deletedNewOrder.Delete();

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      CheckIfObjectIsDeleted(DomainObjectIDs.Order1);
      CheckIfObjectIsDeleted(DomainObjectIDs.OrderItem1);
      CheckIfObjectIsDeleted(DomainObjectIDs.OrderItem2);

      order4 = DomainObjectIDs.Order4.GetObject<Order>();
      Assert.That(order4.OrderNumber, Is.EqualTo(7));

      newOrder = newOrderID.GetObject<Order>();
      Assert.That(newOrder, Is.Not.Null);

      official1 = DomainObjectIDs.Official1.GetObject<Official>();
      Assert.That(official1.Orders[newOrderID], Is.Not.Null);
      Assert.That(newOrder.Official, Is.SameAs(official1));
      Assert.That(official1.Orders[DomainObjectIDs.Order1], Is.Null);

      orderTicket1 = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      Assert.That(orderTicket1.FileName, Is.EqualTo(@"C:\NewFile.tif"));
      Assert.That(orderTicket1.Order, Is.SameAs(newOrder));
      Assert.That(newOrder.OrderTicket, Is.SameAs(orderTicket1));

      newOrderItem1 = newOrderItem1ID.GetObject<OrderItem>();
      Assert.That(newOrderItem1, Is.Not.Null);
      Assert.That(newOrderItem1.Position, Is.EqualTo(1));
      Assert.That(newOrderItem1.Order, Is.SameAs(newOrder));
      Assert.That(newOrder.OrderItems[newOrderItem1ID], Is.Not.Null);

      newOrderItem2 = newOrderItem2ID.GetObject<OrderItem>();
      Assert.That(newOrderItem2, Is.Not.Null);
      Assert.That(newOrderItem2.Position, Is.EqualTo(2));
      Assert.That(newOrderItem2.Order, Is.SameAs(order4));
      Assert.That(order4.OrderItems[newOrderItem2ID], Is.Not.Null);

      newCustomer = newCustomerID.GetObject<Customer>();
      newCeo = newCeoID.GetObject<Ceo>();

      Assert.That(newCeo.Company, Is.SameAs(newCustomer));
      Assert.That(newCustomer.Ceo, Is.SameAs(newCeo));
      Assert.That(newCustomer.Orders.Contains(DomainObjectIDs.Order3), Is.True);
      Assert.That(newCustomer.Orders[DomainObjectIDs.Order3].Customer, Is.SameAs(newCustomer));

      orderTicket3 = DomainObjectIDs.OrderTicket3.GetObject<OrderTicket>();
      Assert.That(orderTicket3.FileName, Is.EqualTo(@"C:\NewFile.gif"));
    }

    [Test]
    public void TestAllOperationsWithHierarchy ()
    {
      Employee newSupervisor1 = Employee.NewObject();
      newSupervisor1.Name = "Supervisor 1";
      ObjectID newSupervisor1ID = newSupervisor1.ID;

      Employee newSubordinate1 = Employee.NewObject();
      newSubordinate1.Name = "Subordinate 1";
      ObjectID newSubordinate1ID = newSubordinate1.ID;
      newSubordinate1.Supervisor = newSupervisor1;

      Employee supervisor1 = DomainObjectIDs.Employee1.GetObject<Employee>();
      Employee subordinate4 = DomainObjectIDs.Employee4.GetObject<Employee>();

      Employee supervisor2 = DomainObjectIDs.Employee2.GetObject<Employee>();
      Employee subordinate3 = DomainObjectIDs.Employee3.GetObject<Employee>();
      supervisor2.Supervisor = supervisor1;
      supervisor2.Name = "New name of supervisor";
      subordinate3.Name = "New name of subordinate";

      Employee supervisor6 = DomainObjectIDs.Employee6.GetObject<Employee>();
      Dev.Null = DomainObjectIDs.Employee7.GetObject<Employee>();

      Employee newSubordinate2 = Employee.NewObject();
      newSubordinate2.Name = "Subordinate 2";
      ObjectID newSubordinate2ID = newSubordinate2.ID;

      Employee newSubordinate3 = Employee.NewObject();
      newSubordinate3.Name = "Subordinate 3";
      ObjectID newSubordinate3ID = newSubordinate3.ID;

      newSupervisor1.Supervisor = supervisor2;
      newSubordinate2.Supervisor = supervisor1;
      newSubordinate3.Supervisor = supervisor6;

      supervisor1.Delete();
      subordinate4.Delete();

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      newSupervisor1 = newSupervisor1ID.GetObject<Employee>();
      newSubordinate1 = newSubordinate1ID.GetObject<Employee>();

      Assert.That(newSubordinate1.Supervisor, Is.SameAs(newSupervisor1));
      Assert.That(newSupervisor1.Subordinates.Contains(newSubordinate1ID), Is.True);

      supervisor2 = DomainObjectIDs.Employee2.GetObject<Employee>();

      Assert.That(supervisor2.Supervisor, Is.Null);
      Assert.That(supervisor2.Name, Is.EqualTo("New name of supervisor"));

      subordinate3 = DomainObjectIDs.Employee3.GetObject<Employee>();

      Assert.That(subordinate3.Supervisor, Is.SameAs(supervisor2));
      Assert.That(supervisor2.Subordinates.Contains(DomainObjectIDs.Employee3), Is.True);
      Assert.That(subordinate3.Name, Is.EqualTo("New name of subordinate"));

      Assert.That(newSupervisor1.Supervisor, Is.SameAs(supervisor2));
      Assert.That(supervisor2.Subordinates.Contains(newSupervisor1ID), Is.True);

      newSubordinate2 = newSubordinate2ID.GetObject<Employee>();

      Assert.That(newSubordinate2.Supervisor, Is.Null);

      supervisor6 = DomainObjectIDs.Employee6.GetObject<Employee>();
      newSubordinate3 = newSubordinate3ID.GetObject<Employee>();

      Assert.That(newSubordinate3.Supervisor, Is.SameAs(supervisor6));
      Assert.That(supervisor6.Subordinates.Contains(newSubordinate3ID), Is.True);

      CheckIfObjectIsDeleted(DomainObjectIDs.Employee1);
      CheckIfObjectIsDeleted(DomainObjectIDs.Employee4);
    }

    [Test]
    public void DeleteNewObjectWithExistingRelated ()
    {
      Computer computer4 = DomainObjectIDs.Computer4.GetObject<Computer>();

      Employee newDeletedEmployee = Employee.NewObject();
      computer4.Employee = newDeletedEmployee;

      newDeletedEmployee.Delete();

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      computer4 = DomainObjectIDs.Computer4.GetObject<Computer>();
      Assert.That(computer4.Employee, Is.Null);
    }

    [Test]
    public void ExistingObjectRelatesToNewAndDeleted ()
    {
      Partner partner = DomainObjectIDs.Partner2.GetObject<Partner>();

      Person newPerson = Person.NewObject();
      newPerson.Name = "Jane Doe";
      partner.ContactPerson = newPerson;
      partner.IndustrialSector.Delete();

      TestableClientTransaction.Commit();
      ReInitializeTransaction();

      partner = DomainObjectIDs.Partner2.GetObject<Partner>();
      Assert.That(partner.ContactPerson.ID, Is.EqualTo(newPerson.ID));
      Assert.That(partner.IndustrialSector, Is.Null);
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrder ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      Assert.That(customer.Orders[0].ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(customer.Orders[1].ID, Is.EqualTo(DomainObjectIDs.Order2));
    }

    [Test]
    public void GetRelatedObjectsWithCorrectOrderWithLazyLoad ()
    {
      Customer customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      Dev.Null = DomainObjectIDs.Order2.GetObject<Order>();

      Assert.That(customer.Orders[0].ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(customer.Orders[1].ID, Is.EqualTo(DomainObjectIDs.Order2));
    }


    [Test]
    public void MultiplePropertiesWithSameShortName ()
    {
      var derivedClass = (DerivedClassWithDifferentProperties)LifetimeService.NewObject(TestableClientTransaction, typeof(DerivedClassWithDifferentProperties), ParamList.Empty);
      ClassWithDifferentProperties baseClass = derivedClass;

      derivedClass.String = "Derived";
      baseClass.String = "Base";

      Assert.That(derivedClass.String, Is.EqualTo("Derived"));
      Assert.That(baseClass.String, Is.EqualTo("Base"));

      baseClass.String = "NewBase";
      derivedClass.String = "NewDerived";

      Assert.That(derivedClass.String, Is.EqualTo("NewDerived"));
      Assert.That(baseClass.String, Is.EqualTo("NewBase"));
    }
  }
}
