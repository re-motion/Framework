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
using System.Runtime.Serialization;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies
{
  /// <summary>
  /// Parses the yellow ASP.NET error page for synchronous requests as well as asynchronous postbacks for pages based on <b>Remotion.Web.UI.SmartPage</b>.
  /// </summary>
  public sealed class AspNetRequestErrorDetectionParser
  {
    public sealed class Result
    {
      public static Result CreateErrorResult ([NotNull] string message, [NotNull] string stacktrace)
      {
        ArgumentUtility.CheckNotNull("message", message);
        ArgumentUtility.CheckNotNull("stacktrace", stacktrace);

        return new Result(true, message, stacktrace);
      }

      public static Result CreateEmptyResult ()
      {
        return new Result(false, "", "");
      }

      private readonly bool _hasError;
      private readonly string _message;
      private readonly string _stacktrace;

      private Result (bool hasError, string message, string stacktrace)
      {
        _hasError = hasError;
        _message = message;
        _stacktrace = stacktrace;
      }

      public bool HasError
      {
        get { return _hasError; }
      }

      [NotNull]
      public string Message
      {
        get { return _message; }
      }

      [NotNull]
      public string Stacktrace
      {
        get { return _stacktrace; }
      }
    }

    [DataContract]
    private class ErrorObject
    {
      [DataMember]
      public string? Message { get; set; }

      [DataMember]
      public string? StackTrace { get; set; }
    }

    private const string c_errorPageDetectionJs = @"
function fixNewlines(text) {
  // Ensure strings have a CRLF endings
  return text.replaceAll(/(?<!\r)\n/g, '\r\n');
}

let result = null;

const smartPageErrorMessage = document.getElementById('SmartPageServerErrorMessage');
const errorPageTarget = smartPageErrorMessage
  ? smartPageErrorMessage.querySelector(':scope .SmartPageErrorBody > div')
  : document.body;

const message = errorPageTarget?.querySelector(':scope > span > h2 > i')?.innerText;
const stacktrace = errorPageTarget?.querySelector(':scope > font > table:nth-of-type(2) > tbody > tr > td > code > pre')?.innerText;

if (message && stacktrace) {
  result = {
    Message: fixNewlines(message),
    StackTrace: fixNewlines(stacktrace)
  };
}

return JSON.stringify(result);
";

    public Result Parse ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      // We do the error page detection using JavaScript as it provides better performance than selenium/coypu
      // The JS solution (~2ms) is more than 25x faster than the selenium equivalent, which adds up if the detection is used often
      var resultJson = ((IWebDriver)scope.GetDriver().Native).ExecuteJavaScript<string>(c_errorPageDetectionJs);

      // Shortcut the JSON deserialization for the common case (no error) to improve performance
      if (resultJson is null or "null")
        return Result.CreateEmptyResult();

      var resultObject = DataContractJsonSerializationUtility.Deserialize<ErrorObject>(resultJson);
      if (resultObject != null)
      {
        return Result.CreateErrorResult(
            resultObject.Message ?? "The error message could not be determined.",
            resultObject.StackTrace ?? "The stack trace could not be determined.");
      }
      else
      {
        return Result.CreateEmptyResult();
      }
    }
  }
}
