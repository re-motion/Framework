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

    public IEnumerable<Script> GetScripts (IEnumerable<ClassDefinition> classDefinitions, IEnumerable<TupleDefinition> tupleDefinitions)
    {
      ArgumentUtility.CheckNotNull(nameof(classDefinitions), classDefinitions);
      ArgumentUtility.CheckNotNull(nameof(tupleDefinitions), tupleDefinitions);

      var definitionsByStorageProvider = new Dictionary<RdbmsProviderDefinition, (List<ClassDefinition> ClassDefinitions, List<TupleDefinition> TupleDefinitions)>();
      var rdbmsClassDefinitions = classDefinitions.Select(cd => (cd, cd.StorageEntityDefinition.StorageProviderDefinition as RdbmsProviderDefinition))
          .Where(o => o.Item2 is not null);

      foreach (var rdbmsClassDefinition in rdbmsClassDefinitions)
      {
        var classDefinition = rdbmsClassDefinition.Item1;
        var storageProviderDefinition = rdbmsClassDefinition.Item2!;
        if (!definitionsByStorageProvider.TryGetValue(storageProviderDefinition, out var definitions))
        {
          definitions = new ValueTuple<List<ClassDefinition>, List<TupleDefinition>>(new List<ClassDefinition>(), new List<TupleDefinition>());
          definitionsByStorageProvider.Add(storageProviderDefinition, definitions);
        }
        definitions.ClassDefinitions.Add(classDefinition);
      }

      var rdbmsTupleDefinitions = tupleDefinitions.Select(td => (td, td.StructuredTypeDefinition.StorageProviderDefinition as RdbmsProviderDefinition))
          .Where(o => o.Item2 is not null);

      foreach (var rdbmsTupleDefinition in rdbmsTupleDefinitions)
      {
        var tupleDefinition = rdbmsTupleDefinition.Item1;
        var storageProviderDefinition = rdbmsTupleDefinition.Item2!;
        if (!definitionsByStorageProvider.TryGetValue(storageProviderDefinition, out var definitions))
        {
          definitions = new ValueTuple<List<ClassDefinition>, List<TupleDefinition>>(new List<ClassDefinition>(), new List<TupleDefinition>());
          definitionsByStorageProvider.Add(storageProviderDefinition, definitions);
        }
        definitions.TupleDefinitions.Add(tupleDefinition);
      }

      foreach (var definitionSet in definitionsByStorageProvider)
      {
        var scriptBuilder = _scriptBuilderFactory(definitionSet.Key);

        var types = _structuredTypeDefinitionProvider.GetTypeDefinitions(definitionSet.Value.TupleDefinitions);
        foreach (var typeDefinition in types)
          scriptBuilder.AddStructuredTypeDefinition(typeDefinition);

        var entities = _entityDefinitionProvider.GetEntityDefinitions(definitionSet.Value.ClassDefinitions);
        foreach (var entityDefinition in entities)
          scriptBuilder.AddEntityDefinition(entityDefinition);

        var scripts = _scriptToStringConverter.Convert(scriptBuilder);
        yield return new Script(definitionSet.Key, scripts.SetUpScript, scripts.TearDownScript);
      }
    }
  }
}
