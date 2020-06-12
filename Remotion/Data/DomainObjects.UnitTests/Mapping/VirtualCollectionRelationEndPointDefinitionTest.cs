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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class VirtualCollectionRelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    [Test]
    public void InitializeWithPropertyType ()
    {
      var classDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          classDefinition,
          "VirtualEndPoint",
          true,
          typeof (IObjectList<ProductReview>),
          null);

      Assert.That (endPoint.PropertyInfo.PropertyType, Is.SameAs (typeof (IObjectList<ProductReview>)));
    }

    [Test]
    public void InitializeWithSortExpression ()
    {
      var classDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPointDefinition = VirtualCollectionRelationEndPointDefinitionFactory.Create (
          classDefinition,
          "Reviews",
          false,
          typeof (IObjectList<ProductReview>),
          "CreatedAt desc");

      Assert.That (endPointDefinition.SortExpressionText, Is.EqualTo ("CreatedAt desc"));
    }

    [Test]
    public void IsAnonymous ()
    {
      var reviewsEndPointDefinition = CreateReviewsEndPointDefinition();
      Assert.That (reviewsEndPointDefinition.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinition_Null ()
    {
      var relationEndPointDefinition = CreateReviewsEndPointDefinition();
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void RelationDefinition_NonNull ()
    {
      var relationEndPointDefinition = CreateReviewsEndPointDefinition();
      relationEndPointDefinition.SetRelationDefinition (new RelationDefinition ("Test", relationEndPointDefinition, relationEndPointDefinition));
      Assert.That (relationEndPointDefinition.RelationDefinition, Is.Not.Null);
    }

    [Test]
    public void GetSortExpression_Null ()
    {
      var classDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create (
          classDefinition,
          "Reviews",
          false,
          typeof (IObjectList<ProductReview>),
          null);
      Assert.That (endPoint.SortExpressionText, Is.Null);

      Assert.That (endPoint.GetSortExpression(), Is.Null);
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var endPoint = CreateFullVirtualEndPointAndClassDefinition_WithProductProperty ("CreatedAt asc");

      Assert.That (endPoint.GetSortExpression(), Is.Not.Null);
      Assert.That (
          endPoint.GetSortExpression().ToString(),
          Is.EqualTo ("CreatedAt ASC"));
    }

    [Test]
    public void GetSortExpression_Error ()
    {
      var endPoint = CreateFullVirtualEndPointAndClassDefinition_WithProductProperty ("CreatedAt asc asc");
      Assert.That (
          () => endPoint.GetSortExpression(),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo (
                  "SortExpression 'CreatedAt asc asc' cannot be parsed: Expected 1 or 2 parts (a property name and an optional identifier), found 3 parts instead.\r\n\r\n"+
                  "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Product\r\nProperty: Reviews"));
    }

    [Test]
    public void PropertyInfo ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Product));
      var relationEndPointDefinition = 
          (VirtualCollectionRelationEndPointDefinition) classDefinition.GetRelationEndPointDefinition (typeof (Product) + ".Reviews");
      Assert.That (relationEndPointDefinition.PropertyInfo, Is.EqualTo (PropertyInfoAdapter.Create(typeof (Product).GetProperty ("Reviews"))));
    }

    private VirtualCollectionRelationEndPointDefinition CreateFullVirtualEndPointAndClassDefinition_WithProductProperty (string sortExpressionString)
    {
      var classDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create (
          classDefinition,
          "Reviews",
          false,
          typeof (IObjectList<ProductReview>),
          sortExpressionString);
      var productReviewClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (ProductReview));
      var oppositeProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (productReviewClassDefinition, "ProductReview");
      var valueProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (productReviewClassDefinition, "CreatedAt");
      productReviewClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{oppositeProperty, valueProperty}, true));
      productReviewClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var oppositeEndPoint = new RelationEndPointDefinition (oppositeProperty, false);
      var relationDefinition = new RelationDefinition ("test", endPoint, oppositeEndPoint);
      productReviewClassDefinition.SetReadOnly ();
      endPoint.SetRelationDefinition (relationDefinition);
      return endPoint;
    }

    private ClassDefinition CreateProductDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses ("Product", classType: typeof (Product));
    }

    private VirtualCollectionRelationEndPointDefinition CreateReviewsEndPointDefinition ()
    {
      var productClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Product));
      return VirtualCollectionRelationEndPointDefinitionFactory.Create (
          productClassDefinition,
          "Reviews",
          false,
          typeof (IObjectList<ProductReview>),
          "CreatedAt desc");
    }
  }
}
