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
using System.Runtime.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Provides methods for detecting an ASP.NET error page.
  /// Only works with synchronous postbacks or GET requests.
  /// </summary>
  public static class AspNetErrorPageDetection
  {
    [DataContract]
    private class ErrorObject
    {
      [DataMember]
      public string? Message { get; set; }

      [DataMember]
      public string? Details { get; set; }
    }

    private const string c_errorPageDetectionJs = @"
let result = null;
// The following CSS selector matches specific parts of the ASP.NET error page, which make it improbable that a non error page will match it
if (document.querySelector('body[bgcolor=""white""] > span:first-child > h1:first-child > hr[color=""silver""]:last-child')) {
  result = {};
  result.Message = document.querySelector('body > span > h2').innerText;
  result.Details = document.querySelector('body > font').innerText;
}
return JSON.stringify(result);
";

    public static void ThrowOnErrorPage (PageObject pageObject)
    {
      ArgumentUtility.CheckNotNull("pageObject", pageObject);

      ThrowOnErrorPage((IWebDriver)pageObject.Driver.Native);
    }

    public static void ThrowOnErrorPage (IWebDriver webDriver)
    {
      ArgumentUtility.CheckNotNull("webDriver", webDriver);

      var resultJson = webDriver.ExecuteJavaScript<string>(c_errorPageDetectionJs);

      // Shortcut the JSON deserialization for the common case to improve performance
      if (resultJson is null or "null")
        return;

      var resultObject = DataContractJsonSerializationUtility.Deserialize<ErrorObject>(resultJson);
      if (resultObject != null)
        throw new AspNetErrorPageException(resultObject.Message ?? "The error message could not be determined.", resultObject.Details ?? "The error details could not be determined.");
    }
  }
}
