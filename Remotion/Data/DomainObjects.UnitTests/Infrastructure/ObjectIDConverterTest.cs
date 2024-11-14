// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.ComponentModel.Design.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure;

[TestFixture]
public class ObjectIDConverterTest : StandardMappingTest
{
  [Test]
  public void CanConvertFrom_String ()
  {
    var converter = new ObjectIDConverter();

    var result = converter.CanConvertFrom(null, typeof(string));

    Assert.That(result, Is.True);
  }

  [Test]
  public void CanConvertFrom_InstanceDescriptor ()
  {
    var converter = new ObjectIDConverter();

    var result = converter.CanConvertFrom(null, typeof(InstanceDescriptor));

    Assert.That(result, Is.True);
  }

  [Test]
  [TestCase(typeof(int))]
  [TestCase(typeof(object))]
  [TestCase(typeof(Enum))]
  [TestCase(typeof(DateTime))]
  public void CanConvertFrom_OtherTypes (Type type)
  {
    var converter = new ObjectIDConverter();

    var result = converter.CanConvertFrom(null, type);

    Assert.That(result, Is.False);
  }

  [Test]
  public void CanConvertTo_String ()
  {
    var converter = new ObjectIDConverter();

    var result = converter.CanConvertTo(null, typeof(string));

    Assert.That(result, Is.True);
  }

  [Test]
  public void CanConvertTo_InstanceDescriptor ()
  {
    var converter = new ObjectIDConverter();

    var result = converter.CanConvertTo(null, typeof(InstanceDescriptor));

    Assert.That(result, Is.False);
  }

  [Test]
  [TestCase(typeof(int))]
  [TestCase(typeof(object))]
  [TestCase(typeof(Enum))]
  [TestCase(typeof(DateTime))]
  public void CanConvertTo_OtherTypes (Type type)
  {
    var converter = new ObjectIDConverter();

    var result = converter.CanConvertTo(null, type);

    Assert.That(result, Is.False);
  }

  [Test]
  public void CanConvertTo_Null_IsFalse ()
  {
    var converter = new ObjectIDConverter();

    var result = converter.CanConvertTo(null, null);

    Assert.That(result, Is.False);
  }

  [Test]
  public void ConvertFrom_String ()
  {
    var objectID = DomainObjectIDs.Ceo1;

    var idAsString = objectID.ToString();

    var converter = new ObjectIDConverter();

    var result = converter.ConvertFrom(null, null, idAsString);

    Assert.That(result, Is.InstanceOf<ObjectID>());
    Assert.That(result!.ToString(), Is.EqualTo(idAsString));
  }

  [Test]
  public void ConvertFrom_WithInvalidType_ThrowsNotSupportedException ()
  {
    var input = 32;

    var converter = new ObjectIDConverter();

    Assert.That(() => converter.ConvertFrom(null, null, input),
        Throws.InstanceOf<NotSupportedException>()
            .With.Message.EqualTo("ObjectIDConverter cannot convert from System.Int32."));
  }

  [Test]
  public void ConvertFrom_WithInvalidStringSupplied_ThrowsInvalidOperationException ()
  {
    var input = "NotAnObjectID";

    var converter = new ObjectIDConverter();

    Assert.That(() => converter.ConvertFrom(null, null, input),
        Throws.InstanceOf<FormatException>()
            .With.Message.EqualTo("Serialized ObjectID 'NotAnObjectID' is not correctly formatted: it must have three parts."));
  }

  [Test]
  public void ConvertFrom_Null_ReturnsNull ()
  {
    var input = (string)null;

    var converter = new ObjectIDConverter();

    var result = converter.ConvertFrom(null, null, input);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void ConvertTo ()
  {
    var objectID = DomainObjectIDs.Ceo1;

    var converter = new ObjectIDConverter();

    var result = converter.ConvertTo(null, null, objectID, typeof(string));

    Assert.That(result, Is.InstanceOf<string>());
    Assert.That(result, Is.EqualTo(objectID.ToString()));
  }

  [Test]
  public void ConvertTo_WithInvalidType_ThrowsNotSupportedException ()
  {
    var input = DomainObjectIDs.Ceo1;

    var converter = new ObjectIDConverter();

    Assert.That(() => converter.ConvertTo(null, null, input, typeof(int)),
        Throws.InstanceOf<NotSupportedException>()
            .With.Message.EqualTo("'ObjectIDConverter' is unable to convert 'Remotion.Data.DomainObjects.ObjectID' to 'System.Int32'."));
  }

  [Test]
  public void ConvertTo_Null_ReturnsNull ()
  {
    var input = (string)null;

    var converter = new ObjectIDConverter();

    var result = converter.ConvertTo(null, null, input, typeof(string));

    Assert.That(result, Is.Null);
  }
}
