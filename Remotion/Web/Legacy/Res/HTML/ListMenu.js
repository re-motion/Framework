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
var _listMenu_listMenuInfos = new Object();

var _listMenu_itemClassName = 'listMenuItem';
var _listMenu_itemFocusClassName = 'listMenuItemFocus';
var _listMenu_itemDisabledClassName = 'listMenuItemDisabled';
var _listMenu_requiredSelectionAny = 0;
var _listMenu_requiredSelectionExactlyOne = 1;
var _listMenu_requiredSelectionOneOrMore = 2;

function ListMenu_MenuInfo (id, itemInfos) {
    this.ID = id;
    this.ItemInfos = itemInfos;
}

function ListMenuItemInfo (id, category, text, icon, iconDisabled, requiredSelection, isDisabled, href, target) {
    this.ID = id;
    this.Category = category;
    this.Text = text;
    this.Icon = icon;
    this.IconDisabled = iconDisabled;
    this.RequiredSelection = requiredSelection;
    this.IsDisabled = isDisabled;
    this.Href = href;
    this.Target = target;
}

function ListMenu() { }

function ListMenu_AddMenuInfo (listMenu, menuInfo) {
    _listMenu_listMenuInfos[listMenu.id] = menuInfo;
}

function ListMenu_Update (listMenu, getSelectionCount) {
    var menuInfo = _listMenu_listMenuInfos[listMenu.id];
    if (menuInfo == null)
        return;

    var itemInfos = menuInfo.ItemInfos;
    var selectionCount = -1;
    if (getSelectionCount != null)
        selectionCount = getSelectionCount();

    for (var i = 0; i < itemInfos.length; i++) {
        var itemInfo = itemInfos[i];
        var isEnabled = true;
        if (itemInfo.IsDisabled) {
            isEnabled = false;
        }
        else {
            if (itemInfo.RequiredSelection == _listMenu_requiredSelectionExactlyOne
          && selectionCount != 1) {
                isEnabled = false;
            }
            if (itemInfo.RequiredSelection == _listMenu_requiredSelectionOneOrMore
          && selectionCount < 1) {
                isEnabled = false;
            }
        }
        var item = document.getElementById(itemInfo.ID);
        var anchor = $(item).children().eq(0)[0];
        var icon = $(anchor).children().eq(0)[0];
        if (isEnabled) {
            if (icon != null && icon.nodeType==1)
                icon.src = itemInfo.Icon;
            item.className = _listMenu_itemClassName;
            if (itemInfo.Href != null) {
                if (itemInfo.Href.toLowerCase().indexOf('javascript:') >= 0) {
                    anchor.href = '#';
                    anchor.removeAttribute('target');
                    anchor.setAttribute('javascript', itemInfo.Href);
                    anchor.onclick = function() { eval(this.getAttribute('javascript')); };
                }
                else {
                    anchor.href = itemInfo.Href;
                    if (itemInfo.Target != null)
                        anchor.target = itemInfo.Target;
                    anchor.onclick = null;
                    anchor.removeAttribute('javascript');
                }
            }
        }
        else {
            if (icon != null && icon.nodeType == 1) {
                if (itemInfo.IconDisabled != null)
                    icon.src = itemInfo.IconDisabled;
                else
                    icon.src = itemInfo.Icon;
            }
            item.className = _listMenu_itemDisabledClassName;
            anchor.removeAttribute('href');
            anchor.removeAttribute('target');
            anchor.onclick = null;
            anchor.removeAttribute('javascript');
        }
    }
}