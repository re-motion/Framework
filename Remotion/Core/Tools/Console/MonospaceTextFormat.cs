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
using System.Text;

namespace Remotion.Tools.Console
{

/// <summary>
/// Utility functions for formatting text with fixed character widths.
/// </summary>
public sealed class MonospaceTextFormat
{
  // static members

  /// <summary>
  /// Formats the specified text with word breaks within the specified line width.
  /// </summary>
  /// <param name="sb">The StringBuilder object the text is written to.</param>
  /// <param name="lineWidth">The line width of the output.</param>
  /// <param name="text">The text that is to be formatted.</param>
  public static void AppendWrappedText (StringBuilder sb, int lineWidth, string text)
  {
    string line;
    while (text != null)
    {
      SplitTextOnSeparator (text, out line, out text, lineWidth, new char[] {' '});
      sb.Append (line);
      if (text != null)
        sb.Append ('\n');
    }
  }

  /// <summary>
  /// Formats the specified text with word breaks within the specified line width. All lines but the first are indented.
  /// </summary>
  /// <param name="sb">The StringBuilder object the text is written to.</param>
  /// <param name="indent">The number of characters already contained in the current line, which is also the number of spaces
  /// that preceed each following line.</param>
  /// <param name="lineWidth">The line width of the output.</param>
  /// <param name="text">The text that is to be formatted.</param>
  public static void AppendIndentedText (StringBuilder sb, int indent, int lineWidth, string text)
  {
    string line;
    while (text != null)
    {
      SplitTextOnSeparator (text, out line, out text, lineWidth - indent, new char[] {' '});
      sb.Append (line);
      if (text != null)
      {
        sb.Append ('\n');
        for (int i = 0; i < indent; ++i)
          sb.Append (' ');
      }
    }
  }

  /// <summary>
  /// Splits a string on the last separator character before the specified split position.
  /// </summary>
  /// <param name="text">Input string.</param>
  /// <param name="beforeSplit">Returns the part of the string before the split.</param>
  /// <param name="afterSplit">Returns the part of the string after the split, or a null reference if the complete string was returned in <c>beforeSplit</c>.</param>
  /// <param name="splitAt">Specifies the position to split at. No more than <c>splitAt</c> characters will be returned in <c>beforeSplit</c>.</param>
  /// <param name="separators">Valid separator characters.</param>
  public static void SplitTextOnSeparator (string text, out string beforeSplit, out string afterSplit, int splitAt, char[] separators)
  {
    if (text == null) throw new ArgumentNullException ("text");
    if (splitAt < 0) throw new ArgumentOutOfRangeException ("splitAt", splitAt, "Argument must not be less than zero.");

    if (text.Length <= splitAt)
    {
      beforeSplit = text;
      afterSplit = null;
    }
    else
    {
      int pos = text.LastIndexOfAny (separators, splitAt);
      if (pos >= 0)
      {
        beforeSplit = text.Substring (0, pos);
        afterSplit = text.Substring (pos + 1);
      }
      else
      {
        beforeSplit = text.Substring (0, splitAt);
        afterSplit = text.Substring (splitAt);
      }
      if (afterSplit.Length == 0)
        afterSplit = null;
    }
  }

  // construction and disposal

  private MonospaceTextFormat ()
  {
  }
}


}
