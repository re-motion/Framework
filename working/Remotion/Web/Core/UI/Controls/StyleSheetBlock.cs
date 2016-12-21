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
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Represents a <c>style</c> element. Use types derived from <see cref="StyleSheetElement"/> to provide the element's content.
  /// <seealso cref="StyleSheetImportRule"/>
  /// </summary>
  public class StyleSheetBlock : HtmlHeadElement
  {
    private static readonly string s_tagName = HtmlTextWriterTag.Style.ToString ().ToLower ();
    private static readonly string s_typeAttribute = HtmlTextWriterAttribute.Type.ToString ().ToLower ();
   
    private readonly StyleSheetElement[] _styleSheetElements;

    public StyleSheetBlock (IEnumerable<StyleSheetElement> styleSheetElements)
    {
      ArgumentUtility.CheckNotNull ("styleSheetElements", styleSheetElements);
      _styleSheetElements = styleSheetElements.ToArray();
    }

    public ReadOnlyCollection<StyleSheetElement> StyleSheetElements
    {
      get { return new ReadOnlyCollection<StyleSheetElement> (_styleSheetElements); }
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.WriteBeginTag (s_tagName);
      writer.WriteAttribute (s_typeAttribute, "text/css");
      writer.WriteLine ('>');
      writer.Indent++;

      foreach (var element in _styleSheetElements)
        element.Render (writer);

      writer.Indent--;
      writer.WriteEndTag(s_tagName);
      writer.WriteLine();
    }
  }
}