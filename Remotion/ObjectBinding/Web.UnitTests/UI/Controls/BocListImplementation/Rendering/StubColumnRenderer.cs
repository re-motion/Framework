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
using System.Linq;
using System.Web.UI;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  public class StubColumnRenderer : BocColumnRendererBase<StubColumnDefinition>
  {
    public StubColumnRenderer (IResourceUrlFactory resourceUrlFactory)
        : base(resourceUrlFactory, Remotion.Web.UI.Controls.Rendering.RenderingFeatures.Default, new BocListCssClassDefinition(), new FakeFallbackNavigationUrlProvider())
    {
    }

    protected override void RenderTitleCell (BocColumnRenderingContext<StubColumnDefinition> renderingContext, in BocTitleCellRenderArguments arguments)
    {
      renderingContext.Writer.AddAttribute("arguments-CellID", arguments.CellID);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Th);
      renderingContext.Writer.RenderEndTag();
    }

    protected override void RenderDataCell (BocColumnRenderingContext<StubColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      renderingContext.Writer.AddAttribute("arguments-CellID", arguments.CellID ?? "null");
      renderingContext.Writer.AddAttribute("arguments-HeaderIDs", arguments.HeaderIDs.Any() ? string.Join(", ", arguments.HeaderIDs) : "empty");
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);
      renderingContext.Writer.RenderEndTag();
    }

    protected override void RenderCellContents (BocColumnRenderingContext<StubColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      throw new NotImplementedException();
    }
  }
}
