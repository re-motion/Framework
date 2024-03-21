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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.IntegrationTests
{
  [TestFixture]
  public class NotSupportedRelationsIntegrationTests : ValidationIntegrationTestBase
  {
    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    public void OneToOne_ContainsForeignKeyIsTrueOnBothSites ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.OneToOne_ContainsForeignKeyIsTrueOnBothSites"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The relation between property 'RelationProperty2', declared on type 'InvalidRelationClass1', and property 'RelationProperty1' declared on type "
                  + "'InvalidRelationClass2', contains two non-virtual end points. One of the two properties must set 'ContainsForeignKey' to 'false' on the "
                  + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1\r\n"
                  + "Property: RelationProperty2\r\n"
                  + "Relation ID: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1:Remotion.Data.DomainObjects.UnitTests.Mapping."
                  + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOne_ContainsForeignKeyIsTrueOnBothSites."
                  + "InvalidRelationClass1.RelationProperty2->Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration."
                  + "NotSupportedRelations.OneToOne_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2.RelationProperty1\r\n"
                  + "----------\r\n"
                  + "The relation between property 'RelationProperty1', declared on type 'InvalidRelationClass2', and property 'RelationProperty2' declared on type "
                  + "'InvalidRelationClass1', contains two non-virtual end points. One of the two properties must set 'ContainsForeignKey' to 'false' on the "
                  + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2\r\n"
                  + "Property: RelationProperty1\r\n"
                  + "Relation ID: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2:Remotion.Data.DomainObjects.UnitTests.Mapping."
                  + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOne_ContainsForeignKeyIsTrueOnBothSites."
                  + "InvalidRelationClass2.RelationProperty1->Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration."
                  + "NotSupportedRelations.OneToOne_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass1.RelationProperty2"));
    }

    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    public void OneToOne_ContainsForeignKeyIsFalseOnBothSites ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.OneToOne_ContainsForeignKeyIsFalseOnBothSites"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The relation between property 'RelationProperty2', declared on type 'InvalidRelationClass1', and property 'RelationProperty1' declared on type "
                  + "'InvalidRelationClass2', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
                  + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass1\r\n"
                  + "Property: RelationProperty2\r\n"
                  + "Relation ID: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass2:Remotion.Data.DomainObjects.UnitTests.Mapping."
                  + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOne_ContainsForeignKeyIsFalseOnBothSites."
                  + "InvalidRelationClass2.RelationProperty1->Remotion.Data.DomainObjects.UnitTests.Mapping."
                  + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOne_ContainsForeignKeyIsFalseOnBothSites."
                  + "InvalidRelationClass1.RelationProperty2\r\n"
                  + "----------\r\n"
                  + "The relation between property 'RelationProperty1', declared on type 'InvalidRelationClass2', and property 'RelationProperty2' declared on type "
                  + "'InvalidRelationClass1', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
                  + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass2\r\n"
                  + "Property: RelationProperty1\r\n"
                  + "Relation ID: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_ContainsForeignKeyIsFalseOnBothSites.InvalidRelationClass1:Remotion.Data.DomainObjects.UnitTests.Mapping."
                  + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOne_ContainsForeignKeyIsFalseOnBothSites."
                  + "InvalidRelationClass1.RelationProperty2->Remotion.Data.DomainObjects.UnitTests.Mapping."
                  + "TestDomain.Validation.Integration.NotSupportedRelations.OneToOne_ContainsForeignKeyIsFalseOnBothSites."
                  + "InvalidRelationClass2.RelationProperty1"));
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    public void OneToMany_ContainsForeignKeyIsTrueOnTheManySite ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.OneToMany_ContainsForeignKeyIsTrueOnManySite"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n" +
                  "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToMany_ContainsForeignKeyIsTrueOnManySite.InvalidRelationClass2\r\nProperty: RelationProperty2"));
    }

    //ForeignKeyIsSupportedForCardinalityOfRelationPropertyValidationRule
    [Test]
    public void OneToMany_ContainsForeignKeyIsTrueOnBothSites ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.OneToMany_ContainsForeignKeyIsTrueOnBothSites"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Only relation end points with a property type of 'DomainObject' can contain the foreign key.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToMany_ContainsForeignKeyIsTrueOnBothSites.InvalidRelationClass2\r\nProperty: RelationProperty2"));
    }

    //SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule
    [Test]
    public void OneToOne_WithSortExpression ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.OneToOne_WithSortExpression"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Property 'RelationProperty1' of type 'InvalidRelationClass1' must not specify a SortExpression, because cardinality is equal to 'one'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToOne_WithSortExpression.InvalidRelationClass1\r\nProperty: RelationProperty1"));
    }

    //SortExpressionIsValidValidationRule
    [Test]
    public void OneToMany_WithInvalidSortExpression ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.OneToMany_WithInvalidSortExpression"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "SortExpression 'InvalidProperty' cannot be parsed: 'InvalidProperty' is not a valid mapped property name. Expected the .NET property name of a property declared by the "
                  + "'InvalidRelationClass1' class or its base classes. Alternatively, to resolve ambiguities or to use a property declared by a mixin or a "
                  + "derived class of 'InvalidRelationClass1', the full unique re-store property identifier can be specified.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "OneToMany_WithInvalidSortExpression.InvalidRelationClass2\r\nProperty: RelationProperty2"));
    }

    //RelationEndPointNamesAreConsistentValidationRule
    [Test]
    public void Bidirectional_WithBidirectionalRelationAttributeOnOneSite ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.Bidirectional_WithBidirectionalRelationAttributeOnOneSite"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Opposite relation property 'RelationProperty1' declared on type 'InvalidRelationClass1' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "Bidirectional_WithBidirectionalRelationAttributeOnOneSite.InvalidRelationClass2\r\n"
                  + "Property: RelationProperty2"));
    }

    //RelationEndPointNamesAreConsistentValidationRule / CheckForInvalidRelationEndPointsValidationRule
    [Test]
    public void Bidirectional_RelationEndPointDefinitionsDoNotMatch ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.Bidirectional_RelationEndPointDefinitionsDoNotMatch"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The property type of an uni-directional relation property must be assignable to 'DomainObject'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "Bidirectional_RelationEndPointDefinitionsDoNotMatch.InvalidRelationClass1\r\n"
                  + "Property: RelationProperty2\r\n"
                  + "----------\r\n"
                  + "Opposite relation property 'RelationProperty1' declared on type 'InvalidRelationClass2' defines a 'DBBidirectionalRelationAttribute' whose "
                  + "opposite property does not match.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "Bidirectional_RelationEndPointDefinitionsDoNotMatch.InvalidRelationClass1\r\n"
                  + "Property: RelationProperty1\r\n"
                  + "----------\r\n"
                  + "Opposite relation property 'RelationProperty2' declared on type 'InvalidRelationClass1' does not "
                  + "define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "Bidirectional_RelationEndPointDefinitionsDoNotMatch.InvalidRelationClass2\r\n"
                  + "Property: RelationProperty1\r\n"
                  + "----------\r\n"
                  + "Property 'RelationProperty2' on type 'InvalidRelationClass1' could not be found.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "Bidirectional_RelationEndPointDefinitionsDoNotMatch.InvalidRelationClass1"));
    }

    //RelationEndPointTypesAreConsistentValidationRule
    [Test]
    public void Bidirectional_RelatedObjectTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.Bidirectional_RelatedObjectTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The type 'BaseRelationClass2' does not match the type of the opposite relation propery 'RelationProperty1' declared on type 'InvalidRelationClass1'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "Bidirectional_RelatedObjectTypeDoesNotMatchOppositeProperty_BelowInheritanceRoot.BaseRelationClass2\r\n"
                  + "Property: RelationProperty3"));
    }

    [Test]
    public void Bidirectional_RelatedObjectTypeIsBaseClassOfOppositePropertyType_BelowInheritanceRoot ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.Bidirectional_RelatedObjectTypeIsBaseClassOfOppositePropertyType_BelowInheritanceRoot"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The type 'Base' does not match the type of the opposite relation propery 'RelationPropertyPointingToDerived' declared on type 'InvalidRelationClass'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "Bidirectional_RelatedObjectTypeIsBaseClassOfOppositePropertyType_BelowInheritanceRoot.Base\r\n"
                  + "Property: RelationProperty"));
    }

    //CheckForTypeNotFoundClassDefinitionValidationRule
    [Test]
    public void RelationPropertyTypeNotInMapping ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.RelationPropertyTypeNotInMapping"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Opposite relation property 'RelationProperty' declared on type 'ClassNotInMapping' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "RelationPropertyTypeNotInMapping.InvalidRelationClass1\r\n"
                  + "Property: RelationProperty\r\n"
                  + "----------\r\n"
                  + "Property 'RelationProperty' on type 'ClassNotInMapping' could not be found.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "RelationPropertyTypeNotInMapping.ClassNotInMapping\r\n"
                  + "----------\r\n"
                  + "The relation property 'RelationProperty' has return type 'ClassNotInMapping', which is not a part of the mapping. Relation properties must "
                  + "not point to types above the inheritance root.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "RelationPropertyTypeNotInMapping.InvalidRelationClass1\r\n"
                  + "Property: RelationProperty"));
    }

    //RelationEndPointCombinationIsSupportedValidationRule
    [Test]
    public void ManyToMany ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.ManyToMany"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The relation between property 'RelationProperty', declared on type 'InvalidRelationClass1', and property 'RelationProperty' declared on type "
                  + "'InvalidRelationClass2', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
                  + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "ManyToMany.InvalidRelationClass1\r\n"
                  + "Property: RelationProperty\r\n"
                  + "Relation ID: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "ManyToMany.InvalidRelationClass2:Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration."
                  + "NotSupportedRelations.ManyToMany.InvalidRelationClass2.RelationProperty->Remotion.Data.DomainObjects.UnitTests.Mapping."
                  + "TestDomain.Validation.Integration.NotSupportedRelations.ManyToMany.InvalidRelationClass1.RelationProperty\r\n"
                  + "----------\r\n"
                  + "The relation between property 'RelationProperty', declared on type 'InvalidRelationClass2', and property 'RelationProperty' declared on type "
                  + "'InvalidRelationClass1', contains two virtual end points. One of the two properties must set 'ContainsForeignKey' to 'true' on the "
                  + "'DBBidirectionalRelationAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "ManyToMany.InvalidRelationClass2\r\n"
                  + "Property: RelationProperty\r\n"
                  + "Relation ID: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations."
                  + "ManyToMany.InvalidRelationClass1:Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration."
                  + "NotSupportedRelations.ManyToMany.InvalidRelationClass1.RelationProperty->Remotion.Data.DomainObjects.UnitTests."
                  + "Mapping.TestDomain.Validation.Integration.NotSupportedRelations.ManyToMany.InvalidRelationClass2.RelationProperty"));
    }

    [Test]
    public void Bidirectional_ReferencingDomainObjectType ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.Bidirectional_ReferencingDomainObjectType"),
          Throws.InstanceOf<MappingException>()
              .With.Message.Contains(
                  "The relation property 'BidirectionalRelationProperty' has return type 'DomainObject', which is not a part of the mapping. "
                  + "Relation properties must not point to types above the inheritance root.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.Bidirectional_ReferencingDomainObjectType.ClassReferencingDomainObjectType\r\n"
                  + "Property: BidirectionalRelationProperty"));
    }

    [Test]
    public void Bidirectional_ReferencingNonDomainObject ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.Bidirectional_ReferencingNonDomainObject"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The 'DBBidirectionalRelationAttribute' may be only applied to properties assignable to types 'DomainObject', 'ObjectList`1', or 'IObjectList`1'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.Bidirectional_ReferencingNonDomainObject.ClassReferencingNonDomainObject\r\n"
                  + "Property: BidirectionalRelationProperty"));
    }

    [Test]
    public void Bidirectional_ReferencingObjectType ()
    {
      Assert.That(
          () => ValidateMapping("NotSupportedRelations.Bidirectional_ReferencingObject"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The 'DBBidirectionalRelationAttribute' may be only applied to properties assignable to types 'DomainObject', 'ObjectList`1', or 'IObjectList`1'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.Bidirectional_ReferencingObject.ClassReferencingObject\r\n"
                  + "Property: BidirectionalRelationProperty\r\n"
                  + "----------\r\n"
                  + "The property type 'Object' is not supported. If you meant to declare a relation, 'Object' must be derived from 'DomainObject'. "
                  + "For non-mapped properties, use the 'StorageClassNoneAttribute'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedRelations.Bidirectional_ReferencingObject.ClassReferencingObject\r\n"
                  + "Property: BidirectionalRelationProperty"));
    }
  }
}
