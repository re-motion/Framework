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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Represents the current query configuration.
  /// </summary>
  [Obsolete("QueryConfiguration is no longer supported. (Version 6.0.0)", true)]
  public class QueryConfiguration : ExtendedConfigurationSection
  {
    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    public QueryConfiguration () : this(new AppContextProvider(), new string[0])
    {
    }

    public QueryConfiguration (IAppContextProvider provider) : this(provider, new string[0])
    {
    }

    public QueryConfiguration (params string[] configurationFiles) : this(new AppContextProvider(), configurationFiles)
    {
    }

    public QueryConfiguration (IAppContextProvider provider, params string[] configurationFiles)
    {
      ArgumentUtility.CheckNotNull("provider", provider);
      ArgumentUtility.CheckNotNull("configurationFiles", configurationFiles);

      throw new NotSupportedException("QueryConfiguration is no longer supported. (Version 6.0.0)");
    }

    public string GetDefaultQueryFilePath ()
    {
      throw new NotSupportedException("QueryConfiguration is no longer supported. (Version 6.0.0)");
    }

    [Obsolete("Use IQueryFileFinder or BaseDirectoryBasedQueryFileFinder instead. (Version 6.0.0)")]
    public ConfigurationElementCollection<QueryFileElement> QueryFiles => throw new NotSupportedException("Use IQueryFileFinder or BaseDirectoryBasedQueryFileFinder instead. (Version 6.0.0)");

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    [Obsolete("Use ObjectFactory or IQueryDefinitionRepository instead. (Version 6.0.0)")]
    public QueryDefinitionCollection QueryDefinitions => throw new NotSupportedException("Use ObjectFactory or IQueryDefinitionRepository instead. (Version 6.0.0)");
  }
}
