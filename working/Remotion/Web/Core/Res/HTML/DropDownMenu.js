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
var _dropDownMenu_menuInfos = new Object();

var _dropDownMenu_itemClassName = 'DropDownMenuItem';
var _dropDownMenu_itemDisabledClassName = 'DropDownMenuItemDisabled';
var _dropDownMenu_itemIconClassName = 'DropDownMenuItemIcon';
var _dropDownMenu_itemSeparatorClassName = 'DropDownMenuSeparator';
var _dropDownMenu_currentMenu = null;
var _dropDownMenu_currentPopup = null;

var _dropDownMenu_menuItemIDPrefix = 'menuItem_';

var _dropDownMenu_requiredSelectionAny = 0;
var _dropDownMenu_requiredSelectionExactlyOne = 1;
var _dropDownMenu_requiredSelectionOneOrMore = 2;

var _dropDownMenu_currentItemIndex = -1;

var _dropDownMenu_itemClickHandler = null;
var _dropDownMenu_itemClicked = false;

var _dropDownMenu_repositionInterval = 200;
var _dropDownMenu_repositionTimer = null;

function DropDownMenu_MenuInfo(id, itemInfos)
{
  this.ID = id;
  this.ItemInfos = itemInfos;
}

function DropDownMenu_AddMenuInfo(menuInfo)
{
  _dropDownMenu_menuInfos[menuInfo.ID] = menuInfo;
}

function DropDownMenu_BindOpenEvent(node, menuID, eventType, getSelectionCount, moveToMousePosition)
{
  ArgumentUtility.CheckNotNull('node', node);
  ArgumentUtility.CheckNotNullAndTypeIsString('menuID', menuID);
  ArgumentUtility.CheckNotNullAndTypeIsString('eventType', eventType);
  ArgumentUtility.CheckNotNullAndTypeIsBoolean('moveToMousePosition', moveToMousePosition);

  $(node).bind(eventType, function (evt) { DropDownMenu_OnClick (node, menuID, getSelectionCount, moveToMousePosition ? evt : null); });
}

function DropDownMenu_ItemInfo(id, category, text, icon, iconDisabled, requiredSelection, isDisabled, href, target, diagnosticMetadata)
{
  this.ID = id;
  this.Category = category;
  this.Text = text;
  this.Icon = icon;
  this.IconDisabled = iconDisabled;
  this.RequiredSelection = requiredSelection;
  this.IsDisabled = isDisabled;
  this.Href = href;
  this.Target = target;
  this.DiagnosticMetadata = diagnosticMetadata;
}

function DropDownMenu_OnClick(context, menuID, getSelectionCount, evt)
{
  if (_dropDownMenu_itemClicked)
  {
    _dropDownMenu_itemClicked = false;
    return;
  }
  if (context != _dropDownMenu_currentMenu)
  {
    DropDownMenu_ClosePopUp();
  }
  if (_dropDownMenu_currentMenu == null)
  {
    DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, evt);
    _dropDownMenu_currentMenu = context;
  }
}

function DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, evt)
{
  var itemInfos = _dropDownMenu_menuInfos[menuID].ItemInfos;
  var selectionCount = -1;
  if (getSelectionCount != null)
    selectionCount = getSelectionCount();

  if (itemInfos.length == 0)
    return;

  var div = document.createElement('div');
  div.className = 'DropDownMenuOptions';
  _dropDownMenu_currentPopup = div;

  var ul = document.createElement('ul');
  ul.className = 'DropDownMenuOptions';
  div.appendChild(ul);

  $('body')[0].appendChild(div);

  _dropDownMenu_itemClickHandler = function()
  {
    _dropDownMenu_itemClicked = true;
    DropDownMenu_ClosePopUp();
    try
    {
      eval(this.getAttribute('javascript'));
    }
    catch (e)
    {
    }
    setTimeout('_dropDownMenu_itemClicked = false;', 10);
  };
  setTimeout("$('body').bind('click', DropDownMenu_ClosePopUp);", 10);

  for (var i = 0; i < itemInfos.length; i++)
  {
    var item = DropDownMenu_CreateItem(itemInfos[i], selectionCount, true);
    if (item != null)
      ul.appendChild(item);
  }

  var titleDiv = $(context).children().eq(0);
  DropDownMenu_ApplyPosition ($(div), evt, titleDiv);
  $(div).iFrameShim({ top: '0px', left: '0px', width: '100%', height: '100%' });

  if (_dropDownMenu_repositionTimer) 
    clearTimeout(_dropDownMenu_repositionTimer);
  var repositionHandler = function ()
  {
    if (_dropDownMenu_repositionTimer)
      clearTimeout (_dropDownMenu_repositionTimer);

    if (_dropDownMenu_currentPopup && _dropDownMenu_currentPopup == div && $(div).is (':visible'))
    {
      DropDownMenu_ApplyPosition ($(div), null, titleDiv);
      _dropDownMenu_repositionTimer = setTimeout (repositionHandler, _dropDownMenu_repositionInterval);
    }
  };

  // Only reposition if opened via titleDiv
  if (evt == null)
    _dropDownMenu_repositionTimer = setTimeout(repositionHandler, _dropDownMenu_repositionInterval);
}

function DropDownMenu_ApplyPosition (popUpDiv, clickEvent, referenceElement)
{
  var space_top = Math.round(referenceElement.offset().top - $(document).scrollTop());
  var space_bottom = Math.round($(window).height() - referenceElement.offset().top - referenceElement.height() + $(document).scrollTop());
  var space_left = referenceElement.offset().left;
  var space_right = $(window).width() - referenceElement.offset().left - referenceElement.width();

  // position drop-down list
  var top = clickEvent ? clickEvent.clientY : Math.max(0, referenceElement.offset().top + referenceElement.outerHeight());
  var left = clickEvent ? clickEvent.clientX : 'auto';
  var right = clickEvent ? 'auto' : Math.max(0, $(window).width() - referenceElement.offset().left - referenceElement.outerWidth());

  popUpDiv.css('top', top);
  popUpDiv.css('bottom', 'auto');
  popUpDiv.css('right', right);
  popUpDiv.css('left', left);

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

function DropDownMenu_ClosePopUp()
{
  if (_dropDownMenu_currentMenu == null)
    return;

  $(_dropDownMenu_currentMenu).find('a[href]').first().focus();

  var div = $(_dropDownMenu_currentPopup);

  $('body').unbind('click', DropDownMenu_ClosePopUp);
  _dropDownMenu_currentMenu = null;
  _dropDownMenu_currentPopup = null;

  div.remove();

  _dropDownMenu_currentItemIndex = -1;
}

function DropDownMenu_CreateItem(itemInfo, selectionCount)
{
  if (itemInfo == null)
    return null;

  var item;
  if (itemInfo.Text == '-')
    item = DropDownMenu_CreateSeparatorItem();
  else
    item = DropDownMenu_CreateTextItem(itemInfo, selectionCount);

  return item;
}

function DropDownMenu_CreateTextItem(itemInfo, selectionCount)
{
  var isEnabled = DropDownMenu_GetItemEnabled(itemInfo, selectionCount);

  var item = document.createElement("li");

  var className = _dropDownMenu_itemClassName;
  if (!isEnabled)
    className = _dropDownMenu_itemDisabledClassName;

  item.className = className;

  var anchor = document.createElement("a");
  anchor.setAttribute('href', '#');
  if (isEnabled)
    $(anchor).bind('click', _dropDownMenu_itemClickHandler);
  
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

    icon.className = _dropDownMenu_itemIconClassName;
    icon.style.verticalAlign = 'middle';
    anchor.appendChild(icon);
  }
  else
  {
    var iconPlaceholder = document.createElement('span');
    iconPlaceholder.className = _dropDownMenu_itemIconClassName;
    anchor.appendChild(iconPlaceholder);
  }

  if (itemInfo.DiagnosticMetadata)
  {
    // Do not render empty diagnostic metadata attributes
    $.each(itemInfo.DiagnosticMetadata, function (key, value) {
      if (value === "" || value === null) {
        delete itemInfo.DiagnosticMetadata[key];
      }
    });

    $ (item).attr (itemInfo.DiagnosticMetadata);
  }

  var span = document.createElement('span');
  span.innerHTML = itemInfo.Text;
  anchor.appendChild(span);

  return item;
}

function DropDownMenu_CreateSeparatorItem()
{
  var item = document.createElement('li');

  var textPane = document.createElement('span');
  textPane.className = _dropDownMenu_itemSeparatorClassName;
  textPane.innerHTML = '&nbsp;';

  item.appendChild(textPane);

  return textPane;
}

function DropDownMenu_OnHeadControlClick()
{
  _dropDownMenu_itemClicked = true;
}

function DropDownMenu_OnKeyDown(event, dropDownMenu, getSelectionCount)
{
  // alert(event.keyCode + ', ' + dropDownMenu.nodeName + '#' + dropDownMenu.id);

  var itemInfos = _dropDownMenu_menuInfos[dropDownMenu.id].ItemInfos;
  var oldIndex = _dropDownMenu_currentItemIndex;
  var selectionCount = -1;
  if (getSelectionCount != null)
    selectionCount = getSelectionCount();

  switch (event.keyCode)
  {
    case 13: //enter
    case 32: //space
      if (_dropDownMenu_currentItemIndex >= 0)
      {
        var itemAnchor = $($(dropDownMenu).find('ul').children()[_dropDownMenu_currentItemIndex]).children('a');
        itemAnchor.click();
      }

      if (dropDownMenu != _dropDownMenu_currentMenu)
        DropDownMenu_OnClick(dropDownMenu, dropDownMenu.id, getSelectionCount, null);
      else
        DropDownMenu_ClosePopUp();
      break;
    case 27: //escape
      DropDownMenu_ClosePopUp();
      break;
    case 39: // right arrow
    case 40: // down arrow
      do
      {
        if (_dropDownMenu_currentItemIndex < itemInfos.length - 1)
          _dropDownMenu_currentItemIndex++;
        else
          _dropDownMenu_currentItemIndex = 0;
      } while (!DropDownMenu_IsSelectableItem(itemInfos, _dropDownMenu_currentItemIndex, selectionCount))
      break;
    case 37: // left arrow
    case 38: // up arrow
      do
      {
        if (_dropDownMenu_currentItemIndex > 0)
          _dropDownMenu_currentItemIndex--;
        else
          _dropDownMenu_currentItemIndex = itemInfos.length - 1;
      } while (!DropDownMenu_IsSelectableItem(itemInfos, _dropDownMenu_currentItemIndex, selectionCount))
      break;
  }
  if (0 <= _dropDownMenu_currentItemIndex && _dropDownMenu_currentItemIndex < itemInfos.length)
  {
    if (_dropDownMenu_currentPopup != null)
    {
      var menuItems = $(_dropDownMenu_currentPopup).children('ul:first').children();
      var liGainSelected = menuItems[_dropDownMenu_currentItemIndex];
      liGainSelected.className += " selected";

      if (oldIndex >= 0)
      {
        var liLoseSelected = menuItems[oldIndex];
        liLoseSelected.className = _dropDownMenu_itemClassName;
      }
    }
  }
}

function DropDownMenu_IsSelectableItem(itemInfos, index, selectionCount)
{
  var isSeparator = (itemInfos[_dropDownMenu_currentItemIndex].ID == -1);

  return !isSeparator && DropDownMenu_GetItemEnabled(itemInfos[index], selectionCount);
}

function DropDownMenu_GetItemEnabled(itemInfo, selectionCount)
{
  var isEnabled = true;
  if (itemInfo.IsDisabled)
  {
    isEnabled = false;
  }
  else
  {
    if (itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionExactlyOne
        && selectionCount != 1)
    {
      isEnabled = false;
    }
    if (itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionOneOrMore
        && selectionCount < 1)
    {
      isEnabled = false;
    }
  }
  return isEnabled;
}