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
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  public class QueryDefinitionCollectionTest : StandardMappingTest
  {
    private QueryDefinitionCollection _collection;
    private QueryDefinition _definition;

    public override void SetUp ()
    {
      base.SetUp();

      _collection = new QueryDefinitionCollection();

      _definition = new QueryDefinition(
          "OrderQuery",
          TestDomainStorageProviderDefinition,
          "select Order.* from Order inner join Customer where Customer.ID = @customerID order by OrderNo asc;",
          QueryType.CollectionReadOnly,
          typeof(OrderCollection));
    }

    [Test]
    public void DuplicateQueryIDs ()
    {
      _collection.Add(_definition);
      Assert.That(
          () => _collection.Add(_definition),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "QueryDefinition 'OrderQuery' already exists in collection.", "queryDefinition"));
    }

    [Test]
    public void ContainsQueryDefinitionTrue ()
    {
      _collection.Add(_definition);

      Assert.That(_collection.Contains(_definition), Is.True);
    }

    [Test]
    public void ContainsQueryDefinitionFalse ()
    {
      _collection.Add(_definition);

      QueryDefinition copy = new QueryDefinition(
          _definition.ID, _definition.StorageProviderDefinition, _definition.Statement, _definition.QueryType, _definition.CollectionType);

      Assert.That(_collection.Contains(copy), Is.False);
    }

    [Test]
    public void ContainsNullQueryDefinition ()
    {
      Assert.That(
          () => _collection.Contains((QueryDefinition)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void GetMandatory ()
    {
      _collection.Add(_definition);

      Assert.That(_collection.GetMandatory(_definition.ID), Is.SameAs(_definition));
    }

    [Test]
    public void GetMandatoryForNonExisting ()
    {
      Assert.That(
          () => _collection.GetMandatory("OrderQuery"),
          Throws.InstanceOf<QueryConfigurationException>()
              .With.Message.EqualTo("QueryDefinition 'OrderQuery' does not exist."));
    }

    [Test]
    public void Merge ()
    {
      QueryDefinition query1 = new QueryDefinition("id1", TestDomainStorageProviderDefinition, "bla", QueryType.CollectionReadOnly);
      QueryDefinition query2 = new QueryDefinition("id2", TestDomainStorageProviderDefinition, "bla", QueryType.CollectionReadOnly);
      QueryDefinition query3 = new QueryDefinition("id3", TestDomainStorageProviderDefinition, "bla", QueryType.CollectionReadOnly);
      QueryDefinition query4 = new QueryDefinition("id4", TestDomainStorageProviderDefinition, "bla", QueryType.CollectionReadOnly);
      QueryDefinition query5 = new QueryDefinition("id5", TestDomainStorageProviderDefinition, "bla", QueryType.CollectionReadOnly);

      QueryDefinitionCollection source = new QueryDefinitionCollection();
      source.Add(query1);
      source.Add(query2);

      QueryDefinitionCollection target = new QueryDefinitionCollection();
      target.Add(query3);
      target.Add(query4);
      target.Add(query5);

      target.Merge(source);

      Assert.That(source.Count, Is.EqualTo(2));
      Assert.That(source[0], Is.SameAs(query1));
      Assert.That(source[1], Is.SameAs(query2));

      Assert.That(target.Count, Is.EqualTo(5));
      Assert.That(target[0], Is.SameAs(query3));
      Assert.That(target[1], Is.SameAs(query4));
      Assert.That(target[2], Is.SameAs(query5));
      Assert.That(target[3], Is.SameAs(query1));
      Assert.That(target[4], Is.SameAs(query2));
    }

    [Test]
    public void Merge_ThrowsOnDuplicates ()
    {
      QueryDefinition query1 = new QueryDefinition("id1", TestDomainStorageProviderDefinition, "bla", QueryType.CollectionReadOnly);
      QueryDefinition query2 = new QueryDefinition("id1", TestDomainStorageProviderDefinition, "bla", QueryType.CollectionReadOnly);

      QueryDefinitionCollection source = new QueryDefinitionCollection();
      source.Add(query1);

      QueryDefinitionCollection target = new QueryDefinitionCollection();
      target.Add(query2);

      Assert.That(
          () => target.Merge(source),
          Throws.InstanceOf<DuplicateQueryDefinitionException>()
              .With.Message.EqualTo("The query with ID 'id1' has a duplicate."));

      Assert.That(target.Count, Is.EqualTo(1));
      Assert.That(target[0], Is.SameAs(query2));
    }
  }
}
