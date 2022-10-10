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

StyleUtility.IsNovaGray = true;

StyleUtility.CreateBorderSpans = function (selector)
{
};

StyleUtility.AddBrowserSwitch = function ()
{
  var browser;
  if (navigator.appVersion.indexOf ('Edge') !== -1)
  {
    browser = 'edge';
  }
  else if (navigator.appVersion.indexOf ('Chrome') !== -1)
  {
    browser = 'blink';
  }
  else if (navigator.appVersion.indexOf ('OPR') !== -1)
  {
    browser = 'blink';
  }
  else if (navigator.appVersion.indexOf('WebKit') !== -1)
  {
    browser = 'webkit';
  }
  else if (navigator.appName === 'Netscape')
  {
    browser = 'mozilla';
  }
  else
  {
    browser = 'browserUnknown';
  }
  StyleUtility.AddPlatformSwitch();

  if (!document.body.classList.contains (browser))
    document.body.classList.add (browser);
};

StyleUtility.AddPlatformSwitch = function ()
{
  var platform;
  if (navigator.appVersion.indexOf ('Win') !== -1)
    platform = 'win';
  else if (navigator.appVersion.indexOf ('Mac') !== -1)
    platform = 'mac';
  else if (navigator.appVersion.indexOf ('X11') !== -1)
    platform = 'x11';
  else
    platform = 'platformUnknown';

  if (!document.body.classList.contains (platform))
    document.body.classList.add (platform);
};
