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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class MappingReflectorTest : MappingReflectionTestBase
  {
    private interface ITest
    {
    }

    private interface ITestDomainObject : IDomainObject
    {
    }

    private IMappingLoader _mappingReflector;

    [SetUp]
    public new void SetUp ()
    {
      base.SetUp();
      _mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(TestMappingConfiguration.GetTypeDiscoveryService());
    }

    [Test]
    public void CreateDomainObjectCreator ()
    {
      var registryStub = new Mock<IPipelineRegistry>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IPipelineRegistry>(() => registryStub.Object);
      using (new ServiceLocatorScope(serviceLocator))
      {
        var creator = MappingReflector.CreateDomainObjectCreator();
        Assert.That(creator.PipelineRegistry, Is.SameAs(registryStub.Object));
      }
    }

    [Test]
    public void Initialization_DefaultTypeDiscoveryService ()
    {
      var reflector = new MappingReflector();

      Assert.That(reflector.TypeDiscoveryService, Is.SameAs(ContextAwareTypeUtility.GetTypeDiscoveryService()));
    }

    [Test]
    public void Initialization_MappingObjectFactory_InstanceCreator ()
    {
      var defaultCreator = new MappingReflector().MappingObjectFactory.CreateClassDefinition(typeof(Order), null, Enumerable.Empty<InterfaceDefinition>()).InstanceCreator;
      Assert.That(defaultCreator, Is.TypeOf<DomainObjectCreator>());
   }

    [Test]
    public void GetResolveTypes ()
    {
      Assert.That(_mappingReflector.ResolveTypes, Is.True);
    }

    [Test]
    public void GetRelationDefinitions ()
    {
      var actualClassDefinitions = _mappingReflector.GetTypeDefinitions().ToDictionary(td => td.Type);
      var actualRelationDefinitions = _mappingReflector.GetRelationDefinitions(actualClassDefinitions).ToDictionary(rd => rd.ID);

      var relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check(FakeMappingConfiguration.Current.RelationDefinitions.Values, actualRelationDefinitions, true);
    }

    [Test]
    public void Get_WithDuplicateAssembly ()
    {
      var assembly = GetType().Assembly;
      var expectedMappingReflector = MappingReflectorObjectMother.CreateMappingReflector(BaseConfiguration.GetTypeDiscoveryService(assembly));
      var expectedClassDefinitions = expectedMappingReflector.GetTypeDefinitions().ToDictionary(td => td.Type);
      var expectedRelationDefinitions = expectedMappingReflector.GetRelationDefinitions(expectedClassDefinitions);

      var mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(BaseConfiguration.GetTypeDiscoveryService(assembly, assembly));
      var actualClassDefinitions = mappingReflector.GetTypeDefinitions().ToDictionary(td => td.Type);

      var typeDefinitionChecker = new TypeDefinitionChecker();
      typeDefinitionChecker.Check(expectedClassDefinitions.Values, actualClassDefinitions, false, false);

      var actualRelationDefinitions = mappingReflector.GetRelationDefinitions(actualClassDefinitions).ToDictionary(rd => rd.ID);
      var relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check(expectedRelationDefinitions, actualRelationDefinitions, false);
    }

    [Test]
    public void GetTypeDefinitions ()
    {
      var assembly = GetType().Assembly;
      var mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(BaseConfiguration.GetTypeDiscoveryService(assembly, assembly));
      var typeDefinitions = mappingReflector.GetTypeDefinitions();

      Assert.That(typeDefinitions, Is.Not.Empty);
    }

    [Test]
    public void GetTypeDefinitions_DiscoversInterfacesTypes ()
    {
      var fixedTypeDiscoveryService = new FixedTypeDiscoveryService(typeof(ITest), typeof(ITestDomainObject));
      var mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(fixedTypeDiscoveryService);
      var typeDefinitions = mappingReflector.GetTypeDefinitions();

      Assert.That(typeDefinitions.Length, Is.EqualTo(1));
      Assert.That(typeDefinitions[0].Type, Is.EqualTo(typeof(ITestDomainObject)));
    }

    [Test]
    public void CreateClassDefinitionValidator ()
    {
      var validator = (TypeDefinitionValidator)_mappingReflector.CreateTypeDefinitionValidator();

      Assert.That(validator.ValidationRules.Count, Is.EqualTo(7));
      Assert.That(validator.ValidationRules[0], Is.TypeOf(typeof(DomainObjectTypeDoesNotHaveLegacyInfrastructureConstructorValidationRule)));
      Assert.That(validator.ValidationRules[1], Is.TypeOf(typeof(DomainObjectTypeIsNotGenericValidationRule)));
      Assert.That(validator.ValidationRules[2], Is.TypeOf(typeof(InheritanceHierarchyFollowsClassHierarchyValidationRule)));
      Assert.That(validator.ValidationRules[3], Is.TypeOf(typeof(StorageGroupAttributeIsOnlyDefinedOncePerInheritanceHierarchyValidationRule)));
      Assert.That(validator.ValidationRules[4], Is.TypeOf(typeof(ClassDefinitionTypeIsSubclassOfDomainObjectValidationRule)));
      Assert.That(validator.ValidationRules[5], Is.TypeOf(typeof(StorageGroupTypesAreSameWithinInheritanceTreeRule)));
      Assert.That(validator.ValidationRules[6], Is.TypeOf(typeof(CheckForClassIDIsValidValidationRule)));
    }

    [Test]
    public void CreatePropertyDefinitionValidator ()
    {
      var validator = (PropertyDefinitionValidator)_mappingReflector.CreatePropertyDefinitionValidator();

      Assert.That(validator.ValidationRules.Count, Is.EqualTo(6));
      Assert.That(validator.ValidationRules[0], Is.TypeOf(typeof(MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule)));
      Assert.That(validator.ValidationRules[1], Is.TypeOf(typeof(MappingAttributesAreSupportedForPropertyTypeValidationRule)));
      Assert.That(validator.ValidationRules[2], Is.TypeOf(typeof(StorageClassIsSupportedValidationRule)));
      Assert.That(validator.ValidationRules[3], Is.TypeOf(typeof(PropertyTypeIsSupportedValidationRule)));
      Assert.That(validator.ValidationRules[4], Is.TypeOf(typeof(MandatoryNetEnumTypeHasValuesDefinedValidationRule)));
      Assert.That(validator.ValidationRules[5], Is.TypeOf(typeof(MandatoryExtensibleEnumTypeHasValuesDefinedValidationRule)));
    }

    [Test]
    public void CreateRelationDefinitionValidator ()
    {
      var validator = (RelationDefinitionValidator)_mappingReflector.CreateRelationDefinitionValidator();

      Assert.That(validator.ValidationRules.Count, Is.EqualTo(10));
      Assert.That(validator.ValidationRules[0], Is.TypeOf(typeof(RdbmsRelationEndPointCombinationIsSupportedValidationRule)));
      Assert.That(validator.ValidationRules[1], Is.TypeOf(typeof(SortExpressionIsSupportedForCardinalityOfRelationPropertyValidationRule)));
      Assert.That(validator.ValidationRules[2], Is.TypeOf(typeof(VirtualRelationEndPointCardinalityMatchesPropertyTypeValidationRule)));
      Assert.That(validator.ValidationRules[3], Is.TypeOf(typeof(VirtualRelationEndPointPropertyTypeIsSupportedValidationRule)));
      Assert.That(validator.ValidationRules[4], Is.TypeOf(typeof(ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule)));
      Assert.That(validator.ValidationRules[5], Is.TypeOf(typeof(RelationEndPointPropertyTypeIsSupportedValidationRule)));
      Assert.That(validator.ValidationRules[6], Is.TypeOf(typeof(RelationEndPointNamesAreConsistentValidationRule)));
      Assert.That(validator.ValidationRules[7], Is.TypeOf(typeof(RelationEndPointTypesAreConsistentValidationRule)));
      Assert.That(validator.ValidationRules[8], Is.TypeOf(typeof(CheckForInvalidRelationEndPointsValidationRule)));
      Assert.That(validator.ValidationRules[9], Is.TypeOf(typeof(CheckForTypeNotFoundTypeDefinitionValidationRule)));
    }

    [Test]
    public void CreateSortExpressionValidator ()
    {
      var validator = (SortExpressionValidator)_mappingReflector.CreateSortExpressionValidator();

      Assert.That(validator.ValidationRules.Count, Is.EqualTo(1));
      Assert.That(validator.ValidationRules[0], Is.TypeOf(typeof(SortExpressionIsValidValidationRule)));
    }
  }
}
