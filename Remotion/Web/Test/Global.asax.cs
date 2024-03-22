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
using System.IO;
using System.Web;
using Remotion.Development.Web.ResourceHosting;
using Remotion.Logging;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Test.Shared.ErrorHandling;
using Remotion.Web.UI;

namespace Remotion.Web.Test
{
  public class Global : HttpApplication
  {
    private static ILog s_log = LogManager.GetLogger(typeof(Global));
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (Object sender, EventArgs e)
    {
      var defaultServiceLocator = DefaultServiceLocator.Create();
      var wxeLifetimeManagementSettings = WxeLifetimeManagementSettings.Create(functionTimeout: 16, refreshInterval: 5);
      defaultServiceLocator.RegisterSingle(() => wxeLifetimeManagementSettings);

      ServiceLocator.SetLocatorProvider(() => defaultServiceLocator);
      LogManager.Initialize();

#if DEBUG
      const string configuration = "Debug";
#else
      const string configuration = "Release";
#endif

      _resourceVirtualPathProvider = new ResourceVirtualPathProvider(
          new[]
          {
              new ResourcePathMapping("Remotion.Web.Test.Shared", @"..\..\Web\Test.Shared"),
              new ResourcePathMapping("Remotion.Web/Html", @$"..\..\Web\ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping("Remotion.Web/Image", @"..\..\Web\Core\res\Image"),
              new ResourcePathMapping("Remotion.Web/Themes", @"..\..\Web\Core\res\Themes"),
              new ResourcePathMapping("Remotion.Web/UI", @"..\..\Web\Core\res\UI")
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register();
    }

    protected void Session_Start (Object sender, EventArgs e)
    {
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
      _resourceVirtualPathProvider.HandleBeginRequest();
    }

    protected void Application_PostRequestHandlerExecute (Object sender, EventArgs e)
    {
      var mimeType = GetMimeType(Path.GetExtension((ReadOnlySpan<char>)Request.PhysicalPath));

      if (mimeType != null)
        Response.ContentType = mimeType;

      static string GetMimeType (ReadOnlySpan<char> extension)
      {
        var svg = (ReadOnlySpan<char>)".svg";
        if (extension.Equals(svg, StringComparison.OrdinalIgnoreCase))
          return "image/svg+xml";

        return null;
      }
    }

    protected void Application_EndRequest (Object sender, EventArgs e)
    {
      if (Context.Handler is WxeHandler && WxeFunctionStateManager.Current.CleanUpExpired() == 0)
      {
        // Perform cleanup. Cannot call Session.Abandon() at this point because the session is no longer available during EndRequest.
      }
    }

    protected void Application_AuthenticateRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_Error (Object sender, EventArgs e)
    {
      var exception = Server.GetLastError();

      s_log.Error("Application Error:", exception);

      if (exception is AsyncUnhandledException)
      {
        Server.ClearError();
        Response.Redirect(VirtualPathUtility.ToAbsolute("~/ErrorHandling/ErrorForm.aspx"));
        return;
      }
      if (!Context.IsCustomErrorEnabled)
        return;

      if (exception is HttpUnhandledException && exception.InnerException is ErrorHandlingException)
        Server.Transfer("~/ErrorHandling/ErrorForm.aspx");
    }

    protected void Session_End (Object sender, EventArgs e)
    {
    }

    protected void Application_End (Object sender, EventArgs e)
    {
    }
  }
}
