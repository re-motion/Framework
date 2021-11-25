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
using System.Threading;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  internal static class PageObjectExtensions
  {
    //TODO RM-7284: Remove this workaround once the underlying problem is resolved.
    public static WebButtonControlObject GetValidateButton (this PageObject pageObject)
    {
      Func<WebButtonControlObject> getValidateButtonFunction = () => pageObject.WebButtons().GetByLocalID("ValidateButton");

      if (pageObject.Scope.Browser.IsFirefox())
        return Retry(getValidateButtonFunction, 3, TimeSpan.FromMilliseconds(500));

      return getValidateButtonFunction.Invoke();
    }

    private static TReturn Retry<TReturn> (Func<TReturn> func, int retries, TimeSpan interval)
    {
      if (retries < 0)
        throw new ArgumentOutOfRangeException("retries", "Retries must be greater than or equal to zero.");

      for (var i = 0; i < retries; i++)
      {
        try
        {
          Thread.Sleep(interval);
          return func();
        }
        catch (Exception)
        {
          // Ignore all errors except the last
        }
      }

      return func();
    }
  }
}