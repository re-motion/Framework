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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  public class UnloadTestBase : ClientTransactionBaseTest
  {
    protected void CheckDataContainerExists (DomainObject domainObject, bool dataContainerShouldExist)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      var dataContainer = DataManagementService.GetDataManager(ClientTransaction.Current).DataContainers[domainObject.ID];
      if (dataContainerShouldExist)
        Assert.That(dataContainer, Is.Not.Null, "Data container '{0}' does not exist.", domainObject.ID);
      else
        Assert.That(dataContainer, Is.Null, "Data container '{0}' should not exist.", domainObject.ID);
    }

    protected void CheckEndPointExists (DomainObject owningObject, string shortPropertyName, bool endPointShouldExist)
    {
      ArgumentUtility.CheckNotNull("owningObject", owningObject);
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(owningObject.ID, shortPropertyName);
      CheckEndPointExists(endPointID, endPointShouldExist);
    }

    protected void CheckEndPointExists (RelationEndPointID endPointID, bool shouldEndPointExist)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);

      var endPoint = DataManagementService.GetDataManager(ClientTransaction.Current).GetRelationEndPointWithoutLoading(endPointID);
      if (shouldEndPointExist)
        Assert.That(endPoint, Is.Not.Null, "End point '{0}' does not exist.", endPointID);
      else
        Assert.That(endPoint, Is.Null, "End point '{0}' should not exist.", endPointID);
    }

    protected void CheckVirtualEndPointExistsAndComplete (DomainObject owningObject, string shortPropertyName, bool shouldEndPointExist, bool shouldDataBeComplete)
    {
      ArgumentUtility.CheckNotNull("owningObject", owningObject);
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(owningObject.ID, shortPropertyName);

      CheckVirtualEndPointExistsAndComplete(endPointID, shouldEndPointExist, shouldDataBeComplete);
    }

    protected void CheckVirtualEndPointExistsAndComplete (RelationEndPointID endPointID, bool shouldEndPointExist, bool shouldDataBeComplete)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      CheckEndPointExists(endPointID, shouldEndPointExist);

      if (shouldEndPointExist)
      {
        var endPoint = DataManagementService.GetDataManager(ClientTransaction.Current).GetRelationEndPointWithoutLoading(endPointID);
        if (shouldDataBeComplete)
          Assert.That(endPoint.IsDataComplete, Is.True, "End point '{0}' should have complete data.", endPoint.ID);
        else
          Assert.That(endPoint.IsDataComplete, Is.False, "End point '{0}' should not have complete data.", endPoint.ID);
      }
    }

    protected void EnsureTransactionThrowsOnLoad ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvent(
          ClientTransaction.Current,
          mock => mock.ObjectsLoading(It.IsAny<ClientTransaction>(), It.IsAny<ReadOnlyCollection<ObjectID>>()));
    }

    protected void AssertObjectWasLoaded (Mock<IClientTransactionListener> listenerMock, DomainObject loadedObject)
    {
      ArgumentUtility.CheckNotNull("listenerMock", listenerMock);
      ArgumentUtility.CheckNotNull("loadedObject", loadedObject);

      listenerMock.Verify(
          mock => mock.ObjectsLoaded(ClientTransaction.Current, new[] { loadedObject }),
          Times.AtLeastOnce());
    }

    protected void AssertObjectWasLoadedAmongOthers (Mock<IClientTransactionListener> listenerMock, DomainObject loadedObject)
    {
      ArgumentUtility.CheckNotNull("listenerMock", listenerMock);
      ArgumentUtility.CheckNotNull("loadedObject", loadedObject);

      listenerMock.Verify(
          mock => mock.ObjectsLoaded(ClientTransaction.Current, It.Is<ReadOnlyCollection<IDomainObject>>(_ => _.Contains(loadedObject))),
          Times.AtLeastOnce());
    }
  }
}
