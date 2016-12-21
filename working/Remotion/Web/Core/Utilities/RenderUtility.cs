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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// The <see cref="RenderUtility"/> provides helper methods used during rendering of a control's value.
  /// </summary>
  public static class RenderUtility
  {
    /// <summary>
    /// Concatenates a sequence of strings with the HTML line-break tag. Each line is individually HTML encoded.
    /// </summary>
    public static string JoinLinesWithEncoding (IEnumerable<string> lines)
    {
      ArgumentUtility.CheckNotNull ("lines", lines);

      return string.Join ("<br />", lines.Select (HttpUtility.HtmlEncode));
    }

    /// <summary>
    /// Write a sequence of strings to the <paramref name="htmlTextWriter"/>, using the HTML line-break tag for concatenation. Each line is individually HTML encoded.
    /// </summary>
    public static void WriteEncodedLines (this HtmlTextWriter htmlTextWriter, IEnumerable<string> lines)
    {
      ArgumentUtility.CheckNotNull ("lines", lines);

      var enumerator = lines.GetEnumerator();
      if (!enumerator.MoveNext())
        return;

      while (true)
      {
        htmlTextWriter.WriteEncodedText (enumerator.Current);

        if (enumerator.MoveNext())
          htmlTextWriter.WriteBreak();
        else
          break;
      }
    }
  }
}