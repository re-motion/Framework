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
using System.Linq;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.ServiceLocation;
using Remotion.Tools;

namespace Remotion.Data.DomainObjects.RdbmsTools
{
  /// <summary>
  /// The <see cref="RdbmsToolsRunner"/> type contains the encapsulates the execution of the various functionality provided by the 
  /// <b>Remotion.Data.DomainObjects.RdbmsTools</b> assembly.
  /// </summary>
  public class RdbmsToolsRunner : CustomAppContextRunnerBase
  {
    private readonly RdbmsToolsParameters _rdbmsToolsParameters;

    public RdbmsToolsRunner (RdbmsToolsParameters rdbmsToolsParameters)
        : base(rdbmsToolsParameters.BaseDirectory, null)
    {
      _rdbmsToolsParameters = rdbmsToolsParameters;
    }

    protected override void RunImplementation ()
    {
      if (_rdbmsToolsParameters.Verbose)
      {
        //TODO: RM-9195
        // Configure console logging 
        // log4net.Config.BaseConfigurator.Configure();
      }

      InitializeConfiguration();

      if ((_rdbmsToolsParameters.Mode & OperationMode.BuildSchema) != 0)
        BuildSchema();

      if ((_rdbmsToolsParameters.Mode & OperationMode.ExportMappingXml) != 0)
        ExportMapping();
    }

    protected virtual void InitializeConfiguration ()
    {
      if (ServiceLocator.IsLocationProviderSet == false)
      {
        var storageSettingsFactory = StorageSettingsFactory.CreateForSqlServer(_rdbmsToolsParameters.ConnectionString);
        var serviceLocator = DefaultServiceLocator.Create();
        serviceLocator.RegisterSingle(() => storageSettingsFactory);
        ServiceLocator.SetLocatorProvider(() => serviceLocator);
      }

      MappingConfiguration.SetCurrent(
          MappingConfiguration.Create(
              SafeServiceLocator.Current.GetInstance<IMappingLoader>(),
              SafeServiceLocator.Current.GetInstance<IPersistenceModelLoader>()));
    }

    protected virtual void BuildSchema ()
    {
      var scriptGenerator = new ScriptGenerator(
          pd => pd.Factory.CreateSchemaScriptBuilder(pd), new RdbmsStorageEntityDefinitionProvider(), new RdbmsStructuredTypeDefinitionProvider(),
          new ScriptToStringConverter());
      var scripts = scriptGenerator.GetScripts(MappingConfiguration.Current.GetTypeDefinitions());
      var fileGenerator = new FileGenerator(_rdbmsToolsParameters.SchemaOutputDirectory);
      var includeStorageProviderName = scripts.Count() > 1;
      foreach (var script in scripts)
        fileGenerator.WriteScriptsToDisk(script, includeStorageProviderName);
    }

    protected virtual void ExportMapping ()
    {
      var mappingSerializer = new MappingSerializer(
          pd => pd.Factory.CreateEnumSerializer(),
          (pd, enumSerializer) => pd.Factory.CreateStorageProviderSerializer(enumSerializer));

      var xml = mappingSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinitions());
      xml.Save(_rdbmsToolsParameters.MappingExportOutputFileName);
    }
  }
}
