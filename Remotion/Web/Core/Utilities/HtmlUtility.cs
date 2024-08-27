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
using System.Web;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Obsolete;

namespace Remotion.Web.Utilities
{
  public class HtmlUtility
  {
    [Obsolete("HtmlUtility.Format(string, params string[]) is obsolete. (Version 3.0.0)")]
    public static string Format (string htmlFormatString, params object[] nonHtmlParameters)
    {
      string[] htmlParameters = new string[nonHtmlParameters.Length];
      for (int i = 0; i < nonHtmlParameters.Length; ++i)
        htmlParameters[i] = WebString.CreateFromText(nonHtmlParameters[i].ToString()).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
      return string.Format(htmlFormatString, (object[])htmlParameters);
    }

    [Obsolete(
        "HtmlUtility.HtmlEncode(string) is obsolete. Use WebString.CreateFromText(string).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks) instead. (Version 3.0.0)",
        DiagnosticId = ObsoleteDiagnosticIDs.HtmlUtility)]
    public static string HtmlEncode (string nonHtmlString)
    {
      return WebString.CreateFromText(nonHtmlString).ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
    }

    [Obsolete("HtmlUtility.HtmlEncode(string, HtmlTextWriter) is obsolete. Use WebString.CreateFromText(string).WriteTo(HtmlTextWriter) instead. (Version 3.0.0)")]
    public static void HtmlEncode (string nonHtmlString, HtmlTextWriter writer)
    {
      WebString.CreateFromText(nonHtmlString).WriteTo(writer);
    }

    private static readonly Regex s_replaceLineBreaks = new Regex("<br\\s*/>", RegexOptions.Compiled);
    private static readonly Regex s_stripHtmlTagsRegex = new Regex("<.*?>", RegexOptions.Compiled);

    [Obsolete("Use HtmlUtility.ExtractPlainText instead. (Version 3.0.0)", true)]
    public static string StripHtmlTags ([NotNull] string text)
    {
      throw new NotSupportedException("Use HtmlUtility.ExtractPlainText instead. (Version 3.0.0)");
    }

    [Obsolete("Use HtmlUtility.ExtractPlainText instead. (Version 3.0.0)", true)]
    public static string StripHtmlTags (WebString text)
    {
      throw new NotSupportedException("Use HtmlUtility.ExtractPlainText instead. (Version 3.0.0)");
    }

    /// <summary>
    /// Creates an approximation of <c>innerText</c> for an HTML element. Use this method when creating diagnostic metadata from a <see cref="WebString"/>.
    /// </summary>
    public static PlainTextString ExtractPlainText (WebString webString)
    {
      if (webString.Type == WebStringType.Encoded)
      {
        var valueWithoutHtmlLineBreaks = s_replaceLineBreaks.Replace(webString.GetValue(), "\n");
        var valueWithoutHtmlTags = s_stripHtmlTagsRegex.Replace(valueWithoutHtmlLineBreaks, string.Empty);
        var htmlDecodedValue = HttpUtility.HtmlDecode(valueWithoutHtmlTags);
        return PlainTextString.CreateFromText(htmlDecodedValue);
      }
      else
      {
        return webString.ToPlainTextString();
      }
    }

    private HtmlUtility ()
    {
    }
  }
}
