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
using System.Web;
using Remotion.Development.Web.ResourceHosting;
using Remotion.ServiceLocation;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.Development.WebTesting.TestSite.NetFramework
{
  public class Global : HttpApplication
  {
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (object sender, EventArgs e)
    {
      RegisterResourceVirtualPathProvider();
      SetRenderingFeatures(RenderingFeatures.WithDiagnosticMetadata, new ResourceTheme.NovaGray());
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
      _resourceVirtualPathProvider.HandleBeginRequest();
    }

    private static void RegisterResourceVirtualPathProvider ()
    {
#if DEBUG
      const string configuration = "Debug";
#else
      const string configuration = "Release";
#endif

      var fileExtensionHandlerMapping = new List<FileExtensionHandlerMapping>(FileExtensionHandlerMapping.Default);
      fileExtensionHandlerMapping.Add(new FileExtensionHandlerMapping("html", ResourceVirtualPathProvider.StaticFileHandler));

      _resourceVirtualPathProvider = new ResourceVirtualPathProvider(
          new[]
          {
              new ResourcePathMapping("Remotion.Web/Html", @$"..\..\Web\ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping("Remotion.Web/Image", @"..\..\Web\Core\res\Image"),
              new ResourcePathMapping("Remotion.Web/Themes", @"..\..\Web\Core\res\Themes"),
              new ResourcePathMapping("Remotion.Web/UI", @"..\..\Web\Core\res\UI"),
              new ResourcePathMapping("Remotion.Web.Development.WebTesting.TestSite.Shared", @"..\..\Web\Development.WebTesting.TestSite.Shared")
          },
          fileExtensionHandlerMapping.ToArray());
      _resourceVirtualPathProvider.Register();
    }

    private void SetRenderingFeatures (IRenderingFeatures renderingFeatures, ResourceTheme resourceTheme)
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => renderingFeatures);
      serviceLocator.RegisterSingle(() => resourceTheme);
      ServiceLocator.SetLocatorProvider(() => serviceLocator);
    }
  }
}
