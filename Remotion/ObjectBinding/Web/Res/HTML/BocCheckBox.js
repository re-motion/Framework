// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

//  The descriptions used for the true, false, and null values
var _bocCheckBox_trueDescription;
var _bocCheckBox_falseDescription;

//  Initializes the strings used to represent the true, false and null values.
//  Call this method once in a startup script.
function BocCheckBox_InitializeGlobals (trueDescription, falseDescription)
{
  _bocCheckBox_trueDescription = trueDescription;
  _bocCheckBox_falseDescription = falseDescription;
}

// Toggle the value of the checkbox.
// checkBox: The check box.
// label: The label containing the description for the value. null for no description.
function BocCheckBox_ToggleCheckboxValue (checkBox, label, trueDescription, falseDescription)
{    
  checkBox.checked = !checkBox.checked;
  BocCheckBox_OnClick (checkBox, label, trueDescription, falseDescription);
}

//  Update the text-represention of the check-box value.
function BocCheckBox_OnClick (checkBox, label, trueDescription, falseDescription)
{    
 // Update the controls
  var checkBoxToolTip;
  var labelText;
  
  if (checkBox.checked)
  {
    var description;
    if (trueDescription == null)
      description = _bocCheckBox_trueDescription;
    else
      description = trueDescription;
    checkBoxToolTip = description;
    labelText = description;
  }
  else
  {
    var description;
    if (falseDescription == null)
      description = _bocCheckBox_falseDescription;
    else
      description = falseDescription;
    labelText = description;
  }
  if (label != null)
    label.innerHTML = labelText;
}
