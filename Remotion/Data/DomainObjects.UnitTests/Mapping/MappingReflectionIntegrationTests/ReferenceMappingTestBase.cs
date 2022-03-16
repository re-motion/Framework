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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  /// <summary>
  /// Provides methods for testing the mapping code using a small test domain that is compared against a set of reference <see cref="TypeDefinition"/>s.
  /// The test domain is defined as nested types (preferably private) while the reference <see cref="TypeDefinition"/>s are created using the <see cref="CreateReferenceTypeDefinitions"/> method.
  /// </summary>
  [TestFixture]
  public abstract class ReferenceMappingTestBase
  {
    protected TypeDefinitionChecker TypeDefinitionChecker { get; } = new();

    protected RelationEndPointDefinitionChecker RelationEndPointDefinitionChecker { get; } = new();

    protected TypeDefinition[] TypeDefinitions { get; private set; }

    protected IReadOnlyDictionary<Type, TypeDefinition> TypeDefinitionLookup { get; private set; }

    protected ReferenceMappingTestBase ()
    {
    }

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      var nestedTypes = GetType().GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
      var typeDiscoveryService = new FixedTypeDiscoveryService(nestedTypes);
      MappingReflector mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(typeDiscoveryService);

      var actualTypeDefinitions = mappingReflector.GetTypeDefinitions().ToDictionary(cd => cd.Type);
      mappingReflector.GetRelationDefinitions(actualTypeDefinitions);
      Assert.That(actualTypeDefinitions, Is.Not.Null);

      var inheritanceRoots = TypeDefinitionHierarchy.GetHierarchyRoots(actualTypeDefinitions.Values);

      // Pretend that all classes have the storage provider definition used by FakeMappingConfiguration
      var defaultStorageProviderDefinition = FakeMappingConfiguration.Current.DefaultStorageProviderDefinition;
      var defaultStorageProviderDefinitionFinderStub = new Mock<IStorageProviderDefinitionFinder>();
      defaultStorageProviderDefinitionFinderStub
          .Setup(stub => stub.GetStorageProviderDefinition(It.IsAny<TypeDefinition>(), It.IsAny<string>()))
          .Returns(defaultStorageProviderDefinition);

      var nonPersistentStorageProviderDefinition = FakeMappingConfiguration.Current.NonPersistentStorageProviderDefinition;
      var nonPersistentStorageProviderDefinitionFinderStub = new Mock<IStorageProviderDefinitionFinder>();
      nonPersistentStorageProviderDefinitionFinderStub
          .Setup(stub => stub.GetStorageProviderDefinition(It.IsAny<TypeDefinition>(), It.IsAny<string>()))
          .Returns(nonPersistentStorageProviderDefinition);

      foreach (var typeDefinition in inheritanceRoots)
      {
        var persistenceModelLoader = defaultStorageProviderDefinition.Factory.CreatePersistenceModelLoader(
            defaultStorageProviderDefinition,
            defaultStorageProviderDefinitionFinderStub.Object);
        persistenceModelLoader.ApplyPersistenceModelToHierarchy(typeDefinition);
      }

      TypeDefinitions = actualTypeDefinitions.Values.ToArray();
      TypeDefinitionLookup = actualTypeDefinitions.Values.ToDictionary(e => e.Type, e => e);
    }

    protected abstract void CreateReferenceTypeDefinitions (ReferenceTypeDefinitionCollectionBuilder builder);

    public void RunVerificationAgainstReferenceTypeDefinitions ()
    {
      var builder = new ReferenceTypeDefinitionCollectionBuilder();
      CreateReferenceTypeDefinitions(builder);

      var referenceTypeDefinitions = builder.BuildTypeDefinitions();
      var referenceTypeDefinitionLookup = referenceTypeDefinitions.ToDictionary(e => e.Type, e => e);

      EnsureAllActualExistInReference(referenceTypeDefinitionLookup);
      EnsureAllReferenceExistInActual(referenceTypeDefinitions);

      CheckActualAgainstReference(referenceTypeDefinitionLookup);
    }

    private void CheckActualAgainstReference (IReadOnlyDictionary<Type, TypeDefinition> referenceTypeDefinitions)
    {
      foreach (var actualTypeDefinition in TypeDefinitions)
      {
        var referenceTypeDefinition = referenceTypeDefinitions[actualTypeDefinition.Type];

        TypeDefinitionChecker.Check(referenceTypeDefinition, actualTypeDefinition);
        if (referenceTypeDefinition is ClassDefinition) // TODO R2I Persistence: Enable persistence model checks for interfaces
          TypeDefinitionChecker.CheckPersistenceModel(referenceTypeDefinition, actualTypeDefinition);

        RelationEndPointDefinitionChecker.Check(referenceTypeDefinition.MyRelationEndPointDefinitions, actualTypeDefinition.MyRelationEndPointDefinitions, true);
      }
    }

    private void EnsureAllActualExistInReference (IReadOnlyDictionary<Type, TypeDefinition> referenceTypeDefinitions)
    {
      foreach (var actualTypeDefinition in TypeDefinitions)
      {
        if (!referenceTypeDefinitions.ContainsKey(actualTypeDefinition.Type))
          throw new InvalidOperationException($"Could not find a reference type definition for type '{actualTypeDefinition.Type.Name}'");
      }
    }

    private void EnsureAllReferenceExistInActual (TypeDefinition[] referenceTypeDefinitions)
    {
      foreach (var referenceTypeDefinition in referenceTypeDefinitions)
      {
        if (!TypeDefinitionLookup.ContainsKey(referenceTypeDefinition.Type))
          throw new InvalidOperationException($"Could not find an actual type definition for the reference type '{referenceTypeDefinition.Type.Name}'");
      }
    }
  }
}
