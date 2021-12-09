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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  public static class RelationEndPointObjectMother
  {
    public static DomainObjectCollectionEndPoint CreateDomainObjectCollectionEndPoint (
        RelationEndPointID endPointID,
        IEnumerable<DomainObject> initialContents,
        ClientTransaction clientTransaction = null)
    {
      clientTransaction = clientTransaction ?? ClientTransactionScope.CurrentTransaction;
      var dataManager = ClientTransactionTestHelper.GetDataManager(clientTransaction);
      var changeDetectionStrategy = new RootDomainObjectCollectionEndPointChangeDetectionStrategy();
      var dataStrategyFactory = new AssociatedDomainObjectCollectionDataStrategyFactory(dataManager);
      var collectionEndPoint = new DomainObjectCollectionEndPoint(
          clientTransaction,
          endPointID,
          new DomainObjectCollectionEndPointCollectionManager(endPointID, new DomainObjectCollectionEndPointCollectionProvider(dataStrategyFactory), dataStrategyFactory),
          dataManager,
          dataManager,
          ClientTransactionTestHelper.GetEventBroker(clientTransaction),
          new DomainObjectCollectionEndPointDataManagerFactory(changeDetectionStrategy));

      if (initialContents != null)
        DomainObjectCollectionEndPointTestHelper.FillCollectionEndPointWithInitialContents(collectionEndPoint, initialContents);

      return collectionEndPoint;
    }

    public static RealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID)
    {
      var dataManager = (DataManager)PrivateInvoke.GetNonPublicProperty(ClientTransaction.Current, "DataManager");
      var dataContainer = dataManager.GetDataContainerWithLazyLoad(endPointID.ObjectID, true);
      return CreateRealObjectEndPoint(endPointID, dataContainer);
    }

    public static RealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      var clientTransaction = dataContainer.ClientTransaction;
      var endPointProvider = ClientTransactionTestHelper.GetDataManager(clientTransaction);
      var transactionEventSink = ClientTransactionTestHelper.GetEventBroker(clientTransaction);
      return new RealObjectEndPoint(clientTransaction, endPointID, dataContainer, endPointProvider, transactionEventSink);
    }

    public static VirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID, ClientTransaction clientTransaction)
    {
      var lazyLoader = ClientTransactionTestHelper.GetDataManager(clientTransaction);
      var endPointProvider = ClientTransactionTestHelper.GetDataManager(clientTransaction);
      var transactionEventSink = ClientTransactionTestHelper.GetEventBroker(clientTransaction);
      var dataManagerFactory = new VirtualObjectEndPointDataManagerFactory();
      return new VirtualObjectEndPoint(
          clientTransaction,
          endPointID,
          lazyLoader,
          endPointProvider,
          transactionEventSink,
          dataManagerFactory);
    }

    public static ObjectEndPoint CreateObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      if (endPointID.Definition.IsVirtual)
      {
        var clientTransaction = ClientTransaction.Current;
        VirtualObjectEndPoint endPoint = CreateVirtualObjectEndPoint(endPointID, clientTransaction);
        endPoint.MarkDataComplete(LifetimeService.GetObjectReference(clientTransaction, oppositeObjectID));
        return endPoint;
      }
      else
      {
        var endPoint = CreateRealObjectEndPoint(endPointID);
        RealObjectEndPointTestHelper.SetValueViaDataContainer(endPoint, oppositeObjectID);
        endPoint.Commit();
        return endPoint;
      }
    }

    public static RelationEndPointID CreateRelationEndPointID (ObjectID objectID = null, string shortPropertyName = null)
    {
      objectID = objectID ?? new ObjectID(typeof(Order), Guid.NewGuid());
      shortPropertyName = shortPropertyName ?? "OrderItems";
      return RelationEndPointID.Create(objectID, objectID.ClassDefinition.Type, shortPropertyName);
    }

    public static RelationEndPointID CreateAnonymousEndPointID ()
    {
      var anonymousEndPointDefinition = GetEndPointDefinition(typeof(Location), "Client").GetOppositeEndPointDefinition();
      var endPointID = RelationEndPointID.Create(new DomainObjectIDs(MappingConfiguration.Current).Client1, anonymousEndPointDefinition);
      Assert.That(endPointID.Definition.IsAnonymous, Is.True);
      return endPointID;
    }

    public static IRelationEndPointDefinition GetEndPointDefinition (Type declaringType, string shortPropertyName)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(declaringType);
      var propertyAccessorData = classDefinition.PropertyAccessorDataCache.FindPropertyAccessorData(declaringType, shortPropertyName);
      Assert.That(propertyAccessorData, Is.Not.Null);
      return propertyAccessorData.RelationEndPointDefinition;
    }

    public static DomainObjectCollectionEndPoint CreateCollectionEndPoint_Customer1_Orders (params Order[] initialContents)
    {
      var customerEndPointID = CreateRelationEndPointID(new DomainObjectIDs(MappingConfiguration.Current).Customer1, "Orders");
      return CreateDomainObjectCollectionEndPoint(customerEndPointID, initialContents);
    }

    public static Mock<IRelationEndPoint> CreateStub (RelationEndPointID endPointID = null)
    {
      endPointID = endPointID ?? CreateRelationEndPointID();
      var endPoint = new Mock<IRelationEndPoint>();
      endPoint.Setup(stub => stub.ID).Returns(endPointID);
      endPoint.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPoint.Setup(stub => stub.ObjectID).Returns(endPointID.ObjectID);
      return endPoint;
    }
  }
}
