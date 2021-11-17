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
using NUnit.Framework;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class BadgeConverterTest
  {
    [Test]
    public void ConvertTo_CorrectlyEncodesWebStrings ()
    {
      var badgeConverter = new BadgeConverter();

      var value = WebString.CreateFromText("a");
      var description = WebString.CreateFromHtml("b");
      var badge = new Badge(value, description);

      var stringRepresentation = (string)badgeConverter.ConvertTo(badge, typeof(string));
      Assert.That(stringRepresentation, Is.EqualTo("a\0(html)b"));
    }

    [Test]
    public void ConvertFrom_CorrectlyDecodesWebStrings ()
    {
      var badgeConverter = new BadgeConverter();

      var stringRepresentation = "(text)a\0(html)b";
      var deserializedBadge = (Badge)badgeConverter.ConvertFrom(stringRepresentation);

      Assert.That(deserializedBadge, Is.Not.Null);
      Assert.That(deserializedBadge.Value, Is.EqualTo(WebString.CreateFromText("a")));
      Assert.That(deserializedBadge.Description, Is.EqualTo(WebString.CreateFromHtml("b")));
    }
  }
}
