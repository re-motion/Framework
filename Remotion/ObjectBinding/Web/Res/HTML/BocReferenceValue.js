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

function BocReferenceValue()
{
}

BocReferenceValue.Initialize = function (
    dropDownList,
    command,
    nullValueString,
    isAutoPostBackEnabled,
    iconServiceUrl,
    iconContext,
    commandInfo,
    resources)
{
  ArgumentUtility.CheckNotNullAndTypeIsObject('dropDownList', dropDownList);
  ArgumentUtility.CheckTypeIsObject('command', command);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('isAutoPostBackEnabled', isAutoPostBackEnabled);
  ArgumentUtility.CheckTypeIsString('iconServiceUrl', iconServiceUrl);
  ArgumentUtility.CheckTypeIsObject('iconContext', iconContext);
  ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
  ArgumentUtility.CheckNotNullAndTypeIsObject('resources', resources);

  dropDownList.change(function ()
  {
    BocReferenceValue.ClearError(dropDownList);

    if (command == null || command.length == 0)
      return;

    if (isAutoPostBackEnabled)
    {
      command = BocReferenceValueBase.UpdateCommand(command, null, null, null, null, function () { });
    }
    else
    {
      var errorHandler = function (error)
      {
        BocReferenceValue.SetError(dropDownList, resources.LoadIconFailedErrorMessage);
      };

      var businessObject = BocReferenceValue.GetSelectedValue(dropDownList, nullValueString);
      command = BocReferenceValueBase.UpdateCommand(command, businessObject, iconServiceUrl, iconContext, commandInfo, errorHandler);
    }
  });
};

BocReferenceValue.GetSelectedValue = function (dropDownList, nullValueString)
{
  ArgumentUtility.CheckNotNullAndTypeIsObject('dropDownList', dropDownList);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

  if (dropDownList.length == 0 || dropDownList.prop('selectedIndex') < 0)
    return nullValueString;
  var selectedValue = dropDownList.children()[dropDownList.prop('selectedIndex')].value;
  if (selectedValue == nullValueString)
    return null;
  return selectedValue;
};

//  Returns the number of rows selected for the specified BocList
BocReferenceValue.GetSelectionCount = function (referenceValueDropDownListID, nullValueString)
{
  ArgumentUtility.CheckNotNullAndTypeIsString('referenceValueDropDownListID', referenceValueDropDownListID);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

  var dropDownList = $('#' + referenceValueDropDownListID);
  if (BocReferenceValue.GetSelectedValue(dropDownList, nullValueString) == null)
    return 0;
  return 1;
};

BocReferenceValue.ClearError = function (dropDownList)
{
  if (dropDownList.hasClass('error'))
  {
    dropDownList.attr('title', dropDownList.data('title-backup'));
    dropDownList.removeData('title-backup');
    dropDownList.removeClass('error');
  }
};

BocReferenceValue.SetError = function (dropDownList, message)
{
  if (!dropDownList.hasClass('error'))
  {
    var oldTitle = dropDownList.attr('title');
    if (TypeUtility.IsUndefined(oldTitle))
      oldTitle = null;

    dropDownList.data('title-backup', oldTitle);
  }
  dropDownList.attr('title', message);
  dropDownList.addClass('error');
};
