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
  function containsBrowser(brands, expectedBrand)
  {
    return brands.some(c => c.brand === expectedBrand);
  }

  let browser;
  if (navigator.userAgentData)
  {
    const brands = navigator.userAgentData.brands;
    if (containsBrowser(brands, 'Microsoft Edge'))
      browser = 'blink';
    else if (containsBrowser(brands, 'Google Chrome'))
      browser = 'blink';
    else if (containsBrowser(brands, 'Opera'))
      browser = 'blink';
  }

  // Fallback to the old api navigator.userAgent to prevent incorrect detection in future
  // if a browser adds support for userAgentData
  if(!browser)
  {
    if (navigator.userAgent.indexOf('Edg/') !== -1)
      browser = 'blink';
    else if (navigator.userAgent.indexOf('Chrome') !== -1)
      browser = 'blink';
    else if (navigator.userAgent.indexOf('OPR') !== -1)
      browser = 'blink';
    else if (navigator.userAgent.indexOf('WebKit') !== -1)
      browser = 'webkit';
    else if (navigator.userAgent.indexOf('Gecko') !== -1)
      browser = 'mozilla';
    else
      browser = 'browserUnknown';
  }

  StyleUtility.AddPlatformSwitch();

  if (!document.body.classList.contains (browser))
    document.body.classList.add (browser);
};

StyleUtility.AddPlatformSwitch = function ()
{
  let platform;
  if (navigator.userAgentData)
  {
    if (navigator.userAgentData.platform === 'Windows')
      platform = 'win';
    else if (navigator.userAgentData.platform === 'macOS')
      platform = 'mac';
    else if (navigator.userAgentData.platform === 'Linux')
      platform = 'x11';
    else
      platform = 'platformUnknown';
  }
  else
  {
    if (navigator.userAgent.indexOf ('Win') !== -1)
      platform = 'win';
    else if (navigator.userAgent.indexOf ('Mac') !== -1)
      platform = 'mac';
    else if (navigator.userAgent.indexOf ('X11') !== -1)
      platform = 'x11';
    else
      platform = 'platformUnknown';
  }

  if (!document.body.classList.contains (platform))
    document.body.classList.add (platform);
};
