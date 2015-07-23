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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  ///   When added to the webform (inside the head element), the <see cref="HtmlHeadContents"/> 
  ///   control renderes the controls registered with <see cref="HtmlHeadAppender"/>.
  /// </summary>
  [ToolboxData ("<{0}:HtmlHeadContents runat=\"server\" id=\"HtmlHeadContents\"></{0}:HtmlHeadContents>")]
  public class HtmlHeadContents : Control, IControl
  {
    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      var htmlHeadAppender = HtmlHeadAppender.Current;

      foreach (var element in htmlHeadAppender.GetHtmlHeadElements())
        element.Render (writer);

      if (!ControlHelper.IsDesignMode (this))
        htmlHeadAppender.SetAppended();
    }

    public new IPage Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }
  }
}