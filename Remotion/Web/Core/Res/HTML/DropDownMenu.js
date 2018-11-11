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

var _dropDownMenu_buttonClassName = 'DropDownMenuSelect';
var _dropDownMenu_itemClassName = 'DropDownMenuItem';
var _dropDownMenu_itemDisabledClassName = 'DropDownMenuItemDisabled';
var _dropDownMenu_itemIconClassName = 'DropDownMenuItemIcon';
var _dropDownMenu_itemSeparatorClassName = 'DropDownMenuSeparator';
var _dropDownMenu_itemSelectedClassName = 'selected';
var _dropDownMenu_focusClassName = 'focus';
var _dropDownMenu_nestedHoverClassName = 'nestedHover';
var _dropDownMenu_currentMenu = null;
var _dropDownMenu_currentPopup = null;

var _dropDownMenu_menuItemIDPrefix = 'menuItem_';

var _dropDownMenu_requiredSelectionAny = 0;
var _dropDownMenu_requiredSelectionExactlyOne = 1;
var _dropDownMenu_requiredSelectionOneOrMore = 2;

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

function DropDownMenu_BindOpenEvent (openTarget, menuID, eventType, getSelectionCount, moveToMousePosition)
{
  ArgumentUtility.CheckNotNullAndTypeIsJQuery ('openTarget', openTarget);
  ArgumentUtility.CheckNotNullAndTypeIsString ('menuID', menuID);
  ArgumentUtility.CheckNotNullAndTypeIsString ('eventType', eventType);
  ArgumentUtility.CheckNotNullAndTypeIsBoolean ('moveToMousePosition', moveToMousePosition);

  var menuButton = openTarget[0].tagName.toLowerCase() === 'a' ? openTarget : openTarget.find ('a[href]:last');
  menuButton.attr('aria-haspopup', 'menu');
  menuButton.attr('aria-expanded', 'false');
  menuButton.removeAttr('aria-expanded');

  openTarget.bind(
      eventType,
      function (evt)
      {
        menuButton.focus();
        DropDownMenu_OnClick (openTarget, menuID, getSelectionCount, moveToMousePosition ? evt : null);
      });

  if (!moveToMousePosition)
  {
    menuButton
        .bind ("focus", function (evt) { openTarget.find ('.' + _dropDownMenu_buttonClassName).addClass (_dropDownMenu_focusClassName); })
        .bind("blur", function (evt) { openTarget.find('.' + _dropDownMenu_buttonClassName).removeClass (_dropDownMenu_focusClassName); });

    var allButMenuButton = openTarget.find ('a[href]').not(menuButton);
    allButMenuButton
        .bind ("mouseover", function (evt) { openTarget.find ('.' + _dropDownMenu_buttonClassName).addClass (_dropDownMenu_nestedHoverClassName); })
        .bind ("mouseout", function (evt) { openTarget.find ('.' + _dropDownMenu_buttonClassName).removeClass (_dropDownMenu_nestedHoverClassName); });
  }
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
  ArgumentUtility.CheckNotNullAndTypeIsJQuery('context', context);

  if (_dropDownMenu_itemClicked)
  {
    _dropDownMenu_itemClicked = false;
    return;
  }
  if (_dropDownMenu_currentMenu != null && _dropDownMenu_currentMenu[0] !== context[0])
  {
    DropDownMenu_ClosePopUp();
  }
  if (_dropDownMenu_currentMenu == null)
  {
    var event = null;
    if (evt != null)
    {
      var bounds = context[0].getBoundingClientRect();
      if (bounds.left <= evt.clientX &&
        bounds.right >= evt.clientX &&
        bounds.top <= evt.clientY &&
        bounds.bottom >= evt.clientY)
        event = evt;
    }
    DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, event);
    _dropDownMenu_currentMenu = context;
  }
}

function DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, evt)
{
  ArgumentUtility.CheckNotNullAndTypeIsJQuery('context', context);

  var itemInfos = _dropDownMenu_menuInfos[menuID].ItemInfos;
  var selectionCount = -1;
  if (getSelectionCount != null)
    selectionCount = getSelectionCount();

  if (itemInfos.length == 0)
    return;

  var menuOptionsID = menuID + '_DropDownMenuOptions';
  var menuButton = $('#' + menuID + ' a[aria-haspopup=menu]');
  menuButton.attr('aria-controls', menuOptionsID);
  menuButton.attr('aria-expanded', 'true');

  var div = document.createElement('div');
  div.className = 'DropDownMenuOptions';
  div.id = menuOptionsID;
  div.setAttribute('role', 'menu');
  div.setAttribute('aria-labelledby', menuButton[0].id);
  div.setAttribute('tabindex', '-1');
  _dropDownMenu_currentPopup = div;

  var ul = document.createElement('ul');
  ul.className = 'DropDownMenuOptions';
  ul.setAttribute('role', 'none');
  div.appendChild(ul);

  $('body')[0].appendChild(div);

  $(ul).mouseover (function (event)
  {
    var eventTarget = DropDownMenu_GetTarget (event, "LI");
    $("li", ul).removeClass(_dropDownMenu_itemSelectedClassName);
    if (eventTarget.firstChild != null && eventTarget.firstChild.nodeName.toLowerCase() === 'a')
    {
      $(eventTarget).addClass (_dropDownMenu_itemSelectedClassName);
      eventTarget.firstChild.focus();
    }
  }).keydown (function (event)
  {
    DropDownMenu_Options_OnKeyDown (event, _dropDownMenu_currentMenu);
  });

  _dropDownMenu_itemClickHandler = function()
  {
    if (this.href == null || this.href === '')
      return false;

    _dropDownMenu_itemClicked = true;
    DropDownMenu_ClosePopUp();
    try
    {
      eval(this.getAttribute('javascript'));
    }
    catch (e)
    {
    }
    setTimeout (function () { _dropDownMenu_itemClicked = false; }, 10);
    return false;
  };
  setTimeout (function () { $ ('body').bind ('click', DropDownMenu_ClosePopUp); }, 10);

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

  var div = $(_dropDownMenu_currentPopup);
  var menuButtonID = div.attr('aria-labelledBy');
  var menuButton = document.getElementById (menuButtonID);
  menuButton.setAttribute('aria-expanded', 'false');
  menuButton.removeAttribute('aria-controls');
  menuButton.focus();

  $("li", div).removeClass(_dropDownMenu_itemSelectedClassName);

  $('body').unbind('click', DropDownMenu_ClosePopUp);
  _dropDownMenu_currentMenu = null;
  _dropDownMenu_currentPopup = null;

  div.remove();
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
    icon.alt = '';
    icon.setAttribute('aria-hidden', 'true');
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
    $.each(itemInfo.DiagnosticMetadata, function (key, value)
    {
      if (value === "" || value === null)
      {
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
  item.setAttribute('role', 'none');
  textPane.innerHTML = '&nbsp;';

  item.appendChild(textPane);

  return item;
}

function DropDownMenu_OnHeadControlClick()
{
  _dropDownMenu_itemClicked = true;
}

function DropDownMenu_OnKeyDown(event, dropDownMenu, getSelectionCount, hasDedicatedDropDownMenuElement)
{
  ArgumentUtility.CheckNotNullAndTypeIsJQuery('dropDownMenu', dropDownMenu);

  if (_dropDownMenu_currentMenu === null && !hasDedicatedDropDownMenuElement)
    return;

  switch (event.keyCode)
  {
    case 9: // tab
      DropDownMenu_ClosePopUp();
      return;
    case 13: //enter
    case 32: //space
      event.preventDefault();
      event.stopPropagation();
      if (dropDownMenu !== _dropDownMenu_currentMenu)
      {
        DropDownMenu_OnClick (dropDownMenu, dropDownMenu[0].id, getSelectionCount, null);
        event.keyCode = 40; // always act as if the down-arrow was used when opening the drop down menu.
        DropDownMenu_Options_OnKeyDown (event, dropDownMenu);
      }
      else
      {
        DropDownMenu_ClosePopUp();
      }
      return;
    case 27: //escape
      event.preventDefault();
      event.stopPropagation();
      DropDownMenu_ClosePopUp();
      return;
    case 38: // up arrow
    case 40: // down arrow
      event.preventDefault();
      event.stopPropagation();
      if (dropDownMenu !== _dropDownMenu_currentMenu)
      {
        DropDownMenu_OnClick (dropDownMenu, dropDownMenu[0].id, getSelectionCount, null);
        event.keyCode = 40; // always act as if the down-arrow was used when opening the drop down menu.
      }
      DropDownMenu_Options_OnKeyDown (event, dropDownMenu);
      return;
    default:
      DropDownMenu_Options_OnKeyDown(event, dropDownMenu);
      return;
  }
}

function DropDownMenu_Options_OnKeyDown(event, dropDownMenu)
{
  ArgumentUtility.CheckNotNullAndTypeIsJQuery('dropDownMenu', dropDownMenu);

  if (_dropDownMenu_currentPopup == null)
    return;

  var itemInfos = _dropDownMenu_menuInfos[dropDownMenu[0].id].ItemInfos;
  if (itemInfos.length === 0)
    return;

  var oldIndex;
  var isSelectionUpdated = false;
  var dropDownMenuItems = $(_dropDownMenu_currentPopup).find('ul').children();
  var currentItemIndex = $('.' + _dropDownMenu_itemSelectedClassName, _dropDownMenu_currentPopup).index();

  switch (event.keyCode)
  {
    case 9: // tab
      DropDownMenu_ClosePopUp();
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
      DropDownMenu_ClosePopUp();
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
    $("li", _dropDownMenu_currentPopup).removeClass (_dropDownMenu_itemSelectedClassName);
    if (currentItemIndex >= 0 && currentItemIndex < itemInfos.length)
    {
      var dropDownMenuItem = $(dropDownMenuItems[currentItemIndex]);
      if (dropDownMenuItem.children('a').length > 0) // skip separators
      {
        dropDownMenuItem.addClass(_dropDownMenu_itemSelectedClassName);
        dropDownMenuItem.children ('a').focus();
      }
    }
  }
}

function DropDownMenu_GetItemEnabled (itemInfo, selectionCount)
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

function DropDownMenu_GetTarget (event, tagName)
{
  var element = event.target;
  while (element && element.tagName != tagName)
    element = element.parentNode;
  // more fun with IE, sometimes event.target is empty, just ignore it then
  if (!element)
    return [];
  return element;
}
