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
  /// Provides the state and implementation for <see cref="DomainObjectTransactionContext"/> to allow the value type <see cref="DomainObjectTransactionContext"/> to remain lightweight.
  /// Represents the context of a <see cref="DomainObjects.DomainObject"/> that is associated with a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public sealed class DomainObjectTransactionContextImplementation
  {
    public bool IsDomainObjectReferenceInitializing { get; private set; }

    public DomainObject DomainObject { get; }

    public DomainObjectTransactionContextImplementation (DomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull(nameof(domainObject), domainObject);
      DomainObject = domainObject;
    }

    public object? GetTimestamp (ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull(nameof(clientTransaction), clientTransaction);
      DomainObjectCheckUtility.DebugCheckIfRightTransaction(DomainObject, clientTransaction);

      CheckForDomainObjectReferenceInitializing();
      return clientTransaction.DataManager.GetDataContainerWithLazyLoad(DomainObject.ID, true)!.Timestamp;
    }

    public DomainObjectState GetState (ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull(nameof(clientTransaction), clientTransaction);
      DomainObjectCheckUtility.DebugCheckIfRightTransaction(DomainObject, clientTransaction);

      CheckForDomainObjectReferenceInitializing();
      return clientTransaction.DataManager.GetState(DomainObject.ID);
    }

    public void RegisterForCommit (ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull(nameof(clientTransaction), clientTransaction);
      DomainObjectCheckUtility.DebugCheckIfRightTransaction(DomainObject, clientTransaction);

      CheckForDomainObjectReferenceInitializing();
      var dataContainer = clientTransaction.DataManager.GetDataContainerWithLazyLoad(DomainObject.ID, true)!;
      if (dataContainer.State.IsDeleted)
        return;

      if (dataContainer.State.IsNew)
        return;

      dataContainer.MarkAsChanged();
    }

    public void EnsureDataAvailable (ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull(nameof(clientTransaction), clientTransaction);
      DomainObjectCheckUtility.DebugCheckIfRightTransaction(DomainObject, clientTransaction);

      CheckForDomainObjectReferenceInitializing();
      clientTransaction.EnsureDataAvailable(DomainObject.ID);

      DataContainer? dataContainer;
      Assertion.DebugAssert(
          (dataContainer = clientTransaction.DataManager.DataContainers[DomainObject.ID]) != null
          && dataContainer.DomainObject == DomainObject,
          "Guaranteed because CheckIfRightTransaction ensures that DomainObject is enlisted.");
    }

    public bool TryEnsureDataAvailable (ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull(nameof(clientTransaction), clientTransaction);
      DomainObjectCheckUtility.DebugCheckIfRightTransaction(DomainObject, clientTransaction);

      CheckForDomainObjectReferenceInitializing();

      return clientTransaction.TryEnsureDataAvailable(DomainObject.ID);
    }

    public void CheckForDomainObjectReferenceInitializing ()
    {
      if (IsDomainObjectReferenceInitializing)
        throw new InvalidOperationException("While the DomainObject.OnReferenceInitializing event is executing, this member cannot be used.");
    }

    public void BeginDomainObjectReferenceInitializing ()
    {
      IsDomainObjectReferenceInitializing = true;
    }

    public void EndDomainObjectReferenceInitializing ()
    {
      IsDomainObjectReferenceInitializing = false;
    }
  }
}
