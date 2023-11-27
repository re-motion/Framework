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
using System.Configuration;
using System.IO;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Represents the current query configuration.
  /// </summary>
  public class QueryConfiguration : ExtendedConfigurationSection
  {
    private const string c_defaultConfigurationFile = "queries.xml";

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty _queryFilesProperty;

    private readonly IAppContextProvider _contextProvider;

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

      _contextProvider = provider;

      _queryFilesProperty = new ConfigurationProperty(
          "queryFiles",
          typeof(ConfigurationElementCollection<QueryFileElement>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add(_queryFilesProperty);

      for (int i = 0; i < configurationFiles.Length; i++)
      {
        string configurationFile = configurationFiles[i];
        QueryFileElement element = new QueryFileElement(configurationFile);
        QueryFiles.Add(element);
      }
    }

    public string GetDefaultQueryFilePath ()
    {
      List<string> potentialPaths = GetPotentialDefaultQueryFilePaths();

      string? path = null;
      foreach (string potentialPath in potentialPaths)
      {
        if (File.Exists(potentialPath))
        {
          if (path != null)
          {
            string message = string.Format("Two default query configuration files found: '{0}' and '{1}'.", path, potentialPath);
            throw new ConfigurationException(message);
          }
          path = potentialPath;
        }
      }

      if (path == null)
      {
        string message = string.Format("No default query file found. Searched for one of the following files:\n{0}",
            string.Join("\n", potentialPaths));
        throw new ConfigurationException(message);
      }
      return path;
    }

    private List<string> GetPotentialDefaultQueryFilePaths ()
    {
      List<string> potentialPaths = new List<string>();
      potentialPaths.Add(Path.Combine(_contextProvider.BaseDirectory, c_defaultConfigurationFile));
      if (_contextProvider.RelativeSearchPath != null)
      {
        foreach (string part in _contextProvider.RelativeSearchPath.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries))
        {
          string absoluteSearchPath = Path.GetFullPath(Path.Combine(_contextProvider.BaseDirectory, part));
          potentialPaths.Add(Path.Combine(absoluteSearchPath, c_defaultConfigurationFile));
        }
      }
      return potentialPaths;
    }

    public ConfigurationElementCollection<QueryFileElement> QueryFiles
    {
      get { return (ConfigurationElementCollection<QueryFileElement>)this[_queryFilesProperty]; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    [Obsolete("Use ObjectFactory or IQueryDefinitionRepository instead. (Version 6.0.0)")]
    public QueryDefinitionCollection QueryDefinitions => throw new NotSupportedException("Use ObjectFactory or IQueryDefinitionRepository instead. (Version 6.0.0)");
  }
}
