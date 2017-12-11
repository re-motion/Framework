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

//  BocBooleanValue.js contains client side scripts used by BocBooleanValue.

var _bocBooleanValue_Resources = new Object();

BocBooleanValue_Resource = function(
  trueValue,
  falseValue,
  nullValue,
  trueDescription,
  falseDescription,
  nullDescription,
  trueIconUrl,
  falseIconUrl,
  nullIconUrl) {
    var _trueValue = trueValue;
    var _falseValue = falseValue;
    var _nullValue = nullValue;

    var _trueDescription = trueDescription;
    var _falseDescription = falseDescription;
    var _nullDescription = nullDescription;

    var _trueIconUrl = trueIconUrl;
    var _falseIconUrl = falseIconUrl;
    var _nullIconUrl = nullIconUrl;

    this.SelectNextCheckboxValue = function (
      link,
      icon,
      label,
      hiddenField,
      isRequired,
      trueDescription,
      falseDescription,
      nullDescription) {
        var trueValue = _trueValue;
        var falseValue = _falseValue;
        var nullValue = _nullValue;

        var oldValue = hiddenField.value;
        var newValue;

        //  Select the next value.
        //  true -> false -> null -> true
        if (isRequired) {
            if (oldValue == falseValue)
                newValue = trueValue;
            else
                newValue = falseValue;
        }
        else {
            if (oldValue == falseValue)
                newValue = nullValue;
            else if (oldValue == nullValue)
                newValue = trueValue;
            else
                newValue = falseValue;
        }

        // Update the controls
        hiddenField.value = newValue;
        var checkedState;
        var iconSrc;
        var description;

        if (newValue == falseValue) {
            checkedState = 'false';
            iconSrc = _falseIconUrl;
            if (falseDescription == null)
                description = _falseDescription;
            else
                description = falseDescription;
        }
        else if (newValue == nullValue) {
            checkedState = 'mixed';
            iconSrc = _nullIconUrl;
            if (nullDescription == null)
                description = _nullDescription;
            else
                description = nullDescription;
        }
        else if (newValue == trueValue) {
            checkedState = 'true';
            iconSrc = _trueIconUrl;
            if (trueDescription == null)
                description = _trueDescription;
            else
                description = trueDescription;
        }

        link.setAttribute('aria-checked', checkedState);
        icon.src = iconSrc;
        if (label == null)
        {
          link.title = description;
        }
        else
        {
          label.innerHTML = description;
        }
      $(hiddenField).change();
    };
}

//  Initializes the strings used to represent the true, false and null values.
//  Call this method once in a startup script.
function BocBooleanValue_InitializeGlobals(
    key,
    trueValue, 
    falseValue, 
    nullValue, 
    trueDescription,
    falseDescription,
    nullDescription,
    trueIconUrl, 
    falseIconUrl, 
    nullIconUrl)
{
  _bocBooleanValue_Resources[key] = new BocBooleanValue_Resource(
      trueValue,
      falseValue,
      nullValue,
      trueDescription,
      falseDescription,
      nullDescription,
      trueIconUrl,
      falseIconUrl,
      nullIconUrl);
}

// Selected the next value of the tri-state checkbox, skipping the null value if isRequired is true.
// link: The anchor tag representing the clickable area.
// icon: The icon representing the tri-state checkbox.
// label: The label containing the description for the value. null for no description.
// hiddenField: The hidden input field used to store the value between postbacks.
// isRequired: true to enqable the null value, false to limit the choices to true and false.
function BocBooleanValue_SelectNextCheckboxValue (
  key,
  link,
  icon,
  label,
  hiddenField,
  isRequired,
  trueDescription,
  falseDescription,
  nullDescription)
{
  var resource = _bocBooleanValue_Resources[key];
  resource.SelectNextCheckboxValue(
  link,
  icon,
  label,
  hiddenField,
  isRequired,
  trueDescription,
  falseDescription,
  nullDescription);
}

function BocBooleanValue_OnKeyDown (context)
{
  if (event.keyCode == 32)
  {
    context.click();
    event.cancelBubble = true;
    event.returnValue = false;
  }
}
