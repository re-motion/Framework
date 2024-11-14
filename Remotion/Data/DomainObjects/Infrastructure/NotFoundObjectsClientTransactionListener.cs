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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Listens to objects not found in the data source underlying a <see cref="ClientTransaction"/> and marks the respective 
  /// <see cref="DomainObject"/> references invalid within all descendant <see cref="ClientTransaction"/> instances in the hierarchy.
  /// </summary>
  public class NotFoundObjectsClientTransactionListener : ClientTransactionListenerBase
  {
    public override void ObjectsNotFound (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      foreach (var objectID in objectIDs)
      {
        var objectReference = clientTransaction.GetObjectReference(objectID);
        for (var tx = clientTransaction; tx != null; tx = tx.SubTransaction)
        {
          using (tx.HierarchyManager.UnlockIfRequired())
          {
            tx.DataManager.MarkInvalid(objectReference);
          }
        }
      }
    }
  }
}
