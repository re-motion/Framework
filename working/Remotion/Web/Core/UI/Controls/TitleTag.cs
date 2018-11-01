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

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Represents the HTML element <c>title</c>.
  /// </summary>
  public class TitleTag : HtmlHeadElement
  {
    private readonly string _title;

    public TitleTag (string title)
    {
      ArgumentUtility.CheckNotNull ("title", title);

      _title = title;
    }

    public string Title
    {
      get { return _title; }
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.RenderBeginTag (HtmlTextWriterTag.Title);
      writer.WriteEncodedText (_title);
      writer.RenderEndTag();
      writer.WriteLine ();
    }
  }
}