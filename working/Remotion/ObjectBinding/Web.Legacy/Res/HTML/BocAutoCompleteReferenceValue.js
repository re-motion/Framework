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

function BocAutoCompleteReferenceValue()
{
}

BocAutoCompleteReferenceValue.Initialize = function (
    textbox, hiddenField, button, command, searchServiceUrl,
    completionSetCount, dropDownDisplayDelay, dropDownRefreshDelay, selectionUpdateDelay,
    nullValueString,
    isAutoPostBackEnabled,
    searchContext,
    iconServiceUrl,
    iconContext,
    commandInfo)
{
  ArgumentUtility.CheckNotNullAndTypeIsObject('textbox', textbox);
  ArgumentUtility.CheckNotNullAndTypeIsObject('hiddenField', hiddenField);
  ArgumentUtility.CheckNotNullAndTypeIsObject('button', button);
  ArgumentUtility.CheckTypeIsObject('command', command);
  ArgumentUtility.CheckNotNullAndTypeIsString('searchServiceUrl', searchServiceUrl);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('completionSetCount', completionSetCount);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownDisplayDelay', dropDownDisplayDelay);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownRefreshDelay', dropDownRefreshDelay);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('selectionUpdateDelay', selectionUpdateDelay);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);
  ArgumentUtility.CheckTypeIsBoolean('isAutoPostBackEnabled', isAutoPostBackEnabled);
  ArgumentUtility.CheckNotNullAndTypeIsObject('searchContext', searchContext);
  ArgumentUtility.CheckTypeIsString('iconServiceUrl', iconServiceUrl);
  ArgumentUtility.CheckTypeIsObject('iconContext', iconContext);
  ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);

  textbox.autocomplete(searchServiceUrl, 'Search', 'SearchExact', 
        {
          extraParams: searchContext,
          isAutoPostBackEnabled: isAutoPostBackEnabled,
          nullValue: nullValueString, // the hidden field value indicating that no value has been selected
          minChars: 0,
          max: completionSetCount, // Set query limit

          dropDownDisplayDelay: dropDownDisplayDelay,
          dropDownRefreshDelay: dropDownRefreshDelay,
          selectionUpdateDelay: selectionUpdateDelay,

          autoFill: true,
          mustMatch: false, // set true if should clear input on no results
          matchSubset: false, // set false to disable partial cache hits
          matchContains: true,
          multiple: false, // not supprted
          scrollHeight: 220,
          dropDownButtonId: button.attr('id'),
          // this can be set to true/removed once the problem is fixed that an empty textbox still selects the first element, making it impossible to clear the selection
          selectFirst: function (inputValue, searchTerm)
          {
            return inputValue.length > 0;
          },
          dataType: 'json',
          parse: function(data)
          {
            return $.map(data, function(row)
            {
              return {
                data: row,
                value: row.UniqueIdentifier,
                result: row.DisplayName
              }
            });
          },
          formatItem: function (item) //What we display on input box
          {
            var row = $('<div/>');
            row.text(item.DisplayName);

            if (item.IconUrl != '')
            {
              var img = $('<img/>');
              img.attr({ src: item.IconUrl });
              row.prepend(' ');
              row.prepend(img);
            }

            return row.html();
          },
          formatMatch: function (item) //The value used by the cache
          {
            return item.DisplayName;
          }
        }
    ).invalidateResult(function (e, item)
    {
      hiddenField.val(nullValueString);
      UpdateCommand(nullValueString);
      //Do not fire change-event
    }).updateResult(function (e, item)
    {
      hiddenField.val(item.UniqueIdentifier);
      UpdateCommand(item.UniqueIdentifier);
      hiddenField.trigger('change');
    });

  function UpdateCommand(selectedValue)
  {
    if (command == null || command.length == 0)
      return;

    if (isAutoPostBackEnabled)
    {
      command = BocReferenceValueBase.UpdateCommand(command, null, null, null, null);
    }
    else
    {
      var businessObject = null;
      if (selectedValue != nullValueString)
        businessObject = selectedValue;

      command = BocReferenceValueBase.UpdateCommand(command, businessObject, iconServiceUrl, iconContext, commandInfo);
    }
  }
};

//  Returns the number of rows selected for the specified ReferenceValue
BocAutoCompleteReferenceValue.GetSelectionCount = function (referenceValueHiddenFieldID, nullValueString)
{
  ArgumentUtility.CheckNotNullAndTypeIsString('referenceValueHiddenFieldID', referenceValueHiddenFieldID);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

  var hiddenField = document.getElementById(referenceValueHiddenFieldID);
  if (hiddenField == null || hiddenField.value == nullValueString)
    return 0;

  return 1;
}
