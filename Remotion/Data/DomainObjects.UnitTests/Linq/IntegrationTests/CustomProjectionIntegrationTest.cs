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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class CustomProjectionIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void SequenceOfDomainObjectProperties ()
    {
      var result = from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber <= 5 select o.OrderNumber;

      Assert.That (result, Is.EquivalentTo (new[] { 1, 2, 3, 4, 5 }));
    }

    [Test]
    public void SequenceOfObjectIDs ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order> () where o.OrderNumber < 3 select o.ID).ToArray ();

      Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }));
    }

    [Test]
    public void SequenceOfObjectIDs_ConstructedInMemory ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Order>() where o.OrderNumber < 3 select new ObjectID(o.ID.ClassID, o.ID.Value)).ToArray();

      Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }));
    }

    [Test]
    public void SequenceOfForeignKeyIDs ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Order> ()
           where o.OrderNumber == 1
           select o.Customer.ID).ToArray ();

      Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Customer1 }));
    }

    [Test]
    public void ForeignKeyID_Null ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Computer> ()
           where o.ID == DomainObjectIDs.Computer4
           select o.Employee.ID).Single();

      Assert.That (result, Is.Null);
    }

    [Test]
    public void SequenceOfForeignKeyIDs_ConstructedInMemory ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Order> () 
           where o.OrderNumber == 1 
           select new ObjectID (o.Customer.ID.ClassID, o.Customer.ID.Value)).ToArray ();

      Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Customer1 }));
    }

    [Test]
    public void SequenceOfForeignKeyIDs_NonPersistedClassID_ConstructedInMemory ()
    {
      var endPointDefinition = GetNonVirtualEndPointDefinition (typeof (Computer), "Employee");
      Assert.That (endPointDefinition.PropertyDefinition.StoragePropertyDefinition, Is.TypeOf<ObjectIDWithoutClassIDStoragePropertyDefinition>());

      var result =
          (from c in QueryFactory.CreateLinqQuery<Computer> ()
           where c.ID == DomainObjectIDs.Computer1
           select new ObjectID(c.Employee.ID.ClassID, c.Employee.ID.Value)).ToArray ();

      Assert.That (result, Is.EquivalentTo (new[] { DomainObjectIDs.Employee3 }));
    }

    [Test]
    public void ComplexProjection ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order>()
                    where o.OrderNumber == 1
                    select new { o.OrderNumber, o.DeliveryDate, Property = new { o.OrderTicket.FileName, o.OrderItems.Count } }).ToArray();

      var expectedObject = new { OrderNumber = 1, DeliveryDate = new DateTime (2005, 1, 1), Property = new { FileName = @"C:\order1.png", Count = 2 } };
      Assert.That (result, Is.EqualTo (new[] { expectedObject}));
    }

    [Test]
    public void ComplexProjection_WithSingleQuery ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order> ()
                    where o.OrderNumber == 1
                    select new { o.OrderNumber, o.DeliveryDate, Property = new { o.OrderTicket.FileName, o.OrderItems.Count } }).Single ();

      var expectedObject = new { OrderNumber = 1, DeliveryDate = new DateTime (2005, 1, 1), Property = new { FileName = @"C:\order1.png", Count = 2 } };
      Assert.That (result, Is.EqualTo (expectedObject));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
      "This LINQ provider does not support queries with complex projections that include DomainObjects.\r\n"
      + "Either change the query to return just a sequence of DomainObjects (e.g., 'from o in QueryFactory.CreateLinqQuery<Order>() select o') "
      + "or change the complex projection to contain no DomainObjects "
      + "(e.g., 'from o in QueryFactory.CreateLinqQuery<Order>() select new { o.OrderNumber, o.OrderDate }').")]
    public void ComplexProjection_ContainingDomainObject ()
    {
      var result = (from o in QueryFactory.CreateLinqQuery<Order> () select new { o.OrderNumber, o });

      result.ToArray();
    }

    [Test]
    public void SingleBoolean ()
    {
      var result = (from c in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
                    where c.ID == DomainObjectIDs.ClassWithAllDataTypes1
                    select c.BooleanProperty).ToArray();

      Assert.That (result, Is.EqualTo (new[] { false }));
    }

    [Test]
    public void SingleNullableBoolean ()
    {
      var result = (from c in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
                    where c.ID == DomainObjectIDs.ClassWithAllDataTypes1
                    select c.NaBooleanProperty).ToArray();

      Assert.That (result, Is.EqualTo (new[] { true }));
    }

    [Test]
    public void ComplexProjection_WithBooleans ()
    {
      var result = (from c in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
                    where c.ID == DomainObjectIDs.ClassWithAllDataTypes1
                    select new { c.BooleanProperty, c.NaBooleanProperty, c.NaBooleanWithNullValueProperty }).ToArray();


      Assert.That (
          result,
          Is.EqualTo (new[] { new { BooleanProperty = false, NaBooleanProperty = (bool?) true, NaBooleanWithNullValueProperty = (bool?) null } }));
    }

  }
}