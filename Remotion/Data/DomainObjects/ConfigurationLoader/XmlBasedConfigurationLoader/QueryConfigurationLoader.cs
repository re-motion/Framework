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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Schemas;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public class QueryConfigurationLoader : BaseFileLoader
  {
    private readonly StorageGroupBasedStorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    // types

    // static members and constants

    // member fields

    // construction and disposing

    //TODO: resolve parameter
    public QueryConfigurationLoader (string configurationFile, StorageGroupBasedStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      try
      {
        Initialize(
            configurationFile,
            SchemaLoader.Queries,
            true,
            PrefixNamespace.QueryConfigurationNamespace);
      }
      catch (ConfigurationException e)
      {
        throw CreateQueryConfigurationException(
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath(configurationFile));
      }
      catch (XmlSchemaException e)
      {
        throw CreateQueryConfigurationException(
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath(configurationFile));
      }
      catch (XmlException e)
      {
        throw CreateQueryConfigurationException(
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath(configurationFile));
      }
      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
    }

    // methods and properties

    public QueryDefinitionCollection GetQueryDefinitions ()
    {
      QueryDefinitionCollection queries = new QueryDefinitionCollection();
      FillQueryDefinitions(queries);
      return queries;
    }

    private void FillQueryDefinitions (QueryDefinitionCollection queries)
    {
      XmlNodeList? queryNodeList = Document.SelectNodes(FormatXPath("{0}:queries/{0}:query"), NamespaceManager);

      Assertion.DebugIsNotNull(queryNodeList, "queryNodeList != null");
      foreach (XmlNode queryNode in queryNodeList)
        queries.Add(GetQueryDefinition(queryNode));
    }

    private QueryDefinition GetQueryDefinition (XmlNode queryNode)
    {
      var queryIDNode = queryNode.SelectSingleNode("@id", NamespaceManager);
      Assertion.DebugIsNotNull(queryIDNode, "queryIDNode != null");
      string queryID = queryIDNode.InnerText;

      var queryTypeNode = queryNode.SelectSingleNode("@type", NamespaceManager);
      Assertion.DebugIsNotNull(queryTypeNode, "queryTypeNode != null");
      string queryTypeAsString = queryTypeNode.InnerText.Replace("-", "");
      QueryType queryType = (QueryType)Enum.Parse(typeof(QueryType), queryTypeAsString, true);

      XmlNode? node = queryNode.SelectSingleNode(FormatXPath("{0}:storageGroupType"), NamespaceManager);
      StorageProviderDefinition storageProviderDefinition;
      if (node != null)
        storageProviderDefinition = GetStorageProviderDefinition(node.InnerText);
      else
        storageProviderDefinition = GetStorageProviderDefinition(null);

      var queryStatementNode = queryNode.SelectSingleNode(FormatXPath("{0}:statement"), NamespaceManager);
      Assertion.DebugIsNotNull(queryStatementNode, "queryStatementNode != null");
      string statement = queryStatementNode.InnerText;

      Type? collectionType = LoaderUtility.GetOptionalType(queryNode, FormatXPath("{0}:collectionType"), NamespaceManager);

      if (queryType == QueryType.ScalarReadOnly && collectionType != null)
        throw CreateQueryConfigurationException("A scalar query '{0}' must not specify a collectionType.", queryID);

      if (queryType == QueryType.ScalarReadWrite && collectionType != null)
        throw CreateQueryConfigurationException("A scalar query '{0}' must not specify a collectionType.", queryID);

      return new QueryDefinition(queryID, storageProviderDefinition, statement, queryType, collectionType);
    }

    private string FormatXPath (string xPath)
    {
      return NamespaceManager.FormatXPath(xPath, PrefixNamespace.QueryConfigurationNamespace.Uri);
    }

    private StorageProviderDefinition GetStorageProviderDefinition (string? storageGroupName)
    {
      Type? storageGroupType = storageGroupName == null ? null : TypeUtility.GetType(storageGroupName, true);
      return _storageProviderDefinitionFinder.GetStorageProviderDefinition(storageGroupType, "File: " + ConfigurationFile);
    }

    private QueryConfigurationException CreateQueryConfigurationException (string message, params object[] args)
    {
      return CreateQueryConfigurationException(null, message, args);
    }

    private QueryConfigurationException CreateQueryConfigurationException (Exception? inner, string message, params object[] args)
    {
      return new QueryConfigurationException(string.Format(message, args), inner);
    }
  }
}
