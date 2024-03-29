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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class ConvertFromNativePropertyType : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
    }

    [Test]
    public void ValidValue ()
    {
      PropertyBase property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");

      Assert.That(property.ConvertFromNativePropertyType(TestEnum.Value1), Is.EqualTo(TestEnum.Value1));
    }

    [Test]
    public void Null ()
    {
      PropertyBase property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");

      Assert.That(property.ConvertFromNativePropertyType(null), Is.Null);
    }

    [Test]
    public void UndefinedValue ()
    {
      PropertyBase property = CreateProperty(typeof(ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That(property.ConvertFromNativePropertyType(EnumWithUndefinedValue.UndefinedValue), Is.Null);
    }

    [Test]
    public void InvalidEnumValue ()
    {
      PropertyBase property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");

      Assert.That(property.ConvertFromNativePropertyType((TestEnum)(-1)), Is.EqualTo((TestEnum)(-1)));
    }

    [Test]
    public void EnumValueFromOtherType ()
    {
      PropertyBase property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");

      property.ConvertFromNativePropertyType(EnumWithUndefinedValue.Value1);
      Assert.That(property.ConvertFromNativePropertyType(EnumWithUndefinedValue.Value1), Is.EqualTo(EnumWithUndefinedValue.Value1));
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty(
        GetPropertyParameters(GetPropertyInfo(type, propertyName), _businessObjectProvider));
    }
  }
}
