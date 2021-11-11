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
  public class WebStringConverterTest
  {
    [Test]
    [TestCaseSource(nameof(TestCaseSource_ConvertFromString))]
    public void ConvertFromString (string input, WebString? expectedOutput)
    {
      var converter = new WebStringConverter();

      var canConvert = converter.CanConvertFrom(typeof(string));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertFromString(input);
      Assert.That(result, Is.EqualTo(expectedOutput));
    }

    private static object[] TestCaseSource_ConvertFromString =
    {
        new object[] { null, null },
        new object[] { string.Empty, WebString.CreateFromText(string.Empty) },
        new object[] { "test", WebString.CreateFromText("test") },
        new object[] { "(text)test", WebString.CreateFromText("test") },
        new object[] { "(text)(html)test", WebString.CreateFromText("(html)test") },
        new object[] { "(html)test", WebString.CreateFromHtml("test") },
    };

    [Test]
    [TestCaseSource(nameof(TestCaseSource_ConvertToString))]
    public void ConvertToString (WebString? input, string expectedOutput)
    {
      var converter = new WebStringConverter();

      var canConvert = converter.CanConvertTo(typeof(string));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertToString(input);
      Assert.That(result, Is.EqualTo(expectedOutput));
    }

    private static object[] TestCaseSource_ConvertToString =
    {
        new object[] { null, null },
        new object[] { WebString.CreateFromText(string.Empty), string.Empty },
        new object[] { WebString.CreateFromText("test"), "test" },
        new object[] { WebString.CreateFromText("(html)test"), "(text)(html)test" },
        new object[] { WebString.CreateFromText("(text)test"), "(text)(text)test" },
        new object[] { WebString.CreateFromText("test"), "test" },
        new object[] { WebString.CreateFromHtml("test"), "(html)test" },
    };

    [Test]
    public void ConvertToInstanceDescriptor_WithPlainTextWebString ()
    {
      var converter = new WebStringConverter();
      var input = WebString.CreateFromText("test");

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

      var instance = (WebString)instanceDescriptorResult.Invoke();
      Assert.That(instance.Type, Is.EqualTo(WebStringType.PlainText));
      Assert.That(instance.GetValue(), Is.EqualTo("test"));
    }

    [Test]
    public void ConvertToInstanceDescriptor_WithEncodedWebString ()
    {
      var converter = new WebStringConverter();
      var input = WebString.CreateFromHtml("test");

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

      var instance = (WebString)instanceDescriptorResult.Invoke();
      Assert.That(instance.Type, Is.EqualTo(WebStringType.Encoded));
      Assert.That(instance.GetValue(), Is.EqualTo("test"));
    }

    [Test]
    public void ConvertToInstanceDescriptor_WithNull_ReturnsNull ()
    {
      var converter = new WebStringConverter();
      var input = (WebString?)null;

      var canConvert = converter.CanConvertTo(typeof(InstanceDescriptor));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertTo(input, typeof(InstanceDescriptor));
      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertFromInstanceDescriptor ()
    {
      var converter = new WebStringConverter();
      var input = new InstanceDescriptor(
          MemberInfoFromExpressionUtility.GetMethod(() => WebString.CreateFromText(null)),
          new[] { "test" });

      var canConvert = converter.CanConvertFrom(typeof(InstanceDescriptor));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertFrom(input);
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.EqualTo(WebString.CreateFromText("test")));
    }

    [Test]
    public void ConvertFromInstanceDescriptor_WithNull_ReturnsNull ()
    {
      var converter = new WebStringConverter();
      var input = (InstanceDescriptor)null;

      var canConvert = converter.CanConvertFrom(typeof(InstanceDescriptor));
      Assert.That(canConvert, Is.True);

      var result = converter.ConvertFrom(input);
      Assert.That(result, Is.Null);
    }
  }
}
