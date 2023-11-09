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

  public static UpdateIcon (iconMarker: HTMLElement, icon: HTMLImageElement, businessObject: Nullable<string>, controlServiceUrl: Nullable<string>, iconContext: Nullable<BocReferenceValueBase_IconContext>, onFailure: BocReferenceValueBase_ErrorHandler): Nullable<HTMLImageElement>
  {
    ArgumentUtility.CheckNotNull('iconMarker', iconMarker);
    ArgumentUtility.CheckNotNull('icon', icon);
    ArgumentUtility.CheckTypeIsString('businessObject', businessObject);
    ArgumentUtility.CheckNotNullAndTypeIsString('controlServiceUrl', controlServiceUrl);
    ArgumentUtility.CheckNotNullAndTypeIsObject('iconContext', iconContext);
    ArgumentUtility.CheckTypeIsFunction('onFailure', onFailure);

    const oldIcon = icon;
    const newIcon = BocReferenceValueBase.CreateEmptyIcon(oldIcon);

    oldIcon.replaceWith(newIcon);

    const pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
    if (pageRequestManager.get_isInAsyncPostBack())
      return oldIcon;

    const params: Dictionary<string> = { businessObject: businessObject! };
    for (const propertyName in iconContext) // TODO RM-7714 - Simplify copiyng JavaScript properties in TypeScript files
      params[propertyName] = (iconContext as any)[propertyName];

    WebServiceUtility.Execute<BocReferenceValueBase_IconInformation>(
        controlServiceUrl!,
        'GetIcon',
        params,
        function (result: BocReferenceValueBase_IconInformation): void
        {
          const hasUpdatedIcon = BocReferenceValueBase.UpdateIconFromWebService(newIcon, result);
          if (hasUpdatedIcon)
            iconMarker.classList.add('hasIcon');
        },
        function (err: Sys.Net.WebServiceError)
        {
          onFailure(err);
          return
        });

    iconMarker.classList.remove('hasIcon');
    return newIcon;
  };

  public static CreateEmptyIcon (oldIcon: HTMLImageElement): HTMLImageElement
  {
    ArgumentUtility.CheckNotNull('oldIcon', oldIcon);

    const newIcon = (window.document.createElement('img') as HTMLImageElement);
    newIcon.setAttribute('src', BocReferenceValueBase._nullIconUrl!);
    newIcon.setAttribute('alt', '');
    newIcon.style.width = LayoutUtility.GetWidth(oldIcon) + 'px';
    newIcon.style.height = LayoutUtility.GetHeight(oldIcon) + 'px';
  
    return newIcon;
  };

  public static UpdateIconFromWebService (icon: HTMLImageElement, iconInformation: Nullable<BocReferenceValueBase_IconInformation>): boolean
  {
    ArgumentUtility.CheckNotNull('icon', icon);
    ArgumentUtility.CheckTypeIsObject('iconInformation', iconInformation);

    if (iconInformation == null)
      return false;

    icon.setAttribute('src', iconInformation.Url);

    icon.setAttribute('alt', '');
    if (!StringUtility.IsNullOrEmpty(iconInformation.AlternateText))
      icon.setAttribute('alt', iconInformation.AlternateText);
  
    if (!StringUtility.IsNullOrEmpty(iconInformation.ToolTip))
      icon.setAttribute('title', iconInformation.ToolTip);

    icon.style.width = iconInformation.Width;
    icon.style.height = iconInformation.Height;

    return true;
  };
}
