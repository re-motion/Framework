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
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.WebButtonImplementation;
using Remotion.Web.UI.Controls.WebButtonImplementation.Rendering;

namespace Remotion.Web.Legacy.UI.Controls.Rendering
{
  /// <summary>
  /// Implements <see cref="IWebButtonRenderer"/> for quirks mode rendering of <see cref="WebButton"/> controls.
  /// <seealso cref="IWebButton"/>
  /// </summary>
  public class WebButtonQuirksModeRenderer : QuirksModeRendererBase<IWebButton>, IWebButtonRenderer
  {
    public WebButtonQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    { 
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptKey = typeof (WebButtonQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptKey))
      {
        var scripUrl = ResourceUrlFactory.CreateResourceUrl (typeof (WebButtonQuirksModeRenderer), ResourceType.Html, "WebButton.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptKey, scripUrl);
      }

      string styleKey = typeof (WebButtonQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleKey))
      {
        var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (WebButtonQuirksModeRenderer), ResourceType.Html, "WebButton.css");
        htmlHeadAppender.RegisterStylesheetLink (styleKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public void Render (WebButtonRenderingContext renderingContext)
    {
      throw new NotSupportedException ("The WebButton does not support customized rendering.");
    }
  }
}