﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

function BocAutoCompleteReferenceValue()
{
}

BocAutoCompleteReferenceValue.Initialize = function (
    baseID,
    combobox, textbox, hiddenField, button, command, searchServiceUrl,
    completionSetCount, dropDownDisplayDelay, dropDownRefreshDelay, selectionUpdateDelay,
    searchStringValidationInfo,
    nullValueString,
    isAutoPostBackEnabled,
    searchContext,
    iconServiceUrl,
    iconContext,
    commandInfo,
    resources)
{
  ArgumentUtility.CheckTypeIsString('baseID', baseID);
  ArgumentUtility.CheckNotNullAndTypeIsObject('combobox', combobox);
  ArgumentUtility.CheckNotNullAndTypeIsObject('textbox', textbox);
  ArgumentUtility.CheckNotNullAndTypeIsObject('hiddenField', hiddenField);
  ArgumentUtility.CheckNotNullAndTypeIsObject('button', button);
  ArgumentUtility.CheckTypeIsObject('command', command);
  ArgumentUtility.CheckNotNullAndTypeIsString('searchServiceUrl', searchServiceUrl);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('completionSetCount', completionSetCount);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownDisplayDelay', dropDownDisplayDelay);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownRefreshDelay', dropDownRefreshDelay);
  ArgumentUtility.CheckNotNullAndTypeIsNumber('selectionUpdateDelay', selectionUpdateDelay);
  ArgumentUtility.CheckNotNullAndTypeIsObject('searchStringValidationInfo', searchStringValidationInfo);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAutoPostBackEnabled', isAutoPostBackEnabled);
  ArgumentUtility.CheckNotNullAndTypeIsObject('searchContext', searchContext);
  ArgumentUtility.CheckTypeIsString('iconServiceUrl', iconServiceUrl);
  ArgumentUtility.CheckTypeIsObject('iconContext', iconContext);
  ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
  ArgumentUtility.CheckNotNullAndTypeIsObject('resources', resources);

  var _itemBackUp = null;
  var _isInvalidated = false;
  BackupItemData(hiddenField.val(), textbox.val());
  var _command = command;
  var _commandBackUp = command;
  var _selectListID = baseID + '_Results';
  var _informationPopUpID = baseID + '_Information';

  if (BrowserUtility.GetIEVersion() > 0)
  {
    // For Internet Explorer + JAWS, we must use the ARIA 1.0 combobox pattern.
    // The remaining browsers support ARIA 1.1 with NVDA.

    var internetExplorerScreenReaderLabelID = baseID + '_InternetExplorerScreenReaderLabel';
    var screenReaderLabel = $("<span hidden='hidden' />")
      .attr("id", internetExplorerScreenReaderLabelID)
      .html(resources.InternetExplorerScreenReaderLabel)
      .appendTo(combobox);

    textbox.attr('role', combobox.attr ('role'));
    combobox.removeAttr('role');

    textbox.attr('aria-haspopup', combobox.attr('aria-haspopup'));
    combobox.removeAttr('aria-haspopup');

    textbox.attr('aria-expanded', combobox.attr('aria-expanded'));
    combobox.removeAttr('aria-expanded');

    if (combobox.attr ('aria-labelledby') !== undefined)
    {
      textbox.attr ('aria-labelledby', combobox.attr ('aria-labelledby') + ' ' +internetExplorerScreenReaderLabelID);
      combobox.removeAttr ('aria-labelledby');
    }
    else
    {
      textbox.attr('aria-labelledby', internetExplorerScreenReaderLabelID);
    }

    if (combobox.attr('aria-describedby') !== undefined)
    {
      textbox.attr('aria-describedby', combobox.attr('aria-describedby'));
      combobox.removeAttr('aria-describedby');
    }

    combobox = textbox;
  }

  textbox.autocomplete(searchServiceUrl, 'Search', 'SearchExact',
        {
          extraParams: searchContext,
          isAutoPostBackEnabled: isAutoPostBackEnabled,
          nullValue: nullValueString, // the hidden field value indicating that no value has been selected
          searchStringValidationParams:
          {
            inputRegex: new RegExp(searchStringValidationInfo.ValidSearchStringRegex),
            dropDownTriggerRegex: new RegExp(searchStringValidationInfo.ValidSearchStringForDropDownRegex),
            dropDownTriggerRegexFailedMessage: searchStringValidationInfo.SearchStringForDropDownDoesNotMatchRegexMessage,
            getDropDownSearchString: function (searchString)
            {
              return searchStringValidationInfo.IgnoreSearchStringForDropDownUponValidInput ? GetDropDownSearchStringForValidInput (searchString) : searchString;
            }
          },
          max: completionSetCount, // Set query limit

          dropDownDisplayDelay: dropDownDisplayDelay,
          dropDownRefreshDelay: dropDownRefreshDelay,
          selectionUpdateDelay: selectionUpdateDelay,

          noDataFoundMessage: resources.NoDataFoundMessage,
          autoFill: true,
          matchContains: true,
          combobox: combobox,
          selectListID: _selectListID,
          informationPopUpID: _informationPopUpID,
          dropDownButtonID: button.attr('id'),
          inputAreaClass: 'content',
          // this can be set to true/removed once the problem is fixed that an empty textbox still selects the first element, making it impossible to clear the selection
          selectFirst: function (inputValue, searchTerm)
          {
            return inputValue.length > 0;
          },
          dataType: 'json',
          parse: function (data)
          {
            return $.map(data, function (row)
            {
              row.IsAnnotation = row.UniqueIdentifier === nullValueString;

              return {
                data : row,
                value : row.UniqueIdentifier,
                result : row.IsAnnotation ? '' : row.DisplayName
              };
            });
          },
          formatItem: function (item) //What we display on input box
          {
            var row = $('<li/>');

            if (item.IconUrl != '')
            {
              var img = $('<img/>');
              img.attr ({ src : item.IconUrl });
              row.append ($('<div/>').append (img));
            }

            var displayName = $('<span/>');
            displayName.text (item.DisplayName);
            row.append ($ ('<div/>').append (displayName));

            return {
              html : row.html(),
              class : null,
              isAnnotation : item.IsAnnotation
            };
          },
          formatMatch: function (item) //The value used by the cache
          {
            return item.DisplayName;
          },
          handleRequestError: function (err)
          {
            SetError (resources.LoadDataFailedErrorMessage);
          },
          clearRequestError: function ()
          {
            ClearError();
          }
        }
    ).invalidateResult(function ()
    {
      if (_isInvalidated)
        return;

      _isInvalidated = true;

      hiddenField.val(nullValueString);
      UpdateCommand(nullValueString);
      //Do not fire change-event
    }).updateResult(function (e, item, out)
    {
      try
      {
        var actualItem = item;

        if (item.DisplayName.toLowerCase() == _itemBackUp.DisplayName.toLowerCase()
            && (item.UniqueIdentifier == _itemBackUp.UniqueIdentifier || item.UniqueIdentifier == nullValueString))
        {
          if (item.UniqueIdentifier == nullValueString && _itemBackUp.UniqueIdentifier != nullValueString)
            textbox.val(_itemBackUp.DisplayName); // fall back to the last confirmed user input to preserve correct casing
          else
            textbox.val(item.DisplayName); // keep the latest user input to preservce current casing
          actualItem = _itemBackUp;
        }

        var hasChanged = _itemBackUp != actualItem;

        if (_isInvalidated || hasChanged)
        {
          hiddenField.val (actualItem.UniqueIdentifier);
          BackupItemData (actualItem.UniqueIdentifier, actualItem.DisplayName);

          if (hasChanged)
          {
            UpdateCommand (actualItem.UniqueIdentifier);
            _commandBackUp = _command;
            hiddenField.trigger ('change');
          }
          else
          {
            RestoreCommand();
          }
        }
        out.Value = actualItem;
      }
      finally
      {
        _isInvalidated = false;
      }
    });

  function RestoreCommand()
  {
    if (_commandBackUp == null || _commandBackUp.length == 0)
      return;

    if (_command == null || _command.length == 0)
      return;

    _command.replaceWith(_commandBackUp);
    _command = _commandBackUp;
  }

  function UpdateCommand(selectedValue)
  {
    if (_command == null || _command.length == 0)
      return;

    if (isAutoPostBackEnabled)
    {
      _command = BocReferenceValueBase.UpdateCommand(_command, null, null, null, null, function () { });
    }
    else
    {
      var businessObject = null;
      if (selectedValue != nullValueString)
        businessObject = selectedValue;

      var errorHandler = function (error)
      {
        SetError (resources.LoadIconFailedErrorMessage);
      };

      _command = BocReferenceValueBase.UpdateCommand(_command, businessObject, iconServiceUrl, iconContext, commandInfo, errorHandler);
    }
  }

  function ClearError()
  {
    if (textbox.hasClass('error'))
    {
      textbox.removeClass ('error');
      textbox.trigger ('hideInformationPopUp');
    }
  };

  function SetError(message)
  {
    textbox.addClass('error');
    textbox.trigger ('showInformationPopUp', { 'message' : message });
  };

  function BackupItemData(uniqueIdentifier, displayName)
  {
    _itemBackUp = { UniqueIdentifier : uniqueIdentifier, DisplayName : displayName };
  }

  function GetDropDownSearchStringForValidInput(searchString)
  {
    if (_itemBackUp.UniqueIdentifier != nullValueString && searchString.toLowerCase() == _itemBackUp.DisplayName.toLowerCase())
      return '';
    return searchString;
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
};
