﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Microsoft.Practices.ServiceLocation;
using Remotion.Development.Web.ResourceHosting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Sample;
using Remotion.ServiceLocation;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public class Global : HttpApplication
  {
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (object sender, EventArgs e)
    {
      var objectPath = Server.MapPath ("~/objects");
      if (!Directory.Exists (objectPath))
        Directory.CreateDirectory (objectPath);

      SetObjectStorageProvider (objectPath);
      RegisterAutoCompleteService();
      RegisterIconService();
      RegisterResourceVirtualPathProvider();
      SetRenderingFeatures (RenderingFeatures.WithDiagnosticMetadata);
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
      _resourceVirtualPathProvider.HandleBeginRequest();
    }

    private static void SetObjectStorageProvider (string objectPath)
    {
      var httpContextProvider = SafeServiceLocator.Current.GetInstance<IHttpContextProvider>();
      var reflectionBusinessObjectStorageProvider = new SessionStateReflectionBusinessObjectStorageProvider (
          httpContextProvider,
          new InMemoryWithFileSystemReadFallbackReflectionBusinessObjectStorageProviderFactory (objectPath));

      var provider = new XmlReflectionBusinessObjectStorageProvider (reflectionBusinessObjectStorageProvider);
      XmlReflectionBusinessObjectStorageProvider.SetCurrent (provider);
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>().AddService (typeof (IGetObjectService), provider);
    }

    private static void RegisterAutoCompleteService ()
    {
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
          .AddService (typeof (ISearchAvailableObjectsService), new BindableXmlObjectSearchService());
    }

    private static void RegisterIconService ()
    {
      BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute>()
          .AddService (typeof (IBusinessObjectWebUIService), new ReflectionBusinessObjectWebUIService());
    }

    private static void RegisterResourceVirtualPathProvider ()
    {
#if DEBUG
      const string configuration = "Debug";
#else
      const string configuration = "Release";
#endif

      _resourceVirtualPathProvider = new ResourceVirtualPathProvider (
          new[]
          {
              new ResourcePathMapping ("Remotion.Web/Html/jquery-1.6.4.js", @"..\..\Web\Core\res\Html\jquery-1.6.4.js"),
              new ResourcePathMapping ("Remotion.Web/Html/jquery.iFrameShim.js", @"..\..\Web\Core\res\Html\jquery.iFrameShim.js"),
              new ResourcePathMapping ("Remotion.Web/Html", @$"..\..\Web\ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping ("Remotion.Web/Image", @"..\..\Web\Core\res\Image"),
              new ResourcePathMapping ("Remotion.Web/Themes", @"..\..\Web\Core\res\Themes"),
              new ResourcePathMapping ("Remotion.Web/UI", @"..\..\Web\Core\res\UI"),
              new ResourcePathMapping ("Remotion.ObjectBinding.Web/Html", @$"..\..\ObjectBinding\Web.ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping ("Remotion.ObjectBinding.Web/Themes", @"..\..\ObjectBinding\Web\res\Themes")
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register();
    }

    private void SetRenderingFeatures (IRenderingFeatures renderingFeatures)
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => renderingFeatures);
      ServiceLocator.SetLocatorProvider (() => serviceLocator);
    }
  }
}