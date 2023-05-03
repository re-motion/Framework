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
  /// Provides actual implementation for the <see cref="DomainObjectTransactionContextStruct"/>.
  /// Represents the context of a <see cref="DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public class DomainObjectTransactionContext
  {
    private readonly DomainObject _domainObject;

    public bool IsInitializing { get; set; }
    public DomainObjectTransactionContext (DomainObject domainObject)
    {
      _domainObject = domainObject;
    }

    public object? GetTimestamp (ClientTransaction clientTransaction)
    {
      ThrowOnDomainObjectInWrongState();
      return clientTransaction.DataManager.GetDataContainerWithLazyLoad(_domainObject.ID, throwOnNotFound: true)!.Timestamp;
    }

    public DomainObjectState GetState (ClientTransaction clientTransaction)
    {
      ThrowOnDomainObjectInWrongState();
      return clientTransaction.DataManager.GetState(_domainObject.ID);
    }

    public void RegisterForCommit (ClientTransaction clientTransaction)
    {
      ThrowOnDomainObjectInWrongState();
      var dataContainer = clientTransaction.DataManager.GetDataContainerWithLazyLoad(_domainObject.ID, throwOnNotFound: true)!;
      if (dataContainer.State.IsDeleted)
        return;

      if (dataContainer.State.IsNew)
        return;

      dataContainer.MarkAsChanged();
    }

    public void EnsureDataAvailable (ClientTransaction clientTransaction)
    {
      ThrowOnDomainObjectInWrongState();
      clientTransaction.EnsureDataAvailable(_domainObject.ID);

      DataContainer? dataContainer;
      Assertion.DebugAssert(
          (dataContainer = clientTransaction.DataManager.DataContainers[_domainObject.ID]) != null
          && dataContainer.DomainObject == _domainObject,
          "Guaranteed because CheckIfRightTransaction ensures that DomainObject is enlisted.");
    }

    public bool TryEnsureDataAvailable (ClientTransaction clientTransaction)
    {
      return clientTransaction.TryEnsureDataAvailable(_domainObject.ID);
    }

    private void ThrowOnDomainObjectInWrongState ()
    {
      if(IsInitializing)
        throw new InvalidOperationException("While the OnReferenceInitializing event is executing, this member cannot be used.");
    }
  }
}
