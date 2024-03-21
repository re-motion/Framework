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
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class SortExpressionIsValidValidationRuleTest : ValidationRuleTestBase
  {
    private SortExpressionIsValidValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new SortExpressionIsValidValidationRule();
    }

    [Test]
    public void ValidSortExpressionWithSortingDirection_WithDomainObjectCollectionRelationEndPointDefinition ()
    {
      var typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Order)];
      var propertyInformationStub = new Mock<IPropertyInformation>();
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var sortExpressionDefinition = new Lazy<SortExpressionDefinition>(
          () => sortExpressionDefinitionProvider.GetSortExpression(propertyInformationStub.Object, typeDefinition, "OrderNumber desc"));

      var endPointDefinition = new DomainObjectCollectionRelationEndPointDefinition(
          typeDefinition,
          "Orders",
          false,
          sortExpressionDefinition,
          propertyInformationStub.Object);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition(relationDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ValidSortExpressionWithSortingDirection_WithVirtualCollectionRelationEndPointDefinition ()
    {
      var typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(ProductReview)];
      var propertyInformationStub = new Mock<IPropertyInformation>();
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var sortExpressionDefinition = new Lazy<SortExpressionDefinition>(
          () => sortExpressionDefinitionProvider.GetSortExpression(propertyInformationStub.Object, typeDefinition, "CreatedAt DESC"));

      var endPointDefinition = new VirtualCollectionRelationEndPointDefinition(
          typeDefinition,
          "Reviews",
          false,
          sortExpressionDefinition,
          propertyInformationStub.Object);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition(relationDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ValidSortExpressionWithoutSortingDirection_WithDomainObjectCollectionRelationEndPointDefinition ()
    {
      var typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Order)];
      var propertyInformationStub = new Mock<IPropertyInformation>();
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var sortExpressionDefinition = new Lazy<SortExpressionDefinition>(
          () => sortExpressionDefinitionProvider.GetSortExpression(propertyInformationStub.Object, typeDefinition, "OrderNumber"));

      var endPointDefinition = new DomainObjectCollectionRelationEndPointDefinition(
          typeDefinition, "Orders", false, sortExpressionDefinition, propertyInformationStub.Object);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition(relationDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void ValidSortExpressionWithoutSortingDirection_WithVirtualCollectionRelationEndPointDefinition ()
    {
      var typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(ProductReview)];
      var propertyInformationStub = new Mock<IPropertyInformation>();
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var sortExpressionDefinition = new Lazy<SortExpressionDefinition>(
          () => sortExpressionDefinitionProvider.GetSortExpression(propertyInformationStub.Object, typeDefinition, "CreatedAt"));

      var endPointDefinition = new VirtualCollectionRelationEndPointDefinition(
          typeDefinition, "Reviews", false, sortExpressionDefinition, propertyInformationStub.Object);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition(relationDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void InvalidSortExpression_WithDomainObjectCollectionRelationEndPointDefinition ()
    {
      var typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(Order)];
      var propertyInfo = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("Orders"));
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var sortExpressionDefinition = new Lazy<SortExpressionDefinition>(
          () => sortExpressionDefinitionProvider.GetSortExpression(propertyInfo, typeDefinition, "Test"));

      var endPointDefinition = new DomainObjectCollectionRelationEndPointDefinition(
          typeDefinition,
          "Orders",
          false,
          sortExpressionDefinition,
          propertyInfo);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition(relationDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "SortExpression 'Test' cannot be parsed: 'Test' is not a valid mapped property name. Expected the .NET property name of a property "
          + "declared by the 'Order' class or its base classes. Alternatively, to resolve ambiguities or to use a property declared by a mixin "
          + "or a derived class of 'Order', the full unique re-store property identifier can be specified.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer\r\nProperty: Orders";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void InvalidSortExpression_WithVirtualCollectionRelationEndPointDefinition ()
    {
      var typeDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof(ProductReview)];
      var propertyInfo = PropertyInfoAdapter.Create(typeof(Product).GetProperty("Reviews"));
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var sortExpressionDefinition = new Lazy<SortExpressionDefinition>(
          () => sortExpressionDefinitionProvider.GetSortExpression(propertyInfo, typeDefinition, "Test"));

      var endPointDefinition = new VirtualCollectionRelationEndPointDefinition(
          typeDefinition,
          "Reviews",
          false,
          sortExpressionDefinition,
          propertyInfo);
      var relationDefinition = new RelationDefinition("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition(relationDefinition);

      var validationResult = _validationRule.Validate(relationDefinition);

      var expectedMessage =
          "SortExpression 'Test' cannot be parsed: 'Test' is not a valid mapped property name. Expected the .NET property name of a property "
          + "declared by the 'ProductReview' class or its base classes. Alternatively, to resolve ambiguities or to use a property declared by a mixin "
          + "or a derived class of 'ProductReview', the full unique re-store property identifier can be specified.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Product\r\nProperty: Reviews";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }
  }
}
