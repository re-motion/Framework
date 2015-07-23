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
  /// Represents a <c>script</c> element for javascript includes.
  /// </summary>
  public class JavaScriptInclude : HtmlHeadElement
  {
    private static readonly string s_tagName = HtmlTextWriterTag.Script.ToString().ToLower();
    private static readonly string s_typeAttribute = HtmlTextWriterAttribute.Type.ToString().ToLower();
    private static readonly string s_srcAttribute = HtmlTextWriterAttribute.Src.ToString().ToLower();

    private readonly IResourceUrl _resourceUrl;

    public JavaScriptInclude (IResourceUrl resourceUrl)
    {
      ArgumentUtility.CheckNotNull ("resourceUrl", resourceUrl);

      _resourceUrl = resourceUrl;
    }

    public IResourceUrl ResourceUrl
    {
      get { return _resourceUrl; }
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.WriteBeginTag (s_tagName);
      writer.WriteAttribute (s_typeAttribute, "text/javascript");
      writer.WriteAttribute (s_srcAttribute, _resourceUrl.GetUrl());
      writer.Write ('>');
      writer.WriteEndTag (s_tagName);
      writer.WriteLine();
    }
  }
}