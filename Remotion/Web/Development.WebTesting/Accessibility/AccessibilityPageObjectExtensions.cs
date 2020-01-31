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

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>
  /// Accessibility extension class for <see cref="PageObject"/>.
  /// </summary>
  public static class AccessibilityPageObjectExtensions
  {
    /// <summary>
    /// Analyzes the whole page.
    /// </summary>
    /// <returns>Accessibility analysis test result.</returns>
    [NotNull]
    public static AccessibilityResult Analyze ([NotNull] this PageObject pageObject, [NotNull] AccessibilityAnalyzer analyzer)
    {
      ArgumentUtility.CheckNotNull ("pageObject", pageObject);
      ArgumentUtility.CheckNotNull ("analyzer", analyzer);

      return analyzer.Analyze();
    }
  }
}