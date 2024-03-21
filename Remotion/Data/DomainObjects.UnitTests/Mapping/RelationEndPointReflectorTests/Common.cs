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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Errors;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common : MappingReflectionTestBase
  {
    [Test]
    public void CreateRelationEndPointReflector ()
    {
      var type = typeof(ClassWithVirtualRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("NoAttributeForVirtualCollection"));
      Assert.IsInstanceOf(
          typeof(RdbmsRelationEndPointReflector),
          RelationEndPointReflector.CreateRelationEndPointReflector(
              TypeDefinitionObjectMother.CreateClassDefinition(classType: type),
              propertyInfo,
              Configuration.NameResolver,
              PropertyMetadataProvider,
              DomainModelConstraintProviderStub.Object,
              SortExpressionDefinitionProviderStub.Object));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithoutAttribute ()
    {
      var type = typeof(ClassWithRealRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("NoAttributeForDomainObjectCollection"));
      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          TypeDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithDomainObjectCollectionPropertyAndWithoutAttribute ()
    {
      var type = typeof(ClassWithInvalidUnidirectionalRelation);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("LeftSideForDomainObjectCollection"));
      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          TypeDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_WithVirtualCollectionPropertyAndWithoutAttribute ()
    {
      var type = typeof(ClassWithInvalidUnidirectionalRelation);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("LeftSideForVirtualCollection"));
      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          TypeDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void IsVirtualRelationEndPoint_UnidirectionalRelation ()
    {
      var type = typeof(ClassWithRealRelationEndPoints);
      var propertyInfo = PropertyInfoAdapter.Create(type.GetProperty("Unidirectional"));
      var relationEndPointReflector = new RdbmsRelationEndPointReflector(
          TypeDefinitionObjectMother.CreateClassDefinition(classType: type),
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.False);
    }

    [Test]
    public void GetMetadata_NonVirtualEndPoint_PropertyTypeIsNotObjectID ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(ClassWithRealRelationEndPoints));
      var propertyDefinition =
          PropertyDefinitionObjectMother.CreateForFakePropertyInfo(typeDefinition, "Unidirectional", typeof(string));
      typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      var mappingNameResolverMock = new Mock<IMemberInformationNameResolver>();
      mappingNameResolverMock.Setup(mock => mock.GetPropertyName(propertyDefinition.PropertyInfo)).Returns(propertyDefinition.PropertyName);

      var relationEndPointReflector = RelationEndPointReflector.CreateRelationEndPointReflector(
          typeDefinition,
          propertyDefinition.PropertyInfo,
          mappingNameResolverMock.Object,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object);

      var result = relationEndPointReflector.GetMetadata();

      Assert.That(result, Is.TypeOf(typeof(TypeNotObjectIDRelationEndPointDefinition)));
    }

    [Test]
    public void GetMetadata_VirtualEndPoint_PropertyTypeIsNotCompatible ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(ClassWithVirtualRelationEndPoints));
      var propertyDefinition =
          PropertyDefinitionObjectMother.CreateForFakePropertyInfo(typeDefinition, "BidirectionalOneToManyForDomainObjectCollection", typeof(string));
      typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      var mappingNameResolverMock = new Mock<IMemberInformationNameResolver>();
      mappingNameResolverMock.Setup(mock => mock.GetPropertyName(propertyDefinition.PropertyInfo)).Returns(propertyDefinition.PropertyName);

      var relationEndPointReflectorPartialMock = new Mock<RelationEndPointReflector<BidirectionalRelationAttribute>>(
          typeDefinition,
          propertyDefinition.PropertyInfo,
          mappingNameResolverMock.Object,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object)
          { CallBase = true };

      relationEndPointReflectorPartialMock.Setup(_ => _.IsVirtualEndRelationEndpoint()).Returns(true);

      var result = relationEndPointReflectorPartialMock.Object.GetMetadata();

      Assert.That(result, Is.TypeOf(typeof(TypeNotCompatibleWithVirtualRelationEndPointDefinition)));
    }
  }
}
