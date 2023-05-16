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
  public class SubTransactionCommitValidationTest : ClientTransactionBaseTest
  {
    [Test]
    public void CommitWithOptionalOneToOneRelationNotSet ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Employee employee = DomainObjectIDs.Employee3.GetObject<Employee>();
        employee.Computer = null;

        Assert.That(() => subTransaction.Commit(), Throws.Nothing);
      }
    }

    [Test]
    public void CommitWithOptionalOneToManyRelationNotSet ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
        foreach (var order in customer.Orders.ToArray())
        {
          order.Customer = DomainObjectIDs.Customer2.GetObject<Customer>();
        }

        Assert.That(customer.Orders, Is.Empty);
        Assert.That(() => subTransaction.Commit(), Throws.Nothing);
      }
    }

    [Test]
    public void CommitWithMandatoryOneToOneRelationNotSet ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Order order = DomainObjectIDs.Order1.GetObject<Order>();
        OrderTicket newOrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>();

        order.OrderTicket = newOrderTicket;

        Assert.That(() => subTransaction.Commit(), Throws.Nothing);
      }

      Assert.That(
          () => TestableClientTransaction.Commit(),
          Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
              "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object"
              + " 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be null."));
    }

    [Test]
    public void CommitWithMandatoryOneToManyRelationNotSet ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        IndustrialSector industrialSector = DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector>();
        industrialSector.Companies.Clear();
        Assert.That(() => subTransaction.Commit(), Throws.Nothing);
      }

      Assert.That(
          () => TestableClientTransaction.Commit(),
          Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
              "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object"
              + " 'IndustrialSector|8565a077-ea01-4b5d-beaa-293dc484bddc|System.Guid' contains no items."));
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToOneRelation ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();

      OrderTicket newOrderTicket;
      using (subTransaction.EnterDiscardingScope())
      {
        newOrderTicket = OrderTicket.NewObject();
        newOrderTicket.FileName = @"C:\temp\newOrder.tkt";
        Assert.That(() => subTransaction.Commit(), Throws.Nothing);
      }

      Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                string.Format(
                    "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object '{0}' cannot be null.",
                    newOrderTicket.ID)));
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToManyRelation ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();

      IndustrialSector newIndustrialSector;
      using (subTransaction.EnterDiscardingScope())
      {
        newIndustrialSector = IndustrialSector.NewObject();
        newIndustrialSector.Name = "Sector";
        Assert.That(() => subTransaction.Commit(), Throws.Nothing);
      }

      Assert.That(
            () => TestableClientTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                string.Format(
                    "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object '{0}' contains no items.",
                    newIndustrialSector.ID)));
    }

    [Test]
    public void CommitWithMandatoryOneToOneRelationNotSet_WithValidationExtension ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      subTransaction.Extensions.Add(new CommitValidationClientTransactionExtension(new MandatoryRelationValidator()));

      using (subTransaction.EnterDiscardingScope())
      {
        Order order = DomainObjectIDs.Order1.GetObject<Order>();
        OrderTicket newOrderTicket = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>();

        order.OrderTicket = newOrderTicket;

        Assert.That(
            () => subTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object"
                + " 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be null."));
      }
    }

    [Test]
    public void CommitWithMandatoryOneToManyRelationNotSet_WithValidationExtension ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      subTransaction.Extensions.Add(new CommitValidationClientTransactionExtension(new MandatoryRelationValidator()));

      using (subTransaction.EnterDiscardingScope())
      {
        IndustrialSector industrialSector = DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector>();
        industrialSector.Companies.Clear();

        Assert.That(
            () => subTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object"
                + " 'IndustrialSector|8565a077-ea01-4b5d-beaa-293dc484bddc|System.Guid' contains no items."));
      }
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToOneRelation_WithValidationExtension ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      subTransaction.Extensions.Add(new CommitValidationClientTransactionExtension(new MandatoryRelationValidator()));

      using (subTransaction.EnterDiscardingScope())
      {
        OrderTicket newOrderTicket = OrderTicket.NewObject();

        Assert.That(
            () => subTransaction.Commit(),
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
    public void MandatoryRelationNotSetExceptionForOneToManyRelation_WithValidationExtension ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      subTransaction.Extensions.Add(new CommitValidationClientTransactionExtension(new MandatoryRelationValidator()));

      using (subTransaction.EnterDiscardingScope())
      {
        IndustrialSector newIndustrialSector = IndustrialSector.NewObject();

        Assert.That(
            () => subTransaction.Commit(),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo(
                string.Format(
                    "Mandatory relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object '{0}' contains no items.",
                    newIndustrialSector.ID))
                .And.Property<MandatoryRelationNotSetException>(ex => ex.PropertyName).EqualTo(
                    "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies")
                .And.Property<MandatoryRelationNotSetException>(ex => ex.DomainObject).SameAs(newIndustrialSector));
      }
    }
  }
}
