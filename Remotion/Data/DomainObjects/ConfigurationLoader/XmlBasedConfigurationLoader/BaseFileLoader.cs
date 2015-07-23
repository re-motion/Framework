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
using Remotion.Data.DomainObjects.Schemas;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
// TODO for all configuration loaders: Check if every field is trimmed during loading.
  public class BaseFileLoader
  {
    // types

    // static members and constants

    // member fields

    private XmlDocument _document;
    private ConfigurationNamespaceManager _namespaceManager;
    private string _configurationFile;
    private bool _resolveTypes;

    // construction and disposing

    protected BaseFileLoader ()
    {
    }

    protected void Initialize (
        string configurationFile,
        SchemaLoader schemaLoader,
        bool resolveTypes,
        PrefixNamespace schemaNamespace)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);
      ArgumentUtility.CheckNotNull ("schemaLoader", schemaLoader);
      ArgumentUtility.CheckNotNull ("schemaNamespace", schemaNamespace);

      if (!File.Exists (configurationFile))
        throw new FileNotFoundException (string.Format ("Configuration file '{0}' could not be found.", configurationFile), configurationFile);

      _configurationFile = Path.GetFullPath (configurationFile);
      _resolveTypes = resolveTypes;

      _document = LoadConfigurationFile (_configurationFile, schemaLoader, schemaNamespace.Uri);
      _namespaceManager = new ConfigurationNamespaceManager (_document, new PrefixNamespace[] { schemaNamespace });
    }

    // methods and properties

    private XmlDocument LoadConfigurationFile (
        string configurationFile,
        SchemaLoader schemaLoader,
        string schemaNamespace)
    {
      using (XmlTextReader textReader = new XmlTextReader (configurationFile))
      {
        XmlReaderSettings validatingReaderSettings = new XmlReaderSettings ();
        validatingReaderSettings.ValidationType = ValidationType.Schema;
        validatingReaderSettings.Schemas.Add (schemaLoader.LoadSchemaSet ());

        using (XmlReader validatingReader = XmlReader.Create (textReader, validatingReaderSettings))
        {
          XmlDocument document = new XmlDocument (new NameTable ());
          document.Load (validatingReader);

          if (document.DocumentElement.NamespaceURI != schemaNamespace)
          {
            throw CreateConfigurationException (
                "The namespace '{0}' of the root element is invalid. Expected namespace: '{1}'.",
                document.DocumentElement.NamespaceURI, schemaNamespace);
          }

          return document;
        }
      }
    }

    public string ConfigurationFile
    {
      get { return _configurationFile; }
    }

    public bool ResolveTypes
    {
      get { return _resolveTypes; }
    }

    protected XmlDocument Document
    {
      get { return _document; }
    }

    protected ConfigurationNamespaceManager NamespaceManager
    {
      get { return _namespaceManager; }
    }

    private ConfigurationException CreateConfigurationException (string format, params string[] args)
    {
      return new ConfigurationException (string.Format (format, args));
    }
  }
}
