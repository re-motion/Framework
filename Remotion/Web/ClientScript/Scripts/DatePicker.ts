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
  DatePicker: JQuery;
  Target: JQuery;
  TargetDocument: Nullable<Document>;
  ClickHandler: Nullable<EventHandler<JQueryMouseEventObject>>;
};

interface Window
{
  DatePicker: typeof DatePicker;
}

class DatePicker
{
  private static _datePicker_current: Nullable<DatePickerContext> = null;
  private static _datePicker_repositionInterval = 200;
  private static _datePicker_repositionTimer: Nullable<number> = null;

  public static ShowDatePicker (button: string, container: HTMLElement, target: HTMLInputElement, src: string, width: number, height: number): void
  {
    var datePickerID = container.id + '_DatePicker';
    //  Tried to open the already open date picker?
    //  Close it and return.
    if (DatePicker.CloseVisibleDatePickerFrame (datePickerID))
      return;
      
    if (target.disabled || target.readOnly)
      return;

    var datePicker = DatePicker.Create(datePickerID, button, target, src, width, height);

    var targetDocument = null;
    if (TypeUtility.IsDefined(target.ownerDocument))
      targetDocument = target.ownerDocument;
    else if (TypeUtility.IsDefined(target.document))
      targetDocument = target.document;
    var clickHandler: Nullable<EventHandler<JQueryMouseEventObject>> = null;
    if (targetDocument != null)
    {
      var isEventAfterDatePickerButtonClick = true;
      clickHandler = function (event: JQueryMouseEventObject)
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
      $(targetDocument).bind('click', clickHandler);
    }

    DatePicker._datePicker_current = { DatePicker: datePicker, Target: $(target), TargetDocument: targetDocument, ClickHandler: clickHandler };
    datePicker.css('visibility', 'visible');
  }

  private static CloseVisibleDatePickerFrame (newDatePickerID: string): boolean
  {
    var isSameDatePicker = false;
    var newDatePicker = $('#' + newDatePickerID);
    if (newDatePicker.is(':visible'))
      isSameDatePicker = true;

      DatePicker.CloseDatePicker(isSameDatePicker);

    return isSameDatePicker;
  }

  private static CloseDatePicker(setFocusOnTarget: boolean): void
  {
    if (DatePicker._datePicker_current == null)
      return;

    var current = DatePicker._datePicker_current;
    DatePicker._datePicker_current = null;

    if (setFocusOnTarget)
    {
      current.Target.focus();
    }

    current.DatePicker.remove();
    if (current.TargetDocument != null)
    {
      $(current.TargetDocument).unbind('click', current.ClickHandler!); // TODO RM-7647: DatePick.DatePicker_CloseDatePicker removes all click handlers from target element if no click handler was specified for the DatePicker
    }
  }

  public static UpdateValue(value: string): void
  {
    var current = DatePicker._datePicker_current;
    if (current == null)
      return;

    DatePicker.CloseDatePicker(true);

    var isValueChanged = current.Target.val() != value;
    if (isValueChanged)
    {
      current.Target.val(value);
      current.Target.change();
    } 
  }

  private static Create(datePickerID: string, button: string, target: HTMLInputElement, src: string, width: number, height: number): JQuery
  {
    var datePicker = $('<div/>');
    datePicker.attr('id', datePickerID);
    datePicker.addClass ('DatePicker');
    datePicker.width(width);
    datePicker.height(height);
    datePicker.css('visibility', 'hidden');

    var frame = window.document.createElement("iframe");
    datePicker.append($(frame));
    var queryStringConcatenator = src.indexOf ('?') as unknown === '-1' ? '?' : '&'; // TODO RM-7646: IndexOf comparison in DatePicker.DatePicker_Create will always yield false due to type mismatch
    frame.src = src + queryStringConcatenator + 'DateValueField=' + target.value;
    frame.frameBorder = 'no';
    frame.scrolling = 'no';
    frame.style.width = '100%';
    frame.style.height = '100%';
    frame.marginWidth = "0";
    frame.marginHeight = "0";

    $(button).closest('div, td, th, body').append(datePicker);

    if (DatePicker._datePicker_repositionTimer) 
      clearTimeout(DatePicker._datePicker_repositionTimer);
    var repositionHandler = function ()
    {
      if (DatePicker._datePicker_repositionTimer)
        clearTimeout (DatePicker._datePicker_repositionTimer);

      if (DatePicker._datePicker_current && DatePicker._datePicker_current.DatePicker == datePicker && datePicker.is (':visible'))
      {
        DatePicker.ApplyPosition (datePicker, $(button));
        DatePicker._datePicker_repositionTimer = setTimeout (repositionHandler, DatePicker._datePicker_repositionInterval);
      }
    };

    DatePicker.ApplyPosition (datePicker, $(button));
    DatePicker._datePicker_repositionTimer = setTimeout(repositionHandler, DatePicker._datePicker_repositionInterval);

    return datePicker;
  }

  private static ApplyPosition (datePicker: JQuery, button: JQuery): void
  {
    var datePickerLeft;
    var datePickerTop;
    var datePickerWidth;
    var datePickerHeight;

    var body = $ ('body');
    var left = $(button).offset().left;
    var top = $(button).offset().top;

    //  Adjust position so the date picker is shown below 
    //  and aligned with the right border of the button.
    datePicker.css('left', Math.max (0, left - datePicker.width() + $(button).width()));
    datePicker.css('top', Math.max (0, top + $(button).height()));
    datePickerLeft = datePicker.offset().left;
    datePickerTop = datePicker.offset().top;
    datePickerWidth = datePicker.width();
    datePickerHeight = datePicker.height();

    //  Re-adjust the button, in case available screen space is insufficient
    var visibleBodyTop = body.scrollTop();
    var visibleBodyHeight = $(window).height();

    var datePickerTopAdjusted = datePickerTop;
    if (visibleBodyTop + visibleBodyHeight < datePickerTop + datePickerHeight)
    {
      var newTop = Math.max (0, $(button).offset().top - datePickerHeight);
      if (newTop >= 0)
        datePickerTopAdjusted = newTop;
    }

    var visibleBodyLeft = body.scrollLeft();
    var visibleBodyWidth = $(window).width();

    var datePickerLeftAdjusted = datePickerLeft;
    if (datePickerLeft < visibleBodyLeft && datePickerWidth <= visibleBodyWidth)
      datePickerLeftAdjusted = visibleBodyLeft;

    datePicker.css('left', datePickerLeftAdjusted);
    datePicker.css('top', datePickerTopAdjusted);
  }
}

class DatePickerFrame
{
  public static Calendar_SelectionChanged (value: string): void
  {
    window.parent.DatePicker.UpdateValue(value);
  }
}

function DatePicker_ShowDatePicker (button: string, container: HTMLElement, target: HTMLInputElement, src: string, width: number, height: number): void
{
  DatePicker.ShowDatePicker (button, container, target, src, width, height);
}

function DatePickerFrame_Calendar_SelectionChanged (value: string): void
{
  DatePickerFrame.Calendar_SelectionChanged(value);
}
