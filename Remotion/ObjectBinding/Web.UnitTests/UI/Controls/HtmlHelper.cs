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
using System.Runtime.CompilerServices;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  public class HtmlHelper : HtmlHelperBase
  {
    public HtmlHelper ()
        : base(
            (expected, actual, message, args) => Assert.That(actual, Is.EqualTo(expected), FormattableStringFactory.Create(message, args)),
            (left, right, message, args) => Assert.That(left, Is.GreaterThan(right), FormattableStringFactory.Create(message, args)),
            (actual, message, args) => Assert.That(actual, Is.Not.Null, FormattableStringFactory.Create(message, args)),
            (actual, message, args) => Assert.That(actual, Is.Null, FormattableStringFactory.Create(message, args)),
            (actual, message, args) => Assert.That(actual, Is.True, FormattableStringFactory.Create(message, args))
            )
    {
    }

    public void AssertIcon (XmlNode parentNode, IBusinessObject businessObject, string imageSourcePart)
    {
      XmlNode img = GetAssertedChildElement(parentNode, "img", 0);
      if (imageSourcePart == null)
      {
        string businessObjectClass = businessObject.BusinessObjectClass.Identifier;
        imageSourcePart = businessObjectClass.Substring(0, businessObjectClass.IndexOf(", "));
      }
      AssertAttribute(img, "src", imageSourcePart, AttributeValueCompareMode.Contains);
      AssertStyleAttribute(img, "width", "16px");
      AssertStyleAttribute(img, "height", "16px");
      AssertAttribute(img, "alt", "");
    }
  }
}
