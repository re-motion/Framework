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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class OneSideRelationProperty : MappingReflectionTestBase
  {
    private ClassDefinition _classDefinition;
    private Type _classType;

    public override void SetUp ()
    {
      base.SetUp();

      _classType = typeof(ClassWithVirtualRelationEndPoints);
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: _classType);
    }

    [Test]
    public void GetMetadata_ForOptional_DomainObjectCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("NoAttributeForDomainObjectCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      DomainModelConstraintProviderStub.Setup(stub => stub.IsNullable(propertyInfo)).Returns(true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.That(actual, Is.InstanceOf(typeof(DomainObjectCollectionRelationEndPointDefinition)));
      Assert.That(
          actual.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NoAttributeForDomainObjectCollection"));
      Assert.That(actual.IsMandatory, Is.False);
      DomainModelConstraintProviderStub.Verify();
    }

    [Test]
    public void GetMetadata_ForOptional_VirtualCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("NoAttributeForVirtualCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      DomainModelConstraintProviderStub.Setup(stub => stub.IsNullable(propertyInfo)).Returns(true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.That(actual, Is.InstanceOf(typeof(VirtualCollectionRelationEndPointDefinition)));
      Assert.That(
          actual.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NoAttributeForVirtualCollection"));
      Assert.That(actual.IsMandatory, Is.False);
      DomainModelConstraintProviderStub.Verify();
    }

    [Test]
    public void GetMetadata_ForMandatory_DomainObjectCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("NotNullableForDomainObjectCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      DomainModelConstraintProviderStub.Setup(stub => stub.IsNullable(propertyInfo)).Returns(false);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.That(actual, Is.InstanceOf(typeof(DomainObjectCollectionRelationEndPointDefinition)));
      Assert.That(
          actual.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NotNullableForDomainObjectCollection"));
      Assert.That(actual.IsMandatory, Is.True);
      DomainModelConstraintProviderStub.Verify();
    }

    [Test]
    public void GetMetadata_ForMandatory_VirtualCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("NotNullableForVirtualCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      DomainModelConstraintProviderStub.Setup(stub => stub.IsNullable(propertyInfo)).Returns(false);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.That(actual, Is.InstanceOf(typeof(VirtualCollectionRelationEndPointDefinition)));
      Assert.That(
          actual.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NotNullableForVirtualCollection"));
      Assert.That(actual.IsMandatory, Is.True);
      DomainModelConstraintProviderStub.Verify();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("BidirectionalOneToOne"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      DomainModelConstraintProviderStub.Setup(stub => stub.IsNullable(propertyInfo)).Returns(true);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.That(actual, Is.InstanceOf(typeof(VirtualObjectRelationEndPointDefinition)));
      VirtualObjectRelationEndPointDefinition relationEndPointDefinition = (VirtualObjectRelationEndPointDefinition)actual;
      Assert.That(relationEndPointDefinition.ClassDefinition, Is.SameAs(_classDefinition));
      Assert.That(
          relationEndPointDefinition.PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne"));
      Assert.That(relationEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs(typeof(ClassWithRealRelationEndPoints)));
      Assert.That(relationEndPointDefinition.Cardinality, Is.EqualTo(CardinalityType.One));
      Assert.That(relationEndPointDefinition.HasRelationDefinitionBeenSet, Is.False);
      DomainModelConstraintProviderStub.Verify();
    }

    [Test]
    public void GetMetadata_BidirectionalOneToManyForDomainObjectCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("BidirectionalOneToManyForDomainObjectCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      DomainModelConstraintProviderStub.Setup(stub => stub.IsNullable(propertyInfo)).Returns(true);
      SortExpressionDefinitionProviderStub
          .Setup(stub => stub.GetSortExpression(It.IsAny<IPropertyInformation>(), It.IsAny<ClassDefinition>(), It.IsAny<string>()))
          .Throws(new InvalidOperationException("GetSortExpression() should not be called during GetMetadata()"));

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.That(actual, Is.InstanceOf(typeof(DomainObjectCollectionRelationEndPointDefinition)));
      DomainObjectCollectionRelationEndPointDefinition relationEndPointDefinition = (DomainObjectCollectionRelationEndPointDefinition)actual;
      Assert.That(relationEndPointDefinition.ClassDefinition, Is.SameAs(_classDefinition));
      Assert.That(
          relationEndPointDefinition.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToManyForDomainObjectCollection"));
      Assert.That(relationEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs(typeof(ObjectList<ClassWithRealRelationEndPoints>)));
      Assert.That(relationEndPointDefinition.Cardinality, Is.EqualTo(CardinalityType.Many));
      Assert.That(relationEndPointDefinition.HasRelationDefinitionBeenSet, Is.False);

      var oppositeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ClassWithRealRelationEndPoints));
      var oppositeEndPointDefinition = new Mock<IRelationEndPointDefinition>();
      oppositeEndPointDefinition.Setup(stub => stub.ClassDefinition).Returns(oppositeClassDefinition);
      var relationDefinition = new RelationDefinition("relation", relationEndPointDefinition, oppositeEndPointDefinition.Object);
      relationEndPointDefinition.SetRelationDefinition(relationDefinition);
      var oppositePropertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(
          oppositeClassDefinition,
          oppositeClassDefinition.ClassType,
          "NoAttributeForDomainObjectCollection");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(oppositePropertyDefinition) });
      SortExpressionDefinitionProviderStub.Reset();
      SortExpressionDefinitionProviderStub
          .Setup(stub => stub.GetSortExpression(relationEndPointDefinition.PropertyInfo, oppositeClassDefinition, "NoAttributeForDomainObjectCollection"))
          .Returns(sortExpressionDefinition);

      Assert.That(relationEndPointDefinition.GetSortExpression(), Is.SameAs(sortExpressionDefinition));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToManyForVirtualCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("BidirectionalOneToManyForVirtualCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      DomainModelConstraintProviderStub.Setup(stub => stub.IsNullable(propertyInfo)).Returns(true);
      SortExpressionDefinitionProviderStub
          .Setup(stub => stub.GetSortExpression(It.IsAny<IPropertyInformation>(), It.IsAny<ClassDefinition>(), It.IsAny<string>()))
          .Throws(new InvalidOperationException("GetSortExpression() should not be called during GetMetadata()"));

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata();

      Assert.That(actual, Is.InstanceOf(typeof(VirtualCollectionRelationEndPointDefinition)));
      VirtualCollectionRelationEndPointDefinition relationEndPointDefinition = (VirtualCollectionRelationEndPointDefinition)actual;
      Assert.That(relationEndPointDefinition.ClassDefinition, Is.SameAs(_classDefinition));
      Assert.That(
          relationEndPointDefinition.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToManyForVirtualCollection"));
      Assert.That(relationEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs(typeof(IObjectList<ClassWithRealRelationEndPoints>)));
      Assert.That(relationEndPointDefinition.Cardinality, Is.EqualTo(CardinalityType.Many));
      Assert.That(relationEndPointDefinition.HasRelationDefinitionBeenSet, Is.False);

      var oppositeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ClassWithRealRelationEndPoints));
      var oppositeEndPointDefinition = new Mock<IRelationEndPointDefinition>();
      oppositeEndPointDefinition.Setup(stub => stub.ClassDefinition).Returns(oppositeClassDefinition);
      var relationDefinition = new RelationDefinition("relation", relationEndPointDefinition, oppositeEndPointDefinition.Object);
      relationEndPointDefinition.SetRelationDefinition(relationDefinition);
      var oppositePropertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(
          oppositeClassDefinition,
          oppositeClassDefinition.ClassType,
          "NoAttributeForVirtualCollection");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(oppositePropertyDefinition) });
      SortExpressionDefinitionProviderStub.Reset();
      SortExpressionDefinitionProviderStub
          .Setup(stub => stub.GetSortExpression(relationEndPointDefinition.PropertyInfo, oppositeClassDefinition, "NoAttributeForVirtualCollection"))
          .Returns(sortExpressionDefinition);

      Assert.That(relationEndPointDefinition.GetSortExpression(), Is.SameAs(sortExpressionDefinition));
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("BidirectionalOneToOne"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.True);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToManyForDomainObjectCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("BidirectionalOneToManyForDomainObjectCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.True);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToManyForVirtualCollection ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(_classType.GetProperty("BidirectionalOneToManyForVirtualCollection"));
      var relationEndPointReflector = CreateRelationEndPointReflector(propertyInfo);

      Assert.That(relationEndPointReflector.IsVirtualEndRelationEndpoint(), Is.True);
    }

    private RdbmsRelationEndPointReflector CreateRelationEndPointReflector (PropertyInfoAdapter propertyInfo)
    {
      return new RdbmsRelationEndPointReflector(
          _classDefinition,
          propertyInfo,
          Configuration.NameResolver,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object);
    }
  }
}
