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

namespace Remotion.Web.UI
{
  /// <summary>
  /// Defines additional HTML attributes not already included in <see cref="HtmlTextWriterAttribute"/>.
  /// </summary>
  public static class HtmlTextWriterAttribute2
  {
    public const string AriaActiveDescendant = "aria-activedescendant";
    public const string AriaAutoComplete = "aria-autocomplete";
    public const string AriaChecked = "aria-checked";
    public const string AriaControls = "aria-controls";
    public const string AriaDescribedBy = "aria-describedby";
    public const string AriaDisabled = "aria-disabled";
    public const string AriaExpanded = "aria-expanded";
    public const string AriaHasPopup = "aria-haspopup";
    public const string AriaHidden = "aria-hidden";
    public const string AriaInvalid = "aria-invalid";
    public const string AriaLabel = "aria-label";
    public const string AriaLabelledBy = "aria-labelledby";
    public const string AriaOwns = "aria-owns";
    public const string AriaReadOnly = "aria-readonly";
    public const string AriaRequired = "aria-required";
    public const string AriaRoleDescription = "aria-roledescription";
    public const string AriaSelected = "aria-selected";
    public const string AriaLive = "aria-live";
    public const string Hidden = "hidden";
    public const string Label = "label";
    [Obsolete("Use 'AriaRequired' instead to prevent the browser from adding a tooltip to the input fields for 'input required'.")]
    public const string Required = "required";
    public const string Role = "role";
    public const string Tabindex = "tabindex";
    public const string Title = "title";
  }
}
