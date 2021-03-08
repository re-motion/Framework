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
class SmartScrolling_Element
{
  constructor (
    public readonly ID: string, 
    public readonly Top: number, 
    public readonly Left: number)
  {
  }

  public ToString()
  {
    if (StringUtility.IsNullOrEmpty (this.ID))
      return '';
    else
      return this.ID + ' ' + this.Top + ' ' + this.Left;
  };

  public static Parse (value: Nullable<string>): Nullable<SmartScrolling_Element>
  {
    ArgumentUtility.CheckTypeIsString ('value', value);
    if (StringUtility.IsNullOrEmpty (value))
      return null;
  
    var fields = value.split (' ');
    // TODO RM-7697: SmartScrolling_Element.Parse does not convert strings into numbers when parsing
    return new SmartScrolling_Element (fields[0], fields[1] as unknown as number, fields[2] as unknown as number);
  };
}

class SmartScrolling
{
  public static Restore (data: Nullable<string>): void
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
    // TODO RM-7698: SmartScrolling.Restore can fail when parsing the specified SmartScrolling_Element
    window.document.body.scrollTop = sseBody!.Top;
    window.document.body.scrollLeft = sseBody!.Left;
    
    for (var i = 0; i < dataFields.length; i++)
    {
      var scrollElement = SmartScrolling_Element.Parse (dataFields[i]);
      SmartScrolling.SetScrollPosition (scrollElement);
    } 
  }

  public static Backup(): string
  {
    var data = '';
    var scrollElements: SmartScrolling_Element[] = [];
    
    if (TypeUtility.IsUndefined (window.document.body.id) || StringUtility.IsNullOrEmpty (window.document.body.id))
    {
      var sseBody = 
          new SmartScrolling_Element ('body', window.document.body.scrollTop, window.document.body.scrollLeft);
      scrollElements[scrollElements.length] = sseBody;
    }
    scrollElements = scrollElements.concat (SmartScrolling.GetScrollPositions (window.document.body));
    
    for (var i = 0; i < scrollElements.length; i++)
    {
      if (i > 0)
        data += '*'; 
      var scrollElement = scrollElements[i];
      data += scrollElement.ToString();
    }

    return data;
  }

  public static GetScrollPositions (currentElement: Nullable<HTMLElement>): SmartScrolling_Element[]
  {
    var scrollElements: SmartScrolling_Element[] = [];
    if (currentElement != null)
    {
      if (   ! TypeUtility.IsUndefined (currentElement.id) && ! StringUtility.IsNullOrEmpty (currentElement.id)
          && (currentElement.scrollTop != 0 || currentElement.scrollLeft != 0))
      {
        var sseCurrentElement = SmartScrolling.GetScrollPosition (currentElement)!;
        scrollElements[scrollElements.length] = sseCurrentElement;
      }
      
      for (var i = 0; i < currentElement.childNodes.length; i++)
      {
        var element = currentElement.childNodes[i];
        var scrollChilden = SmartScrolling.GetScrollPositions (element as HTMLElement);
        scrollElements = scrollElements.concat (scrollChilden);
      }
    }
    return scrollElements;  
  }

  public static GetScrollPosition (htmlElement: Nullable<HTMLElement>): Nullable<SmartScrolling_Element>
  {
    if (htmlElement != null)
      return new SmartScrolling_Element (htmlElement.id, htmlElement.scrollTop, htmlElement.scrollLeft);
    else
      return null;
  }

  public static SetScrollPosition (scrollElement: Nullable<SmartScrolling_Element>): void
  {
    if (scrollElement == null)
      return;
    var htmlElement = window.document.getElementById (scrollElement.ID);
    if (htmlElement == null)
      return;
    htmlElement.scrollTop = scrollElement.Top;
    htmlElement.scrollLeft = scrollElement.Left;
  }
}

class SmartFocus
{
  public static Backup(): string
  {
    var data = '';
    var activeElement = window.document.activeElement;
    if (activeElement != null)
    {
      data += activeElement.id;
    }
    return data;
  }

  public static Restore (data: string): boolean
  {
    var activeElementID = data;
    if (! StringUtility.IsNullOrEmpty (activeElementID))
    {
      var activeElement = $('#' + activeElementID);
      if (activeElement)
      {
        var isEnabledFilter = function (this: HTMLElement)
        {
          var _this = $ (this);
          if (_this.is ('button'))
            return true;
          if (_this.is ('input[type=button]'))
            return true;
          if (_this.is ('input[type=submit]'))
            return true;
          if (_this.is ('a'))
            return true;
          if (!_this.is ('a') && _this.is (':enabled') && !_this.is ('input[type=hidden]'))
            return true;
          if (!_this.is ('a') && _this.is ('*[tabindex]'))
            return true;
          return false;
        };

        if (activeElement.is(isEnabledFilter))
        {
          SmartFocus.SetFocus (activeElement);
        }
        else
        {
          var focusableElements = $('input, textarea, select, button, a');
          var elementIndex = focusableElements.index(activeElement);
          var fallBackElement = focusableElements.slice(elementIndex).filter(isEnabledFilter).first();
          if (fallBackElement.length > 0)
          {
            SmartFocus.SetFocus (fallBackElement);
          }
          else
          {
            fallBackElement = focusableElements.slice(0, elementIndex).filter(isEnabledFilter).last();
            if (fallBackElement)
            {
              SmartFocus.SetFocus (fallBackElement);
            }
          }
        }
      }
    }
    // TODO RM-7702: SmartFocus.Restore implicitly returns undefined instead of a boolean
    return undefined as unknown as boolean;
  }

  public static SetFocus (element: JQuery): void
  {
    setTimeout(function () { element.focus(); }, 0);
  }
}
