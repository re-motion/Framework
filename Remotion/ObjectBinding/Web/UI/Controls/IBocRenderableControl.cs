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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Common interface for controls which exposes properties relevant to rendering.
  /// </summary>
  public interface IBocRenderableControl : IStyledControl, IControlWithDiagnosticMetadata
  {
    /// <summary>Evalutes whether this control is in <b>Design Mode</b>.</summary>
    bool IsDesignMode { get; }

    /// <summary>
    /// <para>Gets whether the control currently accepts user input; useful for temporarily disabling input during ongoing interactions
    /// in other controls.</para>
    /// <para>This is not the same as <see cref="IBusinessObjectBoundEditableControl.IsReadOnly"/>, which determines if the value shown
    /// by the control can be manipulated by the current user at all.</para>
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Interface exposure of <see cref="WebControl.Style"/>.
    /// </summary>
    CssStyleCollection Style { get; }

    /// <summary>
    /// Interface exposure of <see cref="WebControl.Width"/>.
    /// </summary>
    Unit Width { get; set; }

    /// <summary>
    /// Interface exposure of <see cref="WebControl.Height"/>.
    /// </summary>
    Unit Height { get; set; }
  }
}