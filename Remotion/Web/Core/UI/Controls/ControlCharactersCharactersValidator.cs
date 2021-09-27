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
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Validates that the text does not contains control characters according to the Unicode character definition.
  /// </summary>
  public class ControlCharactersCharactersValidator : BaseValidator
  {
    private const int c_sampleTextLengthDefaultValue = 5;

    public ControlCharactersCharactersValidator ()
    {
      SampleTextLength = c_sampleTextLengthDefaultValue;
    }

    /// <summary> 
    ///   Gets or sets a flag that determines whether to ignore line feed and line break characters during validation. 
    /// </summary>
    /// <value> <see langword="true"/> to ignore line feed and line break characters. Defaults to <see langword="false"/>. </value>
    [Category ("Behavior")]
    [Description ("Set this flag to allow line feed and line break characters in the text.")]
    [DefaultValue (false)]
    public bool EnableMultilineText { get; set; }

    /// <summary>
    /// Gets or sets the format string to be used for the <see cref="BaseValidator.ErrorMessage"/>.
    /// Use <c>{0}</c> for the text fragment where the error occurred. Use <c>{1}</c> for the line position and <c>{2}</c> for the line number.
    /// </summary>
    [Category("Appearance")]
    [Description ("Set this property to specify a format string to be used as ErrorMessage. Use '{0}' for the text fragment where the error occurred. Use '{1}' for the line position and '{2}' for the line number.")]
    [DefaultValue ("")]

    public string? ErrorMessageFormat { get; set; }
    
    /// <summary>
    /// Gets or sets length of the leading and trailing text sample to be included in the <see cref="BaseValidator.ErrorMessage"/>.
    /// </summary>
    /// <value> Defaults to <c>5</c>. </value>
    [Category("Appearance")]
    [Description ("Set length of the leading and trailing text sample to be included in the error message.")]
    [DefaultValue (c_sampleTextLengthDefaultValue)]

    public int SampleTextLength { get; set; }

    protected override bool EvaluateIsValid ()
    {
      string text = base.GetControlValidationValue (base.ControlToValidate);

      return EvaluateIsTextValid (text);
    }

    protected bool EvaluateIsTextValid (string text)
    {
      if (string.IsNullOrEmpty (text))
        return true;

      int linePosition = 0;
      int lineNumber = 1;
      // ReSharper disable once ForCanBeConvertedToForeach
      for (int textPosition = 0; textPosition < text.Length; textPosition++)
      {
        linePosition++;

        var c = text[textPosition];
        if (char.IsControl (c))
        {
          if (c == '\t')
            continue;

          if (EnableMultilineText)
          {
            if (c == '\n')
            {
              lineNumber++;
              linePosition = 0;
              continue;
            }

            if (c == '\r' && textPosition < text.Length - 1 && text[textPosition + 1] == '\n')
              continue;
          }

          return HandleError (textPosition);
        }
      }

      return true;

      bool HandleError (int textPosition)
      {
        if (!string.IsNullOrEmpty (ErrorMessageFormat))
        {
          var sampleTextLength = Math.Max (0, SampleTextLength);
          var sampleTextStart = Math.Max (0, textPosition - sampleTextLength);
          var sampleTextEnd = Math.Min (textPosition + sampleTextLength, text.Length - 1);
          var sampleText = text.Substring (sampleTextStart, sampleTextEnd + 1 - sampleTextStart);

          if (EnableMultilineText)
          {
            var indexOfLastLeadingLineBreak = sampleText.LastIndexOf ('\n', textPosition - sampleTextStart) + 1;

            var indexOfFirstTrailingLineBreak = sampleText.IndexOf ('\n', textPosition - sampleTextStart);
            if (indexOfFirstTrailingLineBreak == -1)
              indexOfFirstTrailingLineBreak = sampleText.Length;

            sampleText = sampleText.Substring (indexOfLastLeadingLineBreak, indexOfFirstTrailingLineBreak - indexOfLastLeadingLineBreak);
          }

          ErrorMessage = string.Format (ErrorMessageFormat, sampleText, linePosition, lineNumber);
        }

        return false;
      }
    }
  }
}