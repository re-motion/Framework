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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides the default implementation of the <see cref="IDomainObjectTransactionContext"/> interface.
  /// Represents the context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public class DomainObjectTransactionContext : IDomainObjectTransactionContextStrategy
  {
    public object? GetTimestamp (DomainObject domainObject, ClientTransaction clientTransaction)
    {
      return clientTransaction.DataManager.GetDataContainerWithLazyLoad(domainObject.ID, throwOnNotFound: true)!.Timestamp;
    }

    public DomainObjectState GetState (DomainObject domainObject, ClientTransaction clientTransaction)
    {
      return clientTransaction.DataManager.GetState(domainObject.ID);
    }

    public void RegisterForCommit (DomainObject domainObject, ClientTransaction clientTransaction)
    {
      var dataContainer = clientTransaction.DataManager.GetDataContainerWithLazyLoad(domainObject.ID, throwOnNotFound: true)!;
      if (dataContainer.State.IsDeleted)
        return;

      if (dataContainer.State.IsNew)
        return;

      dataContainer.MarkAsChanged();
    }

    public void EnsureDataAvailable (DomainObject domainObject, ClientTransaction clientTransaction)
    {
      clientTransaction.EnsureDataAvailable(domainObject.ID);

      DataContainer? dataContainer;
      Assertion.DebugAssert(
          (dataContainer = clientTransaction.DataManager.DataContainers[domainObject.ID]) != null
          && dataContainer.DomainObject == domainObject,
          "Guaranteed because CheckIfRightTransaction ensures that DomainObject is enlisted.");
    }

    public bool TryEnsureDataAvailable (DomainObject domainObject, ClientTransaction clientTransaction)
    {
      return clientTransaction.TryEnsureDataAvailable(domainObject.ID);
    }
  }
}
