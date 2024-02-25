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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.EagerFetching
{
  [TestFixture]
  public class EagerFetchQueryCollectionTest : StandardMappingTest
  {
    private IQuery _query1;
    private IQuery _query2;
    private EagerFetchQueryCollection _collection;
    private IRelationEndPointDefinition _endPointDefinition1;
    private IRelationEndPointDefinition _endPointDefinition2;
    private IRelationEndPointDefinition _objectEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _query1 = QueryFactory.CreateQuery(TestQueryFactory.CreateOrderSumQueryDefinition(StorageSettings));
      _query2 = QueryFactory.CreateQuery(TestQueryFactory.CreateOrderQueryWithCustomCollectionType(StorageSettings));

      _endPointDefinition1 = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition(typeof(Order).FullName + ".OrderItems");
      _endPointDefinition2 = DomainObjectIDs.Customer1.ClassDefinition.GetMandatoryRelationEndPointDefinition(typeof(Customer).FullName + ".Orders");

      _objectEndPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition(typeof(Order).FullName + ".OrderTicket");

      _collection = new EagerFetchQueryCollection();
    }

    [Test]
    public void Add ()
    {
      _collection.Add(_endPointDefinition1, _query1);
      Assert.That(_collection.ToArray(), Is.EquivalentTo(
          new[] {
              new KeyValuePair<IRelationEndPointDefinition, IQuery>(_endPointDefinition1, _query1)
          }));

      _collection.Add(_endPointDefinition2, _query2);
      Assert.That(_collection.ToArray(), Is.EquivalentTo(
          new[] {
              new KeyValuePair<IRelationEndPointDefinition, IQuery>(_endPointDefinition1, _query1),
              new KeyValuePair<IRelationEndPointDefinition, IQuery>(_endPointDefinition2, _query2)
          }));
    }

    [Test]
    public void Add_Twice ()
    {
      _collection.Add(_endPointDefinition1, _query1);
      Assert.That(
          () => _collection.Add(_endPointDefinition1, _query2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "There is already an eager fetch query for relation end point "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems'."));
    }

    [Test]
    public void Add_ForObjectEndPoint ()
    {
      _collection.Add(_objectEndPointDefinition, _query1);
      Assert.That(_collection.ToArray(), Is.EquivalentTo(
          new[] {
            new KeyValuePair<IRelationEndPointDefinition, IQuery>(_objectEndPointDefinition, _query1)
          }));
    }

    [Test]
    public void Count ()
    {
      Assert.That(_collection.Count, Is.EqualTo(0));
      _collection.Add(_endPointDefinition1, _query1);
      Assert.That(_collection.Count, Is.EqualTo(1));
      _collection.Add(_endPointDefinition2, _query2);
      Assert.That(_collection.Count, Is.EqualTo(2));
    }
  }
}
