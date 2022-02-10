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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  [TestFixture]
  public class ReferenceMappingComparisonTest
  {
    [Test]
    public void GetTypeDefinitions ()
    {
      MappingReflector mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(TestMappingConfiguration.GetTypeDiscoveryService());

      var actualTypeDefinitions = mappingReflector.GetTypeDefinitions().ToDictionary(td => td.Type);
      mappingReflector.GetRelationDefinitions(actualTypeDefinitions);
      Assert.That(actualTypeDefinitions, Is.Not.Null);

      var inheritanceRoots = TypeDefinitionHierarchy.GetHierarchyRoots(actualTypeDefinitions.Values);

      // Pretend that all classes have the storage provider definition used by FakeMappingConfiguration
      var defaultStorageProviderDefinition = FakeMappingConfiguration.Current.DefaultStorageProviderDefinition;
      var defaultStorageSettingsStub = new Mock<IStorageSettings>();
      defaultStorageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(It.IsAny<TypeDefinition>()))
          .Returns(defaultStorageProviderDefinition);

      var nonPersistentStorageProviderDefinition = FakeMappingConfiguration.Current.NonPersistentStorageProviderDefinition;
      var storageSettingsStub = new Mock<IStorageSettings>();
      storageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(It.IsAny<TypeDefinition>()))
          .Returns(nonPersistentStorageProviderDefinition);

      foreach (var inheritanceRoot in inheritanceRoots)
      {
        if (typeof(OrderViewModel).IsAssignableFrom(inheritanceRoot.Type))
        {
          var persistenceModelLoader = nonPersistentStorageProviderDefinition.Factory.CreatePersistenceModelLoader(
              nonPersistentStorageProviderDefinition);
          persistenceModelLoader.ApplyPersistenceModelToHierarchy(inheritanceRoot);
        }
        else
        {
          var persistenceModelLoader = defaultStorageProviderDefinition.Factory.CreatePersistenceModelLoader(
              defaultStorageProviderDefinition);
          persistenceModelLoader.ApplyPersistenceModelToHierarchy(inheritanceRoot);
        }
      }

      var typeDefinitionChecker = new TypeDefinitionChecker();
      typeDefinitionChecker.Check(FakeMappingConfiguration.Current.TypeDefinitions.Values.Cast<ClassDefinition>(), actualTypeDefinitions, false, true);
      typeDefinitionChecker.CheckPersistenceModel(FakeMappingConfiguration.Current.TypeDefinitions.Values.Cast<ClassDefinition>(), actualTypeDefinitions);
      Assert.That(actualTypeDefinitions.ContainsKey(typeof(TestDomainBase)), Is.False);
    }
  }
}
