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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.SortExpressions
{
  [TestFixture]
  public class SortExpressionParserTest : StandardMappingTest
  {
    private SortExpressionParser _parser;
    private TypeDefinition _orderItemTypeDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;
    private PropertyDefinition _orderPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _orderItemTypeDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(OrderItem));
      _productPropertyDefinition = _orderItemTypeDefinition.GetMandatoryPropertyDefinition(typeof(OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemTypeDefinition.GetMandatoryPropertyDefinition(typeof(OrderItem).FullName + ".Position");
      _orderPropertyDefinition = _orderItemTypeDefinition.GetMandatoryPropertyDefinition(typeof(OrderItem).FullName + ".Order");

      _parser = new SortExpressionParser(_orderItemTypeDefinition);
    }

    [Test]
    public void Parse_Empty ()
    {
      var sortExpression = "";

      var result = _parser.Parse(sortExpression);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Parse_WithFullIdentifier ()
    {
      var sortExpression = "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product";

      var result = _parser.Parse(sortExpression);

      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_productPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithShortPropertyName ()
    {
      var sortExpression = "Product";

      var result = _parser.Parse(sortExpression);

      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_productPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithRealRelationEndPoint ()
    {
      var sortExpression = "Order";

      var result = _parser.Parse(sortExpression);

      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_orderPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithVirtualRelationEndPoint ()
    {
      var orderClassDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Order));
      var parser = new SortExpressionParser(orderClassDefinition);

      var sortExpression = "OrderTicket";
      Assert.That(
          () => parser.Parse(sortExpression),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "SortExpression 'OrderTicket' cannot be parsed: The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' is a "
                  + "virtual relation end point. SortExpressions can only contain relation end points if the object to be sorted contains the foreign key."));
    }

    [Test]
    public void Parse_WithUnknownPropertyName ()
    {
      var sortExpression = "UnknownProduct";
      Assert.That(
          () => _parser.Parse(sortExpression),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "SortExpression 'UnknownProduct' cannot be parsed: 'UnknownProduct' is not a valid mapped property name. Expected the .NET property name of "
                  + "a property declared by the 'OrderItem' class or its base classes. Alternatively, to resolve ambiguities or to use a property declared by a "
                  + "mixin or a derived class of 'OrderItem', the full unique re-store property identifier can be specified."));
    }

    [Test]
    public void Parse_WithDerivedProperty ()
    {
      var partnerClassDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Partner));
      var parser = new SortExpressionParser(partnerClassDefinition);

      var sortExpression = "Remotion.Data.DomainObjects.UnitTests.TestDomain.Distributor.NumberOfShops";

      var result = parser.Parse(sortExpression);

      var distributorClassDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Distributor));
      var numberOfShopsPropertyDefinition = distributorClassDefinition.GetMandatoryPropertyDefinition(sortExpression);
      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(numberOfShopsPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_Ascending ()
    {
      var sortExpression = "Product asc";

      var result = _parser.Parse(sortExpression);

      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_productPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_Descending ()
    {
      var sortExpression = "Product desc";

      var result = _parser.Parse(sortExpression);

      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(_productPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_CaseIsIgnored ()
    {
      var sortExpression = "Product dEsC";

      var result = _parser.Parse(sortExpression);

      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(_productPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_MultipleSpaces ()
    {
      var sortExpression = "Product  desc";

      var result = _parser.Parse(sortExpression);

      var expected = new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(_productPropertyDefinition) };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_Unknown ()
    {
      var sortExpression = "Product unknown";
      Assert.That(
          () => _parser.Parse(sortExpression),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "SortExpression 'Product unknown' cannot be parsed: 'unknown' is not a valid sort order. Expected 'asc' or 'desc'."));
    }

    [Test]
    public void Parse_WithTooManyWords ()
    {
      var sortExpression = "Product asc asc";
      Assert.That(
          () => _parser.Parse(sortExpression),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "SortExpression 'Product asc asc' cannot be parsed: Expected 1 or 2 parts (a property name and an optional identifier), found 3 parts "
                  + "instead."));
    }

    [Test]
    public void Parse_Many ()
    {
      var sortExpression = "Product asc,Position,Order desc";

      var result = _parser.Parse(sortExpression);

      var expected = new[]
                     {
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_productPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_positionPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(_orderPropertyDefinition)
                     };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_Many_Space ()
    {
      var sortExpression = "Product asc, Position, Order desc";

      var result = _parser.Parse(sortExpression);

      var expected = new[]
                     {
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_productPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_positionPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(_orderPropertyDefinition)
                     };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_Many_TrailingComma ()
    {
      var sortExpression = "Product asc,Position,Order desc,";

      var result = _parser.Parse(sortExpression);

      var expected = new[]
                     {
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_productPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_positionPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(_orderPropertyDefinition)
                     };
      Assert.That(result.SortedProperties, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_RoundTrip ()
    {
      var sortExpression = "Product asc, Position, Order desc";

      var result1 = _parser.Parse(sortExpression);
      var result2 = _parser.Parse(result1.ToString());

      var expected = new[]
                     {
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_productPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(_positionPropertyDefinition),
                         SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending(_orderPropertyDefinition)
                     };
      Assert.That(result2.SortedProperties, Is.EqualTo(expected));
    }
  }
}
