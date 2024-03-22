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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.HtmlHeadContentsImplementation;
using Remotion.Web.UI.Controls.HtmlHeadContentsImplementation.Rendering;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  ///   When added to the webform (inside the head element), the <see cref="HtmlHeadContents"/> 
  ///   control renderes the controls registered with <see cref="HtmlHeadAppender"/>.
  /// </summary>
  [ToolboxData("<{0}:HtmlHeadContents runat=\"server\" id=\"HtmlHeadContents\"></{0}:HtmlHeadContents>")]
  public class HtmlHeadContents : Control, IHtmlHeadContents
  {
    public HtmlHeadContents ()
    {
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var htmlHeadAppender = HtmlHeadAppender.Current;
      var htmlHeadElements = htmlHeadAppender.GetHtmlHeadElements().ToArray();

      var renderingContext = CreateRenderingContext(writer, htmlHeadElements);
      var renderer = CreateRenderer();
      renderer.Render(renderingContext);

      htmlHeadAppender.SetAppended();
    }

    protected virtual HtmlHeadContentsRenderingContext CreateRenderingContext (HtmlTextWriter writer, HtmlHeadElement[] htmlHeadElements)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderingContext = new HtmlHeadContentsRenderingContext(Page!.Context!, writer, this, htmlHeadElements); // TODO RM-8118: not null assertion
      return renderingContext;
    }

    protected virtual IHtmlHeadContentsRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IHtmlHeadContentsRenderer>();
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }
  }
}
