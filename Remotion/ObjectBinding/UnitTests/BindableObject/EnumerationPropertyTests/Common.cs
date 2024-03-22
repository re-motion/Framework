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
  public class Common : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
    }

    [Test]
    public void Initialize_NullableWithUndefinedValue ()
    {
      Assert.That(
          () => CreateProperty(typeof(ClassWithValueType<EnumWithUndefinedValue>), "NullableScalar"),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The property 'NullableScalar' defined on type 'Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithValueType`1[Remotion.ObjectBinding.UnitTests.TestDomain.EnumWithUndefinedValue]'"
                  + " must not be nullable since the property's type already defines a 'Remotion.ObjectBinding.UndefinedEnumValueAttribute'."));
    }

    [Test]
    public void Initialize_FlagsEnum ()
    {
      Assert.That(
          () => CreateProperty(typeof(ClassWithValueType<TestFlags>), "Scalar"),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The property 'Scalar' defined on type 'Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithValueType`1[Remotion.ObjectBinding.UnitTests.TestDomain.TestFlags]'"
                  + " is a flags-enum, which is not supported."));
    }

    [Test]
    public void Initialize_WithUndefinedEnumValueFromOtherType ()
    {
      Assert.That(
          () => CreateProperty(typeof(ClassWithValueType<EnumWithUndefinedValueFromOtherType>), "Scalar"),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The enum type 'Remotion.ObjectBinding.UnitTests.TestDomain.EnumWithUndefinedValueFromOtherType' "
                  + "defines a 'Remotion.ObjectBinding.UndefinedEnumValueAttribute' with an enum value that belongs to a different enum type."));
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty(
          GetPropertyParameters(GetPropertyInfo(type, propertyName), _businessObjectProvider));
    }
  }
}
