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
using System.Linq;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Logging;
using Remotion.Tools;

namespace Remotion.Data.DomainObjects.RdbmsTools
{
  /// <summary>
  /// The <see cref="RdbmsToolsRunner"/> type contains the encapsulates the execution of the various functionality provided by the 
  /// <b>Remotion.Data.DomainObjects.RdbmsTools</b> assembly.
  /// </summary>
  [Serializable]
  public class RdbmsToolsRunner : AppDomainRunnerBase
  {
    public static RdbmsToolsRunner Create (RdbmsToolsParameters rdbmsToolsParameters)
    {
      AppDomainSetup appDomainSetup = CreateAppDomainSetup (rdbmsToolsParameters);
      return new RdbmsToolsRunner (appDomainSetup, rdbmsToolsParameters);
    }

    public static AppDomainSetup CreateAppDomainSetup (RdbmsToolsParameters rdbmsToolsParameters)
    {
      AppDomainSetup appDomainSetup = new AppDomainSetup();
      appDomainSetup.ApplicationName = "RdbmsTools";
      appDomainSetup.ApplicationBase = rdbmsToolsParameters.BaseDirectory;

      if (!string.IsNullOrEmpty (rdbmsToolsParameters.ConfigFile))
      {
        appDomainSetup.ConfigurationFile = Path.GetFullPath (rdbmsToolsParameters.ConfigFile);
        if (!File.Exists (appDomainSetup.ConfigurationFile))
        {
          throw new FileNotFoundException (
              string.Format (
                  "The configuration file supplied by the 'config' parameter was not found.\r\nFile: {0}",
                  appDomainSetup.ConfigurationFile),
              appDomainSetup.ConfigurationFile);
        }
      }
      return appDomainSetup;
    }

    private readonly RdbmsToolsParameters _rdbmsToolsParameters;

    protected RdbmsToolsRunner (AppDomainSetup appDomainSetup, RdbmsToolsParameters rdbmsToolsParameters)
        : base (appDomainSetup)
    {
      _rdbmsToolsParameters = rdbmsToolsParameters;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      if (_rdbmsToolsParameters.Verbose)
        LogManager.InitializeConsole();

      InitializeConfiguration();

      if (!string.IsNullOrEmpty (_rdbmsToolsParameters.SchemaFileBuilderTypeName))
      {
        throw new NotSupportedException (
            "The schemaBuilder parameter is obsolete and should no longer be used. "
            + "(The schema file builder is now retrieved from the storage provider definition.)");
      }

      if ((_rdbmsToolsParameters.Mode & OperationMode.BuildSchema) != 0)
        BuildSchema();
      
      if ((_rdbmsToolsParameters.Mode & OperationMode.ExportMappingXml) != 0)
        ExportMapping();
    }

    protected virtual void InitializeConfiguration ()
    {
      DomainObjectsConfiguration.SetCurrent (
          new FakeDomainObjectsConfiguration (
              DomainObjectsConfiguration.Current.MappingLoader, GetPersistenceConfiguration(), new QueryConfiguration()));

      MappingConfiguration.SetCurrent (
          new MappingConfiguration (
              DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader(),
              new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage))));
    }

    protected StorageConfiguration GetPersistenceConfiguration ()
    {
      StorageConfiguration storageConfiguration = DomainObjectsConfiguration.Current.Storage;
      if (storageConfiguration.StorageProviderDefinitions.Count == 0)
      {
        ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
        RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition (
            "Default", new SqlStorageObjectFactory(), "Initial Catalog=DatabaseName;");
        storageProviderDefinitionCollection.Add (providerDefinition);

        storageConfiguration = new StorageConfiguration (storageProviderDefinitionCollection, providerDefinition);
      }

      return storageConfiguration;
    }

    protected virtual void BuildSchema ()
    {
      var scriptGenerator = new ScriptGenerator (
          pd => pd.Factory.CreateSchemaScriptBuilder (pd), new RdbmsStorageEntityDefinitionProvider(), new ScriptToStringConverter());
      var scripts = scriptGenerator.GetScripts (MappingConfiguration.Current.GetTypeDefinitions());
      var fileGenerator = new FileGenerator (_rdbmsToolsParameters.SchemaOutputDirectory);
      var includeStorageProviderName = scripts.Count() > 1;
      foreach (var script in scripts)
        fileGenerator.WriteScriptsToDisk (script, includeStorageProviderName);
    }

    protected virtual void ExportMapping ()
    {
      var mappingSerializer = new MappingSerializer (
          pd => pd.Factory.CreateEnumSerializer(),
          (pd, enumSerializer) => pd.Factory.CreateStorageProviderSerializer (enumSerializer));

      var xml = mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());
      xml.Save (_rdbmsToolsParameters.MappingExportOutputFileName);
    }
  }
}