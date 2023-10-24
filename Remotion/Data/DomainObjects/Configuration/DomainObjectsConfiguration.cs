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
using System.Configuration;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Configuration
{
  /// <summary>
  /// <see cref="ConfigurationSectionGroup"/> for grouping the <see cref="ConfigurationSection"/> in the <b>Remotion.Data.DomainObjects</b> namespace.
  /// </summary>
  public sealed class DomainObjectsConfiguration: ConfigurationSectionGroup, IDomainObjectsConfiguration
  {
    private const string MappingLoaderPropertyName = "mapping";
    private const string StoragePropertyName = "storage";
    private const string QueryPropertyName = "query";

    private static readonly DoubleCheckedLockingContainer<IDomainObjectsConfiguration> s_current =
        new DoubleCheckedLockingContainer<IDomainObjectsConfiguration>(delegate { return new DomainObjectsConfiguration(); });

    public static IDomainObjectsConfiguration Current
    {
      get { return s_current.Value; }
    }

    public static void SetCurrent (IDomainObjectsConfiguration configuration)
    {
      s_current.Value = configuration;
    }

    public DomainObjectsConfiguration ()
    {
      _persistenceConfiguration = new DoubleCheckedLockingContainer<StorageConfiguration>(GetPersistenceConfiguration);
    }

    private readonly DoubleCheckedLockingContainer<StorageConfiguration> _persistenceConfiguration;

    [ConfigurationProperty(StoragePropertyName)]
    public StorageConfiguration Storage
    {
      get { return _persistenceConfiguration.Value; }
    }

    [ConfigurationProperty(QueryPropertyName)]
    [Obsolete("QueryConfiguration is no longer supported. (Version 6.0.0)", true)]
    public QueryConfiguration Query => throw new NotSupportedException("QueryConfiguration is no longer supported. (Version 6.0.0)");

    private StorageConfiguration GetPersistenceConfiguration ()
    {
      return
          (StorageConfiguration?)ConfigurationWrapper.Current.GetSection(ConfigKey + "/" + StoragePropertyName, false)
          ?? new StorageConfiguration();
    }

    private string ConfigKey
    {
      get { return string.IsNullOrEmpty(SectionGroupName) ? "remotion.data.domainObjects" : SectionGroupName; }
    }
  }
}
