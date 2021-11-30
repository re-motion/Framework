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
using Moq;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class DefaultConverterTest
  {
    private DefaultConverter _converterForString;
    private DefaultConverter _converterForObject;
    private DefaultConverter _converterForInt;
    private DefaultConverter _converterForNullableInt;

    private Mock<ITypeDescriptorContext> _typeDescriptorContext;

    [SetUp]
    public void SetUp ()
    {
      _converterForString = new DefaultConverter(typeof(string));
      _converterForObject = new DefaultConverter(typeof(object));
      _converterForInt = new DefaultConverter(typeof(int));
      _converterForNullableInt = new DefaultConverter(typeof(int?));

      _typeDescriptorContext = new Mock<ITypeDescriptorContext>();
    }

    [Test]
    public void Initialization_ReferenceType ()
    {
      Assert.That(_converterForString.Type, Is.SameAs(typeof(string)));
      Assert.That(_converterForString.IsNullableType, Is.True);
    }

    [Test]
    public void Initialization_NonNullableValueType ()
    {
      Assert.That(_converterForInt.Type, Is.SameAs(typeof(int)));
      Assert.That(_converterForInt.IsNullableType, Is.False);
    }

    [Test]
    public void Initialization_NullableValueType ()
    {
      Assert.That(_converterForNullableInt.Type, Is.SameAs(typeof(int?)));
      Assert.That(_converterForNullableInt.IsNullableType, Is.True);
    }

    [Test]
    public void CanConvertFrom_True ()
    {
      // same type
      Assert.That(_converterForString.CanConvertFrom(_typeDescriptorContext.Object, typeof(string)), Is.True);
      // from non-nullable to nullable
      Assert.That(_converterForNullableInt.CanConvertFrom(_typeDescriptorContext.Object, typeof(int)), Is.True);
    }

    [Test]
    public void CanConvertFrom_False ()
    {
      // completely unrelated
      Assert.That(_converterForString.CanConvertFrom(_typeDescriptorContext.Object, typeof(int)), Is.False);
      // from base to derived type
      Assert.That(_converterForString.CanConvertFrom(_typeDescriptorContext.Object, typeof(object)), Is.False);
      // from derived to base type
      Assert.That(_converterForObject.CanConvertFrom(_typeDescriptorContext.Object, typeof(string)), Is.False);
      // from nullable to non-nullable
      Assert.That(_converterForInt.CanConvertFrom(_typeDescriptorContext.Object, typeof(int?)), Is.False);
    }

    [Test]
    public void CanConvertTo_True ()
    {
      // same type
      Assert.That(_converterForString.CanConvertTo(_typeDescriptorContext.Object, typeof(string)), Is.True);
      // from non-nullable to nullable
      Assert.That(_converterForInt.CanConvertTo(_typeDescriptorContext.Object, typeof(int?)), Is.True);
    }

    [Test]
    public void CanConvertTo_False ()
    {
      // completely unrelated
      Assert.That(_converterForString.CanConvertTo(_typeDescriptorContext.Object, typeof(int)), Is.False);
      // from base to derived type
      Assert.That(_converterForObject.CanConvertTo(_typeDescriptorContext.Object, typeof(string)), Is.False);
      // from derived to base type
      Assert.That(_converterForString.CanConvertTo(_typeDescriptorContext.Object, typeof(object)), Is.False);
      // from nullable to non-nullable
      Assert.That(_converterForNullableInt.CanConvertTo(_typeDescriptorContext.Object, typeof(int)), Is.False);
    }

    [Test]
    public void ConvertFrom_ValueIsNullAndNoNullableType ()
    {
      Assert.That(
          () => _converterForInt.ConvertFrom(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, null),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Null cannot be converted to type 'System.Int32'."));
    }

    [Test]
    public void ConvertFrom_ValueIsNullAndReferenceType ()
    {
      var result = _converterForString.ConvertFrom(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, null);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertFrom_ValueIsNullAndNullableValueType ()
    {
      var result = _converterForNullableInt.ConvertFrom(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, null);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertFrom_ValueIsNotNullAndCannotConvertFromType ()
    {
      Assert.That(
          () => _converterForString.ConvertFrom(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, new object()),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Value of type 'System.Object' cannot be connverted to type 'System.String'."));
    }

    [Test]
    public void ConvertFrom_ValueIsNotNullAndCanConvertFromType ()
    {
      var result = _converterForString.ConvertFrom(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, "test");

      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void ConvertFrom_ValueIsUnderlyingType ()
    {
      var result = _converterForNullableInt.ConvertFrom(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, 17);

      Assert.That(result, Is.EqualTo(17));
    }

    [Test]
    public void ConvertTo_ValueIsNullAndNotValid ()
    {
      Assert.That(
          () => _converterForInt.ConvertTo(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, null, typeof(int)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("The given value '' cannot be converted by this TypeConverter for type 'System.Int32'."));
    }

    [Test]
    public void ConvertTo_ValueIsNullAndReferenceType ()
    {
      var result = _converterForString.ConvertTo(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, null, typeof(string));

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertTo_ValueIsNullAndNullableValueType ()
    {
      var result = _converterForNullableInt.ConvertTo(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, null, typeof(int?));

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertTo_ValueIsNotNullAndNotValid ()
    {
      Assert.That(
          () => _converterForString.ConvertTo(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, 5, typeof(string)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("The given value '5' cannot be converted by this TypeConverter for type 'System.String'."));
    }

    [Test]
    public void ConvertTo_ValueIsNotNullAndCannotConvertToDestinationType ()
    {
      Assert.That(
          () => _converterForString.ConvertTo(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, "test", typeof(object)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "This TypeConverter cannot convert to type 'System.Object'."));
    }

    [Test]
    public void ConvertTo_ValueIsNotNullAndCanConvertToDestinationType ()
    {
      var result = _converterForString.ConvertTo(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, "test", typeof(string));

      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void ConvertTo_DestinationTypeIsNullableValueType ()
    {
      var result = _converterForInt.ConvertTo(_typeDescriptorContext.Object, CultureInfo.CurrentCulture, 17, typeof(int?));

      Assert.That(result, Is.EqualTo(17));
    }

    [Test]
    public void IsValid_NullValue_NullableType ()
    {
      var result = _converterForObject.IsValid(null);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_NullValue_NonNullableType ()
    {
      var result = _converterForInt.IsValid(null);

      Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_CannotConvertFrom ()
    {
      var result = _converterForInt.IsValid("test");

      Assert.That(result, Is.False);
    }

    [Test]
    public void IsValid_CanConvertFrom ()
    {
      var result = _converterForInt.IsValid(5);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsValid_CanConvertFromUnderlyingType ()
    {
      var result = _converterForInt.IsValid(new int?(5));

      Assert.That(result, Is.True);
    }
  }
}
