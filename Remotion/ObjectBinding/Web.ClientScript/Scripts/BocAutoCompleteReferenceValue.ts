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

class BocAutoCompleteReferenceValue //TODO RM-7715 - Make the TypeScript classes BocReferenceValue and BocAutoCompleteReferenceValue inherit from BocReferenceValueBase
{
  private _itemBackUp: Nullable<Remotion.BocAutoCompleteReferenceValue.Item>;
  private _isInvalidated: boolean;
  private _iconMarker: Nullable<HTMLElement>;
  private _icon: Nullable<HTMLImageElement>;
  private _iconBackUp: Nullable<HTMLImageElement>;
  private _selectListID: string;
  private _informationPopUpID: string;
  private _nullValueString: string;
  private _isAutoPostBackEnabled: boolean;
  private _isIconUpdateEnabled: boolean
  private _controlServiceUrl: string;
  private _iconContext: Nullable<BocReferenceValueBase_IconContext>;
  private _textbox: Remotion.BocAutoCompleteReferenceValue.AutoCompleteHTMLElement;

  constructor (
    baseID: Nullable<string>,
    comboboxOrSelector: CssSelectorOrElement<HTMLElement>,
    textboxOrSelector: CssSelectorOrElement<HTMLInputElement>,
    hiddenFieldOrSelector: CssSelectorOrElement<HTMLInputElement>,
    buttonOrSelector: CssSelectorOrElement<HTMLElement>,
    iconMarkerOrSelector: Nullable<CssSelectorOrElement<HTMLElement>>,
    iconOrSelector: Nullable<CssSelectorOrElement<HTMLImageElement>>,
    controlServiceUrl: string,
    completionSetCount: number,
    dropDownDisplayDelay: number,
    dropDownRefreshDelay: number,
    selectionUpdateDelay: number,
    searchStringValidationInfo: BocReferenceValueBase_SearchStringValidationInfo,
    nullValueString: string,
    isAutoPostBackEnabled: boolean,
    searchContext: Dictionary<unknown>,
    isIconUpdateEnabled: boolean,
    iconContext: Nullable<BocReferenceValueBase_IconContext>,
    resources: BocReferenceValueBase_Resources)
  {
    ArgumentUtility.CheckTypeIsString('baseID', baseID);
    ArgumentUtility.CheckNotNull('comboboxOrSelector', comboboxOrSelector);
    ArgumentUtility.CheckNotNull('textboxOrSelector', textboxOrSelector);
    ArgumentUtility.CheckNotNull('hiddenFieldOrSelector', hiddenFieldOrSelector);
    ArgumentUtility.CheckNotNull('buttonOrSelector', buttonOrSelector);
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
    {
      ArgumentUtility.CheckNotNull('iconMarkerOrSelector', iconMarkerOrSelector);
      ArgumentUtility.CheckNotNull('iconOrSelector', iconOrSelector);
      ArgumentUtility.CheckNotNullAndTypeIsObject('iconContext', iconContext);
    }
    ArgumentUtility.CheckNotNullAndTypeIsObject('resources', resources);

    let combobox = ElementResolverUtility.ResolveSingle(comboboxOrSelector);
    const textbox = ElementResolverUtility.ResolveSingle(textboxOrSelector);
    const hiddenField = ElementResolverUtility.ResolveSingle(hiddenFieldOrSelector);
    const button = ElementResolverUtility.ResolveSingle(buttonOrSelector);
    const iconMarker = iconMarkerOrSelector != null
        ? ElementResolverUtility.ResolveSingle(iconMarkerOrSelector)
        : null;
    const icon = iconOrSelector !== null
        ? ElementResolverUtility.ResolveSingle(iconOrSelector)
        : null;

    this._itemBackUp = null;
    this._isInvalidated = false;
    this.BackupItemData(hiddenField.value, textbox.value);
    this._iconMarker = iconMarker;
    this._icon = icon;
    this._iconBackUp = icon;
    this._selectListID = baseID + '_Results';
    this._informationPopUpID = baseID + '_Information';
    this._isAutoPostBackEnabled = isAutoPostBackEnabled;
    this._nullValueString = nullValueString;
    this._isIconUpdateEnabled = isIconUpdateEnabled;
    this._controlServiceUrl = controlServiceUrl;
    this._iconContext = iconContext;

    const errorHandler = (error: Sys.Net.WebServiceError) =>
    {
      this.SetError(resources.LoadIconFailedErrorMessage);
    };

    this._textbox = Remotion.BocAutoCompleteReferenceValue.createAutoComplete(textbox, this._controlServiceUrl, 'Search', 'SearchExact',
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
        dropDownButtonID: button.getAttribute('id')!,
        inputAreaClass: 'content',
        // this can be set to true/removed once the problem is fixed that an empty this.textbox still selects the first element, making it impossible to clear the selection
        selectFirst: function (inputValue: string, searchTerm: string)
        {
          return inputValue.length > 0;
        },
        dataType: 'json',
        parse: function (data: Remotion.BocAutoCompleteReferenceValue.BocAutoCompleteReferenceValueSearchResult)
        {
          if (data.Type === 'ValueList')
          {
            const valueList = data as Remotion.BocAutoCompleteReferenceValue.BocAutoCompleteReferenceValueSearchResultWithValueList;

            return valueList.Values.map(function (row)
            {
              row.IsAnnotation = row.UniqueIdentifier === nullValueString;

              return {
                data: row,
                value: row.UniqueIdentifier,
                result: row.IsAnnotation ? '' : row.DisplayName
              };
            });
          }
          else
          {
            throw `Unknown BocAutoCompleteReferenceValueSearchResult variant '${data.Type}'.`;
          }
        },
        formatItem: function (item: Remotion.BocAutoCompleteReferenceValue.Item) //What we display on input box
        {
          const row = document.createElement('li');

          if (item.IconUrl != '')
          {
            const img = document.createElement('img');
            img.src = item.IconUrl;
            img.alt = '';
            img.setAttribute('aria-hidden', 'true');

            const imgContainer = document.createElement('div');
            imgContainer.appendChild(img);

            row.append(imgContainer);
          }

          const displayName = document.createElement('span');
          displayName.innerText = item.DisplayName;

          const displayNameContainer = document.createElement('div');
          displayNameContainer.appendChild(displayName);
          row.append(displayNameContainer);

          return {
            html: row.innerHTML,
            class: null,
            isAnnotation: item.IsAnnotation
          };
        },
        formatMatch: function (item: Remotion.BocAutoCompleteReferenceValue.Item) //The value used by the cache
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

      hiddenField.value = this._nullValueString;
      this.UpdateIcon(this._nullValueString, errorHandler);
      //Do not fire change-event
    }).updateResult((item: Remotion.BocAutoCompleteReferenceValue.UpdateResult, out: OutBox<Remotion.BocAutoCompleteReferenceValue.UpdateResult>) =>
    {
      try
      {
        let actualItem = item;

        if (item.DisplayName.toLowerCase() == this._itemBackUp!.DisplayName.toLowerCase()
          && (item.UniqueIdentifier == this._itemBackUp!.UniqueIdentifier || item.UniqueIdentifier == this._nullValueString))
        {
          if (item.UniqueIdentifier == this._nullValueString && this._itemBackUp!.UniqueIdentifier != this._nullValueString)
            this._textbox.value = this._itemBackUp!.DisplayName; // fall back to the last confirmed user input to preserve correct casing
          else
            this._textbox.value = item.DisplayName; // keep the latest user input to preservce current casing
          actualItem = this._itemBackUp!;
        }

        const hasChanged = this._itemBackUp != actualItem;

        if (this._isInvalidated || hasChanged)
        {
          hiddenField.value = actualItem.UniqueIdentifier;
          this.BackupItemData(actualItem.UniqueIdentifier, actualItem.DisplayName);

          if (hasChanged)
          {
            this.UpdateIcon(actualItem.UniqueIdentifier, errorHandler);
            this._iconBackUp = this._icon;
            hiddenField.dispatchEvent(new Event('change'));
          }
          else
          {
            this.RestoreIcon();
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

  private RestoreIcon(): void
  {
    if (this._iconBackUp == null)
      return;

    if (this._icon == null)
      return;

    this._icon.replaceWith(this._iconBackUp);
    this._icon = this._iconBackUp;

    if (this._iconMarker != null)
      this._iconMarker.classList.add('hasIcon');
  }

  private UpdateIcon (selectedValue: string, errorHandler: (error: Sys.Net.WebServiceError) => void): void
  {
    if (!this._isIconUpdateEnabled)
      return;

    if (this._iconMarker == null)
      return;

    if (this._icon == null)
      return;

    if (this._isAutoPostBackEnabled)
    {
      this._icon = BocReferenceValueBase.UpdateIcon(this._iconMarker, this._icon, null, this._controlServiceUrl, this._iconContext, errorHandler);
    }
    else
    {
      const businessObject = selectedValue != this._nullValueString
          ? selectedValue
          : null;

      this._icon = BocReferenceValueBase.UpdateIcon(this._iconMarker, this._icon, businessObject, this._controlServiceUrl, this._iconContext, errorHandler);
    }
  }

  private ClearError(): void
  {
    if (this._textbox.classList.contains('error'))
    {
      this._textbox.classList.remove('error');
      this._textbox.hideInformationPopUp();
    }
  }

  private SetError (message: string): void
  {
    this._textbox.classList.add('error');
    this._textbox.showInformationPopUp(message);
  }

  private BackupItemData(uniqueIdentifier: string, displayName: string): void
  {
    // TODO RM-7741 - BocAutoCompleteReferenceValue.ts: Not all state of Item is backed up
    this._itemBackUp = { UniqueIdentifier: uniqueIdentifier, DisplayName: displayName, IconUrl: '', IsAnnotation: false };
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

    const hiddenField = document.getElementById(referenceValueHiddenFieldID) as Nullable<HTMLInputElement>;
    if (hiddenField == null || hiddenField.value == nullValueString)
      return 0;

    return 1;
  };
}
