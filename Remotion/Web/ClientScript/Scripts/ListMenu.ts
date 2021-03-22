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
    public readonly Href: string,
    public readonly Target: string,
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

  public static Initialize ($listMenu: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery("$listMenu", $listMenu);

    $listMenu.keydown (function (event)
    {
      ListMenu.OnKeyDown (event, $listMenu);
    });
  }

  public static AddMenuInfo (listMenu: HTMLElement, menuInfo: ListMenu_MenuInfo): void
  {
    ListMenu._listMenuInfos[listMenu.id] = menuInfo;
  }

  public static Update (listMenu: HTMLElement, getSelectionCount: Nullable<() => number>): void
  {
    var menuInfo = ListMenu._listMenuInfos[listMenu.id];
    if (menuInfo == null)
      return;

    var itemInfos = menuInfo.ItemInfos;
    var selectionCount = -1;
    if (getSelectionCount != null)
      selectionCount = getSelectionCount();

    for (var i = 0; i < itemInfos.length; i++)
    {
      var itemInfo = itemInfos[i];
      var isEnabled = true;
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

      var item = document.getElementById (itemInfo.ID) as HTMLElement;
      let anchor = $ (item).children().eq (0)[0] as HTMLAnchorElement;
      var icon = $ (anchor).children().eq (0)[0] as HTMLImageElement;
      if (isEnabled)
      {
        if (icon != null && icon.nodeType == 1)
          icon.src = itemInfo.Icon;
        item.className = ListMenu._itemClassName;
        if (itemInfo.Href != null)
        {
          if (itemInfo.Href.toLowerCase().indexOf ('javascript:') >= 0)
          {
            anchor.href = '#';
            anchor.removeAttribute ('target');
            anchor.setAttribute ('javascript', itemInfo.Href);
            anchor.removeAttribute ('onclick');
            // TODO RM-7694 missing nullcheck
            anchor.onclick = function () { eval (anchor.getAttribute ('javascript')!); return false; };
          }
          else
          {
            anchor.href = itemInfo.Href;
            if (itemInfo.Target != null)
              anchor.target = itemInfo.Target;
            anchor.removeAttribute ('javascript');
            anchor.removeAttribute ('onclick');
            anchor.onclick = null;
          }
        }
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
        anchor.removeAttribute ('href');
        anchor.removeAttribute ('target');
        anchor.removeAttribute ('javascript');
        anchor.removeAttribute ('onclick');
        anchor.onclick = null;
        anchor.setAttribute ('aria-disabled', 'true');
      }

      if (itemInfo.DiagnosticMetadata)
      {
        // Do not render empty diagnostic metadata attributes
        $.each(itemInfo.DiagnosticMetadata, function (key, value)
        {
          if (value === "" || value === null)
          {
            delete itemInfo.DiagnosticMetadata[key];
          }
        });

        $(item).attr(itemInfo.DiagnosticMetadata);
      }

      if (itemInfo.DiagnosticMetadataForCommand)
      {
        // Do not render empty diagnostic metadata attributes
        $.each(itemInfo.DiagnosticMetadataForCommand,
          function (key, value) {
            if (value === "" || value === null)
            {
              delete itemInfo.DiagnosticMetadataForCommand[key];
            }
          });

        itemInfo.DiagnosticMetadataForCommand['data-is-disabled'] = isEnabled ? 'false' : 'true';

        $(anchor).attr(itemInfo.DiagnosticMetadataForCommand);
      }
    }
  }

  public static OnKeyDown (event: JQueryKeyEventObject, $listMenu: JQuery): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$tablist', $listMenu);

    var $menuItems = $listMenu.find ('a');

    var oldMenuItemIndex = -1;

    var selectedMenuItem = document.activeElement;
    if (selectedMenuItem != null && TypeUtility.IsDefined (selectedMenuItem.tagName) && selectedMenuItem.tagName.toUpperCase() === 'A')
    {
      oldMenuItemIndex = $menuItems.index(selectedMenuItem);
    }
    else
    {
      for (var i = 0; i < $menuItems.length; i++)
      {
        // TODO RM-7686 Fix misspellings of tabIndex
        if (($menuItems[i] as any).tabindex === 0)
        {
          oldMenuItemIndex = i;
          break;
        }
      }
    }

    var $oldMenuItem = null;
    if (oldMenuItemIndex >= 0)
      $oldMenuItem = $($menuItems[oldMenuItemIndex]);
    var currentMenuItemIndex = Math.max (0, oldMenuItemIndex);

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
            let $newMenuItem = $($menuItems[currentMenuItemIndex]);
            ListMenu.UpdateFocus ($newMenuItem, $oldMenuItem);

            $newMenuItem[0].click();
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
            currentMenuItemIndex = $menuItems.length - 1;

          let $newMenuItem = $ ($menuItems[currentMenuItemIndex]);
          ListMenu.UpdateFocus ($newMenuItem, $oldMenuItem);

          return;
        }
      case 39: // right arrow
      case 40: // down arrow
        {
          event.preventDefault();
          event.stopPropagation();

          if (currentMenuItemIndex < $menuItems.length - 1)
            currentMenuItemIndex++;
          else
            currentMenuItemIndex = 0;

          var $newMenuItem = $($menuItems[currentMenuItemIndex]);
          ListMenu.UpdateFocus ($newMenuItem, $oldMenuItem);

          return;
        }
    }
  }

  public static UpdateFocus ($newMenuItem: JQuery, $oldMenuItem: Nullable<JQuery>): void
  {
    ArgumentUtility.CheckNotNullAndTypeIsJQuery('$newMenuItem', $newMenuItem);
    if ($oldMenuItem != null)
      ArgumentUtility.CheckNotNullAndTypeIsJQuery ('$oldMenuItem', $oldMenuItem);

    $newMenuItem.focus();
  }
}
