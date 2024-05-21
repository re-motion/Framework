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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderLoadDataContainersByRelatedID : SqlProviderBaseTest
  {
    [Test]
    public void Loading ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Order), "Customer");
      var collection = Provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          null,
          DomainObjectIDs.Customer1).ToList();

      Assert.That(collection, Is.Not.Null);
      Assert.That(collection.Count, Is.EqualTo(2), "DataContainerCollection.Count");
      Assert.IsNotNull(collection.SingleOrDefault(o=>o.ID==DomainObjectIDs.Order1), "ID of Order with OrdnerNo 1");
      Assert.IsNotNull(collection.SingleOrDefault(o=>o.ID==DomainObjectIDs.Order2), "ID of Order with OrdnerNo 2");
    }

    [Test]
    public void LoadOverInheritedProperty ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Partner), "ContactPerson");

      var collection = Provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          null,
          DomainObjectIDs.Person6).ToList();

      Assert.That(collection.Count, Is.EqualTo(1));
      Assert.That(collection[0].ID, Is.EqualTo(DomainObjectIDs.Distributor2));
    }

    [Test]
    public void LoadWithOrderBy ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Order), "Customer");
      var orderNumberProperty = GetPropertyDefinition(typeof(Order), "OrderNumber");
      var sortExpression = new SortExpressionDefinition(new[] { new SortedPropertySpecification(orderNumberProperty, SortOrder.Ascending) });

      var orderContainers = Provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          sortExpression,
          DomainObjectIDs.Customer1).ToList();

      Assert.That(orderContainers.Count, Is.EqualTo(2));
      Assert.That(orderContainers[0].ID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(orderContainers[1].ID, Is.EqualTo(DomainObjectIDs.Order2));
    }

    [Test]
    public void LoadDataContainersByRelatedIDOfDifferentStorageProvider ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Order), "Official");

      var orderContainers = Provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          null,
          DomainObjectIDs.Official1);
      Assert.That(orderContainers, Is.Not.Null);
      Assert.That(orderContainers.Count(), Is.EqualTo(5));
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithStorageClassTransactionProperty ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Computer), "EmployeeTransactionProperty");

      var result = Provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          null,
          DomainObjectIDs.Employee1);
      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(0));
    }
  }
}
