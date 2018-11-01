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
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class NullValueConverterTest
  {
    private NullValueConverter _nullValueConverter;
    private ITypeDescriptorContext _typeDescriptorContextStub;

    [SetUp]
    public void SetUp ()
    {
      _typeDescriptorContextStub = MockRepository.GenerateStub<ITypeDescriptorContext>();

      _nullValueConverter = NullValueConverter.Instance;
    }

    [Test]
    public void CanConvertTo_NonNullableType ()
    {
      var result = _nullValueConverter.CanConvertTo (_typeDescriptorContextStub, typeof (int));

      Assert.That (result, Is.False);
    }

    [Test]
    public void CanConvertTo_NullableType ()
    {
      var result = _nullValueConverter.CanConvertTo (_typeDescriptorContextStub, typeof (string));

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanConvertTo_NullableValueType ()
    {
      var result = _nullValueConverter.CanConvertTo (_typeDescriptorContextStub, typeof (int?));

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanConvertFrom_NonNullableType ()
    {
      var result = _nullValueConverter.CanConvertFrom (_typeDescriptorContextStub, typeof (int));

      Assert.That (result, Is.False);
    }

    [Test]
    public void CanConvertFrom_NullableType ()
    {
      var result = _nullValueConverter.CanConvertFrom (_typeDescriptorContextStub, typeof (string));

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanConvertFrom_NullableValueType ()
    {
      var result = _nullValueConverter.CanConvertFrom (_typeDescriptorContextStub, typeof (int?));

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsValid_NullValue ()
    {
      var result = _nullValueConverter.IsValid (_typeDescriptorContextStub, null);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsValid_NonNullValue ()
    {
      var result = _nullValueConverter.IsValid (_typeDescriptorContextStub, "test");

      Assert.That (result, Is.False);
    }

    [Test]
    public void ConvertFrom_NullValue()
    {
      var result = _nullValueConverter.ConvertFrom (_typeDescriptorContextStub, CultureInfo.CurrentCulture, null);

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Value 'test' cannot be converted to null.")]
    public void ConvertFrom_NonNullValue ()
    {
      _nullValueConverter.ConvertFromString (_typeDescriptorContextStub, CultureInfo.CurrentCulture, "test");
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Null value cannot be converted to type 'System.Int32'.")]
    public void ConvertTo_DestinationTypeNotNullable ()
    {
      _nullValueConverter.ConvertTo (_typeDescriptorContextStub, CultureInfo.CurrentCulture, null, typeof (int));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Value 'test' is not supported by this converter.")]
    public void ConvertTo_NullableDestinationTypeValueNotNull ()
    {
      _nullValueConverter.ConvertTo (_typeDescriptorContextStub, CultureInfo.CurrentCulture, "test", typeof (int));
    }

    [Test]
    public void ConvertTo_NullableDestinationTypeWithNullValue ()
    {
      var result = _nullValueConverter.ConvertTo (_typeDescriptorContextStub, CultureInfo.CurrentCulture, null, typeof (string));

      Assert.That (result, Is.Null);
    }
  }
}