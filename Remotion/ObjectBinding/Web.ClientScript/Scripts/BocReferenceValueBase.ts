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

type BocReferenceValueBase_Resources = {
  LoadIconFailedErrorMessage: string;
  LoadDataFailedErrorMessage: string;
  SearchStringForDropDownDoesNotMatchRegexMessage: string;
  NoDataFoundMessage: string;
  InternetExplorerScreenReaderLabelText: string;
};

type BocReferenceValueBase_SearchStringValidationInfo = {
  ValidSearchStringRegex: string;
  ValidSearchStringForDropDownRegex: string;
  SearchStringForDropDownDoesNotMatchRegexMessage: string;
  IgnoreSearchStringForDropDownUponValidInput: string;
};

type BocReferenceValueBase_CommandInfo = {
  href: Nullable<string>;
  target: Nullable<string>;
  onClick: Nullable<string>;
  title: Nullable<string>;
};

type BocReferenceValueBase_IconContext = {
  businessObjectClass: Nullable<string>;
  arguments: Nullable<string>;
};

type BocReferenceValueBase_ErrorHandler = (error: Sys.Net.WebServiceError) => void;

type BocReferenceValueBase_IconInformation = {
  Url: string;
  AlternateText: string;
  ToolTip: string;
  Width: number;
  Height: number;
};

class BocReferenceValueBase //TODO RM-7715 - Make the TypeScript classes BocReferenceValue and BocAutoCompleteReferenceValue inherit from BocReferenceValueBase
{
  private static _nullIconUrl: Nullable<string>

  public static InitializeGlobals (nullIconUrl: string): void
  {
    BocReferenceValueBase._nullIconUrl = nullIconUrl;
  };

  public static UpdateCommand (oldCommand: JQuery, businessObject: Nullable<string>, isIconUpdateEnabled: boolean, controlServiceUrl: Nullable<string>, iconContext: Nullable<BocReferenceValueBase_IconContext>, commandInfo: Nullable<BocReferenceValueBase_CommandInfo>, onFailure: BocReferenceValueBase_ErrorHandler): JQuery
  {
    ArgumentUtility.CheckNotNull('oldCommand', oldCommand);
    ArgumentUtility.CheckTypeIsString('businessObject', businessObject);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean('isIconUpdateEnabled', isIconUpdateEnabled);
    if (isIconUpdateEnabled)
      ArgumentUtility.CheckNotNullAndTypeIsString('controlServiceUrl', controlServiceUrl);
    if (isIconUpdateEnabled)
      ArgumentUtility.CheckNotNullAndTypeIsObject('iconContext', iconContext);
    ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
    ArgumentUtility.CheckTypeIsFunction('onFailure', onFailure);

    var newCommand = BocReferenceValueBase.CreateCommand(oldCommand, commandInfo, businessObject);

    var oldIcon = oldCommand.find('img');
    var newIcon = BocReferenceValueBase.CreateEmptyIcon(oldIcon, newCommand.prop('title'));
    newCommand.append(newIcon);

    oldCommand.replaceWith(newCommand);

    if (isIconUpdateEnabled)
    {
      var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
      if (pageRequestManager.get_isInAsyncPostBack())
        return oldCommand;

      var params: Dictionary<string> = { businessObject: businessObject! };
      for (var propertyName in iconContext) // TODO RM-7714 - Simplify copiyng JavaScript properties in TypeScript files
        params[propertyName] = (iconContext as any)[propertyName];

      WebServiceUtility.Execute<BocReferenceValueBase_IconInformation>(
          controlServiceUrl!,
          'GetIcon',
          params,
          function (result: BocReferenceValueBase_IconInformation): void
          {
            BocReferenceValueBase.UpdateIconFromWebService(newCommand, newIcon, result);
          },
          function (err: Sys.Net.WebServiceError)
          {
            onFailure(err);
            BocReferenceValueBase.ResetCommand(newCommand);
          });
    }

    return newCommand;
  };

  public static CreateCommand (oldCommand: JQuery, commandInfo: Nullable<BocReferenceValueBase_CommandInfo>, businessObject: Nullable<string>): JQuery
  {
    ArgumentUtility.CheckNotNull('oldCommand', oldCommand);
    ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
    ArgumentUtility.CheckTypeIsString('businessObject', businessObject);

    var newCommand = $('<a/>');

    var tempCommandInfo: Dictionary<Nullable<string>> = {};
    if (commandInfo != null)
    {
      tempCommandInfo = jQuery.extend(true, {}, commandInfo);
      if (businessObject == null)
      {
        tempCommandInfo.href = null;
        tempCommandInfo.target = null;
        tempCommandInfo.onClick = null;
        tempCommandInfo.title = null;
      }
      else if (commandInfo.href != null)
      {
        tempCommandInfo.href = String.format(commandInfo.href, businessObject);
      }
    }

    var oldCommandAttributes = oldCommand[0].attributes;

    for (var i = 0; i < oldCommandAttributes.length; i++)
    {
      const value = oldCommandAttributes[i].nodeValue;
      if (value != null && value != '')
        newCommand.attr(oldCommandAttributes[i].nodeName, value);
    }

    for (var property in tempCommandInfo)
    {
      const value = tempCommandInfo[property];
      if (value == null)
        newCommand.removeAttr(property);
      else
        newCommand.attr(property, value);
    }

    newCommand.removeClass('hasIcon');

    return newCommand;
  };

  public static CreateEmptyIcon (oldIcon: JQuery, title: Nullable<string>): JQuery
  {
    ArgumentUtility.CheckNotNull('oldIcon', oldIcon);
    ArgumentUtility.CheckTypeIsString('title', title);
  
    var newIcon = oldIcon.clone();
    newIcon.attr({ src: BocReferenceValueBase._nullIconUrl, alt: '' });
    newIcon.removeAttr('title');
    if (!StringUtility.IsNullOrEmpty(title))
      newIcon.attr({ title: title });
    newIcon.css({ width: oldIcon.width(), height: oldIcon.height() });
  
    return newIcon;
  };

  public static UpdateIconFromWebService (command: JQuery, icon: JQuery, iconInformation: BocReferenceValueBase_IconInformation): void
  {
    ArgumentUtility.CheckNotNull('icon', icon);
    ArgumentUtility.CheckTypeIsObject('iconInformation', iconInformation);
  
    if (iconInformation == null)
      return;
  
    icon.attr({ src: iconInformation.Url });
  
    icon.attr({ alt: '' });
    if (!StringUtility.IsNullOrEmpty(iconInformation.AlternateText))
      icon.attr({ alt: iconInformation.AlternateText });
  
    if (!StringUtility.IsNullOrEmpty(iconInformation.ToolTip) && StringUtility.IsNullOrEmpty(icon.prop('title')))
      icon.attr({ title: iconInformation.ToolTip });
  
    icon.css({ width: iconInformation.Width, height: iconInformation.Height });
  
    command.addClass('hasIcon');
  };

  public static ResetCommand (command: JQuery): void
  {
    ArgumentUtility.CheckTypeIsObject('command', command);
    command.removeAttr('href');
    command.removeAttr('onclick');
    command.removeAttr('title');
    command.removeAttr('target');
    command.removeClass('hasIcon');
  };
}
