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

class BocReferenceValue //TODO RM-7715 - Make the TypeScript classes BocReferenceValue and BocAutoCompleteReferenceValue inherit from BocReferenceValueBase
{
  public static Initialize (
    dropDownListOrSelector: CssSelectorOrElement<HTMLSelectElement>,
    commandOrSelector: Nullable<CssSelectorOrElement<HTMLAnchorElement>>,
    nullValueString: string,
    isAutoPostBackEnabled: boolean,
    isIconUpdateEnabled: boolean,
    controlServiceUrl: Nullable<string>,
    iconContext: Nullable<BocReferenceValueBase_IconContext>,
    commandInfo: Nullable<BocReferenceValueBase_CommandInfo>,
    resources: BocReferenceValueBase_Resources): void
  {
    ArgumentUtility.CheckNotNull('dropDownListOrSelector', dropDownListOrSelector);
    ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAutoPostBackEnabled', isAutoPostBackEnabled);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isIconUpdateEnabled', isIconUpdateEnabled);
    if (isIconUpdateEnabled)
      ArgumentUtility.CheckNotNullAndTypeIsString('controlServiceUrl', controlServiceUrl);
    if (isIconUpdateEnabled)
      ArgumentUtility.CheckNotNullAndTypeIsObject('iconContext', iconContext);
    ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
    ArgumentUtility.CheckNotNullAndTypeIsObject('resources', resources);

    const dropDownList = ElementResolverUtility.ResolveSingle(dropDownListOrSelector);
    let command = commandOrSelector != null
      ? ElementResolverUtility.ResolveSingle(commandOrSelector)
      : null;

    dropDownList.addEventListener('change', function ()
    {
      BocReferenceValue.ClearError(dropDownList);

      if (command == null)
        return;

      if (isAutoPostBackEnabled)
      {
        command = BocReferenceValueBase.UpdateCommand(command, null, false, null, null, null, function () { });
      }
      else
      {
        var errorHandler = function (error: Sys.Net.WebServiceError)
        {
          BocReferenceValue.SetError(dropDownList, resources.LoadIconFailedErrorMessage);
        };

        var businessObject = BocReferenceValue.GetSelectedValue(dropDownList, nullValueString);
        command = BocReferenceValueBase.UpdateCommand(command, businessObject, isIconUpdateEnabled, controlServiceUrl, iconContext, commandInfo, errorHandler);
      }
    });
  };

  public static GetSelectedValue(dropDownList: Nullable<HTMLSelectElement>, nullValueString: string): Nullable<string>
  {
    ArgumentUtility.CheckTypeIsObject('dropDownList', dropDownList);
    ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

    if (dropDownList === null || dropDownList.selectedIndex < 0)
      return nullValueString;
    var selectedValue = (dropDownList.children[dropDownList.selectedIndex] as HTMLInputElement).value;
    if (selectedValue == nullValueString)
      return null;
    return selectedValue;
  };

  //  Returns the number of rows selected for the specified BocList
  public static GetSelectionCount(referenceValueDropDownListID: string, nullValueString: string): number
  {
    ArgumentUtility.CheckNotNullAndTypeIsString('referenceValueDropDownListID', referenceValueDropDownListID);
    ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

    var dropDownList = document.getElementById(referenceValueDropDownListID) as Nullable<HTMLSelectElement>;
    if (BocReferenceValue.GetSelectedValue(dropDownList, nullValueString) == null)
      return 0;
    return 1;
  };

  public static ClearError(dropDownList: HTMLSelectElement): void
  {
    if (dropDownList.classList.contains('error'))
    {
      const titleBackup = dropDownList.dataset['title-backup'];
      if (titleBackup !== undefined)
      {
        dropDownList.setAttribute('title', titleBackup);
      }
      else
      {
        dropDownList.removeAttribute('title');
      }
      delete dropDownList.dataset['title-backup'];
      dropDownList.classList.remove('error');
    }
  };

  public static SetError(dropDownList: HTMLSelectElement, message: string): void
  {
    if (!dropDownList.classList.contains('error'))
    {
      var oldTitle: Nullable<string> = dropDownList.getAttribute('title');
      if (oldTitle !== null)
      {
        dropDownList.dataset['title-backup'] = oldTitle;
      }
      else
      {
        delete dropDownList.dataset['title-backup'];
      }
    }
    dropDownList.setAttribute('title', message);
    dropDownList.classList.add('error');
  };
}
