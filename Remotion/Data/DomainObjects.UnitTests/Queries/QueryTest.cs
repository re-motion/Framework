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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class QueryTest : StandardMappingTest
  {
    [Test]
    public void Initialize ()
    {
      var parameters = new QueryParameterCollection();
      var definition = Queries.GetMandatory("OrderQuery");
      var query = (Query)QueryFactory.CreateQuery(definition, parameters);

      Assert.That(query.Definition, Is.SameAs(definition));
      Assert.That(query.ID, Is.EqualTo(definition.ID));
      Assert.That(query.CollectionType, Is.EqualTo(definition.CollectionType));
      Assert.That(query.QueryType, Is.EqualTo(definition.QueryType));
      Assert.That(query.Statement, Is.EqualTo(definition.Statement));
      Assert.That(query.StorageProviderDefinition, Is.EqualTo(definition.StorageProviderDefinition));
      Assert.That(query.Parameters, Is.SameAs(parameters));
    }

    [Test]
    public void EagerFetchQueries ()
    {
      QueryDefinition definition = TestQueryFactory.CreateOrderQueryWithCustomCollectionType(StorageSettings);
      var query1 = new Query(definition, new QueryParameterCollection());

      Assert.That(query1.EagerFetchQueries, Is.Not.Null);
      Assert.That(query1.EagerFetchQueries.Count, Is.EqualTo(0));

      var query2 = new Query(definition, new QueryParameterCollection());
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition(typeof(Order).FullName + ".OrderItems");

      query1.EagerFetchQueries.Add(endPointDefinition, query2);
      Assert.That(query1.EagerFetchQueries.Count, Is.EqualTo(1));
    }

    [Test]
    public void EagerFetchQueries_Recursive ()
    {
      QueryDefinition definition = TestQueryFactory.CreateOrderQueryWithCustomCollectionType(StorageSettings);
      var query1 = new Query(definition, new QueryParameterCollection());

      Assert.That(query1.EagerFetchQueries, Is.Not.Null);
      Assert.That(query1.EagerFetchQueries.Count, Is.EqualTo(0));

      var query2 = new Query(definition, new QueryParameterCollection());
      var endPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition(typeof(Order).FullName + ".OrderItems");

      query1.EagerFetchQueries.Add(endPointDefinition, query2);
      Assert.That(query1.EagerFetchQueries.Count, Is.EqualTo(1));

      var query3 = new Query(definition, new QueryParameterCollection());
      query2.EagerFetchQueries.Add(endPointDefinition, query3);
      Assert.That(query2.EagerFetchQueries.Count, Is.EqualTo(1));

    }
  }
}
