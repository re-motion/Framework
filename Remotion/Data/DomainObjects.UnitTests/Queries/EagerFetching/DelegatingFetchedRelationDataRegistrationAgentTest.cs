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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.EagerFetching
{
  [TestFixture]
  public class DelegatingFetchedRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private Mock<IFetchedRelationDataRegistrationAgent> _realObjectAgentMock;
    private Mock<IFetchedRelationDataRegistrationAgent> _virtualObjectAgentMock;
    private Mock<IFetchedRelationDataRegistrationAgent> _collectionAgentMock;

    private DelegatingFetchedRelationDataRegistrationAgent _agent;

    private ILoadedObjectData[] _originatingObjects;
    private LoadedObjectDataWithDataSourceData[] _relatedObjects;

    public override void SetUp ()
    {
      base.SetUp();

      _realObjectAgentMock = new Mock<IFetchedRelationDataRegistrationAgent>(MockBehavior.Strict);
      _virtualObjectAgentMock = new Mock<IFetchedRelationDataRegistrationAgent>(MockBehavior.Strict);
      _collectionAgentMock = new Mock<IFetchedRelationDataRegistrationAgent>(MockBehavior.Strict);

      _agent = new DelegatingFetchedRelationDataRegistrationAgent(_realObjectAgentMock.Object, _virtualObjectAgentMock.Object, _collectionAgentMock.Object);

      _originatingObjects = new[] { new Mock<ILoadedObjectData>().Object };
      _relatedObjects = new[] { LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData() };
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_AnonymousEndPoints ()
    {
      var endPointDefinition = Configuration
          .GetTypeDefinition(typeof(Location))
          .PropertyAccessorDataCache
          .GetPropertyAccessorData(typeof(Location), "Client")
          .RelationEndPointDefinition
          .GetOppositeEndPointDefinition();

      Assert.That(
          () => _agent.GroupAndRegisterRelatedObjects(
              endPointDefinition,
              _originatingObjects,
              _relatedObjects),
          Throws.InvalidOperationException.With.Message.EqualTo("Anonymous relation end-points cannot have data registered."));
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_RealObjectEndPoints ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(OrderItem), "Order");

      _realObjectAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, _relatedObjects))
          .Verifiable();

      _agent.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, _relatedObjects);

      _realObjectAgentMock.Verify();
      _virtualObjectAgentMock.Verify();
      _collectionAgentMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_VirtualObjectEndPoints ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");

      _virtualObjectAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, _relatedObjects))
          .Verifiable();

      _agent.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, _relatedObjects);

      _realObjectAgentMock.Verify();
      _virtualObjectAgentMock.Verify();
      _collectionAgentMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_CollectionEndPoints ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderItems");

      _collectionAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, _relatedObjects))
          .Verifiable();

      _agent.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, _relatedObjects);

      _realObjectAgentMock.Verify();
      _virtualObjectAgentMock.Verify();
      _collectionAgentMock.Verify();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_CollectionEndPoints_EmptyRelatedObjects ()
    {
      var endPointDefinition = GetEndPointDefinition(typeof(Order), "OrderItems");

      var relatedObjects = new LoadedObjectDataWithDataSourceData[0];
      _collectionAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, relatedObjects))
          .Verifiable();

      _agent.GroupAndRegisterRelatedObjects(endPointDefinition, _originatingObjects, relatedObjects);

      _realObjectAgentMock.Verify();
      _virtualObjectAgentMock.Verify();
      _collectionAgentMock.Verify();
    }
  }
}
