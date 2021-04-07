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
  Width: string;
  Height: string;
};

class BocReferenceValueBase //TODO RM-7715 - Make the TypeScript classes BocReferenceValue and BocAutoCompleteReferenceValue inherit from BocReferenceValueBase
{
  private static _nullIconUrl: Nullable<string>

  public static InitializeGlobals (nullIconUrl: string): void
  {
    BocReferenceValueBase._nullIconUrl = nullIconUrl;
  };

  public static UpdateCommand (oldCommand: HTMLAnchorElement, businessObject: Nullable<string>, isIconUpdateEnabled: boolean, controlServiceUrl: Nullable<string>, iconContext: Nullable<BocReferenceValueBase_IconContext>, commandInfo: Nullable<BocReferenceValueBase_CommandInfo>, onFailure: BocReferenceValueBase_ErrorHandler): HTMLAnchorElement
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

    var oldIcon = oldCommand.querySelector('img')!;
    var newIcon = BocReferenceValueBase.CreateEmptyIcon(oldIcon, newCommand.title);
    newCommand.appendChild(newIcon);

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

  public static CreateCommand (oldCommand: HTMLAnchorElement, commandInfo: Nullable<BocReferenceValueBase_CommandInfo>, businessObject: Nullable<string>): HTMLAnchorElement
  {
    ArgumentUtility.CheckNotNull('oldCommand', oldCommand);
    ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
    ArgumentUtility.CheckTypeIsString('businessObject', businessObject);

    var newCommand = document.createElement('a');

    var tempCommandInfo: Dictionary<Nullable<string>> = {};
    if (commandInfo != null)
    {
      tempCommandInfo = Object.assign({}, commandInfo);
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

    var oldCommandAttributes = oldCommand.attributes;

    for (var i = 0; i < oldCommandAttributes.length; i++)
    {
      const value = oldCommandAttributes[i].nodeValue;
      if (value != null && value != '')
        newCommand.setAttribute(oldCommandAttributes[i].nodeName, value);
    }

    for (var property in tempCommandInfo)
    {
      const value = tempCommandInfo[property];
      if (value == null)
        newCommand.removeAttribute(property);
      else
        newCommand.setAttribute(property, value);
    }

    newCommand.classList.remove('hasIcon');

    return newCommand;
  };

  public static CreateEmptyIcon (oldIcon: HTMLImageElement, title: Nullable<string>): HTMLImageElement
  {
    ArgumentUtility.CheckNotNull('oldIcon', oldIcon);
    ArgumentUtility.CheckTypeIsString('title', title);
  
    var newIcon = oldIcon.cloneNode(true) as HTMLImageElement;
    
    newIcon.setAttribute('src', BocReferenceValueBase._nullIconUrl!);

    newIcon.setAttribute('alt', '');
    newIcon.removeAttribute('title');
    if (!StringUtility.IsNullOrEmpty(title))
      newIcon.setAttribute('title', title);

    newIcon.style.width = LayoutUtility.GetWidth(oldIcon) + 'px';
    newIcon.style.height = LayoutUtility.GetHeight(oldIcon) + 'px';
  
    return newIcon;
  };

  public static UpdateIconFromWebService (command: HTMLElement, icon: HTMLImageElement, iconInformation: Nullable<BocReferenceValueBase_IconInformation>): void
  {
    ArgumentUtility.CheckNotNull('icon', icon);
    ArgumentUtility.CheckTypeIsObject('iconInformation', iconInformation);
  
    if (iconInformation == null)
      return;
  
    icon.setAttribute('src', iconInformation.Url);
  
    icon.setAttribute('alt', '');
    if (!StringUtility.IsNullOrEmpty(iconInformation.AlternateText))
      icon.setAttribute('alt', iconInformation.AlternateText);
  
    if (!StringUtility.IsNullOrEmpty(iconInformation.ToolTip) && StringUtility.IsNullOrEmpty(icon.title))
      icon.setAttribute('title', iconInformation.ToolTip);
  
    icon.style.width = iconInformation.Width;
    icon.style.height = iconInformation.Height;
  
    command.classList.add('hasIcon');
  };

  public static ResetCommand (command: Nullable<HTMLElement>): void
  {
    ArgumentUtility.CheckTypeIsObject('command', command);

    if (command === null)
      return;

    command.removeAttribute('href');
    command.removeAttribute('onclick');
    command.removeAttribute('title');
    command.removeAttribute('target');
    command.classList.remove('hasIcon');
  };
}
