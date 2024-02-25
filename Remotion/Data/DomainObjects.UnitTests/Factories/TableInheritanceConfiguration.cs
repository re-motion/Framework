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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public sealed class TableInheritanceConfiguration
  {
    private static TableInheritanceConfiguration s_instance;

    public static TableInheritanceConfiguration Instance
    {
      get
      {
        if (s_instance == null)
          throw new InvalidOperationException("TableInheritanceConfiguration has not been Initialized by invoking Initialize()");
        return s_instance;
      }
    }

    public static void EnsureInitialized ()
    {
      if (s_instance != null)
        return;

      BaseConfiguration.EnsureInitialized();

      s_instance = new TableInheritanceConfiguration();
      s_instance.DisableDatabaseAccess();
    }

    private readonly TableInheritanceDomainObjectIDs _domainObjectIDs;

    private TableInheritanceConfiguration ()
    {
      _domainObjectIDs = new TableInheritanceDomainObjectIDs(GetMappingConfiguration());
    }

    public TableInheritanceDomainObjectIDs GetDomainObjectIDs ()
    {
      return _domainObjectIDs;
    }

    public void Register (DefaultServiceLocator defaultServiceLocator)
    {
      BaseConfiguration.Instance.Register(defaultServiceLocator);
    }

    public IStorageSettings GetStorageSettings ()
    {
      return BaseConfiguration.Instance.GetStorageSettings();
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return BaseConfiguration.Instance.GetMappingConfiguration();
    }

    public void DisableDatabaseAccess ()
    {
      BaseConfiguration.Instance.DisableDatabaseAccess();
    }

    public void EnableDatabaseAccess ()
    {
      BaseConfiguration.Instance.EnableDatabaseAccess();
    }
  }
}
