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
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UI.Controls.WebButtonImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="IWebButtonRenderer"/> for standard mode rendering of <see cref="WebButton"/> controls.
  /// <seealso cref="IWebButton"/>
  /// </summary>
  [ImplementationFor(typeof(IWebButtonRenderer), Lifetime = LifetimeKind.Singleton)]
  public class WebButtonRenderer : RendererBase<IWebButton>, IWebButtonRenderer
  {
    public WebButtonRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterWebClientScriptInclude();
      htmlHeadAppender.RegisterCommonStyleSheet();

      string styleKey = typeof(WebButtonRenderer).GetFullNameChecked() + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(WebButtonRenderer), ResourceType.Html, "WebButton.css");
      htmlHeadAppender.RegisterStylesheetLink(styleKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    public void Render (WebButtonRenderingContext renderingContext)
    {
      throw new NotSupportedException("The WebButton does not support customized rendering.");
    }
  }
}
