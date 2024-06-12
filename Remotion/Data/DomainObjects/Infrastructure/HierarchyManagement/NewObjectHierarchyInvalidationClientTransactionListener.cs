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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement
{
  /// <summary>
  /// Propagates the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag for <b>New</b> objects over the
  /// <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a new object is created (i.e., its <see cref="DataContainer"/> is registered), it is automatically invalidated in all parent transactions
  /// of the respective transaction.
  /// </para>
  /// <para>
  /// When a new object is discarded (i.e., its <see cref="DataContainer"/> is unregistered), it is automatically invalidated in all subtransactions 
  /// of the respective transaction.
  /// </para>
  /// </remarks>
  public class NewObjectHierarchyInvalidationClientTransactionListener : ClientTransactionListenerBase
  {
    public override void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      ArgumentUtility.CheckNotNull("container", container);

      if (container.State.IsNew)
      {
        foreach (var ancestor in clientTransaction.ParentTransaction.CreateSequence(tx => tx.ParentTransaction))
        {
          Assertion.IsNull(ancestor.DataManager.DataContainers[container.ID]);
          using (ancestor.HierarchyManager.Unlock())
          {
            ancestor.DataManager.MarkInvalid(container.DomainObject);
          }
        }
      }
    }

    public override void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("container", container);

      if (container.State.IsNew)
      {
        foreach (var descendant in clientTransaction.SubTransaction.CreateSequence(tx => tx.SubTransaction))
        {
          var descendantDataContainer = descendant.DataManager.DataContainers[container.ID];
          if (descendantDataContainer != null)
            return;

          using (descendant.HierarchyManager.UnlockIfRequired())
          {
            descendant.DataManager.MarkInvalid(container.DomainObject);
          }
        }
      }
    }
  }
}
