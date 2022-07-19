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
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering
{
  /// <summary>
  /// Provides a label for rendering a <see cref="BocMultilineTextValue"/> control in read-only mode. 
  /// Rendering is done by the parent class.
  /// </summary>
  [ImplementationFor(typeof(IBocMultilineTextValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocMultilineTextValueRenderer : BocTextValueRendererBase<IBocMultilineTextValue>, IBocMultilineTextValueRenderer
  {
    public BocMultilineTextValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer)
        : base(resourceUrlFactory, globalizationService, renderingFeatures, labelReferenceRenderer, validationErrorRenderer)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, TextBoxStyle textBoxStyle)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      textBoxStyle.RegisterJavaScriptInclude(ResourceUrlFactory, htmlHeadAppender);

      htmlHeadAppender.RegisterCommonStyleSheet();

      string key = typeof(BocMultilineTextValueRenderer).GetFullNameChecked() + "_Style";
      var url = ResourceUrlFactory.CreateThemedResourceUrl(typeof(BocMultilineTextValueRenderer), ResourceType.Html, "BocMultilineTextValue.css");
      htmlHeadAppender.RegisterStylesheetLink(key, url, HtmlHeadAppender.Priority.Library);
    }

    public void Render (BocMultilineTextValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      base.Render(renderingContext);
    }

    protected override string GetText (BocRenderingContext<IBocMultilineTextValue> renderingContext)
    {
      string[]? lines = renderingContext.Control.Value;
      string text = RenderUtility.JoinLinesWithEncoding(lines ?? Enumerable.Empty<string>());
      return text;
    }

    public override string GetCssClassBase (IBocMultilineTextValue control)
    {
      return "bocMultilineTextValue";
    }
  }
}
