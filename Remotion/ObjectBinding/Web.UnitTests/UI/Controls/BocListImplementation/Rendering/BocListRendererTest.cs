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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListRendererTest : BocListRendererTestBase
  {
    private static readonly Unit s_menuBlockWidth = new Unit (123, UnitType.Pixel);
    private static readonly Unit s_menuBlockOffset = new Unit (12, UnitType.Pixel);
    private BocListCssClassDefinition _bocListCssClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _bocListCssClassDefinition = new BocListCssClassDefinition();
    }

    [Test]
    public void RenderOnlyTableBlock ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (false);

      var renderer = new BocListRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new StubRenderer ("table"),
          new StubRenderer ("navigation"),
          new StubRenderer ("menu"));
      renderer.Render (new BocListRenderingContext(HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var tableBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (tableBlock, "class", "bocListTableBlock");
      Html.AssertChildElementCount (tableBlock, 1);
      Html.GetAssertedChildElement (tableBlock, "table", 0);
    }

    [Test]
    public void RenderWithMenuBlock ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (true);
      List.Stub (mock => mock.MenuBlockWidth).Return (s_menuBlockWidth);
      List.Stub (mock => mock.MenuBlockOffset).Return (s_menuBlockOffset);

      var renderer = new BocListRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new StubRenderer ("table"),
          new StubRenderer ("navigation"),
          new StubRenderer ("menu"));
      renderer.Render (new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var menuBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (menuBlock, "class", _bocListCssClassDefinition.MenuBlock);
      Html.AssertStyleAttribute (menuBlock, "width", s_menuBlockWidth.ToString());
      var menuBlockContent = Html.GetAssertedChildElement (menuBlock, "div", 0);
      Html.AssertStyleAttribute (menuBlockContent, "margin-left", s_menuBlockOffset.ToString());
      Html.GetAssertedChildElement (menuBlockContent, "menu", 0);


      var tableBlock = Html.GetAssertedChildElement (div, "div", 1);
      Html.AssertAttribute (tableBlock, "class", "bocListTableBlock hasMenuBlock");

      Html.AssertStyleAttribute (tableBlock, "right", s_menuBlockWidth.ToString ());

      Html.GetAssertedChildElement (tableBlock, "table", 0);
    }

    [Test]
    public void RenderWithMenuBlockWithoutWidth ()
    {
      List.Stub (mock => mock.HasMenuBlock).Return (true);

      var renderer = new BocListRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new StubRenderer ("table"),
          new StubRenderer ("navigation"),
          new StubRenderer ("menu"));
      renderer.Render (new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var menuBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (menuBlock, "class", _bocListCssClassDefinition.MenuBlock);
      var menuBlockContent = Html.GetAssertedChildElement (menuBlock, "div", 0);
      Html.GetAssertedChildElement (menuBlockContent, "menu", 0);


      var tableBlock = Html.GetAssertedChildElement (div, "div", 1);
      Html.AssertAttribute (tableBlock, "class", "bocListTableBlock hasMenuBlock");

      Html.GetAssertedChildElement (tableBlock, "table", 0);
    }

    [Test]
    public void RenderWithNavigationBlock ()
    {
      List.Stub (mock => mock.HasNavigator).Return (true);

      var renderer = new BocListRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new StubRenderer ("table"),
          new StubRenderer ("navigation"),
          new StubRenderer ("menu"));
      renderer.Render (new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "id", "MyList");

      var tableBlock = Html.GetAssertedChildElement (div, "div", 0);
      Html.AssertAttribute (tableBlock, "class", "bocListTableBlock hasNavigator");
      Html.AssertChildElementCount (tableBlock, 2);
      Html.GetAssertedChildElement (tableBlock, "table", 0);
      Html.GetAssertedChildElement (tableBlock, "navigation", 1);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      var renderer = new BocListRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new StubRenderer ("table"),
          new StubRenderer ("navigation"),
          new StubRenderer ("menu"));
      renderer.Render (new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, DiagnosticMetadataAttributes.ControlType, "BocList");
    }
  }
}