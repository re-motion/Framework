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
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListQuirksModeRendererTest : BocListRendererTestBase
  {
    private BocListQuirksModeCssClassDefinition _bocListQuirksModeCssClassDefinition;
    private IResourceUrlFactory _resourceUrlFactory;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.HasNavigator).Return (true);

      _bocListQuirksModeCssClassDefinition = new BocListQuirksModeCssClassDefinition();

      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [Test]
    public void RenderWithMenuBlock ()
    {
      Unit menuBlockWidth = new Unit (123, UnitType.Pixel);
      Unit menuBlockOffset = new Unit (12, UnitType.Pixel);

      List.Stub (mock => mock.HasMenuBlock).Return (true);
      List.Stub (mock => mock.MenuBlockWidth).Return (menuBlockWidth);
      List.Stub (mock => mock.MenuBlockOffset).Return (menuBlockOffset);

      XmlNode colgroup;
      RenderAndAssertTable (out colgroup);

      var colMenu = Html.GetAssertedChildElement (colgroup, "col", 1);
      Html.AssertStyleAttribute (colMenu, "width", menuBlockWidth.ToString());
      Html.AssertStyleAttribute (colMenu, "padding-left", menuBlockOffset.ToString());
    }

    [Test]
    public void RenderWithoutMenuBlock ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (false);

      XmlNode colgroup;
      RenderAndAssertTable (out colgroup);

      Html.AssertChildElementCount (colgroup, 1);
    }

    private void RenderAndAssertTable (out XmlNode colgroup)
    {
      var renderer = new BocListQuirksModeRenderer (
          _bocListQuirksModeCssClassDefinition,
          new StubQuirksModeRenderer(),
          new StubQuirksModeRenderer(),
          new StubQuirksModeRenderer(),
          _resourceUrlFactory);
      renderer.Render (new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var table = Html.GetAssertedChildElement (div, "table", 0);
      Html.AssertAttribute (table, "cellspacing", "0");
      Html.AssertAttribute (table, "cellpadding", "0");
      Html.AssertStyleAttribute (table, "width", "100%");

      colgroup = Html.GetAssertedChildElement (table, "colgroup", 0);

      var colTableAndNavigation = Html.GetAssertedChildElement (colgroup, "col", 0);

      var tr = Html.GetAssertedChildElement (table, "tr", 1);

      var td = Html.GetAssertedChildElement (tr, "td", 0);
      Html.AssertStyleAttribute (td, "vertical-align", "top");

      Html.GetAssertedChildElement (td, "div", 0);
    }
  }
}