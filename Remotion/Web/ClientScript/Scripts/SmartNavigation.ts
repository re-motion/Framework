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
  
    const fields = value.split (' ');
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
          
    const dataFields = data.split ('*');
    if (dataFields.length == 0)
      return;
    
    const dataField = dataFields[0].slice (1);
    const sseBody = SmartScrolling_Element.Parse (dataField);
    // TODO RM-7698: SmartScrolling.Restore can fail when parsing the specified SmartScrolling_Element
    window.document.body.scrollTop = sseBody!.Top;
    window.document.body.scrollLeft = sseBody!.Left;
    
    for (let i = 0; i < dataFields.length; i++)
    {
      const scrollElement = SmartScrolling_Element.Parse (dataFields[i]);
      SmartScrolling.SetScrollPosition (scrollElement);
    } 
  }

  public static Backup(): string
  {
    const scrollElements: SmartScrolling_Element[] = [];
    
    if (TypeUtility.IsUndefined (window.document.body.id) || StringUtility.IsNullOrEmpty (window.document.body.id))
    {
      const sseBody = new SmartScrolling_Element ('body', window.document.body.scrollTop, window.document.body.scrollLeft);
      scrollElements.push(sseBody);
    }
    scrollElements.push(...SmartScrolling.GetScrollPositions (window.document.body));
    
    let data = '';
    for (let i = 0; i < scrollElements.length; i++)
    {
      if (i > 0)
        data += '*'; 
      const scrollElement = scrollElements[i];
      data += scrollElement.ToString();
    }

    return data;
  }

  public static GetScrollPositions (currentElement: Nullable<HTMLElement>): SmartScrolling_Element[]
  {
    const scrollElements: SmartScrolling_Element[] = [];
    if (currentElement != null)
    {
      if (   ! TypeUtility.IsUndefined (currentElement.id) && ! StringUtility.IsNullOrEmpty (currentElement.id)
          && (currentElement.scrollTop != 0 || currentElement.scrollLeft != 0))
      {
        const sseCurrentElement = SmartScrolling.GetScrollPosition (currentElement)!;
        scrollElements.push(sseCurrentElement);
      }
      
      for (let i = 0; i < currentElement.childNodes.length; i++)
      {
        const element = currentElement.childNodes[i];
        const scrollChilden = SmartScrolling.GetScrollPositions (element as HTMLElement);
        scrollElements.push(...scrollChilden);
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
    const htmlElement = window.document.getElementById (scrollElement.ID);
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
    const activeElement = window.document.activeElement;
    return activeElement?.id || '';
  }

  public static Restore (data: string): boolean
  {
    const activeElementID = data;
    if (! StringUtility.IsNullOrEmpty (activeElementID))
    {
      const activeElement = document.getElementById(activeElementID);
      if (activeElement)
      {
        function isEnabledFilter(el: HTMLElement)
        {
          if (el.closest('[aria-hidden=true]') !== null)
            return false;
          if (el.tagName === 'BUTTON')
            return true;
          if (el.tagName === 'INPUT' && (el as HTMLInputElement).type.toLowerCase() === "button")
            return true;
          if (el.tagName === 'INPUT' && (el as HTMLInputElement).type.toLowerCase() === "submit")
            return true;
          if (el.tagName === 'A')
            return true;
          if (el.tagName !== 'A' && (el as HTMLInputElement).disabled === false && !(el.tagName === 'INPUT' && (el as HTMLInputElement).type.toLowerCase() === "hidden"))
            return true;
          if (el.tagName !== 'A' && el.hasAttribute ('tabindex'))
            return true;
          return false;
        };

        if (isEnabledFilter(activeElement))
        {
          SmartFocus.SetFocus (activeElement);
        }
        else
        {
          const focusableElements = Array.from(document.querySelectorAll<HTMLElement>('input, textarea, select, button, a'));
          const elementIndex = focusableElements.indexOf(activeElement);
          let fallBackElements = focusableElements.slice(elementIndex).filter(isEnabledFilter);
          if (fallBackElements.length > 0)
          {
            SmartFocus.SetFocus (fallBackElements[0]);
          }
          else
          {
            fallBackElements = focusableElements.slice(0, elementIndex).filter(isEnabledFilter);
            if (fallBackElements.length > 0)
            {
              SmartFocus.SetFocus (fallBackElements[fallBackElements.length - 1]);
            }
          }
        }
      }
    }
    // TODO RM-7702: SmartFocus.Restore implicitly returns undefined instead of a boolean
    return undefined as unknown as boolean;
  }

  public static SetFocus (element: HTMLElement): void
  {
    setTimeout(function () { element.focus(); }, 0);
  }
}
