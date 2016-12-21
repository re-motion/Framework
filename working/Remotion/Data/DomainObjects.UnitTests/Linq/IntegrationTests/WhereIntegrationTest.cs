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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class WhereIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithStringLengthProperty ()
    {
      var computers =
          from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
          where oi.Product.Length == "Mainboard".Length
          select oi;

      CheckQueryResult (computers, DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void QueryWithStringIsNullOrEmpty ()
    {
      var computers =
          from oi in QueryFactory.CreateLinqQuery<OrderItem> ()
          where string.IsNullOrEmpty(oi.Product)
          select oi;

      CheckQueryResult (computers);
    }

    [Test]
    public void QueryWithWhereConditions ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.SerialNumber == "93756-ndf-23" || c.SerialNumber == "98678-abc-43"
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhereConditionsAndNull ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee != null
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3);
    }

    [Test]
    public void QueryWithBase ()
    {
      Company partner = DomainObjectIDs.Partner1.GetObject<Company> ();
      IQueryable<Company> result = (from c in QueryFactory.CreateLinqQuery<Company> ()
                                    where c.ID == partner.ID
                                    select c);
      CheckQueryResult (result, DomainObjectIDs.Partner1);
    }

    [Test]
    public void QueryWithWhereConditionAndStartsWith ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.SerialNumber.StartsWith ("9")
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhereConditionAndStartsWith_NonConstantValue ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer> ()
          where c.SerialNumber.StartsWith (QueryFactory.CreateLinqQuery<Computer> ().Select(sc=>sc.SerialNumber.Substring(0,1)).First())
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer2, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhereConditionAndEndsWith ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.SerialNumber.EndsWith ("7")
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer3);
    }

    [Test]
    public void QueryWithWhereConditionAndEndsWith_NonConstantValue ()
    {
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer> ()
          where c.SerialNumber.EndsWith (QueryFactory.CreateLinqQuery<Computer> ().Select (sc => sc.SerialNumber.Substring(1, 1)).First ())
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer5, DomainObjectIDs.Computer2);
    }

    [Test]
    public void QueryWithContains_Like ()
    {
      var ceos = from c in QueryFactory.CreateLinqQuery<Ceo> ()
                 where c.Name.Contains ("Sepp Fischer")
                 select c;
      CheckQueryResult (ceos, DomainObjectIDs.Ceo4);
    }

    [Test]
    public void QueryWithContains_Like_NonConstantValue ()
    {
      var ceos = from c in QueryFactory.CreateLinqQuery<Computer> ()
                 where c.SerialNumber.Contains (QueryFactory.CreateLinqQuery<Computer> ().Select (sc => sc.SerialNumber.Substring(1, 2)).First())
                 select c;
      CheckQueryResult (ceos, DomainObjectIDs.Computer5);
    }

    [Test]
    public void QueryWithWhere_OuterObject ()
    {
      Employee employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e == employee
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee1);
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyOnly()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where e.BooleanProperty
          select e;

      Assert.That(objectsWithAllDataTypes.Count(), Is.EqualTo(1));
    }

    [Test]
    public void QueryWithWhere_BooleanProperty_ExplicitComparison()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
// ReSharper disable RedundantBoolCompare
          where e.BooleanProperty == true
// ReSharper restore RedundantBoolCompare
          select e;

      Assert.That(objectsWithAllDataTypes.Count(), Is.EqualTo(1));
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyOnly_Negate()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where !e.BooleanProperty
          select e;

      Assert.That(objectsWithAllDataTypes.Count(), Is.EqualTo(1));
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother ()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
          where e.Int32Property == -2147483647 && e.BooleanProperty
          select e;

      CheckQueryResult (objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes2);
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother_Negate()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where e.Int32Property == 2147483647 && !e.BooleanProperty
          select e;

      CheckQueryResult(objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes1);
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother_ExplicitComparison_True()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
// ReSharper disable RedundantBoolCompare
          where e.Int32Property == -2147483647 && e.BooleanProperty == true
// ReSharper restore RedundantBoolCompare
          select e;

      CheckQueryResult(objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes2);
    }

    [Test]
    public void QueryWithWhere_BooleanPropertyAndAnother_ExplicitComparison_False()
    {
      var objectsWithAllDataTypes =
          from e in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
          where e.Int32Property == 2147483647 && e.BooleanProperty == false
          select e;

      CheckQueryResult(objectsWithAllDataTypes, DomainObjectIDs.ClassWithAllDataTypes1);
    }

    [Test]
    public void QueryWithWhere_LessThan ()
    {
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber <= 3
          select o;

      CheckQueryResult (orders, DomainObjectIDs.Order2, DomainObjectIDs.Order3, DomainObjectIDs.Order1);
    }

    [Test]
    public void QueryWithVirtualKeySide_EqualsNull ()
    {
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e.Computer == null
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee1, DomainObjectIDs.Employee2, DomainObjectIDs.Employee6, DomainObjectIDs.Employee7);
    }

    [Test]
    public void QueryWithVirtualKeySide_NotEqualsNull ()
    {
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e.Computer != null
          select e;
      CheckQueryResult (employees, DomainObjectIDs.Employee3, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5);
    }

    [Test]
    public void QueryWithVirtualKeySide_EqualsOuterObject ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e.Computer == computer
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee3);
    }

    [Test]
    public void QueryWithVirtualKeySide_NotEqualsOuterObject ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();
      var employees =
          from e in QueryFactory.CreateLinqQuery<Employee>()
          where e.Computer != computer
          select e;

      CheckQueryResult (employees, DomainObjectIDs.Employee4, DomainObjectIDs.Employee5);

    }

    [Test]
    public void QueryWithOuterEntityInCondition ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee == employee
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1);
    }

    [Test]
    public void QueryWithIDInCondition ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee.ID == employee.ID
          select c;

      CheckQueryResult (computers, DomainObjectIDs.Computer1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Query_WithUnsupportedType_NewObject ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where o.OrderNumber == 1
          select new { o, o.Customer };

      query.ToArray ();
    }

    [Test]
    public void QueryWithWhereOnForeignKey_RealSide ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem>()
                  where oi.Order.ID == id
                  select oi;
      CheckQueryResult (query, DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void QueryWithWhereOnForeignKey_VirtualSide ()
    {
      ObjectID id = DomainObjectIDs.Computer1;
      var query = from e in QueryFactory.CreateLinqQuery<Employee>()
                  where e.Computer.ID == id
                  select e;
      CheckQueryResult (query, DomainObjectIDs.Employee3);
    }

    [Test]
    public void TableInheritance_AccessingPropertyFromBaseClass ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<TIClassWithUnidirectionalRelation> ()
                  where c.DomainBase.CreatedAt == new DateTime (2006, 01, 04)
                  select c;
      CheckQueryResult (query, new TableInheritanceDomainObjectIDs (Configuration).ClassWithUnidirectionalRelation);
    }

    [Test]
    public void QueryWithConditionInWherePart ()
    {
      var query1 = from o in QueryFactory.CreateLinqQuery<Order> ()
#pragma warning disable 162
                   where o.OrderNumber == (true ? 1 : o.OrderNumber)
                   select o;

      var query2 = from o1 in QueryFactory.CreateLinqQuery<Order> ()
                   where o1.OrderNumber == (false ? 1 : o1.OrderNumber)
                   select o1;
#pragma warning restore 162

      var query3 = from o1 in QueryFactory.CreateLinqQuery<Order>()
                   where o1.OrderNumber == (o1.OrderNumber == 1 ? 2 : 3)
                   select o1;

      CheckQueryResult (query1, DomainObjectIDs.Order1);
      CheckQueryResult (
          query2,
          DomainObjectIDs.Order1,
          DomainObjectIDs.Order3,
          DomainObjectIDs.Order4,
          DomainObjectIDs.Order5,
          DomainObjectIDs.Order2,
          DomainObjectIDs.InvalidOrder,
          DomainObjectIDs.OrderWithoutOrderItems);
      CheckQueryResult (query3, DomainObjectIDs.Order3);
    }

    
    [Test]
    public void Query_Is ()
    {
      var query = QueryFactory.CreateLinqQuery<Company> ().Where (c => c is Customer);

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_ClassID ()
    {
      var query = QueryFactory.CreateLinqQuery<Company> ().Where (c => c.ID.ClassID == "Customer");

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_ClassID_OnPropertyOfReferencedEntity ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company> () select new { A = c, B = c.ID }).Distinct()
                  where x.A.ID.ClassID == "Customer"
                  select x.A;

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_ClassID_OnReferencedValue ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company> () select new { A = c, B = c.ID }).Distinct ()
                  where x.B.ClassID == "Customer"
                  select x.A;

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_IDValue ()
    {
      var query = QueryFactory.CreateLinqQuery<Company> ().Where (c => c.ID.Value == DomainObjectIDs.Customer1.Value);

      CheckQueryResult (query, DomainObjectIDs.Customer1);
    }

    [Test]
    public void Query_IDValue_OnColumnOfReferencedEntity ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company> () select new { A = c, B = c.ID }).Distinct ()
                  where x.A.ID.Value == DomainObjectIDs.Customer1.Value
                  select x.A;

      CheckQueryResult (query, DomainObjectIDs.Customer1);
    }

    [Test]
    public void Query_IDValue_OnReferencedValue ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company> () select new { A = c, B = c.ID }).Distinct ()
                  where x.B.Value == DomainObjectIDs.Customer1.Value
                  select x.A;

      CheckQueryResult (query, DomainObjectIDs.Customer1);
    }
  }
}
