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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Schemas;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration.Loader
{
  /// <summary>
  /// Provides an API for loading <see cref="QueryDefinition"/>s from a specified file.
  /// </summary>
  public class QueryDefinitionFileLoader
  {
    private readonly IStorageSettings _storageSettings;

    public QueryDefinitionFileLoader (IStorageSettings storageSettings)
    {
      _storageSettings = storageSettings;
    }

    /// <summary>
    /// Loads <see cref="QueryDefinition"/>s from the specified <paramref name="configurationFile"/>.
    /// </summary>
    /// <exception cref="FileNotFoundException">The specified <paramref name="configurationFile"/> was not found.</exception>
    /// <exception cref="QueryConfigurationException">The specified <paramref name="configurationFile"/> could not be read.</exception>
    public IReadOnlyList<QueryDefinition> LoadQueryDefinitions (string configurationFile)
    {
      if (!File.Exists(configurationFile))
        throw new FileNotFoundException($"Configuration file '{configurationFile}' could not be found.", configurationFile);
      configurationFile = Path.GetFullPath(configurationFile);

      XmlDocument document;
      ConfigurationNamespaceManager namespaceManager;
      try
      {
        var schemaNamespace = PrefixNamespace.QueryConfigurationNamespace;
        document = LoadConfigurationFile(configurationFile, SchemaLoader.Queries, schemaNamespace.Uri);
        namespaceManager = new ConfigurationNamespaceManager(document, new[] { schemaNamespace });
      }
      catch (ConfigurationException ex)
      {
        throw new QueryConfigurationException($"Error while reading query configuration: {ex.Message} File: '{configurationFile}'.", ex);
      }
      catch (XmlSchemaException ex)
      {
        throw new QueryConfigurationException($"Error while reading query configuration: {ex.Message} File: '{configurationFile}'.", ex);
      }
      catch (XmlException ex)
      {
        throw new QueryConfigurationException($"Error while reading query configuration: {ex.Message} File: '{configurationFile}'.", ex);
      }

      var queryDefinitions = new List<QueryDefinition>();
      var queryNodeList = document.SelectNodes(FormatXPath("{0}:queries/{0}:query", namespaceManager), namespaceManager);

      Assertion.DebugIsNotNull(queryNodeList, "queryNodeList != null");
      foreach (XmlNode queryNode in queryNodeList)
        queryDefinitions.Add(GetQueryDefinition(queryNode, namespaceManager, configurationFile));

      return queryDefinitions;
    }

    private XmlDocument LoadConfigurationFile (
        string configurationFile,
        SchemaLoader schemaLoader,
        string schemaNamespace)
    {
      using var textReader = new XmlTextReader(configurationFile);
      var validatingReaderSettings = new XmlReaderSettings();
      validatingReaderSettings.ValidationType = ValidationType.Schema;
      validatingReaderSettings.Schemas.Add(schemaLoader.LoadSchemaSet());

      using var validatingReader = XmlReader.Create(textReader, validatingReaderSettings);
      var document = new XmlDocument(new NameTable());
      document.Load(validatingReader);

      Assertion.DebugIsNotNull(document.DocumentElement, "document.DocumentElement != null");
      if (document.DocumentElement.NamespaceURI != schemaNamespace)
      {
        throw new ConfigurationException($"The namespace '{document.DocumentElement.NamespaceURI}' of the root element is invalid. Expected namespace: '{schemaNamespace}'.");
      }

      return document;
    }

    private QueryDefinition GetQueryDefinition (XmlNode queryNode, ConfigurationNamespaceManager namespaceManager, string configurationFile)
    {
      var queryIDNode = queryNode.SelectSingleNode("@id", namespaceManager);
      Assertion.DebugIsNotNull(queryIDNode, "queryIDNode != null");
      var queryID = queryIDNode.InnerText;

      var queryTypeNode = queryNode.SelectSingleNode("@type", namespaceManager);
      Assertion.DebugIsNotNull(queryTypeNode, "queryTypeNode != null");
      var queryTypeAsString = queryTypeNode.InnerText;
      var queryType = (QueryType)Enum.Parse(typeof(QueryType), queryTypeAsString, true);

      var node = queryNode.SelectSingleNode(FormatXPath("{0}:storageGroupType", namespaceManager), namespaceManager);
      var storageProviderDefinition = GetStorageProviderDefinition(node?.InnerText, configurationFile);

      var queryStatementNode = queryNode.SelectSingleNode(FormatXPath("{0}:statement", namespaceManager), namespaceManager);
      Assertion.DebugIsNotNull(queryStatementNode, "queryStatementNode != null");
      var statement = queryStatementNode.InnerText;

      var collectionType = GetOptionalType(queryNode, FormatXPath("{0}:collectionType", namespaceManager), namespaceManager);

      if (queryType == QueryType.Scalar && collectionType != null)
        throw CreateQueryConfigurationException("A scalar query '{0}' must not specify a collectionType.", queryID);

      return new QueryDefinition(queryID, storageProviderDefinition, statement, queryType, collectionType);
    }

    private string FormatXPath (string xPath, ConfigurationNamespaceManager namespaceManager)
    {
      return namespaceManager.FormatXPath(xPath, PrefixNamespace.QueryConfigurationNamespace.Uri);
    }

    private StorageProviderDefinition GetStorageProviderDefinition (string? storageGroupName, string configurationFile)
    {
      var storageGroupType = storageGroupName == null ? null : TypeUtility.GetType(storageGroupName, true);
      return _storageSettings.GetStorageProviderDefinition(storageGroupType);
    }

    private Type? GetOptionalType (XmlNode selectionNode, string xPath, XmlNamespaceManager namespaceManager)
    {
      XmlNode? typeNode = selectionNode.SelectSingleNode(xPath, namespaceManager);
      if (typeNode == null)
        return null;

      return TypeUtility.GetType(typeNode.InnerText.Trim(), throwOnError: true)!;
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
