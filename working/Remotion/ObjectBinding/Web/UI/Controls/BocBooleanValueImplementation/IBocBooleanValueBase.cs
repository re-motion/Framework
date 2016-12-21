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
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation
{
  public interface IBocBooleanValueBase : IBusinessObjectBoundEditableWebControl, IBocRenderableControl
  {
    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="true"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="true"/>. The default value is an empty <see cref="string"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    string TrueDescription { get; set; }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="false"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="false"/>. The default value is an empty <see cref="string"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    string FalseDescription { get; set; }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="null"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="string"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    string NullDescription { get; set; }

    /// <summary> Gets or sets the current value. </summary>
    new bool? Value { get; set; }

    bool IsAutoPostBackEnabled { get; }

    /// <summary>
    ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
    ///   description. 
    /// </summary>
    Style LabelStyle { get; }
  }
}