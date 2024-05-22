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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// The <see cref="ScriptGenerator"/> is responsible to return a collection of <see cref="Script"/>-objects for all <see cref="ClassDefinition"/>s
  /// grouped by their <see cref="StorageProviderDefinition"/>.
  /// </summary>
  public class ScriptGenerator
  {
    private readonly Func<RdbmsProviderDefinition, IScriptBuilder> _scriptBuilderFactory;
    private readonly IRdbmsStructuredTypeDefinitionProvider _structuredTypeDefinitionProvider;
    private readonly IRdbmsStorageEntityDefinitionProvider _entityDefinitionProvider;
    private readonly IScriptToStringConverter _scriptToStringConverter;

    public ScriptGenerator (
        Func<RdbmsProviderDefinition, IScriptBuilder> scriptBuilderFactory,
        IRdbmsStructuredTypeDefinitionProvider structuredTypeDefinitionProvider,
        IRdbmsStorageEntityDefinitionProvider entityDefinitionProvider,
        IScriptToStringConverter scriptToStringConverter)
    {
      ArgumentUtility.CheckNotNull("scriptBuilderFactory", scriptBuilderFactory);
      ArgumentUtility.CheckNotNull("structuredTypeDefinitionProvider", structuredTypeDefinitionProvider);
      ArgumentUtility.CheckNotNull("entityDefinitionProvider", entityDefinitionProvider);
      ArgumentUtility.CheckNotNull("scriptToStringConverter", scriptToStringConverter);

      _scriptBuilderFactory = scriptBuilderFactory;
      _entityDefinitionProvider = entityDefinitionProvider;
      _scriptToStringConverter = scriptToStringConverter;
      _structuredTypeDefinitionProvider = structuredTypeDefinitionProvider;
    }

    public IEnumerable<Script> GetScripts (IEnumerable<ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull("classDefinitions", classDefinitions);

      var classDefinitionsByStorageProvider =
          from cd in classDefinitions
          where cd.StorageEntityDefinition.StorageProviderDefinition is RdbmsProviderDefinition
          group cd by cd.StorageEntityDefinition.StorageProviderDefinition
          into g
            select new { StorageProviderDefinition = (RdbmsProviderDefinition)g.Key, ClassDefinitions = g };

      foreach (var group in classDefinitionsByStorageProvider)
      {
        var scriptBuilder = _scriptBuilderFactory(group.StorageProviderDefinition);

        var types = _structuredTypeDefinitionProvider.GetTypeDefinitions(group.StorageProviderDefinition.Factory.CreateStorageTypeInformationProvider(group.StorageProviderDefinition));
        foreach (var typeDefinition in types)
          scriptBuilder.AddStructuredTypeDefinition(typeDefinition);

        var entities = _entityDefinitionProvider.GetEntityDefinitions(group.ClassDefinitions);
        foreach (var entityDefinition in entities)
          scriptBuilder.AddEntityDefinition(entityDefinition);

        var scripts = _scriptToStringConverter.Convert(scriptBuilder);
        yield return new Script(group.StorageProviderDefinition, scripts.SetUpScript, scripts.TearDownScript);
      }
    }
  }
}
