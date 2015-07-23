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
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// The <see cref="HelpInfo"/> type is used to group all information required for displaying a help-link in a form-grid.
  /// </summary>
  /// <remarks>
  /// When extending <see cref="HelpInfo"/> to add additional state, the <see cref="FormGridManager"/>'s 
  /// <see cref="FormGridManager.CreateHelpProvider(Remotion.Web.UI.Controls.HelpInfo)"/> method must also be extended to make use of the addiontal
  /// state.
  /// </remarks>
  public class HelpInfo
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpInfo"/> type.
    /// </summary>
    /// <param name="navigateUrl">The URL assigned to the hyperlinks <c>href</c>-attribute. Must not be <see langword="null" /> or empty.</param>
    public HelpInfo (string navigateUrl)
        : this (navigateUrl, null, null, null)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpInfo"/> type.
    /// </summary>
    /// <param name="navigateUrl">The URL assigned to the hyperlinks <c>href</c>-attribute. Must not be <see langword="null" /> or empty.</param>
    /// <param name="target">The target for the <paramref name="navigateUrl"/>.</param>
    /// <param name="toolTip">The tool-tip to be displayed when hovering over the help-link.</param>
    /// <param name="onClick">The javascript to be executed for the <c>OnClick</c> event on the client.</param>
    public HelpInfo (string navigateUrl, string target, string toolTip, string onClick)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("navigateUrl", navigateUrl);

      NavigateUrl = navigateUrl;
      Target = target;
      OnClick = onClick;
      ToolTip = toolTip;
    }

    /// <summary>
    /// Gets the URL to be rendered. Use the <c>~/</c> notation to specify an URL relative to the application-root.
    /// </summary>
    public string NavigateUrl { get; private set; }

    /// <summary>
    /// Gets the target for the <see cref="NavigateUrl"/>.
    /// </summary>
    public string Target { get; private set; }

    /// <summary>
    /// Gets the javascript to be executed for the <c>OnClick</c> event on the client.
    /// </summary>
    public string OnClick { get; private set; }

    /// <summary>
    /// Gets the tool-tip to be displayed when hovering over the help-link.
    /// </summary>
    public string ToolTip { get; private set; }
  }
}
