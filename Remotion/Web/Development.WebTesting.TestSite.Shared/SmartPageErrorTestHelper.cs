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
using System.Web;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared
{
  public static class SmartPageErrorTestHelper
  {
    public static void Execute (HttpRequest request, HttpResponse response)
    {
      if (request.Path.Contains("SmartPageErrorTest.wxe") && request.HttpMethod == "POST" && request.Params["TriggerAsyncResponseWithFullPageRenderingErrorButton"] != null)
      {
        response.StatusCode = int.TryParse(request.Params["statusCode"], out var statusCode) ? statusCode : 500;
        response.StatusDescription = "Test status description for synchronous response.";
        response.Write("<html><body><h1>Test error response body</h1><p>This is a test error response body</p></body></html>");
        response.End();
      }
    }
  }
}
