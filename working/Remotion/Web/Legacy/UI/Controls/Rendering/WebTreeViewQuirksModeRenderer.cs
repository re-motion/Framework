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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.WebTreeViewImplementation;
using Remotion.Web.UI.Controls.WebTreeViewImplementation.Rendering;

namespace Remotion.Web.Legacy.UI.Controls.Rendering
{
  /// <summary>
  /// Implements <see cref="IWebTreeViewRenderer"/> for quirks mode rendering of <see cref="WebTreeView"/> controls.
  /// <seealso cref="IWebTreeView"/>
  /// </summary>
  public class WebTreeViewQuirksModeRenderer : QuirksModeRendererBase<IWebTreeView>, IWebTreeViewRenderer
  {
    public WebTreeViewQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    { 
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (WebTreeViewQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleKey))
      {
        var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (WebTreeViewQuirksModeRenderer), ResourceType.Html, "TreeView.css");
        htmlHeadAppender.RegisterStylesheetLink (styleKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public void Render (WebTreeViewRenderingContext renderingContext)
    {
      throw new NotSupportedException ("The WebTreeView does not support customized rendering.");
    }
  }
}