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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Errors;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common : MappingReflectionTestBase
  {
    [Test]
    public void CreateRelationEndPointReflector ()
    {
      var type = typeof (ClassWithVirtualRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("NoAttributeForVirtualCollection"));
      Assert.IsInstanceOf(
          typeof (RdbmsRelationEndPointReflector),
          RelationEndPointReflector.CreateRelationEndPointReflector(
              ClassDefinitionObjectMother.CreateClassDefinition(classType: type),
              propertyInfo,
              Configuration.NameResolver,
              PropertyMetadataProvider,
              DomainModelConstraintProviderStub,
              SortExpressionDefinitionProviderStub));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithoutAttribute ()
    {
      var type = typeof (ClassWithRealRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("NoAttributeForDomainObjectCollection"));
      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          ClassDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub,
          SortExpressionDefinitionProviderStub);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithDomainObjectCollectionPropertyAndWithoutAttribute ()
    {
      var type = typeof (ClassWithInvalidUnidirectionalRelation);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("LeftSideForDomainObjectCollection"));
      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          ClassDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub,
          SortExpressionDefinitionProviderStub);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithVirtualCollectionPropertyAndWithoutAttribute ()
    {
      var type = typeof (ClassWithInvalidUnidirectionalRelation);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("LeftSideForVirtualCollection"));
      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          ClassDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub,
          SortExpressionDefinitionProviderStub);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualRelationEndPoint_UnidirectionalRelation ()
    {
      var type = typeof (ClassWithRealRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("Unidirectional"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector(
          ClassDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub,
          SortExpressionDefinitionProviderStub);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void GetMetadata_NonVirtualEndPoint_PropertyTypeIsNotObjectID ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof (ClassWithRealRelationEndPoints));
      var propertyDefinition = 
          PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition, "Unidirectional", typeof (string));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      var mappingNameResolverMock = MockRepository.GenerateStub<IMemberInformationNameResolver>();
      mappingNameResolverMock.Stub(mock => mock.GetPropertyName(propertyDefinition.PropertyInfo)).Return(propertyDefinition.PropertyName);

      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          classDefinition,
          propertyDefinition.PropertyInfo,
          mappingNameResolverMock,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub,
          SortExpressionDefinitionProviderStub);

      var result = relationEndPointReflector.GetMetadata();

      Assert.That(result, Is.TypeOf(typeof (TypeNotObjectIDRelationEndPointDefinition)));
    }

    [Test]
    public void GetMetadata_VirtualEndPoint_PropertyTypeIsNotCompatible ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof (ClassWithVirtualRelationEndPoints));
      var propertyDefinition = 
          PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition, "BidirectionalOneToManyForDomainObjectCollection", typeof (string));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      var mappingNameResolverMock = MockRepository.GenerateStub<IMemberInformationNameResolver>();
      mappingNameResolverMock.Stub(mock => mock.GetPropertyName(propertyDefinition.PropertyInfo)).Return(propertyDefinition.PropertyName);

      var relationEndPointReflector = MockRepository.GeneratePartialMock<RelationEndPointReflector<BidirectionalRelationAttribute>>(
          classDefinition,
          propertyDefinition.PropertyInfo,
          mappingNameResolverMock,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub,
          SortExpressionDefinitionProviderStub);

      relationEndPointReflector.Stub(_ => _.IsVirtualEndRelationEndpoint()).Return(true);

      var result = relationEndPointReflector.GetMetadata();

      Assert.That(result, Is.TypeOf(typeof (TypeNotCompatibleWithVirtualRelationEndPointDefinition)));
    }
  }
}