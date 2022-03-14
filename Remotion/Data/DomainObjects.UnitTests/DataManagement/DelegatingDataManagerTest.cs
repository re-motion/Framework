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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Moq.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DelegatingDataManagerTest : StandardMappingTest
  {
    [Test]
    public void DelegatingMembers ()
    {
      var objectID = DomainObjectIDs.Order1;
      var dataContainer = DataContainer.CreateNew(objectID);
      var relationEndPointID = RelationEndPointID.Create(objectID, typeof(Order), "OrderTicket");
      var virtualEndPoint = new Mock<IVirtualEndPoint>();
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      var persistableData = new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetUnchanged().Value,
          dataContainer,
          new IRelationEndPoint[0]);
      var dataManagementCommand = new Mock<IDataManagementCommand>();
      var randomBoolean = BooleanObjectMother.GetRandomBoolean();
      Predicate<DomainObjectState> expectedPredicate = state => state.IsUnchanged;
      Predicate<DomainObject> domainObjectFilter = obj => true;

      CheckDelegation(dm => dm.GetOrCreateVirtualEndPoint(relationEndPointID), virtualEndPoint.Object);
      CheckDelegation(dm => dm.GetRelationEndPointWithLazyLoad(relationEndPointID), virtualEndPoint.Object);
      CheckDelegation(dm => dm.GetRelationEndPointWithoutLoading(relationEndPointID), virtualEndPoint.Object);
      CheckDelegation(dm => dm.GetDataContainerWithoutLoading(objectID), dataContainer);
      CheckDelegation(dm => dm.RegisterDataContainer(dataContainer));
      CheckDelegation(dm => dm.Discard(dataContainer));
      CheckDelegation(dm => dm.DataContainers, new Mock<IDataContainerMapReadOnlyView>().Object);
      CheckDelegation(dm => dm.RelationEndPoints, new Mock<IRelationEndPointMapReadOnlyView>().Object);
      CheckDelegation(dm => dm.GetState(objectID), new DomainObjectState.Builder().SetDeleted().Value);
      CheckDelegation(dm => dm.GetDataContainerWithLazyLoad(objectID, randomBoolean), dataContainer);
      CheckDelegation(dm => dm.GetDataContainersWithLazyLoad(new[] { objectID }, true), new[] { dataContainer });
      CheckDelegation(dm => dm.GetLoadedDataByObjectState(expectedPredicate), new[] { persistableData });
      CheckDelegation(dm => dm.MarkInvalid(domainObject));
      CheckDelegation(dm => dm.MarkNotInvalid(objectID));
      CheckDelegation(dm => dm.Commit());
      CheckDelegation(dm => dm.Rollback());
      CheckDelegation(dm => dm.CreateDeleteCommand(domainObject), dataManagementCommand.Object);
      CheckDelegation(dm => dm.CreateUnloadCommand(new[] { objectID }), dataManagementCommand.Object);
      CheckDelegation(dm => dm.CreateUnloadVirtualEndPointsCommand(new[] { relationEndPointID }), dataManagementCommand.Object);
      CheckDelegation(dm => dm.CreateUnloadAllCommand(), dataManagementCommand.Object);
      CheckDelegation(dm => dm.CreateUnloadFilteredDomainObjectsCommand(domainObjectFilter), dataManagementCommand.Object);
      CheckDelegation(dm => dm.LoadLazyCollectionEndPoint(relationEndPointID));
      CheckDelegation(dm => dm.LoadLazyVirtualObjectEndPoint(relationEndPointID));
      CheckDelegation(dm => dm.LoadLazyDataContainer(objectID), dataContainer);
    }

    private void CheckDelegation<TR> (Expression<Func<IDataManager, TR>> func, TR fakeResult)
    {
      var innerMock = new Mock<IDataManager>(MockBehavior.Strict);
      var delegatingDataManager = new DelegatingDataManager();
      delegatingDataManager.InnerDataManager = innerMock.Object;

      var helper = new DecoratorTestHelper<IDataManager>(delegatingDataManager, innerMock);
      helper.CheckDelegation(func, fakeResult);

      delegatingDataManager.InnerDataManager = null;
      var compiledFunc = func.Compile();
      Assert.That(
          () => compiledFunc.Invoke(delegatingDataManager),
          Throws.InvalidOperationException.With.Message.EqualTo("InnerDataManager property must be set before it can be used."));
    }

    private void CheckDelegation (Expression<Action<IDataManager>> action)
    {
      var innerMock = new Mock<IDataManager>(MockBehavior.Strict);
      var delegatingDataManager = new DelegatingDataManager();
      delegatingDataManager.InnerDataManager = innerMock.Object;

      var helper = new DecoratorTestHelper<IDataManager>(delegatingDataManager, innerMock);
      helper.CheckDelegation(action);

      delegatingDataManager.InnerDataManager = null;
      var compiledAction = action.Compile();
      Assert.That(
          () => compiledAction.Invoke(delegatingDataManager),
          Throws.InvalidOperationException.With.Message.EqualTo("InnerDataManager property must be set before it can be used."));
    }
  }
}
