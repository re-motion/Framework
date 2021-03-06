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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocCustomColumnRendererTest : ColumnRendererTestBase<BocCustomColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocCustomColumnDefinition> _renderingContext;

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocCustomColumnDefinition();
      Column.CustomCell = new StubCustomCellDefinition();

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      base.SetUp();

      IBusinessObject firstObject = (IBusinessObject) ((TypeWithReference) BusinessObject).FirstValue;
      IBusinessObject secondObject = (IBusinessObject) ((TypeWithReference) BusinessObject).SecondValue;
      var triplets = new[]
                     {
                         new BocListCustomColumnTuple (firstObject, 10, new WebControl (HtmlTextWriterTag.Div)),
                         new BocListCustomColumnTuple (secondObject, 20, new HtmlGenericControl ("div"))
                     };
      var customColumns =
          new ReadOnlyDictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]> (
              new Dictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]>
              {
                  { Column, triplets }
              });
      List.Stub (mock => mock.CustomColumns).Return (customColumns);

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create (null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocCustomColumnDefinition> (
              new BocColumnRenderingContext (HttpContext, Html.Writer, List, businessObjectWebServiceContext, Column, 0, 0));
    }

    [Test]
    public void RenderCellWithInnerWebControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;

      IBocColumnRenderer renderer = new BocCustomColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      var args = new BocListDataRowRenderEventArgs (10, EventArgs.BusinessObject, EventArgs.IsEditableRow, EventArgs.IsOddRow);
      renderer.RenderDataCell (_renderingContext, 0, false, args);

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute (td, "role", "cell");

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "onclick", "BocList.OnCommandClick();");
    }

    [Test]
    public void RenderCellWithInnerHtmlControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;

      IBocColumnRenderer renderer = new BocCustomColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      var args = new BocListDataRowRenderEventArgs (20, EventArgs.BusinessObject, EventArgs.IsEditableRow, EventArgs.IsOddRow);
      renderer.RenderDataCell (_renderingContext, 0, false,args);

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute (td, "role", "cell");

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "onclick", "BocList.OnCommandClick();");
    }

    [Test]
    public void RenderCellDirectly ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.NoControls;

      IBocColumnRenderer renderer = new BocCustomColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute (td, "role", "cell");
    }
  }
}