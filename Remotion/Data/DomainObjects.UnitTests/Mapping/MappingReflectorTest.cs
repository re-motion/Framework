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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class MappingReflectorTest : MappingReflectionTestBase
  {
    private IMappingLoader _mappingReflector;

    [SetUp]
    public new void SetUp ()
    {
      base.SetUp();
      _mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(TestMappingConfiguration.GetTypeDiscoveryService());
    }

    [Test]
    public void Initialization_MappingObjectFactory_InstanceCreator ()
    {
      var reflector = new MappingReflector(
          SafeServiceLocator.Current.GetInstance<ITypeDiscoveryService>(),
          SafeServiceLocator.Current.GetInstance<IClassIDProvider>(),
          SafeServiceLocator.Current.GetInstance<IMemberInformationNameResolver>(),
          SafeServiceLocator.Current.GetInstance<IPropertyMetadataProvider>(),
          SafeServiceLocator.Current.GetInstance<IDomainModelConstraintProvider>(),
          SafeServiceLocator.Current.GetInstance<IPropertyDefaultValueProvider>(),
          SafeServiceLocator.Current.GetInstance<ISortExpressionDefinitionProvider>(),
          SafeServiceLocator.Current.GetInstance<IDomainObjectCreator>());

      var defaultCreator = reflector.MappingObjectFactory.CreateClassDefinition(typeof(Order), null).InstanceCreator;
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
      var actualClassDefinitions = _mappingReflector.GetClassDefinitions().ToDictionary(cd => cd.ClassType);
      var actualRelationDefinitions = _mappingReflector.GetRelationDefinitions(actualClassDefinitions).ToDictionary(rd => rd.ID);

      var relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check(FakeMappingConfiguration.Current.RelationDefinitions.Values, actualRelationDefinitions, true);
    }

    [Test]
    public void Get_WithDuplicateAssembly ()
    {
      var assembly = GetType().Assembly;
      var expectedMappingReflector = MappingReflectorObjectMother.CreateMappingReflector(BaseConfiguration.GetTypeDiscoveryService(assembly));
      var expectedClassDefinitions = expectedMappingReflector.GetClassDefinitions().ToDictionary(cd=>cd.ClassType);
      var expectedRelationDefinitions = expectedMappingReflector.GetRelationDefinitions(expectedClassDefinitions);

      var mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(BaseConfiguration.GetTypeDiscoveryService(assembly, assembly));
      var actualClassDefinitions = mappingReflector.GetClassDefinitions().ToDictionary(cd => cd.ClassType);

      var classDefinitionChecker = new ClassDefinitionChecker();
      classDefinitionChecker.Check(expectedClassDefinitions.Values, actualClassDefinitions, false, false);

      var actualRelationDefinitions = mappingReflector.GetRelationDefinitions(actualClassDefinitions).ToDictionary(rd => rd.ID);
      var relationDefinitionChecker = new RelationDefinitionChecker();
      relationDefinitionChecker.Check(expectedRelationDefinitions, actualRelationDefinitions, false);
    }

    [Test]
    public void GetClassDefinitions ()
    {
      var assembly = GetType().Assembly;
      var mappingReflector = MappingReflectorObjectMother.CreateMappingReflector(BaseConfiguration.GetTypeDiscoveryService(assembly, assembly));
      var classDefinitions = mappingReflector.GetClassDefinitions();

      Assert.That(classDefinitions, Is.Not.Empty);
    }

    [Test]
    public void CreateClassDefinitionValidator ()
    {
      var validator = (ClassDefinitionValidator)_mappingReflector.CreateClassDefinitionValidator();

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
      Assert.That(validator.ValidationRules[9], Is.TypeOf(typeof(CheckForTypeNotFoundClassDefinitionValidationRule)));
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
