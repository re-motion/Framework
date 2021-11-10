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
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Web.Globalization;
using Remotion.Web.UnitTests.Core.Globalization.TestDomain;

namespace Remotion.Web.UnitTests.Core.Globalization
{
  public class ResourceManagerExtensionsTest
  {
    private const string c_fakeResourceID = "fake";
    private Mock<IResourceManager> _resourceManagerMock;

    [SetUp]
    public void SetUp ()
    {
      _resourceManagerMock = new Mock<IResourceManager>();
    }

    [Test]
    public void GetWebString_WithExistingResourceAndWebStringTypeEncoded_ReturnsWebString ()
    {
      const string resourceID = "resourceID";
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (resourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebString (resourceID, WebStringType.Encoded);

      _resourceManagerMock.Verify();
      Assert.That (result.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Type, Is.EqualTo (WebStringType.Encoded));
    }

    [Test]
    public void GetWebString_WithExistingResourceAndWebStringTypePlaintext_ReturnsWebString ()
    {
      const string resourceID = "resourceID";
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (resourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebString (resourceID, WebStringType.PlainText);

      _resourceManagerMock.Verify();
      Assert.That (result.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Type, Is.EqualTo (WebStringType.PlainText));
    }

    [Test]
    public void GetWebString_WithNonExistingResourceAndWebStringTypeEncoded_ReturnsWebStringWithResourceIDAsValue ()
    {
      var result = _resourceManagerMock.Object.GetWebString (c_fakeResourceID, WebStringType.Encoded);

      Assert.That (result.GetValue(), Is.EqualTo ("fake"));
      Assert.That (result.Type, Is.EqualTo (WebStringType.Encoded));
    }

    [Test]
    public void GetWebString_WithNonExistingResourceAndWebStringTypePlainText_ReturnsWebStringWithResourceIDAsValue ()
    {
      var result = _resourceManagerMock.Object.GetWebString (c_fakeResourceID, WebStringType.PlainText);

      Assert.That (result.GetValue(), Is.EqualTo ("fake"));
      Assert.That (result.Type, Is.EqualTo (WebStringType.PlainText));
    }

    [Test]
    public void GetWebStringOrDefault_WithExistingResourceAndWebStringTypeEncoded_ReturnsWebString ()
    {
      const string resourceID = "resourceID";
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (resourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebStringOrDefault (resourceID, WebStringType.Encoded);

      _resourceManagerMock.Verify();
      Assert.That (result.HasValue, Is.True);
      Assert.That (result.Value.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Value.Type, Is.EqualTo (WebStringType.Encoded));
    }

    [Test]
    public void GetWebStringOrDefault_WithExistingResourceAndWebStringTypePlaintext_ReturnsWebString ()
    {
      const string resourceID = "resourceID";
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (resourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebStringOrDefault (resourceID, WebStringType.PlainText);

      _resourceManagerMock.Verify();
      Assert.That (result.HasValue, Is.True);
      Assert.That (result.Value.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Value.Type, Is.EqualTo (WebStringType.PlainText));
    }

    [Test]
    public void GetWebStringOrDefault_WithNonExistingResource_ReturnsNull ()
    {
      var result = _resourceManagerMock.Object.GetWebStringOrDefault (c_fakeResourceID, WebStringType.PlainText);

      Assert.That (result.HasValue, Is.False);
    }

    [Test]
    public void GetWebString_Enum_WithExistingResourceAndWebStringTypeEncoded_ReturnsWebString ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (enumResourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebString (enumValue, WebStringType.Encoded);

      _resourceManagerMock.Verify();
      Assert.That (result.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Type, Is.EqualTo (WebStringType.Encoded));
    }

    [Test]
    public void GetWebString_Enum_WithExistingResourceAndWebStringTypePlaintext_ReturnsWebString ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (enumResourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebString (enumValue, WebStringType.PlainText);

      _resourceManagerMock.Verify();
      Assert.That (result.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Type, Is.EqualTo (WebStringType.PlainText));
    }

    [Test]
    public void GetWebString_Enum_WithNonExistingResourceAndWebStringTypePlaintext_ReturnsWebStringWithResourceIDAsValue ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue);

      var result = _resourceManagerMock.Object.GetWebString (enumValue, WebStringType.PlainText);

      Assert.That (result.GetValue(), Is.EqualTo (enumResourceID));
      Assert.That (result.Type, Is.EqualTo (WebStringType.PlainText));
    }

    [Test]
    public void GetWebString_Enum_WithNonExistingResourceAndWebStringTypeEncoded_ReturnsWebStringWithResourceIDAsValue ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue);

      var result = _resourceManagerMock.Object.GetWebString (enumValue, WebStringType.Encoded);

      Assert.That (result.GetValue(), Is.EqualTo (enumResourceID));
      Assert.That (result.Type, Is.EqualTo (WebStringType.Encoded));
    }

    [Test]
    public void GetWebStringOrDefault_Enum_WithExistingResourceAndWebStringTypeEncoded_ReturnsWebString ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (enumResourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebStringOrDefault (enumValue, WebStringType.Encoded);

      _resourceManagerMock.Verify();
      Assert.That (result.HasValue, Is.True);
      Assert.That (result.Value.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Value.Type, Is.EqualTo (WebStringType.Encoded));
    }

    [Test]
    public void GetWebStringOrDefault_Enum_WithExistingResourceAndWebStringTypePlaintext_ReturnsWebString ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;
      var enumResourceID = ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue);
      var outValue = "Test";

      _resourceManagerMock
          .Setup (mock => mock.TryGetString (enumResourceID, out outValue))
          .Returns (true)
          .Verifiable();

      var result = _resourceManagerMock.Object.GetWebStringOrDefault (enumValue, WebStringType.PlainText);

      _resourceManagerMock.Verify();
      Assert.That (result.HasValue, Is.True);
      Assert.That (result.Value.GetValue(), Is.EqualTo ("Test"));
      Assert.That (result.Value.Type, Is.EqualTo (WebStringType.PlainText));
    }

    [Test]
    public void GetWebStringOrDefault_Enum_WithNonExistingResource_ReturnsNull ()
    {
      var enumValue = EnumWithMultiLingualNameAttribute.ValueWithLocalizedName;

      var result = _resourceManagerMock.Object.GetWebStringOrDefault (enumValue, WebStringType.Encoded);

      Assert.That (result.HasValue, Is.False);
    }
  }
}