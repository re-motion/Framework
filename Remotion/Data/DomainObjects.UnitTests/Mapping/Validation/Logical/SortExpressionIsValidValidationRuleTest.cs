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
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class SortExpressionIsValidValidationRuleTest : ValidationRuleTestBase
  {
    private SortExpressionIsValidValidationRule _validationRule;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new SortExpressionIsValidValidationRule();
      _classDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Order)];
    }

    [Test]
    public void ValidSortExpressionWithSortingDirection ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "Orders",
          false,
          CardinalityType.Many,
          "OrderNumber desc",
          MockRepository.GenerateStub<IPropertyInformation>());
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition (relationDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ValidSortExpressionWithoutSortingDirection ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition, "Orders", false, CardinalityType.Many, "OrderNumber", MockRepository.GenerateStub<IPropertyInformation>());
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition (relationDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void InvalidSortExpression ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _classDefinition,
          "Orders",
          false,
          CardinalityType.Many,
          "Test",
          PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("Orders")));
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      endPointDefinition.SetRelationDefinition (relationDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage =
          "SortExpression 'Test' cannot be parsed: 'Test' is not a valid mapped property name. Expected the .NET property name of a property "
          + "declared by the 'Order' class or its base classes. Alternatively, to resolve ambiguities or to use a property declared by a mixin "
          + "or a derived class of 'Order', the full unique re-store property identifier can be specified.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer\r\nProperty: Orders";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}