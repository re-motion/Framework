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
function SmartScrolling_Element (id, top, left)
{
  this.ID = id;
  this.Top = top;
  this.Left = left;
  
  this.ToString = function ()
  {
    if (StringUtility.IsNullOrEmpty (this.ID))
      return '';
    else
      return this.ID + ' ' + this.Top + ' ' + this.Left;
  };
}

SmartScrolling_Element.Parse = function (value)
{
  ArgumentUtility.CheckTypeIsString ('value', value);
  if (StringUtility.IsNullOrEmpty (value))
    return null;

  var fields = value.split (' ');
  return new SmartScrolling_Element (fields[0], fields[1], fields[2]);
};

function SmartScrolling_Restore (data)
{
  ArgumentUtility.CheckTypeIsString ('data', data);
  if (StringUtility.IsNullOrEmpty (data))
    return;
        
  var dataFields = data.split ('*');
  if (dataFields.length == 0)
    return;
  
  var dataField = dataFields[0];
  dataFields = dataFields.slice (1);
  var sseBody = SmartScrolling_Element.Parse (dataField);
  window.document.body.scrollTop = sseBody.Top;
  window.document.body.scrollLeft = sseBody.Left;
  
  for (var i = 0; i < dataFields.length; i++)
  {
    var scrollElement = SmartScrolling_Element.Parse (dataFields[i]);
    SmartScrolling_SetScrollPosition (scrollElement);
  } 
}

function SmartScrolling_Backup ()
{
  var data = '';
  var scrollElements = new Array();
  
  if (TypeUtility.IsUndefined (window.document.body.id) || StringUtility.IsNullOrEmpty (window.document.body.id))
  {
    var sseBody = 
        new SmartScrolling_Element ('body', window.document.body.scrollTop, window.document.body.scrollLeft);
    scrollElements[scrollElements.length] = sseBody;
  }
  scrollElements = scrollElements.concat (SmartScrolling_GetScrollPositions (window.document.body));
  
  for (var i = 0; i < scrollElements.length; i++)
  {
    if (i > 0)
      data += '*'; 
    var scrollElement = scrollElements[i];
    data += scrollElement.ToString();
  }

  return data;
}

function SmartScrolling_GetScrollPositions (currentElement)
{
  var scrollElements = new Array();
  if (currentElement != null)
  {
    if (   ! TypeUtility.IsUndefined (currentElement.id) && ! StringUtility.IsNullOrEmpty (currentElement.id)
        && (currentElement.scrollTop != 0 || currentElement.scrollLeft != 0))
    {
      var sseCurrentElement = SmartScrolling_GetScrollPosition (currentElement);
      scrollElements[scrollElements.length] = sseCurrentElement;
    }
    
    for (var i = 0; i < currentElement.childNodes.length; i++)
    {
      var element = currentElement.childNodes[i];
      var scrollChilden = SmartScrolling_GetScrollPositions (element);
      scrollElements = scrollElements.concat (scrollChilden);
    }
  }
  return scrollElements;  
}

function SmartScrolling_GetScrollPosition (htmlElement)
{
  if (htmlElement != null)
    return new SmartScrolling_Element (htmlElement.id, htmlElement.scrollTop, htmlElement.scrollLeft);
  else
    return null;
}

function SmartScrolling_SetScrollPosition (scrollElement)
{
  if (scrollElement == null)
    return;
  var htmlElement = window.document.getElementById (scrollElement.ID);
  if (htmlElement == null)
    return;
  htmlElement.scrollTop = scrollElement.Top;
  htmlElement.scrollLeft = scrollElement.Left;
}

function SmartFocus_Backup ()
{
  var data = '';
  var activeElement = window.document.activeElement;
  if (activeElement != null)
  {
    data += activeElement.id;
  }
  return data;
}

function SmartFocus_Restore (data)
{
  var activeElementID = data;
  if (! StringUtility.IsNullOrEmpty (activeElementID))
  {
    var activeElement = $('#' + activeElementID);
    if (activeElement)
    {
      var isEnabledFilter = function ()
      {
        var _this = $ (this);
        if (_this.is ('a') && _this.is ('a[href]'))
          return true;
        if (!_this.is ('a') && _this.is (':enabled') && !_this.is ('input[type=hidden]'))
          return true;
        return false;
      };

      if (activeElement.is(isEnabledFilter))
      {
        SmartFocus_SetFocus (activeElement);
      }
      else
      {
        var focusableElements = $('input, textarea, select, button, a');
        var elementIndex = focusableElements.index(activeElement);
        var fallBackElement = focusableElements.slice(elementIndex).filter(isEnabledFilter).first();
        if (fallBackElement.length > 0)
        {
          SmartFocus_SetFocus (fallBackElement);
        }
        else
        {
          fallBackElement = focusableElements.slice(0, elementIndex).filter(isEnabledFilter).last();
          if (fallBackElement)
          {
            SmartFocus_SetFocus (fallBackElement);
          }
        }
      }
    }
  }
}

function SmartFocus_SetFocus(element)
{
  setTimeout(function ()
  {
    var ieVersion = BrowserUtility.GetIEVersion();

    if (ieVersion == 8 && (element.is('input[type=text]') || element.is('textarea')))
    {
      // IE8 has problems when restoring the focus during an AutoPostBack inside an UpdatePanel
      var elements = element[0].form.elements;
      var elementsLength = elements.length;
      for (var i = 0; i < elementsLength; i++)
      {
        var currentElement = $(elements[i]);
        if ((currentElement.is('input[type=text]') || currentElement.is('textarea')) && currentElement.is(':enabled'))
        {
          currentElement.focus();
          break;
        }
      }
    }

    element.focus();

    var isAffectedIEVersion = ieVersion == 7 || ieVersion == 8;
    if (!isAffectedIEVersion)
      return;

    if (element.is('a') || element.is('button') || element.is('input[type=submit]') || element.is('input[type=button]'))
      return;

    // special handling for IE7 and IE8
    // Note: for autopostbacks on an input field that results in a new window being opened, this will cause the parent window to jump back into front.
    setTimeout(function ()
    {
      try
      {
        if (window.document.activeElement =! null)
          return;
      }
      catch (e)
      {
      }

      element.focus();

      if (ieVersion == 8)
        return;

      // special handling for IE7, requires extensive decoupling in some cases
      setTimeout(function ()
      {
        try
        {
          if (window.document.activeElement =! null)
            return;
        }
        catch (e1)
        {
        }

        if (!element.is(':visible'))
          return;

        element.css("visibility", "hidden");
        element.css("visibility", "");
        element.focus();
      }, 1);
    }, 1);
  }, 0);
}
