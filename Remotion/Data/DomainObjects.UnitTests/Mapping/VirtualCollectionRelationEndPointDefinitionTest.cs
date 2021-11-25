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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.SortExpressions;
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
          typeof(IObjectList<ProductReview>));

      Assert.That(endPoint.PropertyInfo.PropertyType, Is.SameAs(typeof(IObjectList<ProductReview>)));
    }

    [Test]
    public void IsAnonymous ()
    {
      var reviewsEndPointDefinition = CreateReviewsEndPointDefinition();
      Assert.That(reviewsEndPointDefinition.IsAnonymous, Is.False);
    }

    [Test]
    public void RelationDefinition_Null ()
    {
      var relationEndPointDefinition = CreateReviewsEndPointDefinition();
      Assert.That(relationEndPointDefinition.RelationDefinition, Is.Null);
    }

    [Test]
    public void RelationDefinition_NonNull ()
    {
      var relationEndPointDefinition = CreateReviewsEndPointDefinition();
      relationEndPointDefinition.SetRelationDefinition(new RelationDefinition("Test", relationEndPointDefinition, relationEndPointDefinition));
      Assert.That(relationEndPointDefinition.RelationDefinition, Is.Not.Null);
    }

    [Test]
    public void GetSortExpression_Null ()
    {
      var classDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          classDefinition,
          "Reviews",
          false,
          typeof(IObjectList<ProductReview>));

      Assert.That(endPoint.GetSortExpression(), Is.Null);
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var productReviewClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ProductReview));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(productReviewClassDefinition, typeof(ProductReview), "CreatedAt");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(propertyDefinition) });

      var classDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();

      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          classDefinition,
          "Reviews",
          false,
          typeof(IObjectList<ProductReview>),
          new Lazy<SortExpressionDefinition>(() => sortExpressionDefinition));

      Assert.That(endPoint.GetSortExpression(), Is.SameAs(sortExpressionDefinition));
    }

    [Test]
    public void PropertyInfo ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Product));
      var relationEndPointDefinition =
          (VirtualCollectionRelationEndPointDefinition) classDefinition.GetRelationEndPointDefinition(typeof(Product) + ".Reviews");
      Assert.That(relationEndPointDefinition.PropertyInfo, Is.EqualTo(PropertyInfoAdapter.Create(typeof(Product).GetProperty("Reviews"))));
    }

    private ClassDefinition CreateProductDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("Product", classType: typeof(Product));
    }

    private VirtualCollectionRelationEndPointDefinition CreateReviewsEndPointDefinition ()
    {
      var productClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Product));
      var productReviewClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ProductReview));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(productReviewClassDefinition, typeof(ProductReview), "CreatedAt");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(propertyDefinition) });
      return VirtualCollectionRelationEndPointDefinitionFactory.Create(
          productClassDefinition,
          "Reviews",
          false,
          typeof(IObjectList<ProductReview>),
          new Lazy<SortExpressionDefinition>(() => sortExpressionDefinition));
    }
  }
}
