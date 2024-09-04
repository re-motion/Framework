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

interface DropDownMenuPositioningDetails
{
  align: "left" | "right";
  offsetX: number;
  offsetY: number;
}

interface IKeyboardEventLike
{
  keyCode: number;
  preventDefault(): void;
  stopPropagation(): void;
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
      public readonly ClickHandler: Nullable<() => boolean>,
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
  private static _ignoreHoverMouseEvents: boolean = false;
  private static _lastPositionInformation: Nullable<string> = null;

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

    let menuButton: HTMLAnchorElement;

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
          evt.preventDefault();

          menuButton.focus();
          DropDownMenu.OnClick (openTarget, menuID, getSelectionCount, moveToMousePosition ? evt as MouseEvent : null);
        });

    if (!moveToMousePosition)
    {
      menuButton.addEventListener ("focus", function () { openTarget.querySelectorAll ('.' + DropDownMenu._buttonClassName).forEach (b => b.classList.add (DropDownMenu._focusClassName)); })
      menuButton.addEventListener ("blur", function () { openTarget.querySelectorAll ('.' + DropDownMenu._buttonClassName).forEach (b => b.classList.remove (DropDownMenu._focusClassName)); });

      const allButMenuButton = Array.from (openTarget.querySelectorAll ('a[href]')).filter (b => menuButton !== b);
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
        for (let i = itemInfos.length - 1; i >= 0; i--)
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
      DropDownMenu.ClosePopUp(false);
    }
    if (DropDownMenu._currentMenu == null)
    {
      let event: Nullable<MouseEvent> = null;
      if (evt != null)
      {
        const bounds = context.getBoundingClientRect();
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

    const positioningDetails = this.CalculatePositioningDetails(titleDivFunc(), evt);
    DropDownMenu.ApplyPosition (statusPopup, positioningDetails, titleDivFunc(), false);

    if (DropDownMenu._statusPopupRepositionTimer)
      clearTimeout(DropDownMenu._statusPopupRepositionTimer);
    const repositionHandler = function ()
    {
      if (DropDownMenu._statusPopupRepositionTimer)
        clearTimeout(DropDownMenu._statusPopupRepositionTimer);

      if (DropDownMenu._currentStatusPopup && DropDownMenu._currentStatusPopup === statusPopup)
      {
        DropDownMenu.ApplyPosition (statusPopup, positioningDetails, titleDivFunc(), true);
        DropDownMenu._statusPopupRepositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);
      }
    };

    DropDownMenu._statusPopupRepositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);

    menuButton.addEventListener ('focus', DropDownMenu.FocusHandler);
    menuButton.addEventListener ('blur', DropDownMenu.BlurHandler);
  }

  private static EndOpenPopUp (menuID: string, context: HTMLElement, selectionCount: number, evt: Nullable<MouseEvent>, itemInfos: DropDownMenu_ItemInfo[]): void
  {
    const menuOptionsID = menuID + '_DropDownMenuOptions';
    const menuButtonAnchor = document.querySelector<HTMLAnchorElement> ('#' + menuID + ' a[aria-haspopup=menu]')!;

    if (itemInfos.length == 0)
      return;

    if (DropDownMenu._statusPopupRepositionTimer)
      clearTimeout(DropDownMenu._statusPopupRepositionTimer);
    if (DropDownMenu._currentStatusPopup !== null)
    {
      const statusPopup = DropDownMenu._currentStatusPopup;
      DropDownMenu._currentStatusPopup = null;
      // Clear the role=alert before removing to item to prevent screenreaders (JAWS) from announcing the old value during removal.
      statusPopup.removeAttribute('role');
      statusPopup.parentElement!.removeChild(statusPopup);
    }

    menuButtonAnchor.setAttribute ('aria-controls', menuOptionsID);
    menuButtonAnchor.setAttribute ('aria-expanded', 'true');

    const div = document.createElement('div');
    div.classList.add('DropDownMenuOptions');
    div.classList.add(CssClassDefinition.Themed);
    div.id = menuOptionsID;
    div.setAttribute ('role', 'menu');
    div.setAttribute ('tabindex', '-1');
    if (menuButtonAnchor.getAttribute ('aria-labelledby') !== null)
      div.setAttribute ('aria-labelledby', menuButtonAnchor.getAttribute ('aria-labelledby')!);
    else
      div.setAttribute ('aria-labelledby', menuButtonAnchor.id);
    div.addEventListener("focus", () => this.FocusHandler());
    div.addEventListener("blur", () => this.BlurHandler());

    DropDownMenu._currentPopup = div;

    const ul = document.createElement('ul');
    ul.className = 'DropDownMenuOptions';
    ul.setAttribute('role', 'none');
    div.appendChild(ul);

    const menuButton = document.getElementById (menuID)!;
    menuButton.closest ('div, td, th, body')!.append (div);

    ul.addEventListener ('mouseleave', function ()
    {
      ul.querySelectorAll ("li").forEach (b => b.classList.remove (DropDownMenu._itemSelectedClassName));
    });
    ul.addEventListener ('mouseover', function (event)
    {
      if (DropDownMenu._ignoreHoverMouseEvents)
        return;

      const eventTarget = DropDownMenu.GetTarget (event, "LI");

      ul.querySelectorAll ("li").forEach (b => b.classList.remove (DropDownMenu._itemSelectedClassName));

      if (eventTarget == null)
        return;

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
      const item = DropDownMenu.CreateItem(itemInfos[i], selectionCount);
      if (item != null)
        ul.appendChild(item);
    }

    const titleDivFunc = DropDownMenu.CreateTitleDivGetter (context);
    const positioningDetails = this.CalculatePositioningDetails(titleDivFunc(), evt);
    DropDownMenu.ApplyPosition (div, positioningDetails, titleDivFunc(), false);

    if (DropDownMenu._repositionTimer) 
      clearTimeout(DropDownMenu._repositionTimer);
    const repositionHandler = function ()
    {
      if (DropDownMenu._repositionTimer)
        clearTimeout (DropDownMenu._repositionTimer);

      if (DropDownMenu._currentPopup && DropDownMenu._currentPopup == div && LayoutUtility.IsVisible (div))
      {
        DropDownMenu.ApplyPosition (div, positioningDetails, titleDivFunc(), true);
        DropDownMenu._repositionTimer = setTimeout (repositionHandler, DropDownMenu._repositionInterval);
      }
    };

    DropDownMenu._repositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);

    if (menuButton.dataset[DropDownMenu._button_timestampDataKey] !== undefined)
    {
      delete menuButton.dataset[DropDownMenu._button_timestampDataKey];
    }
    else
    {
      DropDownMenu.ClosePopUp(true);
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

  private static ApplyPosition (popUpDiv: HTMLDivElement, details: DropDownMenuPositioningDetails, referenceElement: HTMLElement, onlyUpdateIfNecessary: boolean): void
  {
    const space_top = Math.round (LayoutUtility.GetOffset (referenceElement).top - window.pageYOffset);
    const space_bottom = Math.round (document.documentElement.clientHeight - LayoutUtility.GetOffset (referenceElement).top - LayoutUtility.GetHeight (referenceElement) + window.pageYOffset);
    let space_left = LayoutUtility.GetOffset (referenceElement).left;
    let space_right = document.documentElement.clientWidth - LayoutUtility.GetOffset (referenceElement).left - LayoutUtility.GetWidth (referenceElement);
    
    const positionString = `${space_top};${space_right};${space_bottom};${space_left};${JSON.stringify(details)}`;
    if (onlyUpdateIfNecessary && positionString === this._lastPositionInformation)
      return;

    this._lastPositionInformation = positionString;

    // position drop-down list
    const top = Math.max (0, space_top + referenceElement.offsetHeight + details.offsetY);
    const left = Math.max (0, LayoutUtility.GetOffset (referenceElement).left + details.offsetX) + 'px';
    const right = Math.max (0, document.documentElement.clientWidth - LayoutUtility.GetOffset (referenceElement).left - referenceElement.offsetWidth - details.offsetX) + 'px';

    const popUpMargin = parseInt(getComputedStyle(popUpDiv).fontSize) * 2;

    // Save and restore the scrollTop value to retain the scrolling after resizing
    const scrollTop = popUpDiv.scrollTop;

    popUpDiv.style.top = top + 'px';
    popUpDiv.style.bottom = 'auto';
    popUpDiv.style.position = 'fixed';

    // Set to values that allow the popup to expand properly - this way the result of GetWidth is actually correct
    popUpDiv.style.left = '0';
    popUpDiv.style.right = 'auto';

    const isContextMenu = details.align === 'left';

    // In case of a context menu being open, the space calculation needs to be adapted to match the actual click location in the context of
    // the reference element.
    if (isContextMenu)
    {
      const currentOffset = referenceElement.getBoundingClientRect().left + details.offsetX;
      space_right = document.documentElement.clientWidth - currentOffset;
      space_left = Math.min(document.documentElement.clientWidth, currentOffset);
    }

    const requiredPopUpWidth = LayoutUtility.GetWidth(popUpDiv);
    const dropDownMenuButtonWidth = isContextMenu ? 0 : LayoutUtility.GetWidth(referenceElement);
    const maxLeftAlignedPopupWidth = (space_right - popUpMargin) + dropDownMenuButtonWidth;
    const maxRightAlignedPopupWidth = (space_left - popUpMargin) + dropDownMenuButtonWidth;
    const isPopUpSmallerThanViewPort = requiredPopUpWidth <= (document.documentElement.clientWidth - 2 * popUpMargin);

    const alignToLeftOfReferenceElement = () =>
    {
      popUpDiv.style.left = left;
      popUpDiv.style.right = 'auto';
    };
    const alignToRightOfReferenceElement = () =>
    {
      popUpDiv.style.left = 'auto';
      popUpDiv.style.right = right;
    };
    const alignToLeftViewPortBorder = () =>
    {
      popUpDiv.style.left = popUpMargin + 'px';
      popUpDiv.style.right = 'auto';
    };
    const alignToRightViewPortBorder = () =>
    {
      popUpDiv.style.left = 'auto';
      popUpDiv.style.right = popUpMargin + 'px';
    };
    const clampToViewPort = () =>
    {
      popUpDiv.style.left = popUpMargin + 'px';
      popUpDiv.style.right = popUpMargin + 'px';
    };

    if (isContextMenu && (requiredPopUpWidth <= maxLeftAlignedPopupWidth))
    {
      alignToLeftOfReferenceElement();
    }
    else if (isContextMenu && (requiredPopUpWidth <= maxRightAlignedPopupWidth))
    {
      alignToRightViewPortBorder();
    }
    else if (requiredPopUpWidth <= maxRightAlignedPopupWidth)
    {
      alignToRightOfReferenceElement();
    }
    else if (requiredPopUpWidth <= maxLeftAlignedPopupWidth)
    {
      alignToLeftOfReferenceElement();
    }
    else if (isContextMenu && isPopUpSmallerThanViewPort)
    {
      alignToRightViewPortBorder();
    }
    else if (isPopUpSmallerThanViewPort)
    {
      alignToLeftViewPortBorder();
    }
    else
    {
      clampToViewPort();
    }

    const documentHeight = document.documentElement.clientHeight;
    const popUpHeight = LayoutUtility.GetHeight (popUpDiv);
    const maximumPopUpHeightConsideringSpacing = documentHeight - popUpMargin * 2;
    if (popUpHeight > maximumPopUpHeightConsideringSpacing)
    {
      // If we do not have enough space to display the element we keep space above and below and enable scrolling
      // If the window gets very small we reduce the spacing above and below until we reach the window borders
      // This effect is in effect when the remaining size gets smaller than the minimum popup height
      const minimumPopUpHeight = popUpMargin * 6;
      const targetPopUpHeight = Math.max(minimumPopUpHeight, maximumPopUpHeightConsideringSpacing);
      let verticalSpaceAround = Math.floor(documentHeight - targetPopUpHeight - popUpMargin);
      let suggestedVerticalSpacing = Math.max(0, verticalSpaceAround);

      // If the popup is small enough we might make it bigger by removing spacing - so we center the element instead
      const popupStyle = window.getComputedStyle(popUpDiv);
      const verticalBorder = parseInt(popupStyle.borderTop) + parseInt(popupStyle.borderBottom);
      console.info(verticalBorder);
      if (documentHeight - suggestedVerticalSpacing * 2 >= popUpHeight - verticalBorder) 
      {
        verticalSpaceAround = documentHeight - LayoutUtility.GetOuterHeight (popUpDiv);
        suggestedVerticalSpacing = Math.max(0, Math.floor(verticalSpaceAround / 2));
        
        popUpDiv.classList.remove(CssClassDefinition.Scrollable);
      }
      else
      {
        popUpDiv.classList.add(CssClassDefinition.Scrollable);
      }

      popUpDiv.style.top = suggestedVerticalSpacing + 'px';
      popUpDiv.style.bottom = suggestedVerticalSpacing + 'px';
    }
    else if (popUpHeight > space_bottom - popUpMargin)
    {
      if (space_top > popUpHeight + popUpMargin)
      {
        popUpDiv.classList.remove(CssClassDefinition.Scrollable);

        const bottom = Math.max (popUpMargin, documentHeight - LayoutUtility.GetOffset (referenceElement).top - (referenceElement.offsetHeight - LayoutUtility.GetHeight (referenceElement)));

        popUpDiv.style.top = 'auto';
        popUpDiv.style.bottom = bottom + 'px';
      }
      else
      {
        popUpDiv.classList.remove(CssClassDefinition.Scrollable);

        popUpDiv.style.top = 'auto';
        popUpDiv.style.bottom = popUpMargin + 'px';
      }
    }

    popUpDiv.scrollTop = scrollTop;
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
      if (updateFocus)
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

    const item = itemInfo.Text == '-'
        ? DropDownMenu.CreateSeparatorItem()
        : DropDownMenu.CreateTextItem(itemInfo, selectionCount)

    return item;
  }

  private static CreateTextItem(itemInfo: DropDownMenu_ItemInfo, selectionCount: number): HTMLLIElement
  {
    const isEnabled = DropDownMenu.GetItemEnabled(itemInfo, selectionCount);

    const item = document.createElement("li");

    const className = isEnabled
        ? DropDownMenu._itemClassName
        : DropDownMenu._itemDisabledClassName;

    item.className = className;
    item.setAttribute('role', 'none');

    const anchor = document.createElement("a");
    anchor.setAttribute('role', 'menuitem');
    anchor.setAttribute('tabindex', '-1');
    if (isEnabled && itemInfo.Href !== null)
    {
      anchor.setAttribute('href', itemInfo.Href);
      if (itemInfo.Target !== null)
        anchor.setAttribute('target', itemInfo.Target);
    }
    else
    {
      anchor.setAttribute('aria-disabled', 'true');
    }

    const itemClickHandler = isEnabled ? itemInfo.ClickHandler : null;
    anchor.addEventListener ('click', function (ev: MouseEvent) { DropDownMenu.OnItemClick(this, ev, itemClickHandler) });

    item.appendChild(anchor);

    if (itemInfo.Icon != null)
    {
      const icon = document.createElement('img');
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
      const iconPlaceholder = document.createElement('span');
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

    const span = document.createElement('span');
    span.innerHTML = itemInfo.Text!;
    anchor.appendChild(span);
    anchor.addEventListener ('focus', DropDownMenu.FocusHandler);
    anchor.addEventListener ('blur', DropDownMenu.BlurHandler);

    return item;
  }

  private static CreateSeparatorItem(): HTMLLIElement
  {
    const item = document.createElement('li');

    const textPane = document.createElement('span');
    textPane.className = DropDownMenu._itemSeparatorClassName;
    item.setAttribute('role', 'none');
    item.setAttribute("aria-hidden", "true");

    textPane.innerHTML = '&nbsp;';

    item.appendChild(textPane);

    return item;
  }

  private static OnItemClick (anchor: HTMLAnchorElement, ev: MouseEvent, clickHandler: Nullable<() => boolean>): boolean
  {
    if (anchor.href === null || anchor.href === '')
      return false;

    DropDownMenu._itemClicked = true;
    DropDownMenu.ClosePopUp(true);

    let result = true;
    try
    {
      if (clickHandler !== null)
        result = clickHandler();
    }
    catch (e)
    {
    }
    setTimeout (function () { DropDownMenu._itemClicked = false; }, 10);
    return result;
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

    let keyboardEvent: IKeyboardEventLike = event;
    switch (keyboardEvent.keyCode)
    {
      case 9: // tab
        DropDownMenu.ClosePopUp(true);
        return;
      case 13: //enter
      case 32: //space
        keyboardEvent.preventDefault();
        keyboardEvent.stopPropagation();
        if (dropDownMenu !== DropDownMenu._currentMenu)
        {
          DropDownMenu.OnClick (dropDownMenu, dropDownMenu.id, getSelectionCount, null);
          keyboardEvent = this.CreateKeyboardEventLikeWithKeyCode(keyboardEvent, 40); //down arrow
          DropDownMenu.Options_OnKeyDown (keyboardEvent, dropDownMenu);
        }
        else
        {
          DropDownMenu.ClosePopUp(true);
        }
        return;
      case 27: //escape
        keyboardEvent.preventDefault();
        keyboardEvent.stopPropagation();
        DropDownMenu.ClosePopUp(true);
        return;
      case 38: // up arrow
      case 40: // down arrow
        keyboardEvent.preventDefault();
        keyboardEvent.stopPropagation();
        if (dropDownMenu !== DropDownMenu._currentMenu)
        {
          DropDownMenu.OnClick (dropDownMenu, dropDownMenu.id, getSelectionCount, null);
          keyboardEvent = this.CreateKeyboardEventLikeWithKeyCode(keyboardEvent, 40); //down arrow
        }
        DropDownMenu.Options_OnKeyDown (keyboardEvent, dropDownMenu);
        return;
      default:
        DropDownMenu.Options_OnKeyDown(keyboardEvent, dropDownMenu);
        return;
    }
  }

  private static Options_OnKeyDown (event: IKeyboardEventLike, dropDownMenu: HTMLElement): void
  {
    if (DropDownMenu._currentPopup == null)
      return;

    const itemInfos = DropDownMenu._menuInfos[dropDownMenu.id]!.ItemInfos!;
    if (itemInfos.length === 0)
      return;

    let oldIndex;
    let isSelectionUpdated = false;
    const dropDownMenuItems = Array.from (DropDownMenu._currentPopup.querySelector ('ul')!.children);
    let currentItemIndex = dropDownMenuItems.findIndex (i => i.classList.contains (DropDownMenu._itemSelectedClassName))

    switch (event.keyCode)
    {
      case 9: // tab
        DropDownMenu.ClosePopUp(true);
        return;
      case 13: //enter
      case 32: //space
        event.preventDefault();
        event.stopPropagation();
        if (currentItemIndex >= 0)
        {
          const itemAnchor = dropDownMenuItems[currentItemIndex].querySelector<HTMLAnchorElement> (':scope > a')!;
          itemAnchor.click();
        }
        break;
      case 27: //escape
        event.preventDefault();
        event.stopPropagation();
        DropDownMenu.ClosePopUp(true);
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
        const dropDownMenuItem = dropDownMenuItems[currentItemIndex];
        if (dropDownMenuItem.querySelectorAll (':scope > a').length > 0) // skip separators
        {
          dropDownMenuItem.classList.add (DropDownMenu._itemSelectedClassName);
          const anchor = dropDownMenuItem.querySelector<HTMLAnchorElement> (':scope > a')!;
          anchor.focus();
        }
      }

      // A selection change might trigger hover events that, in turn, would trigger selection events
      // So we ignore mouse events for a small amount of time to deal with cascading events triggered by the selection change
      DropDownMenu._ignoreHoverMouseEvents = true;
      setTimeout(() => DropDownMenu._ignoreHoverMouseEvents = false, 250);
    }
  }

  private static CreateKeyboardEventLikeWithKeyCode(event: IKeyboardEventLike, keyCode: number): IKeyboardEventLike
  {
    return {
      keyCode: keyCode,
      preventDefault: () => event.preventDefault(),
      stopPropagation: () => event.stopPropagation()
    }
  }

  private static GetItemEnabled (itemInfo: DropDownMenu_ItemInfo, selectionCount: number): boolean
  {
    let isEnabled = true;
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

  private static GetTarget (event: MouseEvent, tagName: string): Nullable<Element>
  {
    let element: Nullable<Element> = event.target as Element;
    while (element && element.tagName != tagName)
      element = element.parentNode as Nullable<Element>;
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

    DropDownMenu._blurTimer = setTimeout (function () { DropDownMenu.ClosePopUp (false); }, 50);
  }

  private static CalculatePositioningDetails (referenceElement: HTMLElement, evt: Nullable<MouseEvent>): DropDownMenuPositioningDetails
  {
    // offset origin is below the element either at the left or right edge depending on the align property
    if (evt === null)
    {
      return {
        align: "right",
        offsetX: 0,
        offsetY: 0
      };
    }

    const referencePosition = referenceElement.getBoundingClientRect();
    return {
      align: "left",
      offsetX: evt.clientX - referencePosition.left,
      offsetY: evt.clientY - referencePosition.top - referencePosition.height
    };
  }
}

// RM-7723: This global function is required until we have a solution for Firefox to execute JavaScript in context of the web page
function DropDownMenu_ClosePopUp (updateFocus: boolean): void
{
  DropDownMenu.ClosePopUp (updateFocus);
}