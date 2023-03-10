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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionCommitValidationTest : ClientTransactionBaseTest
  {
    [Test]
    public void CommitWithOptionalOneToOneRelationNotSet ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee>();
      employee.Computer = null;

      Assert.That(() => TestableClientTransaction.Commit(), Throws.Nothing);
    }

    [Test]
    public void CommitWithOptionalOneToManyRelationNotSet ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      foreach (var order in customer.Orders.ToArray())
      {
        order.Customer = DomainObjectIDs.Customer2.GetObject<Customer>();
      }

      Assert.That(customer.Orders, Is.Empty);
      Assert.That(() => TestableClientTransaction.Commit(), Throws.Nothing);
    }

    [Test]
    public void CommitWithMandatoryOneToOneRelationNotSet ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      OrderTicket newOrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>();

      order.OrderTicket = newOrderTicket;

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object"
                + " 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be null."));
      }
    }

    [Test]
    public void CommitWithMandatoryOneToManyRelationNotSet ()
    {
      IndustrialSector industrialSector = DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector>();
      industrialSector.Companies.Clear();

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object"
                + " 'IndustrialSector|8565a077-ea01-4b5d-beaa-293dc484bddc|System.Guid' contains no items."));
      }
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToOneRelation ()
    {
      OrderTicket newOrderTicket = OrderTicket.NewObject();
      newOrderTicket.FileName = @"C:\temp\newOrder.tkt";

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                    string.Format(
                        "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object '{0}' cannot be null.",
                        newOrderTicket.ID))
                .And.Property<MandatoryRelationNotSetException>(ex => ex.PropertyName).EqualTo(
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order")
                .And.Property<MandatoryRelationNotSetException>(ex => ex.DomainObject).SameAs(newOrderTicket));
      }
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToManyRelation ()
    {
      IndustrialSector newIndustrialSector = IndustrialSector.NewObject();
      newIndustrialSector.Name = "Sector";

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                    string.Format(
                        "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object '{0}' contains no items.",
                        newIndustrialSector.ID))
                .And.Property<MandatoryRelationNotSetException>(ex => ex.PropertyName).EqualTo(
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies")
                .And.Property<MandatoryRelationNotSetException>(ex => ex.DomainObject).SameAs(newIndustrialSector));
      }
    }

    [Test]
    public void CommitWithValidProperties ()
    {
      var newObject = CreateClassWithAllDataTypes();
      newObject.PopulateMandatoryProperties();
      newObject.StringWithNullValueProperty = null;
      newObject.NullableBinaryProperty = null;

      Assert.That(() => TestableClientTransaction.Commit(), Throws.Nothing);
    }

    [Test]
    public void ExceptionForNotNullableStringPropertyNotSet ()
    {
      var newObject = CreateClassWithAllDataTypes();
      newObject.PopulateMandatoryProperties();
      newObject.StringProperty = null;

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<PropertyValueNotSetException>().With.Message.EqualTo(
                    string.Format(
                        "Not-nullable property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty' of domain object '{0}' cannot be null.",
                        newObject.ID))
                .And.Property<PropertyValueNotSetException>(ex => ex.PropertyName).EqualTo(
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty")
                .And.Property<PropertyValueNotSetException>(ex => ex.DomainObject).SameAs(newObject));
      }
    }

    [Test]
    public void ExceptionForNotNullableBinaryPropertyNotSet ()
    {
      var newObject = CreateClassWithAllDataTypes();
      newObject.PopulateMandatoryProperties();
      newObject.BinaryProperty = null;

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<PropertyValueNotSetException>().With.Message.EqualTo(
                    string.Format(
                        "Not-nullable property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty' of domain object '{0}' cannot be null.",
                        newObject.ID))
                .And.Property<PropertyValueNotSetException>(ex => ex.PropertyName).EqualTo(
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty")
                .And.Property<PropertyValueNotSetException>(ex => ex.DomainObject).SameAs(newObject));
      }
    }

    [Test]
    public void ExceptionForStringPropertyExceedingMaxLength ()
    {
      var newObject = CreateClassWithAllDataTypes();
      newObject.PopulateMandatoryProperties();
      newObject.StringProperty = new string('x', 101);

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<PropertyValueTooLongException>()
                .With.Message.EqualTo(
                    string.Format(
                        "Value for property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty' of domain object '{0}' is too long. "
                        + "Maximum number of characters: 100.",
                        newObject.ID))
                .And.Property<PropertyValueTooLongException>(ex => ex.PropertyName)
                .EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty")
                .And.Property<PropertyValueTooLongException>(ex => ex.DomainObject).SameAs(newObject));
      }
    }

    [Test]
    public void ExceptionForBinaryPropertyExceedingMaxLength ()
    {
      var newObject = CreateClassWithAllDataTypes();
      newObject.PopulateMandatoryProperties();
      newObject.TransactionOnlyBinaryProperty = new byte[1000001];

      using (var _ = DatabaseAgent.OpenNoDatabaseWriteSection())
      {
        Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<PropertyValueTooLongException>()
                .With.Message.EqualTo(
                    string.Format(
                        "Value for property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.TransactionOnlyBinaryProperty' of domain object '{0}' is too large. "
                        + "Maximum size: 1000000.",
                        newObject.ID))
                .And.Property<PropertyValueTooLongException>(ex => ex.PropertyName)
                .EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.TransactionOnlyBinaryProperty")
                .And.Property<PropertyValueTooLongException>(ex => ex.DomainObject).SameAs(newObject));
      }
    }

    private ClassWithAllDataTypes CreateClassWithAllDataTypes ()
    {
      ClassWithAllDataTypes newObject = ClassWithAllDataTypes.NewObject();
      newObject.DateProperty = DateTime.Now;
      newObject.DateTimeProperty = DateTime.Now;
      newObject.StringProperty = "value";
      newObject.StringPropertyWithoutMaxLength = "value";
      newObject.TransactionOnlyStringProperty = "value";
      newObject.BinaryProperty = new byte[10];
      newObject.TransactionOnlyBinaryProperty = new byte[10];
      return newObject;
    }
  }
}
