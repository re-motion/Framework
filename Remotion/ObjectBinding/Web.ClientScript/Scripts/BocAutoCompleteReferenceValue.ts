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

type BocAutoCompleteReferenceValue_UpdateResult = { Value: Nullable<BocAutoCompleteReferenceValue_Item> }

type BocAutoCompleteReferenceValue_Item = { UniqueIdentifier: string, DisplayName: string, IconUrl: Nullable<string>, IsAnnotation: boolean }

class BocAutoCompleteReferenceValue //TODO RM-7715 - Make the TypeScript classes BocReferenceValue and BocAutoCompleteReferenceValue inherit from BocReferenceValueBase
{
  private _itemBackUp: Nullable<BocAutoCompleteReferenceValue_Item>;
  private _isInvalidated: boolean;
  private _command: Nullable<JQuery>;
  private _commandBackUp: Nullable<JQuery>;
  private _selectListID: string;
  private _informationPopUpID: string;
  private _nullValueString: string;
  private _isAutoPostBackEnabled: boolean;
  private _isIconUpdateEnabled: boolean
  private _controlServiceUrl: string;
  private _iconContext: Nullable<BocReferenceValueBase_IconContext>;
  private _commandInfo: Nullable<BocReferenceValueBase_CommandInfo>;
  private _textbox: JQuery;

  constructor (
    baseID: Nullable<string>,
    combobox: JQuery, textbox: JQuery, hiddenField: JQuery, button: JQuery, command: Nullable<JQuery>, controlServiceUrl: string,
    completionSetCount: number, dropDownDisplayDelay: number, dropDownRefreshDelay: number, selectionUpdateDelay: number,
    searchStringValidationInfo: BocReferenceValueBase_SearchStringValidationInfo,
    nullValueString: string,
    isAutoPostBackEnabled: boolean,
    searchContext: object,
    isIconUpdateEnabled: boolean,
    iconContext: Nullable<BocReferenceValueBase_IconContext>,
    commandInfo: Nullable<BocReferenceValueBase_CommandInfo>,
    resources: BocReferenceValueBase_Resources)
  {
    ArgumentUtility.CheckTypeIsString('baseID', baseID);
    ArgumentUtility.CheckNotNullAndTypeIsObject('combobox', combobox);
    ArgumentUtility.CheckNotNullAndTypeIsObject('this.textbox', textbox);
    ArgumentUtility.CheckNotNullAndTypeIsObject('hiddenField', hiddenField);
    ArgumentUtility.CheckNotNullAndTypeIsObject('button', button);
    ArgumentUtility.CheckTypeIsObject('command', command);
    ArgumentUtility.CheckNotNullAndTypeIsString('controlServiceUrl', controlServiceUrl);
    ArgumentUtility.CheckNotNullAndTypeIsNumber('completionSetCount', completionSetCount);
    ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownDisplayDelay', dropDownDisplayDelay);
    ArgumentUtility.CheckNotNullAndTypeIsNumber('dropDownRefreshDelay', dropDownRefreshDelay);
    ArgumentUtility.CheckNotNullAndTypeIsNumber('selectionUpdateDelay', selectionUpdateDelay);
    ArgumentUtility.CheckNotNullAndTypeIsObject('searchStringValidationInfo', searchStringValidationInfo);
    ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAutoPostBackEnabled', isAutoPostBackEnabled);
    ArgumentUtility.CheckNotNullAndTypeIsObject('searchContext', searchContext);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isIconUpdateEnabled', isIconUpdateEnabled);
    if (isIconUpdateEnabled)
      ArgumentUtility.CheckNotNullAndTypeIsObject('iconContext', iconContext);
    ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
    ArgumentUtility.CheckNotNullAndTypeIsObject('resources', resources);

    this._itemBackUp = null;
    this._isInvalidated = false;
    this.BackupItemData(hiddenField.val(), textbox.val());
    this._command = command;
    this._commandBackUp = command;
    this._selectListID = baseID + '_Results';
    this._informationPopUpID = baseID + '_Information';
    this._isAutoPostBackEnabled = isAutoPostBackEnabled;
    this._nullValueString = nullValueString;
    this._isIconUpdateEnabled = isIconUpdateEnabled;
    this._controlServiceUrl = controlServiceUrl;
    this._iconContext = iconContext;
    this._commandInfo = commandInfo;
    this._textbox = textbox;

    if (BrowserUtility.GetIEVersion() > 0)
    {
      // For Internet Explorer + JAWS, we must use the ARIA 1.0 combobox pattern.
      // The remaining browsers support ARIA 1.1 with NVDA.

      var internetExplorerScreenReaderLabelID = baseID + '_InternetExplorerScreenReaderLabel';
      var screenReaderLabel = $("<span hidden='hidden' />")
        .attr("id", internetExplorerScreenReaderLabelID)
        .html(resources.InternetExplorerScreenReaderLabelText)
        .appendTo(combobox);

      this._textbox.attr('role', combobox.attr('role'));
      combobox.removeAttr('role');

      this._textbox.attr('aria-haspopup', combobox.attr('aria-haspopup'));
      combobox.removeAttr('aria-haspopup');

      this._textbox.attr('aria-expanded', combobox.attr('aria-expanded'));
      combobox.removeAttr('aria-expanded');

      if (combobox.attr('aria-labelledby') !== undefined)
      {
        this._textbox.attr('aria-labelledby', combobox.attr('aria-labelledby') + ' ' + internetExplorerScreenReaderLabelID);
        combobox.removeAttr('aria-labelledby');

        if (combobox.attr('data-label-id-index') !== undefined)
        {
          this._textbox.attr('data-label-id-index', combobox.attr('data-label-id-index'));
          combobox.removeAttr('data-label-id-index');
        }
      }
      else
      {
        this._textbox.attr('aria-labelledby', internetExplorerScreenReaderLabelID);
      }

      if (combobox.attr('aria-describedby') !== undefined)
      {
        this._textbox.attr('aria-describedby', combobox.attr('aria-describedby'));
        combobox.removeAttr('aria-describedby');
      }

      combobox = this._textbox;
    }

    const errorHandler = (error: Sys.Net.WebServiceError) =>
    {
      this.SetError(resources.LoadIconFailedErrorMessage);
    };

    this._textbox.autocomplete(this._controlServiceUrl, 'Search', 'SearchExact',
      {
        extraParams: searchContext,
        isAutoPostBackEnabled: this._isAutoPostBackEnabled,
        nullValue: this._nullValueString, // the hidden field value indicating that no value has been selected
        searchStringValidationParams:
        {
          inputRegex: new RegExp(searchStringValidationInfo.ValidSearchStringRegex),
          dropDownTriggerRegex: new RegExp(searchStringValidationInfo.ValidSearchStringForDropDownRegex),
          dropDownTriggerRegexFailedMessage: searchStringValidationInfo.SearchStringForDropDownDoesNotMatchRegexMessage,
          getDropDownSearchString: (searchString: string) =>
          {
            return searchStringValidationInfo.IgnoreSearchStringForDropDownUponValidInput ? this.GetDropDownSearchStringForValidInput(searchString) : searchString;
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
        selectListID: this._selectListID,
        informationPopUpID: this._informationPopUpID,
        dropDownButtonID: button.attr('id'),
        inputAreaClass: 'content',
        // this can be set to true/removed once the problem is fixed that an empty this.textbox still selects the first element, making it impossible to clear the selection
        selectFirst: function (inputValue: string, searchTerm: string)
        {
          return inputValue.length > 0;
        },
        dataType: 'json',
        parse: function (data: BocAutoCompleteReferenceValue_Item[])
        {
          return $.map(data, function (row)
          {
            row.IsAnnotation = row.UniqueIdentifier === nullValueString;

            return {
              data: row,
              value: row.UniqueIdentifier,
              result: row.IsAnnotation ? '' : row.DisplayName
            };
          });
        },
        formatItem: function (item: BocAutoCompleteReferenceValue_Item) //What we display on input box
        {
          var row = $('<li/>');

          if (item.IconUrl != '')
          {
            var img = $('<img/>');
            img.attr({ src: item.IconUrl, alt: '', 'aria-hidden': 'true' });
            row.append($('<div/>').append(img));
          }

          var displayName = $('<span/>');
          displayName.text(item.DisplayName);
          row.append($('<div/>').append(displayName));

          return {
            html: row.html(),
            class: null,
            isAnnotation: item.IsAnnotation
          };
        },
        formatMatch: function (item: BocAutoCompleteReferenceValue_Item) //The value used by the cache
        {
          return item.DisplayName;
        },
        handleRequestError: (err: Sys.Net.WebServiceError) =>
        {
          this.SetError(resources.LoadDataFailedErrorMessage);
        },
        clearRequestError: () =>
        {
          this.ClearError();
        }
      }
    ).invalidateResult(() =>
    {
      if (this._isInvalidated)
        return;

        this._isInvalidated = true;

      hiddenField.val(this._nullValueString);
      this.UpdateCommand(this._nullValueString, errorHandler);
      //Do not fire change-event
    }).updateResult((e: JQueryEventObject, item: BocAutoCompleteReferenceValue_Item, out: BocAutoCompleteReferenceValue_UpdateResult) =>
    {
      try
      {
        var actualItem = item;

        if (item.DisplayName.toLowerCase() == this._itemBackUp!.DisplayName.toLowerCase()
          && (item.UniqueIdentifier == this._itemBackUp!.UniqueIdentifier || item.UniqueIdentifier == this._nullValueString))
        {
          if (item.UniqueIdentifier == this._nullValueString && this._itemBackUp!.UniqueIdentifier != this._nullValueString)
            this._textbox.val(this._itemBackUp!.DisplayName); // fall back to the last confirmed user input to preserve correct casing
          else
            this._textbox.val(item.DisplayName); // keep the latest user input to preservce current casing
          actualItem = this._itemBackUp!;
        }

        var hasChanged = this._itemBackUp != actualItem;

        if (this._isInvalidated || hasChanged)
        {
          hiddenField.val(actualItem.UniqueIdentifier);
          this.BackupItemData(actualItem.UniqueIdentifier, actualItem.DisplayName);

          if (hasChanged)
          {
            this.UpdateCommand(actualItem.UniqueIdentifier, errorHandler);
            this._commandBackUp = this._command;
            hiddenField.trigger('change');
          }
          else
          {
            this.RestoreCommand();
          }
        }
        out.Value = actualItem;
      }
      finally
      {
        this._isInvalidated = false;
      }
    });
  };

  private RestoreCommand(): void
  {
    if (this._commandBackUp == null || this._commandBackUp.length == 0)
      return;

    if (this._command == null || this._command.length == 0)
      return;

      this._command.replaceWith(this._commandBackUp);
      this._command = this._commandBackUp;
  }

  private UpdateCommand (selectedValue: string, errorHandler: (error: Sys.Net.WebServiceError) => void): void
  {
    if (this._command == null || this._command.length == 0)
      return;

    if (this._isAutoPostBackEnabled)
    {
      this._command = BocReferenceValueBase.UpdateCommand(this._command, null, false, null, null, null, function () { });
    }
    else
    {
      var businessObject = null;
      if (selectedValue != this._nullValueString)
        businessObject = selectedValue;

      this._command = BocReferenceValueBase.UpdateCommand(this._command, businessObject, this._isIconUpdateEnabled, this._controlServiceUrl, this._iconContext, this._commandInfo, errorHandler);
    }
  }

  private ClearError(): void
  {
    if (this._textbox.hasClass('error'))
    {
      this._textbox.removeClass('error');
      this._textbox.trigger('hideInformationPopUp');
    }
  }

  private SetError (message: string): void
  {
    this._textbox.addClass('error');
    this._textbox.trigger('showInformationPopUp', { 'message': message });
  }

  private BackupItemData(uniqueIdentifier: string, displayName: string): void
  {
    // TODO RM-7741 - BocAutoCompleteReferenceValue.ts: Not all state of Item is backed up
    this._itemBackUp = { UniqueIdentifier: uniqueIdentifier, DisplayName: displayName, IconUrl: null, IsAnnotation: false };
  }

  private GetDropDownSearchStringForValidInput (searchString: string): string
  {
    if (this._itemBackUp!.UniqueIdentifier != this._nullValueString && searchString.toLowerCase() == this._itemBackUp!.DisplayName.toLowerCase())
      return '';
    return searchString;
  }

  //  Returns the number of rows selected for the specified ReferenceValue
  public static GetSelectionCount (referenceValueHiddenFieldID: string, nullValueString: string): number
  {
    ArgumentUtility.CheckNotNullAndTypeIsString('referenceValueHiddenFieldID', referenceValueHiddenFieldID);
    ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

    var hiddenField = <Nullable<HTMLInputElement>>document.getElementById(referenceValueHiddenFieldID);
    if (hiddenField == null || hiddenField.value == nullValueString)
      return 0;

    return 1;
  };
}
