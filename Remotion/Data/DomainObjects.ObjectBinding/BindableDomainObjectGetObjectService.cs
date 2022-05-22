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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectGetObjectService : IGetObjectService
  {
    public IBusinessObjectWithIdentity? GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNull("classWithIdentity", classWithIdentity);
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);

      var clientTransaction = ClientTransaction.Current;
      if (clientTransaction == null)
        throw new InvalidOperationException("No ClientTransaction has been associated with the current thread.");

      var objectID = ObjectID.Parse(uniqueIdentifier);
      var domainObjectOrNull = (DomainObject?)LifetimeService.TryGetObject(clientTransaction, objectID);
      if (domainObjectOrNull == null)
        return null;
      if (domainObjectOrNull.State.IsInvalid)
        return null;
      if (domainObjectOrNull.State.IsDeleted)
        return null;
      return (IBusinessObjectWithIdentity)domainObjectOrNull;
    }
  }
}
