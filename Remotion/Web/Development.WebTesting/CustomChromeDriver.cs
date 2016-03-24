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

using System;
using System.Collections.Generic;
using System.Linq;
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Custom <see cref="SeleniumWebDriver"/> implementation for <see cref="Browser.Chrome"/>. The default implementation of Coypu does not
  /// set all <see cref="ChromeOptions"/> as required.
  /// </summary>
  public class CustomChromeDriver : SeleniumWebDriver
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CustomChromeDriver));

    public CustomChromeDriver (Browser browser)
        : this (browser, null)
    {
    }

    public CustomChromeDriver (Browser browser, IReadOnlyDictionary<string, object> browserPreferences)
        : base (CreateChromeDriver (browserPreferences), browser)
    {
    }

    private static IWebDriver CreateChromeDriver (IReadOnlyDictionary<string, object> browserPreferences)
    {
      var driverService = ChromeDriverService.CreateDefaultService();

      var chromeOptions = new ChromeOptions();
      foreach (var pref in browserPreferences)
        chromeOptions.AddUserProfilePreference (pref.Key, pref.Value);

      var driver = new ChromeDriver (driverService, chromeOptions);

      s_log.InfoFormat (
          "Created CustomChromeDriver with user profile preferences: {0}",
          string.Join (", ", browserPreferences.Select (kvp => kvp.Key + "='" + kvp.Value + "'")));

      return driver;
    }
  }
}