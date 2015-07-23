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

var _datePicker_current = null;
var _datePicker_repositionInterval = 200;
var _datePicker_repositionTimer = null;

function DatePicker_ShowDatePicker (button, container, target, src, width, height)
{
  var datePickerID = container.id + '_DatePicker';
  //  Tried to open the already open date picker?
  //  Close it and return.
  if (DatePicker_CloseVisibleDatePickerFrame (datePickerID))
    return;
    
  if (target.disabled || target.readOnly)
    return;

  var datePicker = DatePicker_Create(datePickerID, button, target, src, width, height);

  var targetDocument = null;
  if (TypeUtility.IsDefined(target.ownerDocument))
    targetDocument = target.ownerDocument;
  else if (TypeUtility.IsDefined(target.document))
    targetDocument = target.document;
  var clickHandler = null;
  if (targetDocument != null)
  {
    var isEventAfterDatePickerButtonClick = true;
    clickHandler = function (event)
    {
      if (isEventAfterDatePickerButtonClick)
      {
        isEventAfterDatePickerButtonClick = false;
      }
      else
      {
        DatePicker_CloseDatePicker(false);
      }
    };
    $(targetDocument).bind('click', clickHandler);
  }

  _datePicker_current = { DatePicker: datePicker, Target: $(target), TargetDocument: targetDocument, ClickHandler: clickHandler };
  datePicker.css('visibility', 'visible');
}

function DatePicker_CloseVisibleDatePickerFrame (newDatePickerID)
{
  var isSameDatePicker = false;
  var newDatePicker = $('#' + newDatePickerID);
  if (newDatePicker.is(':visible'))
    isSameDatePicker = true;

  DatePicker_CloseDatePicker(isSameDatePicker);

  return isSameDatePicker;
}

function DatePicker_CloseDatePicker(setFocusOnTarget)
{
  if (_datePicker_current == null)
    return;

  var current = _datePicker_current;
  _datePicker_current = null;

  if (setFocusOnTarget)
  {
    current.Target.focus();
  }

  current.DatePicker.remove();
  if (current.TargetDocument != null)
  {
    $(current.TargetDocument).unbind('click', current.ClickHandler);
  }
}

function DatePicker_UpdateValue(value)
{
  var current = _datePicker_current;
  if (current == null)
    return;

  DatePicker_CloseDatePicker(true);

  var isValueChanged = current.Target.val() != value;
  if (isValueChanged)
  {
    current.Target.val(value);
    current.Target.change();
  } 
}

function DatePicker_Create(datePickerID, button, target, src, width, height)
{
  var datePicker = $('<div/>');
  datePicker.attr('id', datePickerID);
  datePicker.addClass ('DatePicker');
  datePicker.width(width);
  datePicker.height(height);
  datePicker.css('visibility', 'hidden');

  var frame = window.document.createElement("iframe");
  datePicker.append($(frame));
  frame.src = src + '?DateValueField=' + target.value;
  frame.frameBorder = 'no';
  frame.scrolling = 'no';
  frame.style.width = '100%';
  frame.style.height = '100%';
  frame.marginWidth = 0;
  frame.marginHeight = 0;

  var body = $('body');
  body.append(datePicker);

  if (_datePicker_repositionTimer) 
    clearTimeout(_datePicker_repositionTimer);
  var repositionHandler = function ()
  {
    if (_datePicker_repositionTimer)
      clearTimeout (_datePicker_repositionTimer);

    if (_datePicker_current && _datePicker_current.DatePicker == datePicker && datePicker.is (':visible'))
    {
      DatePicker_ApplyPosition (datePicker, $(button));
      _datePicker_repositionTimer = setTimeout (repositionHandler, _datePicker_repositionInterval);
    }
  };

  DatePicker_ApplyPosition (datePicker, $(button));
  _datePicker_repositionTimer = setTimeout(repositionHandler, _datePicker_repositionInterval);

  return datePicker;
}

function DatePicker_ApplyPosition (datePicker, button)
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

function DatePickerFrame_Calendar_SelectionChanged(value)
{
  window.parent.DatePicker_UpdateValue(value);
}
