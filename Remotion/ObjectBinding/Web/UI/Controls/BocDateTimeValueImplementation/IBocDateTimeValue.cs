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
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation
{
  public interface IBocDateTimeValue : IBusinessObjectBoundEditableWebControl, IBocRenderableControl, IControlWithResourceManager
  {
    new DateTime? Value { get; }

    BocDateTimeValueType ActualValueType { get; }
    Style CommonStyle { get; }
    Style LabelStyle { get; }
    bool ProvideMaxLength { get; }
    SingleRowTextBoxStyle DateTextBoxStyle { get; }
    SingleRowTextBoxStyle TimeTextBoxStyle { get; }
    IDatePickerButton DatePickerButton { get; }
    bool ShowSeconds { get; }
    SingleRowTextBoxStyle DateTimeTextBoxStyle { get; }
    string? DateString { get; }
    string? TimeString { get; }
    string GetDateValueName ();
    string GetTimeValueName ();
    string GetDatePickerText ();
    IDateTimeFormatter DateTimeFormatter { get; }

    /// <summary>
    /// Gets the list of validation errors for the date value of this control.
    /// </summary>
    IEnumerable<string> GetDateValueValidationErrors ();

    /// <summary>
    /// Gets the list of validation errors for the time value of this control.
    /// </summary>
    IEnumerable<string> GetTimeValueValidationErrors ();
  }
}