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
using System.Web.UI.WebControls;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  /// <summary>
  /// Exposes properties relevant to rendering controls that implement <see cref="IBocReferenceValue"/> or <see cref="IBocAutoCompleteReferenceValue"/>.
  /// </summary>
  public interface IBocReferenceValueBase : IBusinessObjectBoundEditableWebControl, IBocRenderableControl, IControlWithResourceManager
  {
    bool HasOptionsMenu { get; }
    bool ReserveOptionsMenuWidth { get; }
    DropDownMenu OptionsMenu { get; }
    Unit OptionsMenuWidth { get; }

    bool IsIconEnabled ();
    new IBusinessObjectReferenceProperty? Property { get; }
    new IBusinessObjectWithIdentity? Value { get; }

    IconInfo? GetIcon ();

    string? GetLabelText ();

    /// <summary>
    ///   Gets the style that you want to apply to the text box (edit mode) 
    ///   and the label (read-only mode).
    /// </summary>
    /// <remarks>
    ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
    ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
    ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
    ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
    /// </remarks>
    Style CommonStyle { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    Style LabelStyle { get; }

    string NullValueString { get; }
    string? BusinessObjectUniqueIdentifier { get; }

    /// <summary>
    /// Gets the list of validation errors for this control.
    /// </summary>
    IEnumerable<PlainTextString> GetValidationErrors ();

    string? ControlServicePath { get; }
  }
}
