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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class PropertyAccessorTest : ClientTransactionBaseTest
  {
    [Test]
    public void Construction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();
      IndustrialSector sector;
      using (transaction.EnterNonDiscardingScope())
      {
        sector = IndustrialSector.NewObject();
      }
      var data = new PropertyAccessorData(sector.ID.ClassDefinition, typeof(IndustrialSector).FullName + ".Name");

      var propertyAccessor = new PropertyAccessor(sector, data, transaction);
      Assert.That(propertyAccessor.DomainObject, Is.SameAs(sector));
      Assert.That(propertyAccessor.PropertyData, Is.SameAs(data));
      Assert.That(propertyAccessor.ClientTransaction, Is.SameAs(transaction));
    }

    [Test]
    public void GetValue_SetValue ()
    {
      IndustrialSector sector = IndustrialSector.NewObject();

      Company company = Company.NewObject();
      company.IndustrialSector = sector;
      Assert.AreSame(sector, company.IndustrialSector, "related object");

      Assert.IsTrue(sector.Companies.ContainsObject(company), "related objects");
      var newCompanies = new ObjectList<Company>();
      sector.Companies = newCompanies;
      Assert.That(sector.Companies, Is.SameAs(newCompanies));

      sector.Name = "Foo";
      Assert.That(sector.Name, Is.EqualTo("Foo"), "property value");
    }

    [Test]
    public void GetValue_SetValue_WithTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();
      IndustrialSector sector;
      using (transaction.EnterNonDiscardingScope())
      {
        sector = IndustrialSector.NewObject();
        sector.Name = "Foo";
      }
      var data = new PropertyAccessorData(sector.ID.ClassDefinition, typeof(IndustrialSector).FullName + ".Name");
      var accessor = new PropertyAccessor(sector, data, transaction);

      Assert.That(accessor.GetValue<string>(), Is.EqualTo("Foo"));
      accessor.SetValue("Bar");

      using (transaction.EnterNonDiscardingScope())
      {
        Assert.That(sector.Name, Is.EqualTo("Bar"));
      }
    }

    [Test]
    public void GetValue_ThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject();
      Assert.That(
          () => CreateAccessor(sector, "Name").GetValue<int>(),
          Throws.InstanceOf<InvalidTypeException>()
              .With.Message.EqualTo(
                  "Actual type 'System.String' of property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'."));
    }

    [Test]
    public void GetValue_ThrowsIfUnexpectedTypeOfResult ()
    {
      var invalidOrder = DomainObjectIDs.InvalidOrder.GetObject<Order>();
      Assert.That(
          () => CreateAccessor(invalidOrder, "Customer").GetValue<Customer>(),
          Throws.InstanceOf<InvalidTypeException>()
              .With.Message.Matches(
                  @"^The property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Order\.Customer' was expected to hold an object of type "
                  + @"'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Customer', but it returned an object of type "
                  + @"'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Company.*'\.$"));
    }

    [Test]
    public void SetValue_WithObjectList ()
    {
      IndustrialSector sector = IndustrialSector.NewObject();
      var newCompanies = new ObjectList<Company>();
      var oldCompanies = sector.Companies;

      CreateAccessor(sector, "Companies").SetValue(newCompanies);

      Assert.That(sector.Companies, Is.SameAs(newCompanies));
      Assert.That(sector.Companies.AssociatedEndPointID, Is.Not.Null);
      Assert.That(oldCompanies.AssociatedEndPointID, Is.Null);
    }

    [Test]
    public void SetValue_WithObjectList_PerformsBidirectionalChange ()
    {
      var sector = IndustrialSector.NewObject();
      var company = Company.NewObject();

      var newCompanies = new ObjectList<Company> { company };

      CreateAccessor(sector, "Companies").SetValue(newCompanies);

      Assert.That(company.IndustrialSector, Is.SameAs(sector));
    }

    [Test]
    public void SetValue_WithObjectList_Notifies ()
    {
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      IndustrialSector sector = IndustrialSector.NewObject();
      var newCompanies = new ObjectList<Company> { Company.NewObject() };

      var propertyAccessor = CreateAccessor(sector, "Companies");
      propertyAccessor.SetValue(newCompanies);

      listenerMock.Verify(
          mock => mock.RelationChanged(TestableClientTransaction, sector, propertyAccessor.PropertyData.RelationEndPointDefinition, null, newCompanies[0]),
          Times.AtLeastOnce());
    }

    [Test]
    public void SetValue_WithObjectList_SelfReplace ()
    {
      var sector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      var previousEndPointID = sector.Companies.AssociatedEndPointID;

      sector.Companies = sector.Companies;

      Assert.That(sector.Companies.AssociatedEndPointID, Is.SameAs(previousEndPointID));
    }

    [Test]
    public void SetValue_WithObjectList_NewCollectionAlreadyAssociated ()
    {
      var sector1 = IndustrialSector.NewObject();
      var sector2 = IndustrialSector.NewObject();
      Assert.That(
          () => CreateAccessor(sector1, "Companies").SetValue(sector2.Companies),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given collection is already associated with an end point.",
                  "value"));
    }

    [Test]
    public void SetValue_WithObjectList_CollectionIsReadOnly ()
    {
      var customer1 = Customer.NewObject();

      var newCollection = (OrderCollection)new OrderCollection().Clone(true);
      CreateAccessor(customer1, "Orders").SetValue(newCollection);

      Assert.That(customer1.Orders, Is.SameAs(newCollection));
    }

    [Test]
    public void SetValue_WithObjectList_DifferentCollectionTypes ()
    {
      var customer1 = Customer.NewObject();

      var newCollection = new ObjectList<Order>();
      Assert.That(
          () => CreateAccessor(customer1, "Orders").SetValueWithoutTypeCheck(newCollection),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The given collection ('Remotion.Data.DomainObjects.ObjectList`1[Remotion.Data.DomainObjects.UnitTests.TestDomain.Order]') is not of the same type "
                  + "as the end point's current opposite collection ('Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderCollection')."));
    }

    [Test]
    public void SetValue_WithObjectList_DifferentRequiredItemType ()
    {
      var customer1 = Customer.NewObject();

      var newCollection = new DomainObjectCollection(typeof(Customer));
      Assert.That(
          () => CreateAccessor(customer1, "Orders").SetValueWithoutTypeCheck(newCollection),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The given collection has a different item type than the end point's current opposite collection."));
    }

    [Test]
    public void SetValue_WithObjectList_ObjectDeleted ()
    {
      var sector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      sector.Delete();
      Assert.That(
          () => CreateAccessor(sector, "Companies").SetValue(new ObjectList<Company>()),
          Throws.InstanceOf<ObjectDeletedException>());
    }

    [Test]
    public void SetValue_WithRelatedObject ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var newTicket = OrderTicket.NewObject();

      CreateAccessor(order, "OrderTicket").SetValue(newTicket);

      Assert.That(order.OrderTicket, Is.SameAs(newTicket));
    }

    [Test]
    public void SetValue_WithRelatedObject_PerformsBidirectionalChange ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var newTicket = OrderTicket.NewObject();
      var oldTicket = order.OrderTicket;

      CreateAccessor(order, "OrderTicket").SetValue(newTicket);

      Assert.That(newTicket.Order, Is.SameAs(order));
      Assert.That(oldTicket.Order, Is.Null);
    }

    [Test]
    public void SetValue_WithRelatedObject_Notifies ()
    {
      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock(TestableClientTransaction);

      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var oldTicket = order.OrderTicket;
      var newTicket = OrderTicket.NewObject();

      var propertyAccessor = CreateAccessor(order, "OrderTicket");
      propertyAccessor.SetValue(newTicket);

      listenerMock.Verify(
          mock => mock.RelationChanged(TestableClientTransaction, order, propertyAccessor.PropertyData.RelationEndPointDefinition, oldTicket, newTicket),
          Times.AtLeastOnce());
    }

    [Test]
    public void SetValue_WithRelatedObject_ObjectDeleted ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      order.Delete();
      Assert.That(
          () => CreateAccessor(order, "OrderTicket").SetValue(OrderTicket.NewObject()),
          Throws.InstanceOf<ObjectDeletedException>());
    }

    [Test]
    public void SetValue_WithRelatedObject_NewObjectDeleted ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var newTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>();
      newTicket.Delete();
      Assert.That(
          () => CreateAccessor(order, "OrderTicket").SetValue(newTicket),
          Throws.InstanceOf<ObjectDeletedException>());
    }

    [Test]
    public void SetValue_WithRelatedObject_WithInvalidType ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var customer = DomainObjectIDs.Customer1.GetObject<Company>();
      Assert.That(
          () => CreateAccessor(order, "OrderTicket").SetValueWithoutTypeCheck(customer),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "DomainObject 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid' cannot be assigned "
                  + "to property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' "
                  + "of DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because it is not compatible with the type of the property.",
                  "newRelatedObject"));
    }

    [Test]
    public void SetValue_WithRelatedObject_WithCorrectDerivedType ()
    {
      var ceo = DomainObjectIDs.Ceo1.GetObject<Ceo>();
      var partnerCompany = DomainObjectIDs.Partner1.GetObject<Partner>();

      CreateAccessor(ceo, "Company").SetValueWithoutTypeCheck(partnerCompany);

      Assert.That(ceo.Company, Is.SameAs(partnerCompany));
    }

    [Test]
    public void SetValue_WithRelatedObject_WithInvalidBaseType ()
    {
      var person = DomainObjectIDs.Person1.GetObject<Person>();
      var company = DomainObjectIDs.Company1.GetObject<Company>();
      Assert.That(
          () => CreateAccessor(person, "AssociatedPartnerCompany").SetValueWithoutTypeCheck(company),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "DomainObject 'Company|c4954da8-8870-45c1-b7a3-c7e5e6ad641a|System.Guid' cannot be assigned "
                  + "to property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany' "
                  + "of DomainObject 'Person|2001bf42-2aa4-4c81-ad8e-73e9145411e9|System.Guid', because it is not compatible with the type of the property.",
                  "newRelatedObject"));
    }

    [Test]
    public void SetValue_WithRelatedObject_WithObjectNotEnlistedInThisTransaction ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderTicketFromOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<OrderTicket>(DomainObjectIDs.OrderTicket1);
      Assert.That(
          () => CreateAccessor(order, "OrderTicket").SetValueWithoutTypeCheck(orderTicketFromOtherTransaction),
          Throws.InstanceOf<ClientTransactionsDifferException>()
              .With.Message.EqualTo(
                  "Property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of DomainObject "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be set to DomainObject "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'. The objects do not belong to the same ClientTransaction."));
    }

    [Test]
    public void SetValue_ThrowsIfWrongType ()
    {
      IndustrialSector sector = IndustrialSector.NewObject();
      Assert.That(
          () => CreateAccessor(sector, "Name").SetValue(5),
          Throws.InstanceOf<InvalidTypeException>()
              .With.Message.EqualTo(
                  "Actual type 'System.String' of property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name' does not match expected type 'System.Int32'."));
    }

    [Test]
    public void HasChangedAndOriginalValueSimple ()
    {
      IndustrialSector sector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      PropertyAccessor property = CreateAccessor(sector, "Name");
      Assert.That(property.HasChanged, Is.False);
      var originalValue = property.GetValue<string>();
      Assert.That(originalValue, Is.Not.Null);
      Assert.That(property.GetOriginalValue<string>(), Is.EqualTo(originalValue));

      property.SetValue("Foo");
      Assert.That(property.HasChanged, Is.True);
      Assert.That(property.GetValue<string>(), Is.EqualTo("Foo"));
      Assert.That(property.GetOriginalValue<string>(), Is.Not.EqualTo(property.GetValue<string>()));
      Assert.That(property.GetOriginalValue<string>(), Is.EqualTo(originalValue));
    }

    [Test]
    public void HasChangedAndOriginalValueRelated ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>();
      PropertyAccessor property = CreateAccessor(computer, "Employee");
      Assert.That(property.HasChanged, Is.False);
      var originalValue = property.GetOriginalValue<Employee>();

      property.GetValue<Employee>().Name = "FooBarBazFred";
      Assert.That(property.HasChanged, Is.False);

      Employee newValue = Employee.NewObject();
      property.SetValue(newValue);
      Assert.That(property.HasChanged, Is.True);
      Assert.That(property.GetValue<Employee>(), Is.EqualTo(newValue));
      Assert.That(property.GetOriginalValue<Employee>(), Is.Not.EqualTo(property.GetValue<Employee>()));
      Assert.That(property.GetOriginalValue<Employee>(), Is.EqualTo(originalValue));
    }

    [Test]
    public void HasChangedAndOriginalValueRelatedCollection ()
    {
      IndustrialSector sector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      PropertyAccessor property = CreateAccessor(sector, "Companies");

      Assert.That(property.HasChanged, Is.False);
      var originalValue = property.GetValue<ObjectList<Company>>();
      int originalCount = originalValue.Count;
      Assert.That(originalValue, Is.Not.Null);
      Assert.That(property.GetOriginalValue<ObjectList<Company>>(), Is.EqualTo(originalValue));

      property.GetValue<ObjectList<Company>>().Add(Company.NewObject());
      Assert.That(property.GetValue<ObjectList<Company>>().Count, Is.Not.EqualTo(originalCount));
      Assert.That(property.HasChanged, Is.True);

      Assert.That(property.GetValue<ObjectList<Company>>(), Is.SameAs(originalValue));
      Assert.That(property.GetOriginalValue<ObjectList<Company>>(), Is.Not.SameAs(property.GetValue<ObjectList<Company>>()));
      Assert.That(property.GetOriginalValue<ObjectList<Company>>().Count, Is.EqualTo(originalCount));
    }

    [Test]
    public void GetOriginalValueThrowsWithWrongType ()
    {
      Assert.That(
          () => CreateAccessor(IndustrialSector.NewObject(), "Companies").GetOriginalValue<int>(),
          Throws.InstanceOf<InvalidTypeException>()
              .With.Message.Matches(
                  "Actual type .* of property .* does not match expected type 'System.Int32'"));
    }

    [Test]
    public void HasBeenTouchedSimple ()
    {
      IndustrialSector sector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      PropertyAccessor property = CreateAccessor(sector, "Name");

      Assert.That(property.HasBeenTouched, Is.False);
      property.SetValueWithoutTypeCheck(property.GetValueWithoutTypeCheck());
      Assert.That(property.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRelated ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>();
      PropertyAccessor property = CreateAccessor(computer, "Employee");

      Assert.That(property.HasBeenTouched, Is.False);
      property.SetValueWithoutTypeCheck(property.GetValueWithoutTypeCheck());
      Assert.That(property.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRelatedCollection ()
    {
      IndustrialSector sector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector>();
      PropertyAccessor property = CreateAccessor(sector, "Companies");

      Assert.That(property.HasBeenTouched, Is.False);
      ((DomainObjectCollection)property.GetValueWithoutTypeCheck())[0] = ((DomainObjectCollection)property.GetValueWithoutTypeCheck())[0];
      Assert.That(property.HasBeenTouched, Is.True);
    }

    [Test]
    public void IsNullPropertyValue ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.NewObject();

      // boolean starts with value, cannot be set to null 
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"].IsNull, Is.False);

      Assert.That(
          () => cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"].SetValue<bool?>(null),
          Throws.Exception.AssignableTo<InvalidTypeException>());
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"].IsNull, Is.False);

      // nullable boolean starts with null, can be set to value or null
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull, Is.True);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?>(true);
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull, Is.False);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?>(null);
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull, Is.True);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].SetValue<bool?>(null);
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"].IsNull, Is.True);

      // nullable string starts with null, can be set to value or null
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull, Is.True);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].SetValue("");
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull, Is.False);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].SetValue<string>(null);
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"].IsNull, Is.True);

      // string behaves like nullable string
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"].IsNull, Is.True);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"].SetValue("");
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"].IsNull, Is.False);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"].SetValue<string>(null);
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"].IsNull, Is.True);

      // nullable extensible enum starts with null, can be set to value or null
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].IsNull, Is.True);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].SetValue(Color.Values.Green());
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].IsNull, Is.False);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].SetValue<Color>(null);
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumWithNullValueProperty"].IsNull, Is.True);

      // extensible enum behaves like nullable extensible enum
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"].IsNull, Is.True);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"].SetValue(Color.Values.Green());
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"].IsNull, Is.False);

      cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"].SetValue<Color>(null);
      Assert.That(cwadt.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ExtensibleEnumProperty"].IsNull, Is.True);
    }

    [Test]
    public void IsNullRelatedObjectCollection ()
    {
      Order newOrder = Order.NewObject();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].IsNull, Is.False);
    }

    [Test]
    public void IsNullRelatedObjectNonVirtualEndPoint ()
    {
      Order newOrder = Order.NewObject();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull, Is.True);

      newOrder.Customer = Customer.NewObject();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull, Is.False);

      newOrder.Customer = null;
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull, Is.True);

      var eventReceiver = new ClientTransactionEventReceiver(ClientTransactionScope.CurrentTransaction);
      Order existingOrder = DomainObjectIDs.Order1.GetObject<Order>();

      eventReceiver.Clear();
      Assert.That(eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));

      Assert.That(existingOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].IsNull, Is.False);
      Assert.That(eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0), "The IsNull check did not cause the object to be loaded.");
    }

    [Test]
    public void IsNullRelatedObjectVirtualEndPoint ()
    {
      Order newOrder = Order.NewObject();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull, Is.True);

      newOrder.OrderTicket = OrderTicket.NewObject();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull, Is.False);

      newOrder.OrderTicket = null;
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull, Is.True);

      var eventReceiver = new ClientTransactionEventReceiver(ClientTransactionScope.CurrentTransaction);
      Order existingOrder = DomainObjectIDs.Order1.GetObject<Order>();

      eventReceiver.Clear();
      Assert.That(eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(0));

      Assert.That(existingOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].IsNull, Is.False);
      Assert.That(eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1), "For virtual end points, the IsNull unfortunately does cause a load.");

      Assert.That(existingOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValue<OrderTicket>() == null, Is.False);
      Assert.That(eventReceiver.LoadedDomainObjectLists.Count, Is.EqualTo(1), "An ordinary check does cause the object to be loaded.");
    }

    [Test]
    public void GetValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject();

      object ticket = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValueWithoutTypeCheck();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetValue<OrderTicket>(), Is.SameAs(ticket));

      object items = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValueWithoutTypeCheck();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetValue<ObjectList<OrderItem>>(), Is.SameAs(items));

      object number = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValueWithoutTypeCheck();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetValue<int>(), Is.EqualTo(number));
    }

    [Test]
    public void GetOriginalValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject();

      newOrder.OrderTicket = OrderTicket.NewObject();

      object ticket = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValueWithoutTypeCheck();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalValue<OrderTicket>(), Is.SameAs(ticket));

      newOrder.OrderItems.Add(OrderItem.NewObject());

      object items = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValueWithoutTypeCheck();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalValue<ObjectList<OrderItem>>(), Is.EqualTo(items));

      ++newOrder.OrderNumber;

      object number = newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValueWithoutTypeCheck();
      Assert.That(newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalValue<int>(), Is.EqualTo(number));
    }

    [Test]
    public void SetValueWithoutTypeCheck ()
    {
      Order newOrder = Order.NewObject();
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheck(7);
      Assert.That(newOrder.OrderNumber, Is.EqualTo(7));

      OrderTicket orderTicket = OrderTicket.NewObject();
      newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].SetValueWithoutTypeCheck(orderTicket);
      Assert.That(newOrder.OrderTicket, Is.SameAs(orderTicket));
    }

    [Test]
    public void SetValueWithoutTypeCheckThrowsOnWrongType ()
    {
      Order newOrder = Order.NewObject();
      Assert.That(
          () => newOrder.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].SetValueWithoutTypeCheck("7"),
          Throws.InstanceOf<InvalidTypeException>()
              .With.Message.EqualTo(
                  "Actual type 'System.String' of property "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber' does not match expected type 'System.Int32'."));
    }

    [Test]
    public void GetRelatedObjectIDSimple ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(
          () => order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetRelatedObjectID(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "This operation can only be used on related object properties."));
    }

    [Test]
    public void GetRelatedObjectIDRelatedRealEndPoint ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetRelatedObjectID(), Is.EqualTo(order.Customer.ID));
    }

    [Test]
    public void GetRelatedObjectIDRelatedVirtualEndPoint ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(
          () => order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetRelatedObjectID(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("ObjectIDs only exist on the real side of a relation, not on the virtual side."));
    }

    [Test]
    public void GetRelatedObjectIDRelatedCollection ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(
          () => order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetRelatedObjectID(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "This operation can only be used on related object properties."));
    }

    [Test]
    public void GetOriginalRelatedObjectIDSimple ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(
          () => order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].GetOriginalRelatedObjectID(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "This operation can only be used on related object properties."));
    }

    [Test]
    public void GetOriginalRelatedObjectIDRelatedRealEndPoint ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      ObjectID originalID = order.Customer.ID;
      order.Customer = Customer.NewObject();
      Assert.That(order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetOriginalRelatedObjectID(), Is.Not.EqualTo(order.Customer.ID));
      Assert.That(order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"].GetOriginalRelatedObjectID(), Is.EqualTo(originalID));
    }

    [Test]
    public void GetOriginalRelatedObjectIDRelatedVirtualEndPoint ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(
          () => order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"].GetOriginalRelatedObjectID(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("ObjectIDs only exist on the real side of a relation, not on the virtual side."));
    }

    [Test]
    public void GetOriginalRelatedObjectIDRelatedCollection ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(
          () => order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"].GetOriginalRelatedObjectID(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "This operation can only be used on related object properties."));
    }

    [Test]
    public void DiscardCheck ()
    {
      var order = Order.NewObject();
      order.Delete();

      PropertyAccessor property = order.Properties[typeof(Order), "OrderNumber"];

      ExpectDiscarded(() => Dev.Null = property.HasChanged);
      ExpectDiscarded(() => Dev.Null = property.HasBeenTouched);
      ExpectDiscarded(() => Dev.Null = property.IsNull);
      ExpectDiscarded(() => Dev.Null = property.GetOriginalRelatedObjectID());
      ExpectDiscarded(() => Dev.Null = property.GetOriginalValue<int>());
      ExpectDiscarded(() => Dev.Null = property.GetOriginalValueWithoutTypeCheck());
      ExpectDiscarded(() => Dev.Null = property.GetRelatedObjectID());
      ExpectDiscarded(() => Dev.Null = property.GetValue<int>());
      ExpectDiscarded(() => Dev.Null = property.GetValueWithoutTypeCheck());
      ExpectDiscarded(() => property.SetValue(0));
      ExpectDiscarded(() => property.SetValueWithoutTypeCheck(0));

      // no exceptions
      Dev.Null = property.PropertyData;
      Dev.Null = property.DomainObject;
    }

    private void ExpectDiscarded (Action action)
    {
      try
      {
        action();
        Assert.Fail("Expected ObjectInvalidException.");
      }
      catch (ObjectInvalidException)
      {
        // ok
      }
    }

    private static PropertyAccessor CreateAccessor (DomainObject domainObject, string shortIdentifier)
    {
      string propertyIdentifier = domainObject.GetPublicDomainObjectType().FullName + "." + shortIdentifier;
      var data = new PropertyAccessorData(domainObject.ID.ClassDefinition, propertyIdentifier);
      return new PropertyAccessor(domainObject, data, ClientTransaction.Current);
    }
  }
}
