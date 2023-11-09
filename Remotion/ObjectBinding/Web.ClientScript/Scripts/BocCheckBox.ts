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

//  BocCheckBox.js contains client side scripts used by BocCheckBox.
class BocCheckBox
{
  //  The descriptions used for the true, false, and null values
  private static _bocCheckBox_trueDescription: string;
  private static _bocCheckBox_falseDescription: string;

  //  Initializes the strings used to represent the true, false and null values.
  //  Call this method once in a startup script.
  public static InitializeGlobals (trueDescription: string, falseDescription: string): void
  {
    BocCheckBox._bocCheckBox_trueDescription = trueDescription;
    BocCheckBox._bocCheckBox_falseDescription = falseDescription;
  }

  // Toggle the value of the checkbox.
  // checkBox: The check box.
  // label: The label containing the description for the value. null for no description.
  public static ToggleCheckboxValue (checkBox: HTMLInputElement, label: HTMLLabelElement, trueDescription: Nullable<string>, falseDescription: Nullable<string>): void
  {    
    checkBox.checked = !checkBox.checked;
    BocCheckBox.OnClick (checkBox, label, trueDescription, falseDescription);
  }

  //  Update the text-represention of the check-box value.
  public static OnClick (checkBox: HTMLInputElement, label: HTMLLabelElement, trueDescription: Nullable<string>, falseDescription: Nullable<string>): void
  {    
  // Update the controls
    let checkBoxToolTip; // TODO RM-7654: BocCheckBox_OnClick sets checkBoxToolTip but does not use it
    let labelText;
    
    if (checkBox.checked)
    {
      let description;
      if (trueDescription == null)
        description = BocCheckBox._bocCheckBox_trueDescription;
      else
        description = trueDescription;
      checkBoxToolTip = description;
      labelText = description;
    }
    else
    {
      let description;
      if (falseDescription == null)
        description = BocCheckBox._bocCheckBox_falseDescription;
      else
        description = falseDescription;
      labelText = description;
    }
    if (label != null)
      label.innerHTML = labelText;
  }
}
