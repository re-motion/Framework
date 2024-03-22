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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Allows event handlers to register objects for additional events to be raised before a <see cref="DomainObjects.ClientTransaction.Commit"/> 
  /// operation is performed.
  /// </summary>
  public class CommittingEventRegistrar : ICommittingEventRegistrar
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly HashSet<DomainObject> _registeredObjects = new HashSet<DomainObject>();

    public CommittingEventRegistrar (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      _clientTransaction = clientTransaction;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IReadOnlyCollection<DomainObject> RegisteredObjects
    {
      get { return _registeredObjects.AsReadOnly(); }
    }

    public void RegisterForAdditionalCommittingEvents (params DomainObject[] domainObjects)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      foreach (var domainObject in domainObjects)
      {
        var state = domainObject.TransactionContext[_clientTransaction].State;

        // Place tests in order of probability to reduce number of checks required until a match for a typical usage scenario
        var isPersistenceRelevantDomainObject = state.IsChanged || state.IsNew || state.IsDeleted;

        if (!isPersistenceRelevantDomainObject)
        {
          var message = string.Format(
              "The given DomainObject '{0}' cannot be registered due to its {1}. Only objects that are part of the commit set can be "
              + "registered. Use RegisterForCommit to add an unchanged object to the commit set.",
              domainObject.ID,
              state);
          throw new ArgumentException(message, "domainObjects");
        }
      }

      _registeredObjects.UnionWith(domainObjects);
    }
  }
}
