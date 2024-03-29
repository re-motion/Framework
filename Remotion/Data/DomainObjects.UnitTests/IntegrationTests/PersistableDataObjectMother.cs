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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  public static class PersistableDataObjectMother
  {
    public static PersistableData Create ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      return new PersistableData(
          domainObject,
          new DomainObjectState.Builder().SetNew().Value,
          DataContainer.CreateNew(domainObject.ID),
          new IRelationEndPoint[0]);
    }

    public static PersistableData Create (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      var dataManager = ClientTransactionTestHelper.GetIDataManager(clientTransaction);
      var dataContainer = dataManager.GetDataContainerWithoutLoading(domainObject.ID);
      return new PersistableData(
          domainObject,
          domainObject.TransactionContext[clientTransaction].State,
          dataContainer,
          dataContainer.AssociatedRelationEndPointIDs.Select(dataManager.GetRelationEndPointWithoutLoading).Where(ep => ep != null));
    }
  }
}
