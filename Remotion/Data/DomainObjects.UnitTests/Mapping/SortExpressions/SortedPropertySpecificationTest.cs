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
  public class SortedPropertySpecificationTest : StandardMappingTest
  {
    private TypeDefinition _orderItemTypeDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;

    private TypeDefinition _customerTypeDefinition;
    private PropertyDefinition _customerSincePropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();
      _orderItemTypeDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(OrderItem));
      _productPropertyDefinition = _orderItemTypeDefinition.GetMandatoryPropertyDefinition(typeof(OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemTypeDefinition.GetMandatoryPropertyDefinition(typeof(OrderItem).FullName + ".Position");

      _customerTypeDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Customer));
      _customerSincePropertyDefinition = _customerTypeDefinition.GetMandatoryPropertyDefinition(typeof(Customer).FullName + ".CustomerSince");
    }

    [Test]
    public void Initialization_NoIComparableType ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(ClassWithAllDataTypes));
      var propertyDefinition = classDefinition.GetPropertyDefinition(typeof(ClassWithAllDataTypes).FullName + ".BinaryProperty");
      Assert.That(
          () => new SortedPropertySpecification(propertyDefinition, SortOrder.Ascending),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Cannot sort by property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty' - its property type "
                  + "('Byte[]') does not implement IComparable."));
    }

    [Test]
    public void Initialization_IComparableType_Nullable ()
    {
      Assert.That(Nullable.GetUnderlyingType(_customerSincePropertyDefinition.PropertyType), Is.Not.Null);
      new SortedPropertySpecification(_customerSincePropertyDefinition, SortOrder.Ascending);
    }

    [Test]
    public void To_String ()
    {
      var specificationAsc = new SortedPropertySpecification(_productPropertyDefinition, SortOrder.Ascending);
      var specificationDesc = new SortedPropertySpecification(_productPropertyDefinition, SortOrder.Descending);

      Assert.That(specificationAsc.ToString(), Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product ASC"));
      Assert.That(specificationDesc.ToString(), Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product DESC"));
    }

    [Test]
    public void Equals ()
    {
      var specification1 = new SortedPropertySpecification(_productPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification(_productPropertyDefinition, SortOrder.Ascending);
      var specification3 = new SortedPropertySpecification(_productPropertyDefinition, SortOrder.Descending);
      var specification4 = new SortedPropertySpecification(_positionPropertyDefinition, SortOrder.Ascending);

      Assert.That(specification1, Is.Not.EqualTo(null));
      Assert.That(specification1, Is.EqualTo(specification2));
      Assert.That(specification1, Is.Not.EqualTo(specification3));
      Assert.That(specification1, Is.Not.EqualTo(specification4));
    }

    [Test]
    public void Get_HashCode ()
    {
      var specification1 = new SortedPropertySpecification(_productPropertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification(_productPropertyDefinition, SortOrder.Ascending);

      Assert.That(specification1.GetHashCode(), Is.EqualTo(specification2.GetHashCode()));
    }
  }
}
