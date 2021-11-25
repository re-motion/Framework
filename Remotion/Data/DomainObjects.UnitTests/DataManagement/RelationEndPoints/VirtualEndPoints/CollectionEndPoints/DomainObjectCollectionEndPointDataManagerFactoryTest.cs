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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class DomainObjectCollectionEndPointDataManagerFactoryTest : StandardMappingTest
  {
    private IDomainObjectCollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;

    private DomainObjectCollectionEndPointDataManagerFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _changeDetectionStrategy = MockRepository.GenerateStub<IDomainObjectCollectionEndPointChangeDetectionStrategy>();

      _factory = new DomainObjectCollectionEndPointDataManagerFactory(_changeDetectionStrategy);
    }

    [Test]
    public void Create ()
    {
      var relationEndPointID = RelationEndPointID.Create(
          DomainObjectIDs.Customer1,
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

      var result = _factory.CreateEndPointDataManager(relationEndPointID);

      Assert.That(result, Is.TypeOf(typeof(DomainObjectCollectionEndPointDataManager)));
      Assert.That(((DomainObjectCollectionEndPointDataManager) result).EndPointID, Is.SameAs(relationEndPointID));
      Assert.That(((DomainObjectCollectionEndPointDataManager) result).ChangeDetectionStrategy, Is.SameAs(_changeDetectionStrategy));
    }

    [Test]
    public void Serializable ()
    {
      var changeDetectionStrategy = new SerializableDomainObjectCollectionEndPointChangeDetectionStrategyFake();
      var factory = new DomainObjectCollectionEndPointDataManagerFactory(changeDetectionStrategy);

      var deserializedInstance = Serializer.SerializeAndDeserialize(factory);

      Assert.That(deserializedInstance.ChangeDetectionStrategy, Is.Not.Null);
    }
  }
}
