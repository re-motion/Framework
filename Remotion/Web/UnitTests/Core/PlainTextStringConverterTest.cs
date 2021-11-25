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
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Web.UnitTests.Core
{
  [TestFixture]
  public class PlainTextStringConverterTest
  {
    [Test]
    [TestCaseSource(nameof(TestCaseSource_ConvertFromString))]
    public void ConvertFromString (string input, PlainTextString? expectedOutput)
    {
      var converter = new PlainTextStringConverter();

      var canConvert = converter.CanConvertFrom(typeof(string));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertFromString(input);
      Assert.That(result, Is.EqualTo(expectedOutput));
    }

    private static object[] TestCaseSource_ConvertFromString =
    {
        new object[] { null, null },
        new object[] { string.Empty, PlainTextString.CreateFromText(string.Empty) },
        new object[] { "test", PlainTextString.CreateFromText("test") },
        new object[] { "aoe   \" & ' < > é \r \n \r\n", PlainTextString.CreateFromText("aoe   \" & ' < > é \r \n \r\n") }
    };

    [Test]
    [TestCaseSource(nameof(TestCaseSource_ConvertToString))]
    public void ConvertToString (PlainTextString? input, string expectedOutput)
    {
      var converter = new PlainTextStringConverter();

      var canConvert = converter.CanConvertTo(typeof(string));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertToString(input);
      Assert.That(result, Is.EqualTo(expectedOutput));
    }

    private static object[] TestCaseSource_ConvertToString =
    {
        new object[] { null, null },
        new object[] { PlainTextString.CreateFromText(string.Empty), string.Empty },
        new object[] { PlainTextString.CreateFromText("test"), "test" },
        new object[] { PlainTextString.CreateFromText("aoe   \" & ' < > é \r \n \r\n"), "aoe   \" & ' < > é \r \n \r\n" }
    };

    [Test]
    public void ConvertToInstanceDescriptor ()
    {
      var converter = new PlainTextStringConverter();
      var input = PlainTextString.CreateFromText("test");

      var canConvert = converter.CanConvertTo(typeof(InstanceDescriptor));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertTo(input, typeof(InstanceDescriptor));
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.InstanceOf<InstanceDescriptor>());

      var instanceDescriptorResult = (InstanceDescriptor)result;
      Assert.That(
          instanceDescriptorResult.MemberInfo,
          Is.InstanceOf<MethodInfo>()
              .And.Property(nameof(MethodInfo.IsPublic)).True);

      var instance = (PlainTextString)instanceDescriptorResult.Invoke();
      Assert.That(instance.GetValue(), Is.EqualTo("test"));
    }

    [Test]
    public void ConvertToInstanceDescriptor_WithNull_ReturnsNull ()
    {
      var converter = new PlainTextStringConverter();

      var canConvert = converter.CanConvertTo(typeof(InstanceDescriptor));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertTo(null!, typeof(InstanceDescriptor));
      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertFromInstanceDescriptor ()
    {
      var converter = new PlainTextStringConverter();
      var input = new InstanceDescriptor(
          MemberInfoFromExpressionUtility.GetMethod(() => PlainTextString.CreateFromText(null)),
          new[] { "test" });

      var canConvert = converter.CanConvertFrom(typeof(InstanceDescriptor));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertFrom(input);
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.EqualTo(PlainTextString.CreateFromText("test")));
    }

    [Test]
    public void ConvertFromInstanceDescriptor_WithNull_ReturnsNull ()
    {
      var converter = new PlainTextStringConverter();
      var input = (InstanceDescriptor)null;

      var canConvert = converter.CanConvertFrom(typeof(InstanceDescriptor));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertFrom(input);
      Assert.That(result, Is.Null);
    }
  }
}
