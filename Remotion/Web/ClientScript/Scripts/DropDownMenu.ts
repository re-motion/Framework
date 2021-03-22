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

interface DropDownMenuResources
{
  LoadFailedErrorMessage: string;
  LoadingStatusMessage: string;
}

type DropDownMenu_MenuInfo_LoadMenuItemsSuccessHandler = (itemInfos: DropDownMenu_ItemInfo[]) => void;
type DropDownMenu_MenuInfo_LoadMenuItemsErrorHandler = () => void;
type DropDownMenu_MenuInfo_LoadMenuItemsCallback = (successHandler: DropDownMenu_MenuInfo_LoadMenuItemsSuccessHandler, errorHandler: DropDownMenu_MenuInfo_LoadMenuItemsErrorHandler) => void;
type DropDownMenu_MenuInfo_LoadMenuItemStatusSuccessHandler = (itemInfos: DropDownMenu_ItemStatus[]) => void;
type DropDownMenu_MenuInfo_LoadMenuItemStatusErrorHandler = () => void;
type DropDownMenu_MenuInfo_LoadMenuItemStatusCallback = (successHandler: DropDownMenu_MenuInfo_LoadMenuItemStatusSuccessHandler, errorHandler: DropDownMenu_MenuInfo_LoadMenuItemStatusErrorHandler) => void;
type DropDownMenu_ItemClickHandler = (event: Event) => boolean;
type DropDownMenu_SelectionCountGetter = () => number;
type DropDownMenu_TitleDivGetter = () => HTMLElement;

class DropDownMenu_MenuInfo
{
  public readonly ID: string;
  public ItemInfos: Nullable<DropDownMenu_ItemInfo[]>;
  public readonly LoadMenuItems: DropDownMenu_MenuInfo_LoadMenuItemsCallback;
  public readonly Resources: DropDownMenuResources;

  constructor (id: string, loadMenuItems: DropDownMenu_MenuInfo_LoadMenuItemsCallback, resources: DropDownMenuResources)
  {
    ArgumentUtility.CheckNotNullAndTypeIsString ('id', id);
    ArgumentUtility.CheckNotNullAndTypeIsFunction ('loadMenuItems', loadMenuItems);
    ArgumentUtility.CheckNotNullAndTypeIsObject ('resources', resources);

    this.ID = id;
    this.ItemInfos = null;
    this.LoadMenuItems = loadMenuItems;
    this.Resources = resources;
  }
}

class DropDownMenu_ItemInfo
{
  constructor (
      public readonly ID: Nullable<string>,
      public readonly Category: string,
      public readonly Text: Nullable<string>,
      public readonly Icon: Nullable<string>,
      public readonly IconDisabled: Nullable<string>,
      public readonly RequiredSelection: number,
      public IsDisabled: boolean,
      public readonly Href: Nullable<string>,
      public readonly Target: Nullable<string>,
      public readonly DiagnosticMetadata: Nullable<Dictionary<string | boolean>>,
      public readonly DiagnosticMetadataForCommand: Nullable<Dictionary<string | boolean>>)
  {
  }
}

interface DropDownMenu_ItemStatus
{
  ID: string;
  IsDisabled: boolean;
}

class DropDownMenu
{
  private static _menuInfos: Dictionary<DropDownMenu_MenuInfo> = {};
  private static _button_timestampDataKey: string = 'clickTimestamp';
  private static _buttonClassName: string = 'DropDownMenuSelect';
  private static _itemClassName: string = 'DropDownMenuItem';
  private static _itemDisabledClassName: string = 'DropDownMenuItemDisabled';
  private static _itemIconClassName: string = 'DropDownMenuItemIcon';
  private static _itemSeparatorClassName: string = 'DropDownMenuSeparator';
  private static _itemSelectedClassName: string = 'selected';
  private static _focusClassName: string = 'focus';
  private static _nestedHoverClassName: string = 'nestedHover';
  private static _currentMenu: Nullable<HTMLElement> = null;
  private static _currentPopup: Nullable<HTMLDivElement> = null;
  private static _currentStatusPopup: Nullable<HTMLDivElement> = null;
  private static _currentLoadOperation: Nullable<Object> = null;
  private static _menuItemIDPrefix: string = 'menuItem_';
  private static _requiredSelectionAny: number = 0;
  private static _requiredSelectionExactlyOne: number = 1;
  private static _requiredSelectionOneOrMore: number = 2;
  private static _itemClicked: boolean = false;
  private static _repositionInterval: number = 200;
  private static _repositionTimer: Nullable<number> = null;
  private static _statusPopupDelay: number = 200;
  private static _statusPopupRepositionTimer: Nullable<number> = null;
  private static _blurTimer: Nullable<number> = null;
  private static _updateFocus: boolean = true;

  public static AddMenuInfo(menuInfo: DropDownMenu_MenuInfo): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsObject ('menuInfo', menuInfo);

    DropDownMenu._menuInfos[menuInfo.ID] = menuInfo;
  }

  public static BindOpenEvent (openTargetOrSelector: CssSelectorOrElement<HTMLElement>, menuID: string, eventType: string, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, moveToMousePosition: boolean): void
  {
    ArgumentUtility.CheckNotNull ('openTargetOrSelector', openTargetOrSelector);
    ArgumentUtility.CheckNotNullAndTypeIsString ('menuID', menuID);
    ArgumentUtility.CheckNotNullAndTypeIsString ('eventType', eventType);
    ArgumentUtility.CheckTypeIsFunction ('getSelectionCount', getSelectionCount);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('moveToMousePosition', moveToMousePosition);

    const openTarget = ElementResolverUtility.ResolveSingle (openTargetOrSelector);

    var menuButton: HTMLAnchorElement;

    if (openTarget.tagName.toLowerCase() === 'a')
    {
      menuButton = openTarget as HTMLAnchorElement;
    }
    else
    {
      const menuButtons = openTarget.querySelectorAll<HTMLAnchorElement> ('a[href]');
      menuButton = menuButtons[menuButtons.length - 1];
    }

    menuButton.setAttribute ('aria-haspopup', 'menu');
    menuButton.setAttribute ('aria-expanded', 'false');

    openTarget.addEventListener (
        eventType,
        function (evt)
        {
          menuButton.focus();
          DropDownMenu.OnClick (openTarget, menuID, getSelectionCount, moveToMousePosition ? evt as MouseEvent : null);
        });

    if (!moveToMousePosition)
    {
      menuButton.addEventListener ("focus", function () { openTarget.querySelectorAll ('.' + DropDownMenu._buttonClassName).forEach (b => b.classList.add (DropDownMenu._focusClassName)); })
      menuButton.addEventListener ("blur", function () { openTarget.querySelectorAll ('.' + DropDownMenu._buttonClassName).forEach (b => b.classList.remove (DropDownMenu._focusClassName)); });

      var allButMenuButton = Array.from (openTarget.querySelectorAll ('a[href]')).filter (b => menuButton !== b);
      allButMenuButton.forEach (b => b.addEventListener ("mouseover", function () { openTarget.querySelectorAll ('.' + DropDownMenu._buttonClassName).forEach (b => b.classList.add (DropDownMenu._nestedHoverClassName)); }));
      allButMenuButton.forEach (b => b.addEventListener ("mouseout", function () { openTarget.querySelectorAll ('.' + DropDownMenu._buttonClassName).forEach (b => b.classList.remove (DropDownMenu._nestedHoverClassName)); }));
    }
  }

  public static LoadFilteredMenuItems(
      itemInfos: DropDownMenu_ItemInfo[],
      loadMenuItemStatus: Nullable<DropDownMenu_MenuInfo_LoadMenuItemStatusCallback>,
      onSuccess: DropDownMenu_MenuInfo_LoadMenuItemsSuccessHandler,
      onError: DropDownMenu_MenuInfo_LoadMenuItemsErrorHandler): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsObject ('itemInfos', itemInfos);
    ArgumentUtility.CheckTypeIsFunction ('loadMenuItemStatus', loadMenuItemStatus);
    ArgumentUtility.CheckNotNullAndTypeIsFunction ('onSuccess', onSuccess);
    ArgumentUtility.CheckNotNullAndTypeIsFunction ('onError', onError);

    if (loadMenuItemStatus !== null)
    {
      const currentLoadOperationID = new Object();
      const onSuccessForLoadMenuItemStatus = function (itemStatusArray: DropDownMenu_ItemStatus[])
      {
        if (DropDownMenu._currentLoadOperation !== currentLoadOperationID)
          return;
        DropDownMenu._currentLoadOperation = null;

        const itemStatusMap = itemStatusArray.reduce(
          function (map: Dictionary<DropDownMenu_ItemStatus>, itemStatus: DropDownMenu_ItemStatus)
          {
            map[itemStatus.ID] = itemStatus;
            return map;
          },
          {});

        let previousVisibleItemIsSeparator = false;
        for (var i = itemInfos.length - 1; i >= 0; i--)
        {
          const itemInfo = itemInfos[i];
          const hasFilterOption = itemInfo.ID !== null;
          const isSeparator = !hasFilterOption && itemInfo.Text === '-';
          if (hasFilterOption)
          {
            const itemStatus = itemStatusMap[itemInfo.ID!]!;
            const isVisible = itemStatus !== undefined;
            if (isVisible)
            {
              itemInfo.IsDisabled = itemStatus.IsDisabled;
              previousVisibleItemIsSeparator = false;
            }
            else
            {
              itemInfos.splice (i, 1);
            }
          }
          else if (isSeparator)
          {
            if (previousVisibleItemIsSeparator)
              itemInfos.splice(i, 1);

            previousVisibleItemIsSeparator = true;
          }
          else
          {
            previousVisibleItemIsSeparator = false;
          }
        }

        onSuccess (itemInfos);
      };

      DropDownMenu._currentLoadOperation = currentLoadOperationID;
      loadMenuItemStatus (onSuccessForLoadMenuItemStatus, onError);
    }
    else
    {
      onSuccess (itemInfos);
    }
  }

  private static OnClick (context: HTMLElement, menuID: string, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, evt: Nullable<MouseEvent>): void
  {
    document.getElementById (menuID)!.dataset[DropDownMenu._button_timestampDataKey] = new Date().getTime().toString();

    if (DropDownMenu._itemClicked)
    {
      DropDownMenu._itemClicked = false;
      return;
    }
    if (DropDownMenu._currentMenu !== context)
    {
      DropDownMenu.ClosePopUp(!DropDownMenu._updateFocus);
    }
    if (DropDownMenu._currentMenu == null)
    {
      var event: Nullable<MouseEvent> = null;
      if (evt != null)
      {
        var bounds = context.getBoundingClientRect();
        if (bounds.left <= evt.clientX &&
          bounds.right >= evt.clientX &&
          bounds.top <= evt.clientY &&
          bounds.bottom >= evt.clientY)
          event = evt;
      }
      DropDownMenu.OpenPopUp(menuID, context, getSelectionCount, event);
      DropDownMenu._currentMenu = context;
    }
  }

  private static OpenPopUp (menuID: string, context: HTMLElement, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, evt: Nullable<MouseEvent>): void
  {
    let selectionCount = -1;
    if (getSelectionCount !== null)
      selectionCount = getSelectionCount();

    const menuInfo = DropDownMenu._menuInfos[menuID]!;
    DropDownMenu.BeginOpenPopUp(menuID, context, evt);
    if (menuInfo.ItemInfos === null)
    {
      const statusPopup = DropDownMenu._currentStatusPopup;
      const statusPopupTimer = setTimeout(
        function () {
          if (DropDownMenu._currentStatusPopup && DropDownMenu._currentStatusPopup === statusPopup)
          {
            statusPopup.setAttribute('role', 'alert');
            statusPopup.className = 'DropDownMenuStatus loading';
            const label = document.createElement('div');
            label.setAttribute('aria-label', menuInfo.Resources.LoadingStatusMessage);
            statusPopup.appendChild (label);
            statusPopup.style.display = 'block';
          }
        },
        DropDownMenu._statusPopupDelay);
      const onSuccess = function (itemInfos: DropDownMenu_ItemInfo[])
      {
        menuInfo.ItemInfos = itemInfos;
        clearTimeout(statusPopupTimer);
        DropDownMenu.EndOpenPopUp(menuID, context, selectionCount, evt, itemInfos);
      };

      const onError = function ()
      {
        clearTimeout(statusPopupTimer);
        if (DropDownMenu._currentStatusPopup && DropDownMenu._currentStatusPopup === statusPopup)
        {
          if (statusPopup.childElementCount > 0 && (statusPopup.lastChild as HTMLElement).tagName.toUpperCase() !== 'IFRAME')
            statusPopup.removeChild(statusPopup.lastChild!);
          // Remove and re-add the element to force the screenreader (JAWS) to always announce the updated value immediately.
          const statusPopupParent = statusPopup.parentElement!;
          statusPopupParent.removeChild(statusPopup);
          statusPopupParent.appendChild(statusPopup);

          statusPopup.setAttribute('role', 'alert');
          statusPopup.className = 'DropDownMenuStatus error';
          const label = document.createElement('div');
          // In some instances, the screenreader will read the label after the text. 
          // By adding space, we can ensure that trailing punctuation marks are not announced.
          label.innerText = menuInfo.Resources.LoadFailedErrorMessage + ' ';
          statusPopup.appendChild(label);
          statusPopup.style.display = 'block';
        }
      };
      menuInfo.LoadMenuItems(onSuccess, onError);
    }
    else
    {
      const itemInfos = menuInfo.ItemInfos;
      DropDownMenu.EndOpenPopUp(menuID, context, selectionCount, evt, itemInfos);
    }
  }

  private static BeginOpenPopUp (menuID: string, context: HTMLElement, evt: Nullable<MouseEvent>): void
  {
    const menuButton = document.querySelector<HTMLAnchorElement> ('#' + menuID + ' a[aria-haspopup=menu]')!;

    const titleDivFunc = DropDownMenu.CreateTitleDivGetter(context);
    const statusPopup = document.createElement('div');
    statusPopup.className = 'DropDownMenuStatus';
    statusPopup.id = menuID + '_DropDownMenuStatus';
    statusPopup.style.position = 'absolute';
    statusPopup.style.display = 'none';
    statusPopup.setAttribute('aria-atomic', 'true');
    // Do not set role=alert before it is needed prevent an alert-update during 'normal' showing of menu.
    //statusPopup.setAttribute('role', 'alert');
    if (menuButton.getAttribute ('aria-labelledby') !== null)
      statusPopup.setAttribute ('aria-labelledby', menuButton.getAttribute ('aria-labelledby')!);
    else
      statusPopup.setAttribute ('aria-labelledby', menuButton.id);
    DropDownMenu._currentStatusPopup = statusPopup;
    document.getElementById (menuID)!.closest ('div, td, th, body')!.append (statusPopup);

    DropDownMenu.ApplyPosition (statusPopup, evt, titleDivFunc());

    if (DropDownMenu._statusPopupRepositionTimer)
      clearTimeout(DropDownMenu._statusPopupRepositionTimer);
    const repositionHandler = function ()
    {
      if (DropDownMenu._statusPopupRepositionTimer)
        clearTimeout(DropDownMenu._statusPopupRepositionTimer);

      if (DropDownMenu._currentStatusPopup && DropDownMenu._currentStatusPopup === statusPopup)
      {
        DropDownMenu.ApplyPosition (statusPopup, null, titleDivFunc());
        DropDownMenu._statusPopupRepositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);
      }
    };

    // Only reposition if opened via titleDiv
    if (evt === null)
      DropDownMenu._statusPopupRepositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);

    menuButton.addEventListener ('focus', DropDownMenu.FocusHandler);
    menuButton.addEventListener ('blur', DropDownMenu.BlurHandler);
  }

  private static EndOpenPopUp (menuID: string, context: HTMLElement, selectionCount: number, evt: Nullable<MouseEvent>, itemInfos: DropDownMenu_ItemInfo[]): void
  {
    var menuOptionsID = menuID + '_DropDownMenuOptions';
    var menuButtonAnchor = document.querySelector<HTMLAnchorElement> ('#' + menuID + ' a[aria-haspopup=menu]')!;

    if (itemInfos.length == 0)
      return;

    if (DropDownMenu._statusPopupRepositionTimer)
      clearTimeout(DropDownMenu._statusPopupRepositionTimer);
    if (DropDownMenu._currentStatusPopup !== null)
    {
      var statusPopup = DropDownMenu._currentStatusPopup;
      DropDownMenu._currentStatusPopup = null;
      // Clear the role=alert before removing to item to prevent screenreaders (JAWS) from announcing the old value during removal.
      statusPopup.removeAttribute('role');
      statusPopup.parentElement!.removeChild(statusPopup);
    }

    menuButtonAnchor.setAttribute ('aria-controls', menuOptionsID);
    menuButtonAnchor.setAttribute ('aria-expanded', 'true');

    var div = document.createElement('div');
    div.className = 'DropDownMenuOptions';
    div.id = menuOptionsID;
    div.setAttribute ('role', 'menu');
    div.setAttribute ('tabindex', '-1');
    if (menuButtonAnchor.getAttribute ('aria-labelledby') !== null)
      div.setAttribute ('aria-labelledby', menuButtonAnchor.getAttribute ('aria-labelledby')!);
    else
      div.setAttribute ('aria-labelledby', menuButtonAnchor.id);
    DropDownMenu._currentPopup = div;

    var ul = document.createElement('ul');
    ul.className = 'DropDownMenuOptions';
    ul.setAttribute('role', 'none');
    div.appendChild(ul);

    const menuButton = document.getElementById (menuID)!;
    menuButton.closest ('div, td, th, body')!.append (div);

    ul.addEventListener ('mouseover', function (event)
    {
      var eventTarget = DropDownMenu.GetTarget (event, "LI");
      ul.querySelectorAll ("li").forEach (b => b.classList.remove (DropDownMenu._itemSelectedClassName));
      if (eventTarget.firstChild != null && eventTarget.firstChild.nodeName.toLowerCase() === 'a')
      {
        eventTarget.classList.add (DropDownMenu._itemSelectedClassName);
        (eventTarget.firstChild as HTMLElement).focus();
      }
    });
    ul.addEventListener ('keydown', function (event)
    {
      // TODO RM-7707 DropDownMenu._currentMenu should not be null
      DropDownMenu.Options_OnKeyDown (event, DropDownMenu._currentMenu!);
    });

    for (let i = 0; i < itemInfos.length; i++)
    {
      var item = DropDownMenu.CreateItem(itemInfos[i], selectionCount);
      if (item != null)
        ul.appendChild(item);
    }

    var titleDivFunc = DropDownMenu.CreateTitleDivGetter (context);
    DropDownMenu.ApplyPosition (div, evt, titleDivFunc());

    if (DropDownMenu._repositionTimer) 
      clearTimeout(DropDownMenu._repositionTimer);
    var repositionHandler = function ()
    {
      if (DropDownMenu._repositionTimer)
        clearTimeout (DropDownMenu._repositionTimer);

      if (DropDownMenu._currentPopup && DropDownMenu._currentPopup == div && LayoutUtility.IsVisible (div))
      {
        DropDownMenu.ApplyPosition (div, null, titleDivFunc());
        DropDownMenu._repositionTimer = setTimeout (repositionHandler, DropDownMenu._repositionInterval);
      }
    };

    // Only reposition if opened via titleDiv
    if (evt == null)
      DropDownMenu._repositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);

    if (menuButton.dataset[DropDownMenu._button_timestampDataKey] !== undefined)
    {
      delete menuButton.dataset[DropDownMenu._button_timestampDataKey];
    }
    else
    {
      DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
    }
  }

  private static CreateTitleDivGetter (context: HTMLElement): DropDownMenu_TitleDivGetter
  {
    const contextID = context.id;
    if (contextID == null)
    {
      return function ()
      {
        return context.firstElementChild as HTMLElement;
      };
    }
    else
    {
      return function ()
      {
        // TODO RM-7709 Check if null could be returned
        return document.getElementById (contextID)!.firstElementChild as HTMLElement;
      };
    }
  }

  private static ApplyPosition (popUpDiv: HTMLDivElement, clickEvent: Nullable<MouseEvent>, referenceElement: HTMLElement): void
  {
    var space_top = Math.round (LayoutUtility.GetOffset (referenceElement).top - window.pageYOffset);
    var space_bottom = Math.round (document.documentElement.clientHeight - LayoutUtility.GetOffset (referenceElement).top - LayoutUtility.GetHeight (referenceElement) + window.pageYOffset);
    var space_left = LayoutUtility.GetOffset (referenceElement).left;
    var space_right = document.documentElement.clientWidth - LayoutUtility.GetOffset (referenceElement).left - LayoutUtility.GetWidth (referenceElement);

    // position drop-down list
    var top = clickEvent ? clickEvent.clientY : Math.max (0, space_top + referenceElement.offsetHeight);
    var left = clickEvent ? clickEvent.clientX : 'auto';
    var right = clickEvent ? 'auto' : Math.max (0, document.documentElement.clientWidth - LayoutUtility.GetOffset (referenceElement).left - referenceElement.offsetWidth);

    popUpDiv.style.top = top + 'px';
    popUpDiv.style.bottom = 'auto';
    popUpDiv.style.right = right + 'px';
    popUpDiv.style.left = left + 'px';
    popUpDiv.style.position = 'fixed';

    // move dropdown if there is not enough space to fit it on the page
    if ((LayoutUtility.GetWidth (popUpDiv) > space_left) && (space_left < space_right))
    {
      if (LayoutUtility.GetOffset (popUpDiv).left < 0)
      {
        left = Math.max (0, LayoutUtility.GetOffset (referenceElement).left);
        popUpDiv.style.left = left + 'px';
        popUpDiv.style.right = 'auto';
      }
    }
    if (LayoutUtility.GetHeight (popUpDiv) > space_bottom)
    {
      if (LayoutUtility.GetHeight (popUpDiv) > document.documentElement.clientHeight)
      {
        popUpDiv.style.top = '0';
      }
      else if (space_top > LayoutUtility.GetHeight (popUpDiv))
      {
        var bottom = Math.max (0, document.documentElement.clientHeight - LayoutUtility.GetOffset (referenceElement).top - (referenceElement.offsetHeight - LayoutUtility.GetHeight (referenceElement)));

        popUpDiv.style.top = 'auto';
        popUpDiv.style.bottom = bottom + 'px';
      }
      else
      {
        popUpDiv.style.top = 'auto';
        popUpDiv.style.bottom = '0';
      }
    }
  }

  public static ClosePopUp (updateFocus: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('updateFocus', updateFocus);

    if (DropDownMenu._blurTimer !== null)
      clearTimeout(DropDownMenu._blurTimer);

    if (DropDownMenu._currentPopup !== null
        && document.body.contains(DropDownMenu._currentPopup))
    {
      const menuPopup = DropDownMenu._currentPopup;
      const menuButton = document.querySelector<HTMLAnchorElement> ('a[aria-controls="' + menuPopup.id + '"]')!;
      menuButton.removeEventListener ('focus', DropDownMenu.BlurHandler);
      menuButton.removeEventListener ('blur', DropDownMenu.BlurHandler);
      menuButton.setAttribute('aria-expanded', 'false');
      menuButton.removeAttribute('aria-controls');
      if (updateFocus === DropDownMenu._updateFocus)
        menuButton.focus();
      menuPopup.parentElement!.removeChild(menuPopup);
    }

    if (DropDownMenu._currentStatusPopup !== null
        && document.body.contains(DropDownMenu._currentStatusPopup))
    {
      const statusPopup = DropDownMenu._currentStatusPopup;
      // Clear the role=alert before removing to item to prevent screenreaders (JAWS) from announcing the old value during removal.
      statusPopup.removeAttribute('role');
      statusPopup.parentElement!.removeChild(statusPopup);
    }

    DropDownMenu._currentPopup = null;
    DropDownMenu._currentStatusPopup = null;
    DropDownMenu._currentMenu = null;
  }

  private static CreateItem(itemInfo: Nullable<DropDownMenu_ItemInfo>, selectionCount: number): Nullable<HTMLLIElement>
  {
    if (itemInfo == null)
      return null;

    var item;
    if (itemInfo.Text == '-')
      item = DropDownMenu.CreateSeparatorItem();
    else
      item = DropDownMenu.CreateTextItem(itemInfo, selectionCount);

    return item;
  }

  private static CreateTextItem(itemInfo: DropDownMenu_ItemInfo, selectionCount: number): HTMLLIElement
  {
    var isEnabled = DropDownMenu.GetItemEnabled(itemInfo, selectionCount);

    var item = document.createElement("li");

    var className = DropDownMenu._itemClassName;
    if (!isEnabled)
      className = DropDownMenu._itemDisabledClassName;

    item.className = className;
    item.setAttribute('role', 'none');

    var anchor = document.createElement("a");
    anchor.setAttribute('role', 'menuitem');
    anchor.setAttribute('tabindex', '-1');
    if (isEnabled)
    {
      anchor.setAttribute('href', '#');
    }
    else
    {
      anchor.setAttribute('aria-disabled', 'true');
    }

    anchor.addEventListener ('click', DropDownMenu.OnItemClick);

    item.appendChild(anchor);
    if (isEnabled && itemInfo.Href != null)
    {
      var isJavaScript = itemInfo.Href.toLowerCase().indexOf('javascript:') >= 0;
      if (isJavaScript)
      {
        anchor.setAttribute('javascript', itemInfo.Href);
      }
      else
      {
        var href = itemInfo.Href;
        var target;
        if (itemInfo.Target != null && itemInfo.Target.length > 0)
          target = itemInfo.Target;
        else
          target = '_self';
        anchor.setAttribute('javascript', 'window.open (\'' + href + '\', \'' + target + '\');');
      }
    }

    if (itemInfo.Icon != null)
    {
      var icon = document.createElement('img');
      if (isEnabled || itemInfo.IconDisabled == null)
        icon.src = itemInfo.Icon;
      else
        icon.src = itemInfo.IconDisabled;

      icon.className = DropDownMenu._itemIconClassName;
      icon.style.verticalAlign = 'middle';
      icon.alt = '';
      icon.setAttribute('aria-hidden', 'true');
      anchor.appendChild(icon);
    }
    else
    {
      var iconPlaceholder = document.createElement('span');
      iconPlaceholder.className = DropDownMenu._itemIconClassName;
      anchor.appendChild(iconPlaceholder);
    }

    if (itemInfo.DiagnosticMetadata)
    {
      for (const [key, value] of Object.entries (itemInfo.DiagnosticMetadata))
      {
        if (value !== "" && value !== null && value !== undefined)
        {
          item.setAttribute (key, value.toString());
        }
      }
    }

    if (itemInfo.DiagnosticMetadataForCommand)
    {
      itemInfo.DiagnosticMetadataForCommand['data-is-disabled'] = isEnabled ? 'false' : 'true';

      for (const [key, value] of Object.entries (itemInfo.DiagnosticMetadataForCommand))
      {
        if (value !== "" && value !== null && value !== undefined)
        {
          anchor.setAttribute (key, value.toString());
        }
      }
    }

    var span = document.createElement('span');
    span.innerHTML = itemInfo.Text!;
    anchor.appendChild(span);
    anchor.addEventListener ('focus', DropDownMenu.FocusHandler);
    anchor.addEventListener ('blur', DropDownMenu.BlurHandler);

    return item;
  }

  private static CreateSeparatorItem(): HTMLLIElement
  {
    var item = document.createElement('li');

    var textPane = document.createElement('span');
    textPane.className = DropDownMenu._itemSeparatorClassName;
    item.setAttribute('role', 'none');
    textPane.innerHTML = '&nbsp;';

    item.appendChild(textPane);

    return item;
  }

  public static OnHeadControlClick(): void
  {
    DropDownMenu._itemClicked = true;
  }

  private static OnItemClick (this: HTMLAnchorElement): boolean
  {
    if (this.href == null || this.href === '')
      return false;

    DropDownMenu._itemClicked = true;
    DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
    try
    {
      eval(this.getAttribute('javascript')!);
    }
    catch (e)
    {
    }
    setTimeout (function () { DropDownMenu._itemClicked = false; }, 10);
    return false;
  }

  public static OnKeyDown (event: KeyboardEvent, dropDownMenuOrSelector: CssSelectorOrElement<HTMLAnchorElement>, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, hasDedicatedDropDownMenuElement: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsObject ('event', event);
    ArgumentUtility.CheckNotNull ('dropDownMenuOrSelector', dropDownMenuOrSelector);
    ArgumentUtility.CheckTypeIsFunction ('getSelectionCount', getSelectionCount);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('hasDedicatedDropDownMenuElement', hasDedicatedDropDownMenuElement);

    const dropDownMenu = ElementResolverUtility.ResolveSingle (dropDownMenuOrSelector);

    if (DropDownMenu._currentMenu === null && !hasDedicatedDropDownMenuElement)
      return;

    switch (event.keyCode)
    {
      case 9: // tab
        DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
        return;
      case 13: //enter
      case 32: //space
        event.preventDefault();
        event.stopPropagation();
        if (dropDownMenu !== DropDownMenu._currentMenu)
        {
          DropDownMenu.OnClick (dropDownMenu, dropDownMenu.id, getSelectionCount, null);
          (event.keyCode as number) = 40; // always act as if the down-arrow was used when opening the drop down menu.
          DropDownMenu.Options_OnKeyDown (event, dropDownMenu);
        }
        else
        {
          DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
        }
        return;
      case 27: //escape
        event.preventDefault();
        event.stopPropagation();
        DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
        return;
      case 38: // up arrow
      case 40: // down arrow
        event.preventDefault();
        event.stopPropagation();
        if (dropDownMenu !== DropDownMenu._currentMenu)
        {
          DropDownMenu.OnClick (dropDownMenu, dropDownMenu.id, getSelectionCount, null);
          (event.keyCode as number) = 40; // always act as if the down-arrow was used when opening the drop down menu.
        }
        DropDownMenu.Options_OnKeyDown (event, dropDownMenu);
        return;
      default:
        DropDownMenu.Options_OnKeyDown(event, dropDownMenu);
        return;
    }
  }

  private static Options_OnKeyDown (event: KeyboardEvent, dropDownMenu: HTMLElement): void
  {
    if (DropDownMenu._currentPopup == null)
      return;

    var itemInfos = DropDownMenu._menuInfos[dropDownMenu.id]!.ItemInfos!;
    if (itemInfos.length === 0)
      return;

    var oldIndex;
    var isSelectionUpdated = false;
    var dropDownMenuItems = Array.from (DropDownMenu._currentPopup.querySelector ('ul')!.children);
    var currentItemIndex = dropDownMenuItems.findIndex (i => i.classList.contains (DropDownMenu._itemSelectedClassName))

    switch (event.keyCode)
    {
      case 9: // tab
        DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
        return;
      case 13: //enter
      case 32: //space
        event.preventDefault();
        event.stopPropagation();
        if (currentItemIndex >= 0)
        {
          var itemAnchor = dropDownMenuItems[currentItemIndex].querySelector<HTMLAnchorElement> (':scope > a')!;
          itemAnchor.click();
        }
        break;
      case 27: //escape
        event.preventDefault();
        event.stopPropagation();
        DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
        break;
      case 40: // down arrow
        event.preventDefault();
        event.stopPropagation();
        isSelectionUpdated = true;
        oldIndex = currentItemIndex;
        while (true) // skip separators
        {
          if (currentItemIndex < itemInfos.length - 1)
          {
            currentItemIndex++;
            if (currentItemIndex === 1 && oldIndex === -1)
              oldIndex = 0;
          }
          else
          {
            currentItemIndex = 0;
          }

          if (oldIndex === currentItemIndex)
            break;
          if (dropDownMenuItems[currentItemIndex].querySelectorAll (':scope > a').length > 0) // skip separators
            break;
        }
        break;
      case 38: // up arrow
        event.preventDefault();
        event.stopPropagation();
        isSelectionUpdated = true;
        oldIndex = currentItemIndex >= 0 ? currentItemIndex : itemInfos.length - 1;
        while (true) // skip separators
        {
          if (currentItemIndex > 0)
            currentItemIndex--;
          else
            currentItemIndex = itemInfos.length - 1;

          if (oldIndex === currentItemIndex)
            break;
          if (dropDownMenuItems[currentItemIndex].querySelectorAll (':scope > a').length > 0)
            break;
        }
        break;
    }

    if (isSelectionUpdated)
    {
      DropDownMenu._currentPopup.querySelectorAll ("li").forEach (b => b.classList.remove (DropDownMenu._itemSelectedClassName));
      if (currentItemIndex >= 0 && currentItemIndex < itemInfos.length)
      {
        var dropDownMenuItem = dropDownMenuItems[currentItemIndex];
        if (dropDownMenuItem.querySelectorAll (':scope > a').length > 0) // skip separators
        {
          dropDownMenuItem.classList.add (DropDownMenu._itemSelectedClassName);
          var anchor = dropDownMenuItem.querySelector<HTMLAnchorElement> (':scope > a')!;
          anchor.focus();
        }
      }
    }
  }

  private static GetItemEnabled (itemInfo: DropDownMenu_ItemInfo, selectionCount: number): boolean
  {
    var isEnabled = true;
    if (itemInfo.IsDisabled)
    {
      isEnabled = false;
    }
    else
    {
      if (itemInfo.RequiredSelection == DropDownMenu._requiredSelectionExactlyOne
          && selectionCount != 1)
      {
        isEnabled = false;
      }
      if (itemInfo.RequiredSelection == DropDownMenu._requiredSelectionOneOrMore
          && selectionCount < 1)
      {
        isEnabled = false;
      }
    }
    return isEnabled;
  }

  private static GetTarget (event: MouseEvent, tagName: string): Element
  {
    var element: Nullable<Element> = event.target as Element;
    while (element && element.tagName != tagName)
      element = element.parentNode as Nullable<Element>;
    // more fun with IE, sometimes event.target is empty, just ignore it then
    if (!element)
      return [] as unknown as Element;
    return element;
  }

  private static FocusHandler (): void
  {
    if (DropDownMenu._blurTimer !== null)
      clearTimeout(DropDownMenu._blurTimer);
  }

  private static BlurHandler (): void
  {
    if (DropDownMenu._blurTimer !== null)
      clearTimeout (DropDownMenu._blurTimer);

    DropDownMenu._blurTimer = setTimeout (function () { DropDownMenu.ClosePopUp (!DropDownMenu._updateFocus); }, 50);
  }
}

// RM-7723: This global function is required until we have a solution for Firefox to execute JavaScript in context of the web page
function DropDownMenu_ClosePopUp (updateFocus: boolean): void
{
  DropDownMenu.ClosePopUp (updateFocus);
}
