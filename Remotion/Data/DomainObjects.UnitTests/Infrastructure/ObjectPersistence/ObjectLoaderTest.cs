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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {

    private Mock<IPersistenceStrategy> _persistenceStrategyMock;
    private Mock<ILoadedObjectDataRegistrationAgent> _loadedObjectDataRegistrationAgentMock;
    private Mock<ILoadedObjectDataProvider> _loadedObjectDataProviderStub;

    private ObjectLoader _objectLoader;

    private IQuery _fakeQuery;
    private Mock<ILoadedObjectData> _loadedObjectDataStub1;
    private Mock<ILoadedObjectData> _loadedObjectDataStub2;

    public override void SetUp ()
    {
      base.SetUp();

      _persistenceStrategyMock = new Mock<IPersistenceStrategy>(MockBehavior.Strict);
      _loadedObjectDataRegistrationAgentMock = new Mock<ILoadedObjectDataRegistrationAgent>(MockBehavior.Strict);
      _loadedObjectDataProviderStub = new Mock<ILoadedObjectDataProvider>();

      _objectLoader = new ObjectLoader(_persistenceStrategyMock.Object, _loadedObjectDataRegistrationAgentMock.Object, _loadedObjectDataProviderStub.Object);

      _fakeQuery = CreateFakeQuery();

      _loadedObjectDataStub1 = new Mock<ILoadedObjectData>();
      _loadedObjectDataStub2 = new Mock<ILoadedObjectData>();
    }

    [Test]
    public void LoadObject ()
    {
      _persistenceStrategyMock
          .Setup(mock => mock.LoadObjectData(DomainObjectIDs.Order1))
          .Returns(_loadedObjectDataStub1.Object)
          .Verifiable();
      var throwOnNotFound = BooleanObjectMother.GetRandomBoolean();
      _loadedObjectDataRegistrationAgentMock
          .Setup(mock => mock.RegisterIfRequired(new[] { _loadedObjectDataStub1.Object }, throwOnNotFound))
          .Returns(Mock.Of<IEnumerable<ILoadedObjectData>>(MockBehavior.Strict))
          .Verifiable();

      var result = _objectLoader.LoadObject(DomainObjectIDs.Order1, throwOnNotFound);

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      Assert.That(result, Is.SameAs(_loadedObjectDataStub1.Object));
    }

    [Test]
    public void LoadObjects ()
    {
      _loadedObjectDataStub1.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      _loadedObjectDataStub2.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order3);

      _persistenceStrategyMock
          .Setup(mock => mock.LoadObjectData(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }))
          .Returns(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object })
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .Setup(mock => mock.RegisterIfRequired(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object }, true))
          .Returns(Mock.Of<IEnumerable<ILoadedObjectData>>(MockBehavior.Strict))
          .Verifiable();

      var result = _objectLoader.LoadObjects(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }, true);

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object }));
    }

    [Test]
    public void GetOrLoadRelatedObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order4, "OrderTicket");

      _persistenceStrategyMock
          .Setup(mock => mock.ResolveObjectRelationData(endPointID, _loadedObjectDataProviderStub.Object))
          .Returns(_loadedObjectDataStub1.Object)
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .Setup(mock => mock.RegisterIfRequired(new[] { _loadedObjectDataStub1.Object }, true))
          .Returns(Mock.Of<IEnumerable<ILoadedObjectData>>(MockBehavior.Strict))
          .Verifiable();

      var result = _objectLoader.GetOrLoadRelatedObject(endPointID);

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      Assert.That(result, Is.SameAs(_loadedObjectDataStub1.Object));
    }

    [Test]
    public void GetOrLoadRelatedObject_NonVirtualID ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      Assert.That(
          () => _objectLoader.GetOrLoadRelatedObject(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("GetOrLoadRelatedObject can only be used with virtual end points.", "relationEndPointID"));
    }

    [Test]
    public void GetOrLoadRelatedObject_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      Assert.That(
          () => _objectLoader.GetOrLoadRelatedObject(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("GetOrLoadRelatedObject can only be used with one-valued end points.", "relationEndPointID"));
    }

    [Test]
    public void GetOrLoadRelatedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");

      _persistenceStrategyMock
          .Setup(mock => mock.ResolveCollectionRelationData(endPointID, _loadedObjectDataProviderStub.Object))
          .Returns(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object })
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .Setup(mock => mock.RegisterIfRequired(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object }, true))
          .Returns(Mock.Of<IEnumerable<ILoadedObjectData>>(MockBehavior.Strict))
          .Verifiable();

      var result = _objectLoader.GetOrLoadRelatedObjects(endPointID);

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object }));
    }

    [Test]
    public void GetOrLoadRelatedObjects_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");
      Assert.That(
          () => _objectLoader.GetOrLoadRelatedObjects(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("GetOrLoadRelatedObjects can only be used with many-valued end points.", "relationEndPointID"));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult ()
    {
      _persistenceStrategyMock
          .Setup(mock => mock.ExecuteCollectionQuery(_fakeQuery, _loadedObjectDataProviderStub.Object))
          .Returns(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object })
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .Setup(mock => mock.RegisterIfRequired(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object }, true))
          .Returns(Mock.Of<IEnumerable<ILoadedObjectData>>(MockBehavior.Strict))
          .Verifiable();

      var result = _objectLoader.GetOrLoadCollectionQueryResult(_fakeQuery);

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object }));
    }

    private IQuery CreateFakeQuery ()
    {
      return QueryFactory.CreateCollectionQuery(
          "test",
          UnitTestStorageProviderDefinition,
          "TEST",
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));
    }
  }
}
