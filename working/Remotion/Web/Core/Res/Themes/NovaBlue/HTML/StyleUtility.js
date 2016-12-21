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
    browser = 'browserUnknown';

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
    platform = "platformUnknown";

  if (!$ ('body').hasClass (platform))
    $ ('body').addClass (platform);
};
