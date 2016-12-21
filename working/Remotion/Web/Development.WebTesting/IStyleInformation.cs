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

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Provides assertable style information for a <see cref="ControlObject"/>.
  /// </summary>
  public interface IStyleInformation
  {
    /// <summary>
    /// Returns whether the control bears the given <paramref name="cssClass"/>.
    /// </summary>
    /// <returns><see langword="true" /> if the control has the given <paramref name="cssClass"/>, otherwise <see langword="false" />.</returns>
    bool HasCssClass ([NotNull] string cssClass);

    /// <summary>
    /// Returns the computed background color of the control. This method ignores background images as well as transparencies - the first
    /// non-transparent color set in the node's DOM hierarchy is returned.
    /// </summary>
    /// <returns>The background color or <see cref="WebColor.Transparent"/> if no background color is set (not even on any parent node).</returns>
    WebColor GetBackgroundColor ();

    /// <summary>
    /// Returns the computed text color of the control. This method ignores transparencies - the first non-transparent color set in the node's
    /// DOM hierarchy is returned.
    /// </summary>
    /// <returns>The text color or <see cref="WebColor.Transparent"/> if no text color is set (not even on any parent node).</returns>
    WebColor GetTextColor ();
  }
}