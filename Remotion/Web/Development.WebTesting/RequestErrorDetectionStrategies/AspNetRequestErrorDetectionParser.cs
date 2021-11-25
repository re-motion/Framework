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
using System.Web;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

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

    public Result Parse ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      var pageRequestManagerException = scope.FindCss(".SmartPageErrorBody:first-child", Options.NoWait);

      var startSelector = GetRootSelectorForErrorInformation(pageRequestManagerException);

      var message = scope.FindCss(startSelector + " > span > h2 > i", Options.NoWait);
      var stacktrace = scope.FindCss(startSelector + " > font > table:nth-of-type(2) > tbody > tr > td > code > pre", Options.NoWait);

      if (message.ExistsWorkaround() && stacktrace.ExistsWorkaround())
        return Result.CreateErrorResult(HttpUtility.HtmlDecode(message.InnerHTML.Replace("<br>", "\r\n")), HttpUtility.HtmlDecode(stacktrace.InnerHTML));

      return Result.CreateEmptyResult();
    }

    private string GetRootSelectorForErrorInformation (ElementScope pageRequestManagerException)
    {
      if (pageRequestManagerException.ExistsWorkaround()
          && pageRequestManagerException.Text.StartsWith("Sys.WebForms.PageRequestManagerServerErrorException:"))
      {
        return "div.SmartPageErrorBody > div";
      }
      else
      {
        return "body";
      }
    }
  }
}
