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
type DropDownMenu_ItemClickHandler = (event: JQueryEventObject) => boolean;
type DropDownMenu_SelectionCountGetter = () => number;
type DropDownMenu_TitleDivGetter = () => JQuery;

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
  private static _currentMenu: Nullable<JQuery> = null;
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
    DropDownMenu._menuInfos[menuInfo.ID] = menuInfo;
  }

  public static BindOpenEvent (openTarget: JQuery, menuID: string, eventType: string, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, moveToMousePosition: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery ('openTarget', openTarget);
    ArgumentUtility.CheckNotNullAndTypeIsString ('menuID', menuID);
    ArgumentUtility.CheckNotNullAndTypeIsString ('eventType', eventType);
    ArgumentUtility.CheckNotNullAndTypeIsBoolean ('moveToMousePosition', moveToMousePosition);

    var menuButton = openTarget[0].tagName.toLowerCase() === 'a' ? openTarget : openTarget.find ('a[href]:last');
    menuButton.attr('aria-haspopup', 'menu');
    menuButton.attr('aria-expanded', 'false');

    openTarget.bind(
        eventType,
        function (evt: JQueryMouseEventObject)
        {
          menuButton.focus();
          DropDownMenu.OnClick (openTarget, menuID, getSelectionCount, moveToMousePosition ? evt : null);
        });

    if (!moveToMousePosition)
    {
      menuButton
          .bind ("focus", function (evt) { openTarget.find ('.' + DropDownMenu._buttonClassName).addClass (DropDownMenu._focusClassName); })
          .bind("blur", function (evt) { openTarget.find('.' + DropDownMenu._buttonClassName).removeClass (DropDownMenu._focusClassName); });

      var allButMenuButton = openTarget.find ('a[href]').not(menuButton);
      allButMenuButton
          .bind ("mouseover", function (evt) { openTarget.find ('.' + DropDownMenu._buttonClassName).addClass (DropDownMenu._nestedHoverClassName); })
          .bind ("mouseout", function (evt) { openTarget.find ('.' + DropDownMenu._buttonClassName).removeClass (DropDownMenu._nestedHoverClassName); });
    }
  }

  public static LoadFilteredMenuItems(
      itemInfos: DropDownMenu_ItemInfo[],
      loadMenuItemStatus: Nullable<DropDownMenu_MenuInfo_LoadMenuItemStatusCallback>,
      onSuccess: DropDownMenu_MenuInfo_LoadMenuItemsSuccessHandler,
      onError: DropDownMenu_MenuInfo_LoadMenuItemsErrorHandler): void
  {
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

  private static OnClick(context: JQuery, menuID: string, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, evt: Nullable<JQueryMouseEventObject>): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery('context', context);

    $('#' + menuID).data(DropDownMenu._button_timestampDataKey, new Date().getTime());

    if (DropDownMenu._itemClicked)
    {
      DropDownMenu._itemClicked = false;
      return;
    }
    if (DropDownMenu._currentMenu != null && DropDownMenu._currentMenu[0] !== context[0])
    {
      DropDownMenu.ClosePopUp(!DropDownMenu._updateFocus);
    }
    if (DropDownMenu._currentMenu == null)
    {
      var event: Nullable<JQueryMouseEventObject> = null;
      if (evt != null)
      {
        var bounds = context[0].getBoundingClientRect();
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

  private static OpenPopUp (menuID: string, context: JQuery, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, evt: Nullable<JQueryMouseEventObject>): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery('context', context);

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

  private static BeginOpenPopUp(menuID: string, context: JQuery, evt: Nullable<JQueryMouseEventObject>): void
  {
    const menuButton = $('#' + menuID + ' a[aria-haspopup=menu]');

    const titleDivFunc = DropDownMenu.CreateTitleDivGetter(context);
    const statusPopup = document.createElement('div');
    statusPopup.className = 'DropDownMenuStatus';
    statusPopup.id = menuID + '_DropDownMenuStatus';
    statusPopup.style.position = 'absolute';
    statusPopup.style.display = 'none';
    statusPopup.setAttribute('aria-atomic', 'true');
    // Do not set role=alert before it is needed prevent an alert-update during 'normal' showing of menu.
    //statusPopup.setAttribute('role', 'alert');
    if (menuButton.attr('aria-labelledby') !== undefined)
      statusPopup.setAttribute('aria-labelledby', menuButton.attr('aria-labelledby'));
    else
      statusPopup.setAttribute('aria-labelledby', menuButton[0].id);
    DropDownMenu._currentStatusPopup = statusPopup;
    $(statusPopup).iFrameShim({ top: '0px', left: '0px', width: '100%', height: '100%' });
    $('#' + menuID).closest('div, td, th, body').append(statusPopup);

    DropDownMenu.ApplyPosition($(statusPopup), evt, titleDivFunc());

    if (DropDownMenu._statusPopupRepositionTimer)
      clearTimeout(DropDownMenu._statusPopupRepositionTimer);
    const repositionHandler = function ()
    {
      if (DropDownMenu._statusPopupRepositionTimer)
        clearTimeout(DropDownMenu._statusPopupRepositionTimer);

      if (DropDownMenu._currentStatusPopup && DropDownMenu._currentStatusPopup === statusPopup)
      {
        DropDownMenu.ApplyPosition($(statusPopup), null, titleDivFunc());
        DropDownMenu._statusPopupRepositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);
      }
    };

    // Only reposition if opened via titleDiv
    if (evt === null)
      DropDownMenu._statusPopupRepositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);

    menuButton.bind('focus', DropDownMenu.FocusHandler);
    menuButton.bind('blur', DropDownMenu.BlurHandler);
  }

  private static EndOpenPopUp (menuID: string, context: JQuery, selectionCount: number, evt: Nullable<JQueryMouseEventObject>, itemInfos: DropDownMenu_ItemInfo[]): void
  {
    var menuOptionsID = menuID + '_DropDownMenuOptions';
    var menuButton = $('#' + menuID + ' a[aria-haspopup=menu]');

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

    menuButton.attr('aria-controls', menuOptionsID);
    menuButton.attr('aria-expanded', 'true');

    var div = document.createElement('div');
    div.className = 'DropDownMenuOptions';
    div.id = menuOptionsID;
    div.setAttribute('role', 'menu');
    div.setAttribute('tabindex', '-1');
    if (menuButton.attr('aria-labelledby') !== undefined)
      div.setAttribute('aria-labelledby', menuButton.attr('aria-labelledby'));
    else
      div.setAttribute('aria-labelledby', menuButton[0].id);
    DropDownMenu._currentPopup = div;

    var ul = document.createElement('ul');
    ul.className = 'DropDownMenuOptions';
    ul.setAttribute('role', 'none');
    div.appendChild(ul);

    const $menuButton = $('#' + menuID);
    $menuButton.closest('div, td, th, body').append(div);

    $(ul).mouseover (function (event)
    {
      var eventTarget = DropDownMenu.GetTarget (event, "LI");
      $("li", ul).removeClass(DropDownMenu._itemSelectedClassName);
      if (eventTarget.firstChild != null && eventTarget.firstChild.nodeName.toLowerCase() === 'a')
      {
        $(eventTarget).addClass (DropDownMenu._itemSelectedClassName);
        (eventTarget.firstChild as HTMLElement).focus();
      }
    }).keydown (function (event)
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
    DropDownMenu.ApplyPosition ($(div), evt, titleDivFunc());
    $(div).iFrameShim({ top: '0px', left: '0px', width: '100%', height: '100%' });

    if (DropDownMenu._repositionTimer) 
      clearTimeout(DropDownMenu._repositionTimer);
    var repositionHandler = function ()
    {
      if (DropDownMenu._repositionTimer)
        clearTimeout (DropDownMenu._repositionTimer);

      if (DropDownMenu._currentPopup && DropDownMenu._currentPopup == div && $(div).is (':visible'))
      {
        DropDownMenu.ApplyPosition ($(div), null, titleDivFunc());
        DropDownMenu._repositionTimer = setTimeout (repositionHandler, DropDownMenu._repositionInterval);
      }
    };

    // Only reposition if opened via titleDiv
    if (evt == null)
      DropDownMenu._repositionTimer = setTimeout(repositionHandler, DropDownMenu._repositionInterval);

    if ($menuButton.data(DropDownMenu._button_timestampDataKey))
    {
      $menuButton.removeData(DropDownMenu._button_timestampDataKey);
    }
    else
    {
      DropDownMenu.ClosePopUp(DropDownMenu._updateFocus);
    }
  }

  private static CreateTitleDivGetter ($context: JQuery): DropDownMenu_TitleDivGetter
  {
    const context = $context[0];
    const contextID = context.id;
    if (contextID == null)
    {
      return function ()
      {
        return $(context.firstElementChild!);
      };
    }
    else
    {
      return function ()
      {
        // TODO RM-7709 Check if null could be returned
        return $(document.getElementById(contextID)!.firstElementChild!);
      };
    }
  }

  private static ApplyPosition (popUpDiv: JQuery, clickEvent: Nullable<JQueryMouseEventObject>, referenceElement: JQuery): void
  {
    var space_top = Math.round(referenceElement.offset().top - $(document).scrollTop());
    var space_bottom = Math.round($(window).height() - referenceElement.offset().top - referenceElement.height() + $(document).scrollTop());
    var space_left = referenceElement.offset().left;
    var space_right = $(window).width() - referenceElement.offset().left - referenceElement.width();

    // position drop-down list
    var top = clickEvent ? clickEvent.clientY : Math.max(0, space_top + referenceElement.outerHeight());
    var left = clickEvent ? clickEvent.clientX : 'auto';
    var right = clickEvent ? 'auto' : Math.max(0, $(window).width() - referenceElement.offset().left - referenceElement.outerWidth());

    popUpDiv.css('top', top);
    popUpDiv.css('bottom', 'auto');
    // TODO RM-7699 Incorrect style assignments
    popUpDiv.css('right', right as number);
    popUpDiv.css('left', left as number);
    popUpDiv.css('position', 'fixed');

    // move dropdown if there is not enough space to fit it on the page
    if ((popUpDiv.width() > space_left) && (space_left < space_right))
    {
      if (popUpDiv.offset().left < 0)
      {
        left = Math.max(0, referenceElement.offset().left);
        popUpDiv.css('left', left);
        popUpDiv.css('right', 'auto');
      }
    }
    if (popUpDiv.height() > space_bottom)
    {
      if (popUpDiv.height() > $(window).height())
      {
        popUpDiv.css('top', 0);
      }
      else if (space_top > popUpDiv.height())
      {
        var bottom = Math.max(0, $(window).height() - referenceElement.offset().top - (referenceElement.outerHeight() - referenceElement.height()));

        popUpDiv.css('top', 'auto');
        popUpDiv.css('bottom', bottom);
      }
      else
      {
        popUpDiv.css('top', 'auto');
        popUpDiv.css('bottom', 0);
      }
    }
  }

  public static ClosePopUp (updateFocus: boolean): void
  {
    if (DropDownMenu._blurTimer !== null)
      clearTimeout(DropDownMenu._blurTimer);

    if (DropDownMenu._currentPopup !== null
        && document.body.contains(DropDownMenu._currentPopup))
    {
      const menuPopup = DropDownMenu._currentPopup;
      const $menuButton = $('a[aria-controls="' + menuPopup.id + '"]');
      $menuButton.unbind('focus', DropDownMenu.BlurHandler);
      $menuButton.unbind('blur', DropDownMenu.BlurHandler);
      const menuButton = $menuButton[0];
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

    $(anchor).bind('click', DropDownMenu.OnItemClick);

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
      // Do not render empty diagnostic metadata attributes
      $.each(itemInfo.DiagnosticMetadata, function (key: string, value: string | boolean)
      {
        if (value === "" || value === null)
        {
          delete itemInfo.DiagnosticMetadata![key];
        }
      });

      $ (item).attr (itemInfo.DiagnosticMetadata);
    }

    if (itemInfo.DiagnosticMetadataForCommand)
    {
      // Do not render empty diagnostic metadata attributes
      $.each(itemInfo.DiagnosticMetadataForCommand,
        function (key, value) {
          if (value === "" || value === null) {
            delete itemInfo.DiagnosticMetadataForCommand![key];
          }
        });

      itemInfo.DiagnosticMetadataForCommand['data-is-disabled'] = isEnabled ? 'false' : 'true';

      $(anchor).attr(itemInfo.DiagnosticMetadataForCommand);
    }

    var span = document.createElement('span');
    span.innerHTML = itemInfo.Text!;
    anchor.appendChild(span);
    $(anchor)
      .bind ('focus', DropDownMenu.FocusHandler)
      .bind ('blur', DropDownMenu.BlurHandler);

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

  public static OnKeyDown(event: JQueryKeyEventObject, dropDownMenu: JQuery, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, hasDedicatedDropDownMenuElement: boolean): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery('dropDownMenu', dropDownMenu);

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
          DropDownMenu.OnClick (dropDownMenu, dropDownMenu[0].id, getSelectionCount, null);
          event.keyCode = 40; // always act as if the down-arrow was used when opening the drop down menu.
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
          DropDownMenu.OnClick (dropDownMenu, dropDownMenu[0].id, getSelectionCount, null);
          event.keyCode = 40; // always act as if the down-arrow was used when opening the drop down menu.
        }
        DropDownMenu.Options_OnKeyDown (event, dropDownMenu);
        return;
      default:
        DropDownMenu.Options_OnKeyDown(event, dropDownMenu);
        return;
    }
  }

  private static Options_OnKeyDown(event: JQueryKeyEventObject, dropDownMenu: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery('dropDownMenu', dropDownMenu);

    if (DropDownMenu._currentPopup == null)
      return;

    var itemInfos = DropDownMenu._menuInfos[dropDownMenu[0].id]!.ItemInfos!;
    if (itemInfos.length === 0)
      return;

    var oldIndex;
    var isSelectionUpdated = false;
    var dropDownMenuItems = $(DropDownMenu._currentPopup).find('ul').children();
    var currentItemIndex = $('.' + DropDownMenu._itemSelectedClassName, DropDownMenu._currentPopup).index();

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
          var itemAnchor = $(dropDownMenuItems[currentItemIndex]).children('a');
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
          if ($(dropDownMenuItems[currentItemIndex]).children('a').length > 0) // skip separators
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
          if ($(dropDownMenuItems[currentItemIndex]).children('a').length > 0)
            break;
        }
        break;
    }

    if (isSelectionUpdated)
    {
      $("li", DropDownMenu._currentPopup).removeClass (DropDownMenu._itemSelectedClassName);
      if (currentItemIndex >= 0 && currentItemIndex < itemInfos.length)
      {
        var dropDownMenuItem = $(dropDownMenuItems[currentItemIndex]);
        if (dropDownMenuItem.children('a').length > 0) // skip separators
        {
          dropDownMenuItem.addClass(DropDownMenu._itemSelectedClassName);
          var nonJQueryAnchor = dropDownMenuItem.children ('a')[0];
          // do not use jQuery focus() here because jQuery causes the focus/blur events to fire in the wrong order (first focus, then blur)
          nonJQueryAnchor.focus();
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

  private static GetTarget (event: JQueryMouseEventObject, tagName: string): Element
  {
    var element: Nullable<Element> = event.target;
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

function DropDownMenu_AddMenuInfo(menuInfo: DropDownMenu_MenuInfo): void
{
  DropDownMenu.AddMenuInfo (menuInfo);
}

function DropDownMenu_BindOpenEvent (openTarget: JQuery, menuID: string, eventType: string, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, moveToMousePosition: boolean): void
{
  DropDownMenu.BindOpenEvent (openTarget, menuID, eventType, getSelectionCount, moveToMousePosition);
}

function DropDownMenu_LoadFilteredMenuItems(
    itemInfos: DropDownMenu_ItemInfo[],
    loadMenuItemStatus: Nullable<DropDownMenu_MenuInfo_LoadMenuItemStatusCallback>,
    onSuccess: DropDownMenu_MenuInfo_LoadMenuItemsSuccessHandler,
    onError: DropDownMenu_MenuInfo_LoadMenuItemsErrorHandler): void
{
  DropDownMenu.LoadFilteredMenuItems (itemInfos, loadMenuItemStatus, onSuccess, onError);
}

function DropDownMenu_ClosePopUp (updateFocus: boolean): void
{
  DropDownMenu.ClosePopUp (updateFocus);
}

function DropDownMenu_OnHeadControlClick(): void
{
  DropDownMenu.OnHeadControlClick();
}

function DropDownMenu_OnKeyDown(event: JQueryKeyEventObject, dropDownMenu: JQuery, getSelectionCount: Nullable<DropDownMenu_SelectionCountGetter>, hasDedicatedDropDownMenuElement: boolean): void
{
  DropDownMenu.OnKeyDown (event, dropDownMenu, getSelectionCount, hasDedicatedDropDownMenuElement);
}
