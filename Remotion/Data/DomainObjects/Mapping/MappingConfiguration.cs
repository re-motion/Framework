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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public class MappingConfiguration : IMappingConfiguration
  {
    /// <summary>Workaround to allow reflection to reset the fields since setting a static readonly field is not supported in .NET 3.0 and later.</summary>
    private class Fields
    {
      // ReSharper disable once MemberHidesStaticFromOuterClass
      public readonly DoubleCheckedLockingContainer<IMappingConfiguration> Current = new DoubleCheckedLockingContainer<IMappingConfiguration>(
          () => new MappingConfiguration(
              DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader(),
              new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage))));
    }

    private static readonly Fields s_fields = new Fields();
    private static readonly ILog s_log = LogManager.GetLogger(typeof(MappingConfiguration));

    public static IMappingConfiguration Current
    {
      get { return s_fields.Current.Value; }
    }

    public static void SetCurrent (IMappingConfiguration mappingConfiguration)
    {
      if (mappingConfiguration != null)
      {
        if (!mappingConfiguration.ResolveTypes)
          throw CreateArgumentException("mappingConfiguration", "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.");
      }

      s_fields.Current.Value = mappingConfiguration!;
    }

    private static ArgumentException CreateArgumentException (Exception? innerException, string argumentName, string message, params object[] args)
    {
      return new ArgumentException(string.Format(message, args), argumentName, innerException);
    }

    private static ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return CreateArgumentException(null, argumentName, message, args);
    }

    // member fields

    private readonly ReadOnlyDictionary<Type, TypeDefinition> _typeDefinitions;
    private readonly ReadOnlyDictionary<string, ClassDefinition> _classDefinitions;
    private readonly ReadOnlyDictionary<string, RelationDefinition> _relationDefinitions;
    private readonly bool _resolveTypes;
    private readonly IMemberInformationNameResolver _nameResolver;

    // construction and disposing

    public MappingConfiguration (IMappingLoader mappingLoader, IPersistenceModelLoader persistenceModelLoader)
    {
      ArgumentUtility.CheckNotNull("mappingLoader", mappingLoader);
      ArgumentUtility.CheckNotNull("persistenceModelLoader", persistenceModelLoader);

      s_log.Info("Building mapping configuration...");

      using (StopwatchScope.CreateScope(s_log, LogLevel.Info, "Time needed to build and validate mapping configuration: {elapsed}."))
      {
        var mappingConfigurationValidationHelper = new MappingConfigurationValidationHelper(mappingLoader, persistenceModelLoader);

        var typeDefinitions = mappingLoader.GetTypeDefinitions();
        _typeDefinitions = new ReadOnlyDictionary<Type, TypeDefinition>(typeDefinitions.ToDictionary(td => td.Type));
        mappingConfigurationValidationHelper.ValidateDuplicateClassIDs(typeDefinitions);
        _classDefinitions = new ReadOnlyDictionary<string, ClassDefinition>(typeDefinitions.OfType<ClassDefinition>().ToDictionary(cd => cd.ID));

        mappingConfigurationValidationHelper.ValidateTypeDefinitions(_typeDefinitions.Values);
        mappingConfigurationValidationHelper.ValidatePropertyDefinitions(_typeDefinitions.Values);

        var relationDefinitions = mappingLoader.GetRelationDefinitions(_typeDefinitions);
        _relationDefinitions = new ReadOnlyDictionary<string, RelationDefinition>(relationDefinitions.ToDictionary(rd => rd.ID));

        mappingConfigurationValidationHelper.ValidateRelationDefinitions(_relationDefinitions.Values);

        foreach (var inheritanceRoot in TypeDefinitionHierarchy.GetHierarchyRoots(typeDefinitions))
        {
          persistenceModelLoader.ApplyPersistenceModelToHierarchy(inheritanceRoot);
          mappingConfigurationValidationHelper.VerifyPersistenceModelApplied(inheritanceRoot);
          mappingConfigurationValidationHelper.ValidatePersistenceMapping(inheritanceRoot);
        }

        _resolveTypes = mappingLoader.ResolveTypes;
        _nameResolver = mappingLoader.NameResolver;

        SetMappingReadOnly();

        mappingConfigurationValidationHelper.ValidateSortExpression(_relationDefinitions.Values);
      }
    }

    /// <summary>
    /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
    /// </summary>
    public bool ResolveTypes
    {
      get { return _resolveTypes; }
    }

    public TypeDefinition[] GetTypeDefinitions ()
    {
      return _typeDefinitions.Values.ToArray();
    }

    public bool ContainsTypeDefinition (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return _typeDefinitions.ContainsKey(type);
    }

    public TypeDefinition GetTypeDefinition (Type type, Func<Type, Exception> missingTypeDefinitionExceptionFactory)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("missingTypeDefinitionExceptionFactory", missingTypeDefinitionExceptionFactory);

      var typeDefinition = _typeDefinitions.GetValueOrDefault(type);
      if (typeDefinition == null)
        throw missingTypeDefinitionExceptionFactory(type);

      return typeDefinition;
    }

    public bool ContainsClassDefinition (string classID)
    {
      ArgumentUtility.CheckNotNull("classID", classID);

      return _classDefinitions.ContainsKey(classID);
    }

    public ClassDefinition GetClassDefinition (string classID, Func<string, Exception> missingClassDefinitionExceptionFactory)
    {
      ArgumentUtility.CheckNotNullOrEmpty("classID", classID);
      ArgumentUtility.CheckNotNull("missingClassDefinitionExceptionFactory", missingClassDefinitionExceptionFactory);

      var classDefinition = _classDefinitions.GetValueOrDefault(classID);
      if (classDefinition == null)
        throw missingClassDefinitionExceptionFactory(classID);

      return classDefinition;
    }

    public IMemberInformationNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    private void SetMappingReadOnly ()
    {
      foreach (var typeDefinition in _typeDefinitions.Values)
        typeDefinition.SetReadOnly();
    }
  }
}
