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
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls
{

/// <summary> Provides client- and server-side e-mail address validation using a regular expression. </summary>
public class EmailAddressValidator : BaseValidator
{
  // static members and constants

  // member fields

  bool _enableTrimming = false;

  // construction and disposing

  // methods and properties

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    base.AddAttributesToRender(writer);
    if (base.RenderUplevel)
    {
      // keys and function name from ASP.NET RegularExpressionValidator.AddAttributesToRender
      writer.AddAttribute("evaluationfunction", "RegularExpressionValidatorEvaluateIsValid");
      string expression;
      if (_enableTrimming)
        expression = @"^(\s*" + ValidationExpressionUserPart + "@" + ValidationExpressionDomainPart + @"\s*)$";
      else
        expression = @"^(" + ValidationExpressionUserPart + "@" + ValidationExpressionDomainPart + @")$";
      writer.AddAttribute("validationexpression", expression);
    }
  }

  protected override bool EvaluateIsValid()
  {
    string text = base.GetControlValidationValue (base.ControlToValidate);
    
    if (text == null)
      return true;
    if (_enableTrimming)
      text = text.Trim();
    if (text.Length == 0)
      return true;

    return IsMatchComplete (text);
  }
 
  /// <summary> Tests the passed <paramref name="emailAddress"/> if it is valid. </summary>
  /// <param name="emailAddress"> The e-mail address to test. </param>
  /// <returns> 
  ///   <see langword="true"/> if only a single <c>@</c> character is found and both <see cref="IsMatchUserPart"/>
  ///   and <see cref="IsMatchDomainPart"/> evaluate <see langword="true"/> for their respective parts of the passed
  ///   e-mail address.
  /// </returns>
  protected bool IsMatchComplete (string emailAddress)
  {
    string[] parts = emailAddress.Split (new char[]{'@'});
    if (parts.Length != 2)
      return false;
    if (! IsMatchUserPart (parts[0]))
      return false;
    if (! IsMatchDomainPart (parts[1]))
      return false;
    return true;
  }

  /// <summary> 
  ///   Tests the passed <paramref name="userPart"/> against <see cref="ValidationExpressionUserPart"/> expression.
  /// </summary>
  /// <param name="userPart"> The user part (i.e. the part before the @) of the e-mail address. </param>
  /// <returns> <see langword="true"/> if the regular expression matches. </returns>
  protected bool IsMatchUserPart (string userPart)
  {
    string expression = "^(" + ValidationExpressionUserPart + ")$";
    return IsMatch (userPart, expression);
  }

  /// <summary> 
  ///   Tests the passed <paramref name="domainPart"/> against <see cref="ValidationExpressionDomainPart"/> expression.
  /// </summary>
  /// <param name="domainPart"> The domain part (i.e. the part after the @) of the e-mail address. </param>
  /// <returns> <see langword="true"/> if the regular expression matches. </returns>
  protected bool IsMatchDomainPart (string domainPart)
  {
    string expression = "^(" + ValidationExpressionDomainPart + ")$";
    return IsMatch (domainPart, expression);
  }

  /// <summary> Tests the <paramref name="text"/> against the <paramref name="expression"/>.</summary>
  /// <param name="text"> The text to be evaluated. </param>
  /// <param name="expression"> The regular expression to be used. </param>
  /// <returns> <see langword="true"/> if the regular expression matches. </returns>
  protected bool IsMatch (string text, string expression)
  {
    Match match = Regex.Match (text, expression);
    bool isMatch = match.Success && match.Index == 0 && match.Length == text.Length;
    return isMatch;
  }

  /// <summary> Gets the validation expression to be used for evaluation of the user part (pre-@). </summary>
  protected virtual string ValidationExpressionUserPart
  {
    get { return @"\w([-.\w]*\w)*"; }
  }

  /// <summary> Gets the validation expression to be used for evaluation of the domain part (post-@). </summary>
  protected virtual string ValidationExpressionDomainPart
  {
    get { return @"(\w[-\w]*\w\.)+[a-zA-Z]{2,9}"; }
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether to ignore leading and trailing whitespaces during validation. 
  /// </summary>
  /// <value> <see langword="true"/> to ignore whitespaces. Defaults to <see langword="false"/>. </value>
  [Category ("Behavior")]
  [Description ("Set this flag to ignore leading and trailing whitespaces during validation. Otherwise, no whitespace is allowed in the input field.")]
  [DefaultValue (false)]
  public bool EnableTrimming
  {
    get { return _enableTrimming; }
    set { _enableTrimming = value; }
  }
}

}
