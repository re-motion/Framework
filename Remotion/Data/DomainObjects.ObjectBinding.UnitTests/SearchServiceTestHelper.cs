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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  /// <summary>
  /// Provides functionality to stub query results used by tests for the search service implementations.
  /// </summary>
  public class SearchServiceTestHelper
  {
    private IPersistenceStrategy _persistenceStrategyStub = MockRepository.GenerateStub<IPersistenceStrategy>();

    public T CreateStubbableTransaction<T> () where T : ClientTransaction
    {
      _persistenceStrategyStub = MockRepository.GenerateStub<IFetchEnabledPersistenceStrategy>();
      _persistenceStrategyStub
          .Stub (stub => stub.CreateNewObjectID (Arg<ClassDefinition>.Is.Anything))
          .Return (null)
          .WhenCalled (mi => { mi.ReturnValue = new ObjectID((ClassDefinition) mi.Arguments[0], Guid.NewGuid ()); });

      IClientTransactionComponentFactory componentFactory = new ComponentFactoryWithSpecificPersistenceStrategy (_persistenceStrategyStub);
      return (T) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (T), componentFactory);
    }

    public void StubQueryResult (string queryID, ILoadedObjectData[] fakeResult)
    {
      _persistenceStrategyStub
          .Stub (stub => stub.ExecuteCollectionQuery (Arg<IQuery>.Matches (q => q.ID == queryID), Arg<ILoadedObjectDataProvider>.Is.Anything))
          .Return (fakeResult);
    }

    public void StubSearchAllObjectsQueryResult (Type domainObjectType, params ILoadedObjectData[] fakeResult)
    {
      var query = (IQuery) PrivateInvoke.InvokeNonPublicMethod (new BindableDomainObjectSearchAllService (), "GetQuery", domainObjectType);

      StubQueryResult (query.ID, fakeResult);
    }

    public T CreateTransactionWithStubbedQuery<T> (string queryID) where T : ClientTransaction
    {
      var transaction = CreateStubbableTransaction<T>();

      var fakeResultDataContainer = CreateFakeResultData (transaction);
      StubQueryResult (queryID, new[] { fakeResultDataContainer });
      
      return transaction;
    }

    public ILoadedObjectData CreateFakeResultData (ClientTransaction clientTransaction)
    {
      var existingDataContainer = DataContainer.CreateForExisting (
          new ObjectID(typeof (OppositeBidirectionalBindableDomainObject), Guid.NewGuid ()),
          null,
          pd => pd.DefaultValue);
      existingDataContainer.SetDomainObject (LifetimeService.GetObjectReference (clientTransaction, existingDataContainer.ID));
      ClientTransactionTestHelper.GetDataManager (clientTransaction).RegisterDataContainer (existingDataContainer);
      return new AlreadyExistingLoadedObjectData (existingDataContainer);
    }
  }
}