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

StyleUtility.CreateBorderSpans = function (selector)
{
  var element = $ (selector);

  StyleUtility.CreateAndAppendBorderSpan (element, 'top');
  StyleUtility.CreateAndAppendBorderSpan (element, 'left');
  StyleUtility.CreateAndAppendBorderSpan (element, 'bottom');
  StyleUtility.CreateAndAppendBorderSpan (element, 'right');
  StyleUtility.CreateAndAppendBorderSpan (element, 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan (element, 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan (element, 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan (element, 'bottomRight');

  if (StyleUtility.ShowBorderSpans (element, topRight, bottomLeft, bottomRight))
    PageUtility.Instance.RegisterResizeHandler (selector, StyleUtility.OnResize);
};

StyleUtility.ShowBorderSpans = function (element, topRight, bottomLeft, bottomRight)
{
  var scrollDiv = element;
  while (scrollDiv.css ('overflow') != 'auto' && scrollDiv.css ('overflow') != 'scroll' && (scrollDiv.length > 0))
    scrollDiv = scrollDiv.children().eq (0);
  var scrolledDiv = scrollDiv.children().eq (0);

  var hasScrollbarsOnOverflow = scrollDiv.css ('overflow') == 'auto' || scrollDiv.css ('overflow') == 'scroll';

  if (scrolledDiv.length == 1 && scrolledDiv.prop ('nodeName').toLowerCase() == 'div' && hasScrollbarsOnOverflow)
  {
    var offset = 1;
    var hasVerticalScrollBar = scrolledDiv[0].scrollHeight > (scrolledDiv.height() + offset); //height includes the scrollbar, if it exists
    var hasHorizontalScrollbar = scrolledDiv[0].scrollWidth > (scrolledDiv.outerWidth() + offset); //width includes the scrollbar, if it exists
    var hasExactlyOneScrollbar = (hasVerticalScrollBar && !hasHorizontalScrollbar) || (!hasVerticalScrollBar && hasHorizontalScrollbar);

    if (hasVerticalScrollBar)
      $ (topRight).css ('display', 'none');
    else
      $ (topRight).css ('display', '');

    if (hasHorizontalScrollbar)
      $ (bottomLeft).css ('display', 'none');
    else
      $ (bottomLeft).css ('display', '');

    if (hasExactlyOneScrollbar)
      $ (bottomRight).css ('display', 'none');
    else
      $ (bottomRight).css ('display', '');

    return true;
  }
  else
    return false;
};

StyleUtility.CreateAndAppendBorderSpan = function (elementBody, className)
{
  var borderSpan = document.createElement ('SPAN');
  borderSpan.className = className;

  elementBody[0].appendChild (borderSpan);

  return borderSpan;
};

StyleUtility.OnResize = function (element)
{

  var topRight = element.find ('.topRight');
  var bottomLeft = element.find ('.bottomLeft');
  var bottomRight = element.find ('.bottomRight');

  StyleUtility.ShowBorderSpans (element, topRight[0], bottomLeft[0], bottomRight[0]);
};

StyleUtility.AddBrowserSwitch = function ()
{
  var browser;
  if ($.browser.msie)
  {
    var majorVersion = BrowserUtility.GetIEVersion();
    if (majorVersion < 9)
      browser = 'msie' + majorVersion;
    else if (majorVersion < 11)
      browser = 'msie msie' + majorVersion;
    else
      browser = 'msie';
  }
  else if ($.browser.mozilla)
    browser = 'mozilla';
  else if ($.browser.webkit)
    browser = 'webkit';
  else if ($.browser.opera)
    browser = 'opera';
  else
    browser = 'unknown';

  StyleUtility.AddPlatformSwitch();

  if (!$ ('body').hasClass (browser))
    $ ('body').addClass (browser);
};

StyleUtility.AddPlatformSwitch = function ()
{
  var platform;
  if (navigator.appVersion.indexOf ("Win") != -1)
    platform = "win";
  else if (navigator.appVersion.indexOf ("Mac") != -1)
    platform = "mac";
  else if (navigator.appVersion.indexOf ("X11") != -1)
    platform = "x11";
  else
    platform = "unknown";

  if (!$ ('body').hasClass (platform))
    $ ('body').addClass (platform);
};
