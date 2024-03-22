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

type DatePickerContext = {
  DatePicker: HTMLElement;
  Target: HTMLInputElement;
  TargetDocument: Nullable<Document>;
  ClickHandler: Nullable<EventHandler<MouseEvent>>;
};

class DatePicker
{
  private static _datePicker_current: Nullable<DatePickerContext> = null;
  private static _datePicker_repositionInterval = 200;
  private static _datePicker_repositionTimer: Nullable<number> = null;

  public static ShowDatePicker (
    buttonOrSelector: CssSelectorOrElement<HTMLElement>,
    containerOrSelector: CssSelectorOrElement<HTMLElement>,
    targetOrSelector: CssSelectorOrElement<HTMLInputElement>,
    src: string,
    width: string,
    height: string): void
  {
    ArgumentUtility.CheckNotNull("buttonOrSelector", buttonOrSelector);
    ArgumentUtility.CheckNotNull("containerOrSelector", containerOrSelector);
    ArgumentUtility.CheckNotNull("targetOrSelector", targetOrSelector);

    const button = ElementResolverUtility.ResolveSingle(buttonOrSelector);
    const container = ElementResolverUtility.ResolveSingle(containerOrSelector);
    const target = ElementResolverUtility.ResolveSingle(targetOrSelector);

    const datePickerID = container.id + '_DatePicker';
    //  Tried to open the already open date picker?
    //  Close it and return.
    if (DatePicker.CloseVisibleDatePickerFrame (datePickerID))
      return;
      
    if (target.disabled || target.readOnly)
      return;

    const datePicker = DatePicker.Create(datePickerID, button, target, src, width, height);

    const targetDocument = target.ownerDocument;
    let clickHandler: Nullable<EventHandler<MouseEvent>> = null;
    if (targetDocument)
    {
      let isEventAfterDatePickerButtonClick = true;
      clickHandler = function (event: MouseEvent)
      {
        if (isEventAfterDatePickerButtonClick)
        {
          isEventAfterDatePickerButtonClick = false;
        }
        else
        {
          DatePicker.CloseDatePicker(false);
        }
      };
      targetDocument.addEventListener('click', clickHandler);
    }

    DatePicker._datePicker_current = { DatePicker: datePicker, Target: target, TargetDocument: targetDocument, ClickHandler: clickHandler };
    datePicker.style.visibility = 'visible';
  }

  private static CloseVisibleDatePickerFrame (newDatePickerID: string): boolean
  {
    let isSameDatePicker = false;
    const newDatePicker = document.getElementById(newDatePickerID);
    if (newDatePicker && LayoutUtility.IsVisible(newDatePicker))
      isSameDatePicker = true;

    DatePicker.CloseDatePicker(isSameDatePicker);

    return isSameDatePicker;
  }

  private static CloseDatePicker(setFocusOnTarget: boolean): void
  {
    if (DatePicker._datePicker_current == null)
      return;

    const current = DatePicker._datePicker_current;
    DatePicker._datePicker_current = null;

    if (setFocusOnTarget)
    {
      current.Target.focus();
    }

    current.DatePicker.remove();
    if (current.TargetDocument != null)
    {
      current.TargetDocument.removeEventListener('click', current.ClickHandler!);
    }
  }

  public static UpdateValue(value: string): void
  {
    const current = DatePicker._datePicker_current;
    if (current == null)
      return;

    DatePicker.CloseDatePicker(true);

    const isValueChanged = current.Target.value != value;
    if (isValueChanged)
    {
      current.Target.value = value;
      current.Target.dispatchEvent(new Event('change'));
    } 
  }

  private static Create(datePickerID: string, button: HTMLElement, target: HTMLInputElement, src: string, width: string, height: string): HTMLElement
  {
    const datePicker = document.createElement('div');
    datePicker.setAttribute('id', datePickerID);
    datePicker.classList.add('DatePicker');
    datePicker.style.width = width;
    datePicker.style.height = height;
    datePicker.style.visibility = 'hidden';

    const frame = window.document.createElement("iframe");
    datePicker.append(frame);
    const queryStringConcatenator = src.indexOf ('?') === -1 ? '?' : (src.endsWith('?') ? '' : '&');
    frame.src = src + queryStringConcatenator + 'DateValueField=' + target.value;
    frame.frameBorder = 'no';
    frame.scrolling = 'no';
    frame.style.width = '100%';
    frame.style.height = '100%';
    frame.marginWidth = "0";
    frame.marginHeight = "0";

    button.closest('div, td, th, body')!.append(datePicker);

    if (DatePicker._datePicker_repositionTimer) 
      clearTimeout(DatePicker._datePicker_repositionTimer);
    const repositionHandler = function ()
    {
      if (DatePicker._datePicker_repositionTimer)
        clearTimeout (DatePicker._datePicker_repositionTimer);

      if (DatePicker._datePicker_current && DatePicker._datePicker_current.DatePicker == datePicker && LayoutUtility.IsVisible(datePicker))
      {
        DatePicker.ApplyPosition (datePicker, button);
        DatePicker._datePicker_repositionTimer = setTimeout (repositionHandler, DatePicker._datePicker_repositionInterval);
      }
    };

    DatePicker.ApplyPosition (datePicker, button);
    DatePicker._datePicker_repositionTimer = setTimeout(repositionHandler, DatePicker._datePicker_repositionInterval);

    return datePicker;
  }

  private static ApplyPosition (datePicker: HTMLElement, button: HTMLElement): void
  {
    const left = LayoutUtility.GetOffset(button).left - window.pageXOffset;
    const top = LayoutUtility.GetOffset(button).top - window.pageYOffset;

    //  Adjust position so the date picker is shown below 
    //  and aligned with the right border of the button.
    datePicker.style.left = Math.max (0, left - LayoutUtility.GetWidth(datePicker) + LayoutUtility.GetWidth(button)) + 'px';
    datePicker.style.top = Math.max (0, top + LayoutUtility.GetHeight(button)) + 'px';
    let datePickerLeft = LayoutUtility.GetOffset(datePicker).left - window.pageXOffset;
    let datePickerTop = LayoutUtility.GetOffset(datePicker).top - window.pageYOffset;
    const datePickerWidth = LayoutUtility.GetWidth(datePicker);
    const datePickerHeight = LayoutUtility.GetHeight(datePicker);

    const visibleBodyWidth = document.documentElement.clientWidth;
    const visibleBodyHeight = document.documentElement.clientHeight;

    //  Move the popup to the top of the button if there is not enough space below
    datePickerTop = visibleBodyHeight < datePickerTop + datePickerHeight
      ? LayoutUtility.GetOffset(button).top - window.pageYOffset - datePickerHeight
      : datePickerTop;

    // Make sure that the popup is always in the visible area
    datePickerLeft = Math.max(0, Math.min(datePickerLeft, visibleBodyWidth - datePickerWidth));
    datePickerTop = Math.max(0, Math.min(datePickerTop, visibleBodyHeight - datePickerHeight));

    datePicker.style.left = datePickerLeft + 'px';
    datePicker.style.top = datePickerTop + 'px';
  }
}

interface Window
{
  DatePickerFrame: typeof DatePickerFrame;
}

class DatePickerFrame
{
  public static Calendar_SelectionChanged (value: string): void
  {
    if (window.parent === window)
    {
      // DatePicker popup is opened directly.
    }

    window.parent.DatePickerFrame.UpdateDatePickerValue(value);
  }

  public static UpdateDatePickerValue (value: string): void
  {
    DatePicker.UpdateValue(value);
  }
}
window.DatePickerFrame = DatePickerFrame;
