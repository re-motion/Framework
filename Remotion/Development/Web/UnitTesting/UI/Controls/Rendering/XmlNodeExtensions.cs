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
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Development.Web.UnitTesting.UI.Controls.Rendering
{
  public static class XmlNodeExtensions
  {
    private static HtmlHelperBase s_helper;

    public static HtmlHelperBase Helper
    {
      get { return Assertion.IsNotNull (s_helper, "XmlNodeExtensions.Hepler must be set during test-setup."); }
      set { s_helper = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public static void AssertChildElementCount (this XmlNode element, int childElementCount)
    {
      Helper.AssertChildElementCount (element, childElementCount);
    }

    public static void AssertAttributeValueEquals (this XmlNode element, string attributeName, string attributeValue)
    {
      Helper.AssertAttribute (element, attributeName, attributeValue, HtmlHelperBase.AttributeValueCompareMode.Equal);
    }

    public static void AssertAttributeValueContains (this XmlNode element, string attributeName, string attributeValuePart)
    {
      Helper.AssertAttribute (element, attributeName, attributeValuePart, HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    public static void AssertStyleAttribute (this XmlNode element, string styleAttributeName, string styleAttributeValue)
    {
      Helper.AssertStyleAttribute (element, styleAttributeName, styleAttributeValue);
    }

    public static void AssertNoAttribute (this XmlNode element, string attributeName)
    {
      Helper.AssertNoAttribute (element, attributeName);
    }

    public static void AssertTextNode (this XmlNode element, string content, int index)
    {
      Helper.AssertTextNode (element, content, index);
    }

    public static XmlNode GetAssertedChildElement (this XmlNode element, string tagName, int index)
    {
      return Helper.GetAssertedChildElement (element, tagName, index);
    }
  }
}