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
using System.Linq;
using System.Runtime.Serialization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  [Serializable]
  [ImplementationFor(typeof(IPersistenceExtensionFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompoundPersistenceExtensionFactory : IPersistenceExtensionFactory,
#pragma warning disable SYSLIB0050
      IObjectReference
#pragma warning restore SYSLIB0050
  {
    private readonly IReadOnlyCollection<IPersistenceExtensionFactory> _persistenceExtensionFactories;

    public CompoundPersistenceExtensionFactory (IEnumerable<IPersistenceExtensionFactory> persistenceExtensionFactories)
    {
      ArgumentUtility.CheckNotNull("persistenceExtensionFactories", persistenceExtensionFactories);

      _persistenceExtensionFactories = persistenceExtensionFactories.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<IPersistenceExtensionFactory> PersistenceExtensionFactories
    {
      get { return _persistenceExtensionFactories; }
    }

    public IEnumerable<IPersistenceExtension> CreatePersistenceExtensions (Guid clientTransactionID)
    {
      return _persistenceExtensionFactories.SelectMany(f => f.CreatePersistenceExtensions(clientTransactionID));
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      var persistenceExtensionFactory = SafeServiceLocator.Current.GetInstance<IPersistenceExtensionFactory>();

      if (persistenceExtensionFactory is not CompoundPersistenceExtensionFactory)
      {
        throw new SerializationException(
            "The instance cannot be deserialized because the instance of the IPersistenceExtensionFactory "
            + "resolved via SafeServiceLocator is not of type 'CompoundPersistenceExtensionFactory'.");
      }

      return persistenceExtensionFactory;
    }
  }
}
