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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectQuerySearchService : ISearchAvailableObjectsService
  {
    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject? referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments? searchArguments)
    {
      var defaultSearchArguments = searchArguments as DefaultSearchArguments;
      if (defaultSearchArguments == null || string.IsNullOrEmpty(defaultSearchArguments.SearchStatement))
        return new IBusinessObject[0];

      var query = QueryFactory.CreateQueryFromConfiguration(defaultSearchArguments.SearchStatement);
      if (query.QueryType != QueryType.Collection)
        throw new ArgumentException(string.Format("The query '{0}' is not a collection query.", defaultSearchArguments.SearchStatement));

      var referencingDomainObject = referencingObject as DomainObject;

      var clientTransaction = referencingDomainObject != null ? referencingDomainObject.DefaultTransactionContext.ClientTransaction : ClientTransaction.Current;
      if (clientTransaction == null)
        throw new InvalidOperationException("No ClientTransaction has been associated with the current thread or the referencing object.");

      var result = clientTransaction.QueryManager.GetCollection(query);
      var availableObjects = new IBusinessObjectWithIdentity[result.Count];

      if (availableObjects.Length > 0)
        result.ToArray().CopyTo(availableObjects, 0);

      return availableObjects;
    }
  }
}
