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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class ObjectIDIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void CoalesceExpression_UsesIDValue ()
    {
      var query = from e in QueryFactory.CreateLinqQuery<Employee>()
        where (e.Computer ?? (DomainObject)e).ID.Value == DomainObjectIDs.Employee2.Value
        select e;

      CheckQueryResult(query, DomainObjectIDs.Employee2);
    }

    [Test]
    public void CoalesceExpression_UsesCompoundID_ThrowsNotSupportedException ()
    {
      var query = from e in QueryFactory.CreateLinqQuery<Employee>()
        where (e.Computer ?? (DomainObject)e).ID == DomainObjectIDs.Employee2
        select e;
      Assert.That(
          () => CheckQueryResult(query, DomainObjectIDs.Employee2),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
#if NETFRAMEWORK
                  "There was an error preparing or resolving query "
                  + "'from Employee e in DomainObjectQueryable<Employee> "
                  + "where (([e].Computer ?? Convert([e])).ID == Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid) select [e]' for SQL generation. "
                  + "Cannot use a complex expression ('new ObjectID(ClassID = [t1].[ClassID] AS ClassID, Value = Convert([t1].[ID] AS Value))') in a place "
                  + "where SQL requires a single value."
#else
                  "There was an error preparing or resolving query "
                  + "'from Employee e in DomainObjectQueryable<Employee> "
                  + "where (([e].Computer ?? Convert([e], DomainObject)).ID == Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid) select [e]' for SQL generation. "
                  + "Cannot use a complex expression ('new ObjectID(ClassID = [t1].[ClassID] AS ClassID, Value = Convert([t1].[ID] AS Value, Object))') in a place "
                  + "where SQL requires a single value."
#endif
                  ));
    }

    [Test]
    public void ConditionalExpression_UsesIDValue ()
    {
      var query = from e in QueryFactory.CreateLinqQuery<Employee>()
        where (e.Computer.ID.Value == DomainObjectIDs.Computer1.Value ? e.Computer : (DomainObject)e).ID.Value == DomainObjectIDs.Computer1.Value
        select e;

      CheckQueryResult(query, DomainObjectIDs.Employee3);
    }

    [Test]
    public void ConditionalExpression_UsesCompundID_ThrowsNotSupportedException ()
    {
      var query = from e in QueryFactory.CreateLinqQuery<Employee>()
        where (e.Computer.ID == DomainObjectIDs.Computer1 ? e.Computer : (DomainObject)e).ID == DomainObjectIDs.Computer1
        select e;
      Assert.That(
          () => CheckQueryResult(query, DomainObjectIDs.Employee3),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
#if NETFRAMEWORK
                  "There was an error preparing or resolving query "
                  + "'from Employee e in DomainObjectQueryable<Employee> "
                  + "where (IIF(([e].Computer.ID == Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid), Convert([e].Computer), Convert([e])).ID == "
                  + "Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid) select [e]' for SQL generation. Cannot use a complex expression "
                  + "('new ObjectID(ClassID = [t1].[ClassID] AS ClassID, Value = Convert([t1].[ID] AS Value))') in a place where SQL requires a single value."
#else
                  "There was an error preparing or resolving query "
                  + "'from Employee e in DomainObjectQueryable<Employee> "
                  + "where (IIF(([e].Computer.ID == Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid), Convert([e].Computer, DomainObject), Convert([e], DomainObject)).ID == "
                  + "Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid) select [e]' for SQL generation. Cannot use a complex expression "
                  + "('new ObjectID(ClassID = [t1].[ClassID] AS ClassID, Value = Convert([t1].[ID] AS Value, Object))') in a place where SQL requires a single value."
#endif
                  ));
    }

    [Test]
    public void Query_ReturnsCompoundID ()
    {
      var result =
          (from o in QueryFactory.CreateLinqQuery<Order>()
            where o.OrderNumber == 1
            select o.ID).Single();

      Assert.That(result, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void Query_UsesCompoundID_InWhereClause ()
    {
      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee>();
      var computers =
          from c in QueryFactory.CreateLinqQuery<Computer>()
          where c.Employee.ID == employee.ID
          select c;

      CheckQueryResult(computers, DomainObjectIDs.Computer1);
    }

    [Test]
    public void Query_UsesIDValue_InWhereClause ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().Where(c => c.ID.Value == DomainObjectIDs.Customer1.Value);

      CheckQueryResult(query, DomainObjectIDs.Customer1);
    }

    [Test]
    public void Query_UsesClassID_InWhereClause ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().Where(c => c.ID.ClassID == "Customer");

      CheckQueryResult(
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_UsesIDValue_OnColumnOfReferencedEntity ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company>() select new { A = c, B = c.ID }).Distinct()
                  where x.A.ID.Value == DomainObjectIDs.Customer1.Value
                  select x.A;

      CheckQueryResult(query, DomainObjectIDs.Customer1);
    }

    [Test]
    public void Query_UsesClassID_OnPropertyOfReferencedEntity ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company>() select new { A = c, B = c.ID }).Distinct()
                  where x.A.ID.ClassID == "Customer"
                  select x.A;

      CheckQueryResult(
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void Query_UsesIDValue_OnReferencedValue ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company>() select new { A = c, B = c.ID }).Distinct()
                  where x.B.Value == DomainObjectIDs.Customer1.Value
                  select x.A;

      CheckQueryResult(query, DomainObjectIDs.Customer1);
    }

    [Test]
    public void Query_UsesClassID_OnReferencedValue ()
    {
      var query = from x in
                    (from c in QueryFactory.CreateLinqQuery<Company>() select new { A = c, B = c.ID }).Distinct()
                  where x.B.ClassID == "Customer"
                  select x.A;

      CheckQueryResult(
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void QueryWithContainsInWhere_OnCollection_WithObjectIDValues ()
    {
      var possibleItems = new[] { DomainObjectIDs.Order1.Value, DomainObjectIDs.Order3.Value };
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where possibleItems.Contains(o.ID.Value)
          select o;

      CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3);
    }

    [Test]
    public void QueryWithContainsInWhere_OnCollection_WithObjectIDs_ThrowsNotSupportedException ()
    {
      var possibleItems = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 };
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where possibleItems.Contains(o.ID)
          select o;
      Assert.That(
          () => CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
#if NETFRAMEWORK
                  "There was an error preparing or resolving query "
                  + "'from Order o in DomainObjectQueryable<Order> where {value(Remotion.Data.DomainObjects.ObjectID[]) => Contains([o].ID)} select [o]' for "
                  + "SQL generation. The SQL 'IN' operator (originally probably a call to a 'Contains' method) requires a single value, so the following "
                  + "expression cannot be translated to SQL: "
                  + "'new ObjectID(ClassID = [t0].[ClassID] AS ClassID, Value = Convert([t0].[ID] AS Value)) "
                  + "IN [Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid, Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid]'."
#else
                  "There was an error preparing or resolving query "
                  + "'from Order o in DomainObjectQueryable<Order> where {value(Remotion.Data.DomainObjects.ObjectID[]) => Contains([o].ID)} select [o]' for "
                  + "SQL generation. The SQL 'IN' operator (originally probably a call to a 'Contains' method) requires a single value, so the following "
                  + "expression cannot be translated to SQL: "
                  + "'new ObjectID(ClassID = [t0].[ClassID] AS ClassID, Value = Convert([t0].[ID] AS Value, Object)) "
                  + "IN [Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid, Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid]'."
#endif
                  ));
    }

    [Test]
    public void QueryWithContainsInWhere_OnCollection_WithObjects_ThrowsNotSupportedException ()
    {
      var possibleItems = new[]
                          {
                              LifetimeService.GetObjectReference(ClientTransaction.Current, DomainObjectIDs.Order1),
                              LifetimeService.GetObjectReference(ClientTransaction.Current, DomainObjectIDs.Order3)
                          };
      var orders =
          from o in QueryFactory.CreateLinqQuery<Order>()
          where possibleItems.Contains(o)
          select o;
      Assert.That(
          () => CheckQueryResult(orders, DomainObjectIDs.Order1, DomainObjectIDs.Order3),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Objects of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order_AssembledTypeProxy_1' cannot be used as data parameter value."));
    }

    [Test]
    public void GroupBy_UsesObjectID_AsKey ()
    {
      var query = from o in QueryFactory.CreateLinqQuery<Order>()
                  group o by o.Customer.ID into ordersByCustomer
                  from c in QueryFactory.CreateLinqQuery<Customer>()
                  where c.ID == ordersByCustomer.Key
                  select c;

      CheckQueryResult(query, DomainObjectIDs.Customer1, DomainObjectIDs.Customer3, DomainObjectIDs.Customer4, DomainObjectIDs.Customer5);
    }
  }
}
