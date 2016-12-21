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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  [TestFixture]
  public class ReferenceMappingComparisonTest
  {
    [Test]
    public void GetClassDefinitions ()
    {
      MappingReflector mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(TestMappingConfiguration.GetTypeDiscoveryService());

      var actualClassDefinitions = mappingReflector.GetClassDefinitions().ToDictionary (cd => cd.ClassType);
      mappingReflector.GetRelationDefinitions (actualClassDefinitions);
      Assert.That (actualClassDefinitions, Is.Not.Null);

      var inheritanceRootClasses = actualClassDefinitions.Values.Select (cd => cd.GetInheritanceRootClass()).Distinct();

      // Pretend that all classes have the storage provider definition used by FakeMappingConfiguration
      var storageProviderDefinition = FakeMappingConfiguration.Current.StorageProviderDefinition;
      var storageProviderDefinitionFinderStub = MockRepository.GenerateStub<IStorageProviderDefinitionFinder> ();
      storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<ClassDefinition>.Is.Anything, Arg<string>.Is.Anything))
          .Return (storageProviderDefinition);

      foreach (ClassDefinition classDefinition in inheritanceRootClasses)
      {
        var persistenceModelLoader = storageProviderDefinition.Factory.CreatePersistenceModelLoader (storageProviderDefinition, storageProviderDefinitionFinderStub);
        persistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);
      }

      var classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check (FakeMappingConfiguration.Current.TypeDefinitions.Values, actualClassDefinitions, false, true);
      classDefinitionChecker.CheckPersistenceModel (FakeMappingConfiguration.Current.TypeDefinitions.Values, actualClassDefinitions);
      Assert.That (actualClassDefinitions.ContainsKey (typeof (TestDomainBase)), Is.False);
    }
  }
}