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
      var typeDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          typeDefinition,
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
    public void RelationDefinition_NotSet ()
    {
      var relationEndPointDefinition = CreateReviewsEndPointDefinition();

      Assert.That(relationEndPointDefinition.HasRelationDefinitionBeenSet, Is.False);
      Assert.That(
          () => relationEndPointDefinition.RelationDefinition,
          Throws.InvalidOperationException.With.Message.EqualTo("RelationDefinition has not been set for this relation end point."));
    }

    [Test]
    public void SetRelationDefinition ()
    {
      var relationEndPointDefinition = CreateReviewsEndPointDefinition();
      var relationDefinition = new RelationDefinition("Test", relationEndPointDefinition, relationEndPointDefinition);
      relationEndPointDefinition.SetRelationDefinition(relationDefinition);

      Assert.That(relationEndPointDefinition.HasRelationDefinitionBeenSet, Is.True);
      Assert.That(relationEndPointDefinition.RelationDefinition, Is.SameAs(relationDefinition));
    }

    [Test]
    public void GetSortExpression_Null ()
    {
      var typeDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();
      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          typeDefinition,
          "Reviews",
          false,
          typeof(IObjectList<ProductReview>));

      Assert.That(endPoint.GetSortExpression(), Is.Null);
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var productReviewTypeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(ProductReview));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(productReviewTypeDefinition, typeof(ProductReview), "CreatedAt");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(propertyDefinition) });

      var typeDefinition = CreateProductDefinition_WithEmptyMembers_AndDerivedClasses();

      var endPoint = VirtualCollectionRelationEndPointDefinitionFactory.Create(
          typeDefinition,
          "Reviews",
          false,
          typeof(IObjectList<ProductReview>),
          new Lazy<SortExpressionDefinition>(() => sortExpressionDefinition));

      Assert.That(endPoint.GetSortExpression(), Is.SameAs(sortExpressionDefinition));
    }

    [Test]
    public void PropertyInfo ()
    {
      var typeDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Product));
      var relationEndPointDefinition =
          (VirtualCollectionRelationEndPointDefinition)typeDefinition.GetRelationEndPointDefinition(typeof(Product) + ".Reviews");
      Assert.That(relationEndPointDefinition.PropertyInfo, Is.EqualTo(PropertyInfoAdapter.Create(typeof(Product).GetProperty("Reviews"))));
    }

    private TypeDefinition CreateProductDefinition_WithEmptyMembers_AndDerivedClasses ()
    {
      return TypeDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("Product", classType: typeof(Product));
    }

    private VirtualCollectionRelationEndPointDefinition CreateReviewsEndPointDefinition ()
    {
      var productTypeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(Product));
      var productReviewTypeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(ProductReview));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(productReviewTypeDefinition, typeof(ProductReview), "CreatedAt");
      var sortExpressionDefinition = new SortExpressionDefinition(new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(propertyDefinition) });
      return VirtualCollectionRelationEndPointDefinitionFactory.Create(
          productTypeDefinition,
          "Reviews",
          false,
          typeof(IObjectList<ProductReview>),
          new Lazy<SortExpressionDefinition>(() => sortExpressionDefinition));
    }
  }
}
