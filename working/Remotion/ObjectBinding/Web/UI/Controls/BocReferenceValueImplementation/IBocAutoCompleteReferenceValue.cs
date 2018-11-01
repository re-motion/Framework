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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  public interface IBocAutoCompleteReferenceValue : IBocReferenceValueBase
  {
    string GetTextValueName ();
    string GetKeyValueName ();
    string SearchServicePath { get; }

    /// <summary> Gets the style that you want to apply to the <see cref="TextBox"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="IBocReferenceValueBase.CommonStyle"/>. </remarks>
    SingleRowTextBoxStyle TextBoxStyle { get; }

    int CompletionSetCount { get; }
    int DropDownDisplayDelay { get; }
    int DropDownRefreshDelay { get; }
    int SelectionUpdateDelay { get; }

    string ValidSearchStringRegex { get; }
    string ValidSearchStringForDropDownRegex { get; }
    string SearchStringForDropDownDoesNotMatchRegexMessage { get; }
    bool IgnoreSearchStringForDropDownUponValidInput { get; }
  }
}