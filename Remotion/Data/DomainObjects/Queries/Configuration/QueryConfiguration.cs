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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Persistence;
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

    private readonly DoubleCheckedLockingContainer<QueryDefinitionCollection> _queries;

    public QueryConfiguration ()
    {
      _queries = new DoubleCheckedLockingContainer<QueryDefinitionCollection> (LoadAllQueryDefinitions);

      _queryFilesProperty = new ConfigurationProperty (
          "queryFiles",
          typeof (ConfigurationElementCollection<QueryFileElement>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_queryFilesProperty);
    }

    public QueryConfiguration (params string[] configurationFiles) : this()
    {
      ArgumentUtility.CheckNotNull ("configurationFiles", configurationFiles);

      for (int i = 0; i < configurationFiles.Length; i++)
      {
        string configurationFile = configurationFiles[i];
        QueryFileElement element = new QueryFileElement (configurationFile);
        QueryFiles.Add (element);
      }
    }

    private QueryDefinitionCollection LoadAllQueryDefinitions ()
    {
      var storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);

      if (QueryFiles.Count == 0)
        return new QueryConfigurationLoader (GetDefaultQueryFilePath(), storageProviderDefinitionFinder).GetQueryDefinitions ();
      else
      {
        QueryDefinitionCollection result = new QueryDefinitionCollection ();

        for (int i = 0; i < QueryFiles.Count; i++)
        {
          QueryConfigurationLoader loader = new QueryConfigurationLoader (QueryFiles[i].RootedFileName, storageProviderDefinitionFinder);
            QueryDefinitionCollection queryDefinitions = loader.GetQueryDefinitions ();
          try
          {
            result.Merge (queryDefinitions);
          }
          catch (DuplicateQueryDefinitionException ex)
          {
            string message = string.Format ("File '{0}' defines a duplicate for query definition '{1}'.", QueryFiles[i].RootedFileName,
              ex.QueryDefinition.ID);
            throw new ConfigurationException (message);
          }
        }
        return result;
      }
    }

    public string GetDefaultQueryFilePath ()
    {
      List<string> potentialPaths = GetPotentialDefaultQueryFilePaths();

      string path = null;
      foreach (string potentialPath in potentialPaths)
      {
        if (File.Exists (potentialPath))
        {
          if (path != null)
          {
            string message = string.Format ("Two default query configuration files found: '{0}' and '{1}'.", path, potentialPath);
            throw new ConfigurationException (message);
          }
          path = potentialPath;
        }
      }

      if (path == null)
      {
        string message = string.Format ("No default query file found. Searched for one of the following files:\n{0}",
            string.Join ("\n", potentialPaths));
        throw new ConfigurationException (message);
      }
      return path;
    }

    private List<string> GetPotentialDefaultQueryFilePaths ()
    {
      List<string> potentialPaths = new List<string> ();
      potentialPaths.Add (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, c_defaultConfigurationFile));
      if (AppDomain.CurrentDomain.RelativeSearchPath != null)
      {
        foreach (string part in AppDomain.CurrentDomain.RelativeSearchPath.Split (new[] {';'}, StringSplitOptions.RemoveEmptyEntries))
        {
          string absoluteSearchPath = Path.GetFullPath (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, part));
          potentialPaths.Add (Path.Combine (absoluteSearchPath, c_defaultConfigurationFile));
        }
      }
      return potentialPaths;
    }

    public ConfigurationElementCollection<QueryFileElement> QueryFiles
    {
      get { return (ConfigurationElementCollection<QueryFileElement>) this[_queryFilesProperty]; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public QueryDefinitionCollection QueryDefinitions
    {
      get { return _queries.Value; }
    }
  }
}
