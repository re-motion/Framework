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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Constants for common JavaScript scripts.
  /// </summary>
  public static class CommonJavaScripts
  {
    /// <summary>
    /// Closes the current window.
    /// </summary>
    // ReSharper disable once ConvertToConstant.Global
    public static readonly string SelfClose = "self.close();";

    /// <summary>
    /// Returns the HTML element's computed background color. You must provide the ElementScope as the first parameter to the JavaScript call.
    /// </summary>
    public static readonly string GetComputedBackgroundColor = CreateGetComputedCssValueScript ("background-color");

    /// <summary>
    /// Returns the HTML element's computed text color. You must provide the ElementScope as the first parameter to the JavaScript call.
    /// </summary>
    public static readonly string GetComputedTextColor = CreateGetComputedCssValueScript ("color");

    private static string CreateGetComputedCssValueScript ([NotNull] string cssProperty)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("cssProperty", cssProperty);

      return string.Format ("return window.getComputedStyle (arguments[0])['{0}'];", cssProperty);
    }
  }
}