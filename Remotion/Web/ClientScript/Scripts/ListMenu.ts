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

class ListMenu_MenuInfo
{
  constructor(
    public readonly ID: string,
    public readonly ItemInfos: ListMenuItemInfo[])
  {
  }
}

class ListMenuItemInfo
{
  constructor(
    public readonly ID: string,
    public readonly Category: string,
    public readonly Text: string,
    public readonly Icon: string,
    public readonly IconDisabled: string,
    public readonly RequiredSelection: number,
    public readonly IsDisabled: boolean,
    public readonly Href: Nullable<string>,
    public readonly Target: Nullable<string>,
    public readonly ClickHandler: Nullable<(this: HTMLAnchorElement, event: MouseEvent) => void>,
    public readonly DiagnosticMetadata: Dictionary<string | boolean>,
    public readonly DiagnosticMetadataForCommand: Dictionary<string | boolean>)
  {
  }
}

class ListMenu
{
  private static readonly _listMenuInfos: Dictionary<ListMenu_MenuInfo> = {};
  private static readonly _itemClassName: string = 'listMenuItem';
  private static readonly _itemFocusClassName: string = 'listMenuItemFocus';
  private static readonly _itemDisabledClassName: string = 'listMenuItemDisabled';
  private static readonly _requiredSelectionAny: number = 0;
  private static readonly _requiredSelectionExactlyOne: number = 1;
  private static readonly _requiredSelectionOneOrMore: number = 2;

  public static Initialize (listMenuOrSelector: CssSelectorOrElement<HTMLElement>): void
  {
    ArgumentUtility.CheckNotNull ('listMenuOrSelector', listMenuOrSelector);

    const listMenu = ElementResolverUtility.ResolveSingle (listMenuOrSelector);

    listMenu.addEventListener('keydown', function (event)
    {
      ListMenu.OnKeyDown (event, listMenu);
    });
  }

  public static AddMenuInfo (listMenuOrSelector: CssSelectorOrElement<HTMLElement>, menuInfo: ListMenu_MenuInfo): void
  {
    ArgumentUtility.CheckNotNull ('listMenuOrSelector', listMenuOrSelector);
    ArgumentUtility.CheckNotNullAndTypeIsObject ('menuInfo', menuInfo);

    const listMenu = ElementResolverUtility.ResolveSingle (listMenuOrSelector);

    ListMenu._listMenuInfos[listMenu.id] = menuInfo;
  }

  public static Update (listMenuOrSelector: CssSelectorOrElement<HTMLElement>, getSelectionCount: Nullable<() => number>): void
  {
    ArgumentUtility.CheckNotNull ('listMenuOrSelector', listMenuOrSelector);
    ArgumentUtility.CheckTypeIsFunction ('getSelectionCount', getSelectionCount);

    const listMenu = ElementResolverUtility.ResolveSingle (listMenuOrSelector);

    const menuInfo = ListMenu._listMenuInfos[listMenu.id];
    if (menuInfo == null)
      return;

    const itemInfos = menuInfo.ItemInfos;
    const selectionCount = getSelectionCount
        ? getSelectionCount()
        : -1;

    for (let i = 0; i < itemInfos.length; i++)
    {
      const itemInfo = itemInfos[i];
      let isEnabled = true;
      if (itemInfo.IsDisabled)
      {
        isEnabled = false;
      }
      else
      {
        if (itemInfo.RequiredSelection == ListMenu._requiredSelectionExactlyOne && selectionCount != 1)
        {
          isEnabled = false;
        }
        if (itemInfo.RequiredSelection == ListMenu._requiredSelectionOneOrMore && selectionCount < 1)
        {
          isEnabled = false;
        }
      }

      const item = document.getElementById (itemInfo.ID) as HTMLElement;
      let anchor = item.children[0] as HTMLAnchorElement;
      const icon = anchor.children[0] as HTMLImageElement;
      anchor.removeAttribute ('href');
      anchor.removeAttribute ('target');
      anchor.removeAttribute ('onclick');
      if (itemInfo.ClickHandler !== null)
        anchor.removeEventListener ('click', itemInfo.ClickHandler);
      if (isEnabled)
      {
        if (icon != null && icon.nodeType == 1)
          icon.src = itemInfo.Icon;
        item.className = ListMenu._itemClassName;
        if (itemInfo.Href != null)
          anchor.href = itemInfo.Href;
        if (itemInfo.Target != null)
          anchor.target = itemInfo.Target;
        if (itemInfo.ClickHandler !== null)
          anchor.addEventListener ('click', itemInfo.ClickHandler);
        anchor.removeAttribute ('aria-disabled');
      }
      else
      {
        if (icon != null && icon.nodeType == 1)
        {
          if (itemInfo.IconDisabled != null)
            icon.src = itemInfo.IconDisabled;
          else
            icon.src = itemInfo.Icon;
        }
        item.className = ListMenu._itemDisabledClassName;
        anchor.setAttribute ('aria-disabled', 'true');
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
    }
  }

  private static OnKeyDown (event: KeyboardEvent, listMenu: HTMLElement): void
  {
    const menuItems = Array.from (listMenu.querySelectorAll ('a'));

    let oldMenuItemIndex = -1;

    const selectedMenuItem = document.activeElement as HTMLAnchorElement;
    if (selectedMenuItem != null && TypeUtility.IsDefined (selectedMenuItem.tagName) && selectedMenuItem.tagName.toUpperCase() === 'A')
    {
      oldMenuItemIndex = menuItems.indexOf (selectedMenuItem);
    }
    else
    {
      for (let i = 0; i < menuItems.length; i++)
      {
        // TODO RM-7686 Fix misspellings of tabIndex
        if ((menuItems[i] as any).tabindex === 0)
        {
          oldMenuItemIndex = i;
          break;
        }
      }
    }

    const oldMenuItem = oldMenuItemIndex >= 0
        ? menuItems[oldMenuItemIndex]
        : null;
    let currentMenuItemIndex = Math.max (0, oldMenuItemIndex);

    switch (event.keyCode)
    {
      case 9: // tab
        // exit tab strip
        return;
      case 13: //enter
      case 32: //space
        {
          event.preventDefault();
          event.stopPropagation();

          if (currentMenuItemIndex >= 0)
          {
            let newMenuItem = menuItems[currentMenuItemIndex];
            ListMenu.UpdateFocus (newMenuItem, oldMenuItem);

            newMenuItem.click();
          }

          return;
        }
      case 37: // left arrow
      case 38: // up arrow
        {
          event.preventDefault();
          event.stopPropagation();

          if (currentMenuItemIndex > 0)
            currentMenuItemIndex--;
          else
            currentMenuItemIndex = menuItems.length - 1;

          let newMenuItem = menuItems[currentMenuItemIndex];
          ListMenu.UpdateFocus (newMenuItem, oldMenuItem);

          return;
        }
      case 39: // right arrow
      case 40: // down arrow
        {
          event.preventDefault();
          event.stopPropagation();

          if (currentMenuItemIndex < menuItems.length - 1)
            currentMenuItemIndex++;
          else
            currentMenuItemIndex = 0;

          const newMenuItem = menuItems[currentMenuItemIndex];
          ListMenu.UpdateFocus (newMenuItem, oldMenuItem);

          return;
        }
    }
  }

  private static UpdateFocus (newMenuItem: HTMLAnchorElement, oldMenuItem: Nullable<HTMLAnchorElement>): void
  {
    newMenuItem.focus();
  }
}
