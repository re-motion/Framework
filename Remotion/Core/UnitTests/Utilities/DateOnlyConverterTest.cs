// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.ComponentModel;
using System.Globalization;
using Moq;
using NUnit.Framework;

namespace Remotion.UnitTests.Utilities;

using DateOnlyConverter = Remotion.Utilities.DateOnlyConverter;

[TestFixture]
public class DateOnlyConverterTest
{
  [Test]
  [TestCase(typeof(DateTime))]
  [TestCase(typeof(DateTime?))]
  public void CanConvertFrom_DateTimeTypes_ReturnsTrue (Type testType)
  {
    var converter = new DateOnlyConverter();

    var result = converter.CanConvertFrom(Mock.Of<ITypeDescriptorContext>(), testType);

    Assert.That(result, Is.True);
  }

  [Test]
  [TestCase(typeof(DateOnly))]
  [TestCase(typeof(DateOnly?))]
  [TestCase(typeof(string))]
  [TestCase(typeof(int))]
  [TestCase(typeof(DateOnlyConverter))]
  public void CanConvertFrom_OtherTypes_ReturnsFalse (Type testType)
  {
    var converter = new DateOnlyConverter();

    var result = converter.CanConvertFrom(Mock.Of<ITypeDescriptorContext>(), testType);

    Assert.That(result, Is.False);
  }

  [Test]
  [TestCase(typeof(DateTime))]
  [TestCase(typeof(DateTime?))]
  [TestCase(typeof(string))]
  public void CanConvertTo_DateTimeTypes_ReturnsTrue (Type testType)
  {
    var converter = new DateOnlyConverter();

    var result = converter.CanConvertTo(Mock.Of<ITypeDescriptorContext>(), testType);

    Assert.That(result, Is.True);
  }

  [Test]
  [TestCase(typeof(DateOnly))]
  [TestCase(typeof(DateOnly?))]
  [TestCase(typeof(int))]
  [TestCase(typeof(DateOnlyConverter))]
  public void CanConvertTo_OtherTypes_ReturnsFalse (Type testType)
  {
    var converter = new DateOnlyConverter();

    var result = converter.CanConvertTo(Mock.Of<ITypeDescriptorContext>(), testType);

    Assert.That(result, Is.False);
  }

  [Test]
  public void ConvertFrom_DateTime_ReturnsDateOnly ()
  {
    var converter = new DateOnlyConverter();

    var dateTime = new DateTime(1, 3, 4);
    var result = converter.ConvertFrom(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, dateTime);

    Assert.That(result, Is.InstanceOf<DateOnly>());
    Assert.That(result, Is.EqualTo(new DateOnly(1, 3, 4)));
  }

  [Test]
  public void ConvertFrom_NullableDateTime_WithValue_ReturnsDateOnly ()
  {
    var converter = new DateOnlyConverter();

    DateTime? dateTime = new DateTime(1, 3, 4);
    var result = converter.ConvertFrom(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, dateTime);

    Assert.That(result, Is.InstanceOf<DateOnly>());
    Assert.That(result, Is.EqualTo(new DateOnly(1, 3, 4)));
  }

  [Test]
  public void ConvertFrom_NullableDateTime_WithNull_ReturnsNull ()
  {
    var converter = new DateOnlyConverter();

    DateTime? dateTime = null;
    var result = converter.ConvertFrom(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, dateTime);

    Assert.That(result, Is.EqualTo(null));
  }

  [Test]
  public void ConvertFrom_WithUnsupportedInputValue_ThrowsNotSupportedException ()
  {
    var converter = new DateOnlyConverter();

    Assert.That(
        () => converter.ConvertFrom(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, "NotDateOnly"),
        Throws.InstanceOf<NotSupportedException>()
            .With.Message.EqualTo($"DateOnlyConverter cannot convert from {typeof(string)}."));
  }

  [Test]
  public void ConvertTo_WithNullableDateOnlyInput_AndDateTimeDestinationType_ReturnsDateTime ()
  {
    var converter = new DateOnlyConverter();

    DateOnly? dateOnly = new DateOnly(1, 3, 4);
    var result = converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, dateOnly, typeof(DateTime));

    Assert.That(result, Is.InstanceOf<DateTime>());
    Assert.That(result, Is.EqualTo(new DateTime(1, 3, 4)));
  }

  [Test]
  public void ConvertTo_WithDateOnlyInput_AndDateTimeDestinationType_ReturnsDateTime ()
  {
    var converter = new DateOnlyConverter();

    var dateOnly = new DateOnly(1, 3, 4);
    var result = converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, dateOnly, typeof(DateTime));

    Assert.That(result, Is.InstanceOf<DateTime>());
    Assert.That(result, Is.EqualTo(new DateTime(1, 3, 4)));
  }

  [Test]
  public void ConvertTo_WithNullInput_AndDateTimeDestinationType_ReturnsNull ()
  {
    var converter = new DateOnlyConverter();

    var result = converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, null, typeof(DateTime));

    Assert.That(result, Is.Null);
  }

  [Test]
  public void ConvertTo_WithDateOnlyInput_AndNullableDateTimeDestinationType_ReturnsNullableDateTime ()
  {
    var converter = new DateOnlyConverter();

    var result = converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, new DateOnly(1, 3, 4), typeof(DateTime?));

    Assert.That(result, Is.InstanceOf<DateTime?>());
    Assert.That(result, Is.EqualTo(new DateTime(1, 3, 4)));
  }

  [Test]
  public void ConvertTo_WithNullableDateOnlyInput_AndNullableDateTimeDestinationType_ReturnsNullableDateTime ()
  {
    var converter = new DateOnlyConverter();

    DateOnly? dateOnly = new DateOnly(1, 3, 4);
    var result = converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, dateOnly, typeof(DateTime?));

    Assert.That(result, Is.InstanceOf<DateTime?>());
    Assert.That(result, Is.EqualTo(new DateTime(1, 3, 4)));
  }

  [Test]
  public void ConvertTo_WithNullInput_AndNullableDateTimeDestinationType_ReturnsNull ()
  {
    var converter = new DateOnlyConverter();

    var result = converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, null, typeof(DateTime?));

    Assert.That(result, Is.EqualTo(null));
  }

  [Test]
  public void ConvertTo_WithStringDestinationType_ThrowsNotSupportedException ()
  {
    var converter = new DateOnlyConverter();

    Assert.That(
        () => converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, new DateOnly(1, 3, 4), typeof(string)),
        Throws.InstanceOf<NotSupportedException>()
            .With.Message.EqualTo($"Cannot convert value to type '{typeof(string)}'. This converter only supports converting to '{typeof(DateTime)}'."));
  }

  [Test]
  public void ConvertTo_WithStringInput_ThrowsNotSupportedException ()
  {
    var converter = new DateOnlyConverter();

    Assert.That(
        () => converter.ConvertTo(Mock.Of<ITypeDescriptorContext>(), CultureInfo.CurrentCulture, "NotDateOnly", typeof(DateTime)),
        Throws.InstanceOf<NotSupportedException>()
            .With.Message.EqualTo($"'DateOnlyConverter' is unable to convert '{typeof(string)}' to '{typeof(DateTime)}'."));
  }
}
