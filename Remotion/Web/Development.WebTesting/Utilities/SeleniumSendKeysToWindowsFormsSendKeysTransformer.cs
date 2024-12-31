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
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Keys = OpenQA.Selenium.Keys;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Utilities to convert a Selenium <see cref="IWebElement.SendKeys"/> call to a <see cref="SendKeys"/> call.
  /// </summary>
  public static class SeleniumSendKeysToWindowsFormsSendKeysTransformer
  {
    // Todo RM-6337: Add excessive unit testing!

    /// <summary>
    /// Converts a Selenium <see cref="IWebElement.SendKeys"/> text parameter to a <see cref="SendKeys.SendWait"/> keys parameter.
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx for more information.
    /// </remarks>
    public static string Convert ([NotNull] string value)
    {
      ArgumentUtility.CheckNotNull("value", value);

      value = EncloseSpecialCharacters(value);
      value = TransformKeys(value);
      value = TransformModifierKeys(value);
      value = TransformNewlines(value);
      return value;
    }

    private static string EncloseSpecialCharacters (string value)
    {
      var charactersToEncloseForSendKeys = new[]
                                           {
                                               Regex.Escape("+"), Regex.Escape("^"), Regex.Escape("%"), Regex.Escape("~"), Regex.Escape("("),
                                               Regex.Escape(")"), Regex.Escape("'"), Regex.Escape("["), Regex.Escape("]"), Regex.Escape("{"),
                                               Regex.Escape("}")
                                           };

      var charactersToEncloseForSendKeysRegex = string.Join("|", charactersToEncloseForSendKeys);
      return Regex.Replace(value, charactersToEncloseForSendKeysRegex, match => "{" + match.Value + "}");
    }

    private static string TransformKeys (string value)
    {
      var replacementDictionary = new Dictionary<string, string>
                                  {
                                      { Keys.Backspace, "{BS}" },
                                      { Keys.Pause, "{BREAK}" },
                                      { Keys.Delete, "{DEL}" },
                                      { Keys.Down, "{DOWN}" },
                                      { Keys.End, "{END}" },
                                      { Keys.Enter, "{ENTER}" },
                                      { Keys.Escape, "{ESC}" },
                                      { Keys.Help, "{HELP}" },
                                      { Keys.Home, "{HOME}" },
                                      { Keys.Insert, "{INS}" },
                                      { Keys.Left, "{LEFT}" },
                                      { Keys.PageDown, "{PGDN}" },
                                      { Keys.PageUp, "{PGUP}" },
                                      { Keys.Right, "{RIGHT}" },
                                      { Keys.Tab, "{TAB}" },
                                      { Keys.Up, "{UP}" },
                                      { Keys.F1, "{F1}" },
                                      { Keys.F2, "{F2}" },
                                      { Keys.F3, "{F3}" },
                                      { Keys.F4, "{F4}" },
                                      { Keys.F5, "{F5}" },
                                      { Keys.F6, "{F6}" },
                                      { Keys.F7, "{F7}" },
                                      { Keys.F8, "{F8}" },
                                      { Keys.F9, "{F9}" },
                                      { Keys.F10, "{F10}" },
                                      { Keys.F11, "{F11}" },
                                      { Keys.F12, "{F12}" },
                                      { Keys.Add, "{ADD}" },
                                      { Keys.Subtract, "{SUBTRACT}" },
                                      { Keys.Multiply, "{MULTIPLY}" },
                                      { Keys.Divide, "{DIVIDE}" }
                                  };

      foreach (var replacement in replacementDictionary)
        value = value.Replace(replacement.Key, replacement.Value);

      return value;
    }

    private static string TransformModifierKeys (string value)
    {
      var replacementDictionary = new Dictionary<string, string>
                                  {
                                      { Keys.Shift, "+" },
                                      { Keys.Control, "^" },
                                      { Keys.Alt, "%" }
                                  };

      foreach (var replacement in replacementDictionary)
      {
        // Todo RM-6337: TransformModifierKeys

        // Replace: Keys.Shift + ABC + Keys.Shift + DEF + Keys.Shift + GHI
        //    with: +(ABC)DEF+(GHI)
        // etc. pp.
      }

      return value;
    }

    private static string TransformNewlines (string value)
    {
      return value.Replace("\r", string.Empty).Replace("\n", "\r\n");
    }
  }
}
