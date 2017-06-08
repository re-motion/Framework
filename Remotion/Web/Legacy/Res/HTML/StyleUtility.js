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
function StyleUtility()
{
}

StyleUtility.CreateBorderSpans = function(selector)
{
  var element = $(selector);
  while (element.length > 0 && element.attr('id') == undefined)
    element = element.parent();

  var elementBody = $(selector)[0];
  if (element.length == 0 || elementBody.length == 0)
    return;

  var elementID = element.attr('id');

  StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'top');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'left');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'right');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, elementID, 'bottomRight');

  StyleUtility.CalculateBorderSpans(element[0], topRight, bottomLeft, bottomRight);

  PageUtility.Instance.RegisterResizeHandler('#' + elementID, StyleUtility.OnResize);
}

StyleUtility.CalculateBorderSpans = function(element, topRight, bottomLeft, bottomRight)
{
  topRight.style.left = topRight.offsetParent.clientLeft + topRight.offsetParent.clientWidth - topRight.offsetWidth  + 'px';
  bottomLeft.style.top = bottomLeft.offsetParent.clientTop + bottomLeft.offsetParent.clientHeight - bottomLeft.offsetHeight  + 'px';
  bottomRight.style.top = bottomRight.offsetParent.clientTop + bottomRight.offsetParent.clientHeight - bottomRight.offsetHeight  + 'px';
  bottomRight.style.left = bottomRight.offsetParent.clientLeft + bottomRight.offsetParent.clientWidth - bottomRight.offsetWidth  + 'px';

  var scrollDiv = element.firstChild.firstChild;
  if (scrollDiv != null && !TypeUtility.IsUndefined (scrollDiv.tagName) && scrollDiv.tagName.toLowerCase() == 'div')
  {
    if (scrollDiv.scrollHeight > scrollDiv.clientHeight)
      topRight.style.display = 'none';
    else
      topRight.style.display = '';

    if (scrollDiv.scrollWidth > scrollDiv.clientWidth)
      bottomLeft.style.display = 'none';
    else
      bottomLeft.style.display = '';

    if (   (scrollDiv.scrollHeight > scrollDiv.clientHeight && scrollDiv.scrollWidth == scrollDiv.clientWidth) 
        || (scrollDiv.scrollHeight == scrollDiv.clientHeight && scrollDiv.scrollWidth > scrollDiv.clientWidth))
    {
      bottomRight.style.display = 'none';
    }
    else
    {
      bottomRight.style.display = '';
    }
  }
}

StyleUtility.CreateAndAppendBorderSpan = function(elementBody, elementID, className)
{
  var borderSpan = document.createElement('SPAN');
  borderSpan.id = elementID + '_' + className;
  borderSpan.className = className;

  elementBody.appendChild(borderSpan);

  return borderSpan
}

StyleUtility.OnResize = function(element)
{
  var elementID = element.attr('id');
  var topRight = document.getElementById(elementID + '_topRight');
  var bottomLeft = document.getElementById(elementID + '_bottomLeft');
  var bottomRight = document.getElementById(elementID + '_bottomRight');

  StyleUtility.CalculateBorderSpans(element[0], topRight, bottomLeft, bottomRight);
}

StyleUtility.AddBrowserSwitch = function()
{
}