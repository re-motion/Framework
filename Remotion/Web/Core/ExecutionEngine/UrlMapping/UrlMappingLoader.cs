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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using Remotion.Utilities;
using Remotion.Web.Schemas;
using Remotion.Xml;

namespace Remotion.Web.ExecutionEngine.UrlMapping
{
  public class UrlMappingLoader
  {
    // types

    // static members and constants

    // member fields

    private string? _configurationFile;
    private Type? _type;
    private XmlSchemaSet? _schemas;

    // construction and disposing

    public UrlMappingLoader (string configurationFile, Type type)
    {
      Initialize(configurationFile, type, new UrlMappingSchema());
    }

    // methods and properties

    public UrlMappingConfiguration CreateUrlMappingConfiguration ()
    {
      return (UrlMappingConfiguration)LoadConfiguration(_configurationFile!, _type!, _schemas!);
    }

    [MemberNotNull(nameof(_configurationFile))]
    [MemberNotNull(nameof(_type))]
    [MemberNotNull(nameof(_schemas))]
    protected void Initialize (string configurationFile, Type type, params SchemaLoaderBase[] schemas)
    {
      ArgumentUtility.CheckNotNullOrEmpty("configurationFile", configurationFile);
      ArgumentUtility.CheckNotNull("type", type);

      _configurationFile = GetMappingFilePath(configurationFile);
      if (!File.Exists(_configurationFile))
        throw new FileNotFoundException(string.Format("Configuration file '{0}' could not be found.", configurationFile), "configurationFile");

      _type = type;
      _schemas = GetSchemas(schemas);
    }

    private string GetMappingFilePath (string configurationFile)
    {
      if (configurationFile.StartsWith("~/"))
        return HttpContext.Current.Server.MapPath(configurationFile);

      if (!Path.IsPathRooted(configurationFile))
        return Path.Combine(GetExecutingAssemblyPath(), configurationFile);

      return configurationFile;
    }

    protected virtual object LoadConfiguration (string configurationFile, Type type, XmlSchemaSet schemas)
    {
      ArgumentUtility.CheckNotNullOrEmpty("configurationFile", configurationFile);
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("schemas", schemas);

      using (XmlTextReader reader = new XmlTextReader(configurationFile))
      {
        return XmlSerializationUtility.DeserializeUsingSchema(reader, type, schemas);
      }
      //    try
      //    {
      //    return XmlSerializationUtility.DeserializeUsingSchema (reader, _configurationFile, _type, _schemas);
      //    }
      //    catch (XmlSchemaException e)
      //    {
      //      throw CreateMappingException (e, "Error while reading mapping: {0}", e.Message);
      //    }
      //    catch (XmlException e)
      //    {
      //      throw CreateMappingException (e, "Error while reading mapping: {0}", e.Message);
      //    }
    }

    protected virtual XmlSchemaSet GetSchemas (SchemaLoaderBase[] schemas)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("schemas", schemas);

      XmlSchemaSet schemaSet = new XmlSchemaSet();
      foreach (SchemaLoaderBase schema in schemas)
        schemaSet.Add(schema.LoadSchemaSet());
      return schemaSet;
    }

    public string? ConfigurationFile
    {
      get { return _configurationFile; }
    }

    public Type? Type
    {
      get { return _type; }
    }

    public XmlSchemaSet? Schemas
    {
      get { return _schemas; }
    }

    private string GetExecutingAssemblyPath ()
    {
      var assembly = typeof(UrlMappingLoader).Assembly;
#if NETFRAMEWORK
      AssemblyName assemblyName = assembly.GetName(copiedName: false);

      Uri codeBaseUri = new Uri(assemblyName.EscapedCodeBase!);
      return Path.GetDirectoryName(codeBaseUri.LocalPath)!; // TODO RM-8118: Add notnull assertion
#else
      var assemblyLocation = assembly.Location;
      if (string.IsNullOrEmpty(assemblyLocation))
        throw new InvalidOperationException(string.Format("Assembly '{0}' does not have a location. It was likely loaded from a byte array.", assembly.FullName));

      var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
      Assertion.IsNotNull(assemblyDirectory, "Assembly location '{0}' does not contain a valid directory name.", assemblyLocation);
      return assemblyDirectory;
#endif
    }
  }
}
