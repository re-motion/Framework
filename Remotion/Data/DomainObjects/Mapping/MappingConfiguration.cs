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
using System.Threading;
using Remotion.Collections;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [ImplementationFor(typeof(IMappingConfiguration), Lifetime = LifetimeKind.Singleton)]
  public class MappingConfiguration : IMappingConfiguration
  {
    /// <summary>Workaround to allow reflection to reset the fields since setting a static readonly field is not supported in .NET 3.0 and later.</summary>
    private class Fields
    {
      // ReSharper disable once MemberHidesStaticFromOuterClass
      public readonly DoubleCheckedLockingContainer<IMappingConfiguration> Current = new DoubleCheckedLockingContainer<IMappingConfiguration>(
          () => SafeServiceLocator.Current.GetInstance<IMappingConfiguration>());
    }

    private class Mapping
    {
      public readonly ReadOnlyDictionary<Type, TypeDefinition> TypeDefinitions;
      public readonly ReadOnlyDictionary<string, ClassDefinition> ClassDefinitions;
      public readonly ReadOnlyDictionary<string, RelationDefinition> RelationDefinitions;

      public Mapping (
          ReadOnlyDictionary<Type, TypeDefinition> typeDefinitions,
          ReadOnlyDictionary<string, ClassDefinition> classDefinitions,
          ReadOnlyDictionary<string, RelationDefinition> relationDefinitions)
      {
        TypeDefinitions = typeDefinitions;
        ClassDefinitions = classDefinitions;
        RelationDefinitions = relationDefinitions;
      }
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

    /// <summary>
    /// Creates a fully initialized <see cref="MappingConfiguration"/>
    /// </summary>
    public static MappingConfiguration Create (IMappingLoader mappingLoader, IPersistenceModelLoader persistenceModelLoader)
    {
      ArgumentUtility.CheckNotNull("mappingLoader", mappingLoader);
      ArgumentUtility.CheckNotNull("persistenceModelLoader", persistenceModelLoader);

      var mappingConfiguration = new MappingConfiguration(mappingLoader, persistenceModelLoader);
      mappingConfiguration.EnsureInitialized();

      return mappingConfiguration;
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

    private readonly Lazy<Mapping> _mapping;
    private readonly bool _resolveTypes;
    private readonly IMemberInformationNameResolver _nameResolver;

    // construction and disposing

    public MappingConfiguration (IMappingLoader mappingLoader, IPersistenceModelLoader persistenceModelLoader)
    {
      ArgumentUtility.CheckNotNull("mappingLoader", mappingLoader);
      ArgumentUtility.CheckNotNull("persistenceModelLoader", persistenceModelLoader);

      _resolveTypes = mappingLoader.ResolveTypes;
      _nameResolver = mappingLoader.NameResolver;

      _mapping = new Lazy<Mapping>(() => InitializeMapping(mappingLoader, persistenceModelLoader), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private Mapping InitializeMapping (IMappingLoader mappingLoader, IPersistenceModelLoader persistenceModelLoader)
    {
      s_log.Info("Building mapping configuration...");

      using (StopwatchScope.CreateScope(s_log, LogLevel.Info, "Time needed to build and validate mapping configuration: {elapsed}."))
      {
        var mappingConfigurationValidationHelper = new MappingConfigurationValidationHelper(mappingLoader, persistenceModelLoader);

        var typeDefinitions = mappingLoader.GetTypeDefinitions();
        var typeDefinitionsDictionary = new ReadOnlyDictionary<Type, TypeDefinition>(typeDefinitions.ToDictionary(td => td.Type));
        mappingConfigurationValidationHelper.ValidateDuplicateClassIDs(typeDefinitions);
        var classDefinitionsDictionary = new ReadOnlyDictionary<string, ClassDefinition>(typeDefinitions.OfType<ClassDefinition>().ToDictionary(cd => cd.ID));

        mappingConfigurationValidationHelper.ValidateTypeDefinitions(typeDefinitionsDictionary.Values);
        mappingConfigurationValidationHelper.ValidatePropertyDefinitions(typeDefinitionsDictionary.Values);

        var relationDefinitions = mappingLoader.GetRelationDefinitions(typeDefinitionsDictionary);
        var relationDefinitionsDictionary = new ReadOnlyDictionary<string, RelationDefinition>(relationDefinitions.ToDictionary(rd => rd.ID));

        mappingConfigurationValidationHelper.ValidateRelationDefinitions(relationDefinitionsDictionary.Values);

        foreach (var inheritanceRoot in TypeDefinitionHierarchy.GetHierarchyRoots(typeDefinitionsDictionary.Values))
        {
          persistenceModelLoader.ApplyPersistenceModelToHierarchy(inheritanceRoot);
          mappingConfigurationValidationHelper.VerifyPersistenceModelApplied(inheritanceRoot);
          mappingConfigurationValidationHelper.ValidatePersistenceMapping(inheritanceRoot);
        }

        foreach (var typeDefinition in typeDefinitionsDictionary.Values)
          typeDefinition.SetReadOnly();

        mappingConfigurationValidationHelper.ValidateSortExpression(relationDefinitionsDictionary.Values);

        return new Mapping(typeDefinitionsDictionary, classDefinitionsDictionary, relationDefinitionsDictionary);
      }
    }

    /// <summary>
    /// Forces the lazy-initialized mapping to be evaluated. 
    /// </summary>
    public void EnsureInitialized ()
    {
      Assertion.IsNotNull(_mapping.Value);
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
      return _mapping.Value.TypeDefinitions.Values.ToArray();
    }

    public bool ContainsTypeDefinition (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return _mapping.Value.TypeDefinitions.ContainsKey(type);
    }

    public TypeDefinition GetTypeDefinition (Type type, Func<Type, Exception> missingTypeDefinitionExceptionFactory)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("missingTypeDefinitionExceptionFactory", missingTypeDefinitionExceptionFactory);

      var typeDefinition = _mapping.Value.TypeDefinitions.GetValueOrDefault(type);
      if (typeDefinition == null)
        throw missingTypeDefinitionExceptionFactory(type);

      return typeDefinition;
    }

    public bool ContainsClassDefinition (string classID)
    {
      ArgumentUtility.CheckNotNull("classID", classID);

      return _mapping.Value.ClassDefinitions.ContainsKey(classID);
    }

    public ClassDefinition GetClassDefinition (string classID, Func<string, Exception> missingClassDefinitionExceptionFactory)
    {
      ArgumentUtility.CheckNotNullOrEmpty("classID", classID);
      ArgumentUtility.CheckNotNull("missingClassDefinitionExceptionFactory", missingClassDefinitionExceptionFactory);

      var classDefinition = _mapping.Value.ClassDefinitions.GetValueOrDefault(classID);
      if (classDefinition == null)
        throw missingClassDefinitionExceptionFactory(classID);

      return classDefinition;
    }

    public IMemberInformationNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException(string.Format(message, args));
    }
  }
}
