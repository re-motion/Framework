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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  public class StubColumnRenderer : IBocColumnRenderer
  {
    public StubColumnRenderer (IResourceUrlFactory resourceUrlFactory)
    {
    }

    public bool IsNull => false;

    void IBocColumnRenderer.RenderTitleCell (BocColumnRenderingContext renderingContext, in BocTitleCellRenderArguments arguments)
    {
      renderingContext.Writer.AddAttribute("arguments-CellID", arguments.CellID);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Th);
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderDataCell (BocColumnRenderingContext renderingContext, in BocDataCellRenderArguments arguments)
    {
      renderingContext.Writer.AddAttribute("arguments-CellID", arguments.CellID ?? "null");
      renderingContext.Writer.AddAttribute("arguments-HeaderIDs", arguments.HeaderIDs.Any() ? string.Join(", ", arguments.HeaderIDs) : "empty");
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderDataColumnDeclaration (BocColumnRenderingContext renderingContext, bool isTextXml)
    {
      renderingContext.Writer.WriteBeginTag("col");
      renderingContext.Writer.Write(" />");
    }
  }
}
