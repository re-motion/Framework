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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class EnumValueFilterProviderTest
  {
    private DisableEnumValuesAttribute _attribute;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new DisableEnumValuesAttribute(typeof(StubEnumerationValueFilter));
    }

    [Test]
    public void GetEnumerationValueFilter_FromPropertyInformation ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.GetCustomAttribute<DisableEnumValuesAttribute>(true)).Returns(_attribute);

      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          propertyInformationStub.Object,
          delegate
          {
            Assert.Fail("Must not be called");
            return null;
          });
      Assert.That(provider.GetEnumerationValueFilter(), Is.TypeOf(typeof(StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromAttributeProvider ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.GetCustomAttribute<DisableEnumValuesAttribute>(true)).Returns((DisableEnumValuesAttribute)null);
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int));

      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          propertyInformationStub.Object,
          delegate (Type type)
          {
            Assert.That(type, Is.SameAs(typeof(int)));
            return new[] { _attribute };
          });
      Assert.That(provider.GetEnumerationValueFilter(), Is.TypeOf(typeof(StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromAttributeProvider_WithNullableProperty ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.GetCustomAttribute<DisableEnumValuesAttribute>(true)).Returns((DisableEnumValuesAttribute)null);
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int?));

      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          propertyInformationStub.Object,
          delegate (Type type)
          {
            Assert.That(type, Is.SameAs(typeof(int)));
            return new[] { _attribute };
          });
      Assert.That(provider.GetEnumerationValueFilter(), Is.TypeOf(typeof(StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromAttributeProvider_Multiple ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.GetCustomAttribute<IDisableEnumValuesAttribute>(true)).Returns((IDisableEnumValuesAttribute)null);
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int));

      var filterStub = new Mock<IEnumerationValueFilter>();

      var additionalAttributeStub = new Mock<IDisableEnumValuesAttribute>();
      additionalAttributeStub.Setup(stub => stub.GetEnumerationValueFilter()).Returns(filterStub.Object);

      var provider = new EnumValueFilterProvider<IDisableEnumValuesAttribute>(
          propertyInformationStub.Object,
          delegate (Type type)
          {
            Assert.That(type, Is.SameAs(typeof(int)));
            return new[] { _attribute, additionalAttributeStub.Object };
          });

      var actualFilter = provider.GetEnumerationValueFilter();

      Assert.That(actualFilter, Is.TypeOf(typeof(CompositeEnumerationValueFilter)));

      var compositeFilter = ((CompositeEnumerationValueFilter)actualFilter);
      Assert.That(compositeFilter.Filters.Count, Is.EqualTo(2));
      Assert.That(compositeFilter.Filters[0], Is.TypeOf(typeof(StubEnumerationValueFilter)));
      Assert.That(compositeFilter.Filters[1], Is.SameAs(filterStub.Object));
    }

    [Test]
    public void GetEnumerationValueFilter_None ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.GetCustomAttribute<DisableEnumValuesAttribute>(true)).Returns((DisableEnumValuesAttribute)null);
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int));

      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          propertyInformationStub.Object,
          delegate (Type type)
          {
            Assert.That(type, Is.SameAs(typeof(int)));
            return new DisableEnumValuesAttribute[0];
          });
      Assert.That(provider.GetEnumerationValueFilter(), Is.TypeOf(typeof(NullEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromPropertyInformation_IntegrationTestWithProperty ()
    {
      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          GetPropertyInformation("DisabledFromProperty"),
          delegate
          {
            Assert.Fail("Must not be called");
            return null;
          });

      var actual = provider.GetEnumerationValueFilter();
      Assert.That(actual, Is.TypeOf(typeof(ConstantEnumerationValueFilter)));
      Assert.That(((ConstantEnumerationValueFilter)actual).DisabledEnumValues, Is.EquivalentTo(new[] { TestEnum.Value1 }));
    }

    [Test]
    public void GetEnumerationValueFilter_FromPropertyInformation_IntegrationTestWithNullableProperty ()
    {
      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          GetPropertyInformation("DisabledFromNullableProperty"),
          delegate
          {
            Assert.Fail("Must not be called");
            return null;
          });

      var actual = provider.GetEnumerationValueFilter();
      Assert.That(actual, Is.TypeOf(typeof(ConstantEnumerationValueFilter)));
      Assert.That(((ConstantEnumerationValueFilter)actual).DisabledEnumValues, Is.EquivalentTo(new[] { TestEnum.Value1 }));
    }

    [Test]
    public void GetEnumerationValueFilter_FromAttributeProvider_IntegrationTestWithPropertyType ()
    {
      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          GetPropertyInformation("DisabledFromPropertyType"),
          t => AttributeUtility.GetCustomAttributes<DisableEnumValuesAttribute>(t, true));

      var actual = provider.GetEnumerationValueFilter();
      Assert.That(actual, Is.TypeOf(typeof(ConstantEnumerationValueFilter)));
      Assert.That(((ConstantEnumerationValueFilter)actual).DisabledEnumValues, Is.EquivalentTo(new[] { TestEnum.Value5 }));
    }

    [Test]
    public void GetEnumerationValueFilter_FromAttributeProvider_IntegrationTestWithNullablePropertyType ()
    {
      var provider = new EnumValueFilterProvider<DisableEnumValuesAttribute>(
          GetPropertyInformation("DisabledFromNullablePropertyType"),
          t => AttributeUtility.GetCustomAttributes<DisableEnumValuesAttribute>(t, true));

      var actual = provider.GetEnumerationValueFilter();
      Assert.That(actual, Is.TypeOf(typeof(ConstantEnumerationValueFilter)));
      Assert.That(((ConstantEnumerationValueFilter)actual).DisabledEnumValues, Is.EquivalentTo(new[] { TestEnum.Value5 }));
    }

    private IPropertyInformation GetPropertyInformation (string name)
    {
      return PropertyInfoAdapter.Create(typeof(ClassWithDisabledEnumValue).GetProperty(name));
    }
  }
}
