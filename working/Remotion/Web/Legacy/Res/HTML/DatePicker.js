// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
//  DatePicker.js contains client side scripts used by DatePickerPage 
//  and the caller of the DatePickerFrom.aspx IFrame contents page.

//  The currently displayed date picker
//  Belongs to the parent page.
var _datePicker_currentDatePicker = null;

//  Helper variable for event handling.
//  Belongs to the parent page.
//  The click event of the document fires after the methods bound to the button click have been 
//  executed. _datePicker_isEventAfterDatePickerButtonClick is used to identify those click events fired
//  because a date picker button had been clicked in contrast to events fired
//  beause of a click somewhere on the page.
var _datePicker_isEventAfterDatePickerButtonClick = false;

//  Shows the date picker frame below the button.
//  Belongs to the parent page.
//  button: The button that opened the date picker frame.
//  container: The page element containing the properties to be passed to the picker.
//  target: The input element receiving the value returned by the date picker.
function DatePicker_ShowDatePicker (button, container, target, src, width, height)
{
  var datePickerID = container.id + '_DatePicker';
  //  Tried to open the already open date picker?
  //  Close it and return.
  if (DatePicker_CloseVisibleDatePickerFrame (datePickerID))
    return;
    
  if (target.disabled || target.readOnly)
    return;

   
  var left = 0;
  var top = 0;
  
  //  Calculate the offset of the frame in respect to the left top corner of the page.
  var frameParent = null;
  for (var currentNode = button; currentNode != null; currentNode = currentNode.offsetParent)
  {
    left += currentNode.offsetLeft + currentNode.clientLeft;
    top += currentNode.offsetTop + currentNode.clientTop;
    if (currentNode != window.document.body) // body does not have to be considered
    {   
      var overflow = currentNode.currentStyle.overflow.toLowerCase();
      if (overflow == 'auto' || overflow == 'scroll')
      {
        left -= currentNode.scrollLeft;
        top -= currentNode.scrollTop;
      }
    }
  }
  
  var datePicker = window.document.createElement ('div');
  datePicker.style.width = width;
  datePicker.style.height = height;
  datePicker.style.position = 'absolute';
  datePicker.style.zIndex = 100; // Required so the DatePicker covers DropDownMenus
  datePicker.id = datePickerID;
  
  var frame = window.document.createElement ("iframe");
  datePicker.appendChild (frame);
  frame.src = src + '?TargetIDField=' + target.id + '&DatePickerIDField=' + datePicker.id + '&DateValueField=' + target.value;
  frame.frameBorder = 'no';
  frame.scrolling = 'no';
  frame.style.width = '100%';
  frame.style.height = '100%';
  frame.marginWidth = 0;
  frame.marginHeight = 0;
  
  var datePickerLeft;
  var datePickerTop;
  var datePickerWidth;
  var datePickerHeight;
  window.document.body.insertBefore (datePicker, window.document.body.children[0]);
  //  Adjust position so the date picker is shown below 
  //  and aligned with the right border of the button.
  datePicker.style.left = left - frame.offsetWidth + button.offsetWidth;
  datePicker.style.top = top + button.offsetHeight;
  datePickerLeft = datePicker.offsetLeft;
  datePickerTop = datePicker.offsetTop;
  datePickerWidth = datePicker.offsetWidth;
  datePickerHeight = datePicker.offsetHeight;
  datePicker.style.display = 'none';
  
  
  //  Re-adjust the button, in case available screen space is insufficient
  var totalBodyHeight = window.document.body.scrollHeight;
  var visibleBodyTop = window.document.body.scrollTop;
  var visibleBodyHeight = window.document.body.offsetHeight;
  
  var datePickerTopAdjusted = datePickerTop;
  if (visibleBodyTop + visibleBodyHeight < datePickerTop + datePickerHeight)
  {
    var newTop = top - datePickerHeight - button.offsetTop - button.clientTop;
    if (newTop >= 0)
      datePickerTopAdjusted = newTop;
  }
  
  var totalBodyWidth = window.document.body.scrollWidth;
  var visibleBodyLeft = window.document.body.scrollLeft;
  var visibleBodyWidth = window.document.body.offsetWidth;
  
  var datePickerLeftAdjusted = datePickerLeft;
  if (datePickerLeft < visibleBodyLeft && datePickerWidth <= visibleBodyWidth)
    datePickerLeftAdjusted = visibleBodyLeft;
  
  datePicker.style.display = '';
  datePicker.style.left = datePickerLeftAdjusted;
  datePicker.style.top = datePickerTopAdjusted;
  datePicker.style.display = 'none';

  if (   visibleBodyTop > 0
      && datePickerTopAdjusted < visibleBodyTop)
  {
    window.document.body.scrollTop = datePickerTopAdjusted;
  }
  
  _datePicker_currentDatePicker = datePicker;
  _datePicker_isEventAfterDatePickerButtonClick = true;
  target.document.onclick = DatePicker_OnDocumentClick;
  datePicker.style.display = '';
}

//  Closes the currently visible date picker frame.
//  Belongs to the parent page.
//  newDatePicker: The newly selected date picker frame, used to test whether the current frame 
//      is identical to the new frame.
//  returns true if the newDatePicker is equal to the visible date picker.
function DatePicker_CloseVisibleDatePickerFrame (newDatePickerID)
{
  var newDatePicker = document.getElementById (newDatePickerID);
  if (   newDatePicker != null
      && newDatePicker == _datePicker_currentDatePicker
      && newDatePicker.style.display != 'none')
  {
    return true;
  }        
  if (_datePicker_currentDatePicker != null)
  {
    var currentDatePicker = window.document.getElementById (_datePicker_currentDatePicker.id);
    var frameContent = currentDatePicker.children[0].contentWindow;
    frameContent.DatePickerFrame_CloseDatePicker();
    _datePicker_currentDatePicker = null;
  }
  return false;
}

//  Called by the date picker when a new date is selected in the calendar. 
//  Belongs to the date picker frame.
function DatePickerFrame_Calendar_SelectionChanged(value)
{
  var target = window.parent.document.getElementById (document.getElementById ('TargetIDField').value);
  var isValueChanged = target.value != value;
  DatePickerFrame_CloseDatePicker();
  target.value = value;
  if (isValueChanged)
  {
    if (typeof (target.fireEvent) != 'undefined')
      target.fireEvent ('onchange');
    else if (typeof (target.dispatchEvent) != 'undefined')
      target.dispatchEvent ('change');
    else if (target.onchange != null)
      target.onchange();
  }
}

//  Closes the date picker frame
//  Belongs to the date picker frame.
function DatePickerFrame_CloseDatePicker() 
{
  var target = window.parent.document.getElementById (document.getElementById ('TargetIDField').value);
  target.document.onclick = null;
  try
  {
    target.focus();
  }
  catch (e)
  {
  }  
  window.parent._datePicker_currentDatePicker = null;
  var datePicker = window.parent.document.getElementById (document.getElementById ('DatePickerIDField').value);
  datePicker.parentNode.removeChild (datePicker);
}

//  Called by th event handler for the onclick event of the parent pages's document.
//  Belongs to the parent page.
//  Closes the currently open date picker frame, 
//  unless _datePicker_isEventAfterDatePickerButtonClick is set to false.
function DatePicker_OnDocumentClick()
{
  if (_datePicker_isEventAfterDatePickerButtonClick)
  {
    _datePicker_isEventAfterDatePickerButtonClick = false;
  }
  else if (_datePicker_currentDatePicker != null)
  {
    var currentDatePicker = window.document.getElementById (_datePicker_currentDatePicker.id);
    var frameContent = currentDatePicker.children[0].contentWindow;
    frameContent.DatePickerFrame_CloseDatePicker();
    _datePicker_currentDatePicker = null;
  }  
}
