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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueImplementation.Rendering
{
  public abstract class BocTextValueRendererTestBase<T> : RendererTestBase
      where T: class, IBocTextValueBase
  {
    protected const string c_firstLineText = "This is my test text.";
    protected const string c_secondLineText = "with two lines now.";
    private const string c_cssClass = "SomeClass";
    private readonly Unit _height = new Unit (17, UnitType.Point);
    private readonly Unit _width = new Unit (123, UnitType.Point);
    protected T TextValue { get; set; }
    
    protected Unit Height
    {
      get { return _height; }
    }

    protected Unit Width
    {
      get { return _width; }
    }

    protected void CheckCssClass (BocTextValueRendererBase<T> renderer, XmlNode span, bool withCssClass, bool inStandardProperties)
    {
      string cssClass = renderer.GetCssClassBase(TextValue);
      if (withCssClass)
      {
        if (inStandardProperties)
          cssClass = TextValue.Attributes["class"];
        else
          cssClass = TextValue.CssClass;
      }
      Html.AssertAttribute (span, "class", cssClass, HtmlHelper.AttributeValueCompareMode.Contains);
    }

    protected virtual void SetStyle (bool withStyle, bool withCssClass, bool inStyleProperty, bool autoPostBack)
    {
      StateBag stateBag = new StateBag();
      TextValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      TextValue.Stub (mock => mock.Style).Return (TextValue.Attributes.CssStyle);
      TextValue.Stub (mock => mock.TextBoxStyle).Return (new TextBoxStyle());
      TextValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      TextValue.TextBoxStyle.AutoPostBack = autoPostBack;

      if (withCssClass)
      {
        if (inStyleProperty)
          TextValue.Attributes["class"] = c_cssClass;
        else
          TextValue.CssClass = c_cssClass;
      }

      if (withStyle)
      {
        if (inStyleProperty)
        {
          TextValue.Style["height"] = Height.ToString();
          TextValue.Style["width"] = Width.ToString();
        }
        else
        {
          TextValue.Stub (mock => mock.Height).Return (Height);
          TextValue.Stub (mock => mock.Width).Return (Width);
          TextValue.ControlStyle.Height = TextValue.Height;
          TextValue.ControlStyle.Width = TextValue.Width;
        }
      }
    }
  }
}