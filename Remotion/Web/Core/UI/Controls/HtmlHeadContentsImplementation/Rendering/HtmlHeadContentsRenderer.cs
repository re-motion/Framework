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
using System.Linq;
using System.Web.UI;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.HtmlHeadContentsImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="IHtmlHeadContentsRenderer"/> for standard mode rendering of <see cref="HtmlHeadContents"/> controls.
  /// <seealso cref="IHtmlHeadContents"/>
  /// </summary>
  [ImplementationFor (typeof(IHtmlHeadContentsRenderer), Lifetime = LifetimeKind.Singleton)]
  public class HtmlHeadContentsRenderer : IHtmlHeadContentsRenderer
  {
    public HtmlHeadContentsRenderer ()
    {
    }

    public void Render (HtmlHeadContentsRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      var titleTags = new List<TitleTag>();
      var javascriptIncludes = new List<JavaScriptInclude>();
      var stylesheetElements = new List<StyleSheetElement>();
      var remainingHtmlHeadElements = new List<HtmlHeadElement>();

      foreach (var element in renderingContext.HtmlHeadElements)
      {
        if (element is TitleTag)
          titleTags.Add((TitleTag)element);
        else if (element is JavaScriptInclude)
          javascriptIncludes.Add((JavaScriptInclude)element);
        else if (element is StyleSheetElement)
          stylesheetElements.Add((StyleSheetElement)element);
        else
          remainingHtmlHeadElements.Add(element);
      }

      if (titleTags.Any())
        RenderTitleTag(renderingContext.Writer, titleTags.First());
      RenderJavascriptIncludes(renderingContext.Writer, javascriptIncludes);
      RenderStylesheetElements(renderingContext.Writer, stylesheetElements);
      RenderUncategorizedHtmlHeadElements(renderingContext.Writer, remainingHtmlHeadElements);
    }

    protected virtual void RenderTitleTag (HtmlTextWriter writer, TitleTag titleTag)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("titleTag", titleTag);

      titleTag.Render(writer);
    }

    protected virtual void RenderJavascriptIncludes (HtmlTextWriter writer, IReadOnlyCollection<JavaScriptInclude> javascriptIncludes)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("javascriptIncludes", javascriptIncludes);

      foreach (var javascriptInclude in javascriptIncludes)
        javascriptInclude.Render(writer);
    }

    protected virtual void RenderStylesheetElements (HtmlTextWriter writer, IReadOnlyCollection<StyleSheetElement> stylesheetElements)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("stylesheetElements", stylesheetElements);

      foreach (var styleSheetElement in stylesheetElements)
        styleSheetElement.Render(writer);
    }

    protected virtual void RenderUncategorizedHtmlHeadElements (HtmlTextWriter writer, IReadOnlyCollection<HtmlHeadElement> htmlHeadElements)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("htmlHeadElements", htmlHeadElements);

      foreach (var htmlHeadElement in htmlHeadElements)
        htmlHeadElement.Render(writer);
    }
  }
}
