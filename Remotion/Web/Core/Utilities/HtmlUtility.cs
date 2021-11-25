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
using System.Text.RegularExpressions;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Obsolete;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  public class HtmlUtility
  {
    [Obsolete ("HtmlUtility.Format(string, params string[]) is obsolete. (Version 3.0.0)")]
    public static string Format (string htmlFormatString, params object[] nonHtmlParameters)
    {
      string[] htmlParameters = new string[nonHtmlParameters.Length];
      for (int i = 0; i < nonHtmlParameters.Length; ++i)
        htmlParameters[i] = WebString.CreateFromText(nonHtmlParameters[i].ToString()).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
      return string.Format(htmlFormatString, (object[])htmlParameters);
    }

#if !NETFRAMEWORK
    [Obsolete (
        "HtmlUtility.HtmlEncode(string) is obsolete. Use WebString.CreateFromText(string).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks) instead. (Version 3.0.0)",
        DiagnosticId = ObsoleteDiagnosticIDs.HtmlUtility)]
#endif
    public static string HtmlEncode (string nonHtmlString)
    {
      return WebString.CreateFromText(nonHtmlString).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
    }

    [Obsolete ("HtmlUtility.HtmlEncode(string, HtmlTextWriter) is obsolete. Use WebString.CreateFromText(string).Write(HtmlTextWriter) instead. (Version 3.0.0)")]
    public static void HtmlEncode (string nonHtmlString, HtmlTextWriter writer)
    {
      WebString.CreateFromText(nonHtmlString).Write(writer);
    }

    private static readonly Regex s_stripHtmlTagsRegex = new Regex("<.*?>", RegexOptions.Compiled);

    public static string StripHtmlTags ([NotNull] string text)
    {
      ArgumentUtility.CheckNotNull("text", text);

      return s_stripHtmlTagsRegex.Replace(text, string.Empty);
    }

    private HtmlUtility ()
    {
    }
  }
}
