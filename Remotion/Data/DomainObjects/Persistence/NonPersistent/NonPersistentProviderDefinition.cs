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
using System.Collections.Specialized;
using System.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent
{
  public class NonPersistentProviderDefinition: StorageProviderDefinition
  {
    public NonPersistentProviderDefinition (string name, INonPersistentStorageObjectFactory factory)
        : base(name, factory)
    {
      ArgumentUtility.CheckNotNull("factory", factory);
    }

    public NonPersistentProviderDefinition (string name, NameValueCollection config)
        : base(name, config)
    {
      ArgumentUtility.CheckNotNull("config", config);

      if (!(base.Factory is INonPersistentStorageObjectFactory))
      {
        var message = string.Format(
            "The factory type for the storage provider defined by '{0}' must implement the 'INonPersistentStorageObjectFactory' interface. "
            + "'{1}' does not implement that interface.",
            name,
            base.Factory.GetType().Name);
        throw new ConfigurationErrorsException(message);
      }
    }

    public new INonPersistentStorageObjectFactory Factory
    {
      get { return (INonPersistentStorageObjectFactory) base.Factory; }
    }

    public override bool IsIdentityTypeSupported (Type identityType)
    {
      ArgumentUtility.CheckNotNull("identityType", identityType);

      return (identityType == typeof(Guid));
    }
  }
}