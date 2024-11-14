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
using Microsoft.Extensions.Logging.Abstractions;
using Remotion.Data.DomainObjects;
using Remotion.Development.Web.ResourceHosting;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Persistence;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.SecurityManager.Clients.Web.Test
{
  public class Global : SecurityManagerHttpApplication
  {
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (object sender, EventArgs e)
    {
      BootstrapServiceConfiguration.SetLoggerFactory(NullLoggerFactory.Instance);

      var defaultServiceLocator = DefaultServiceLocator.Create();

      var storageSettingsFactory =
          StorageSettingsFactory.CreateForSqlServer<SecurityManagerSqlStorageObjectFactory>(
              "Integrated Security=SSPI;Initial Catalog=RemotionSecurityManagerWebClientTest;Data Source=localhost");
      defaultServiceLocator.RegisterSingle(() => storageSettingsFactory);

      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.IClientTransactionExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);
      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.Tracing.IPersistenceExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);
      //defaultServiceLocator.RegisterMultiple<IOrganizationalStructureEditControlFormGridRowProvider<EditUserControl>> (() => new EditUserControlFormGridRowProvider());
      defaultServiceLocator.Register(typeof(IRenderingFeatures), typeof(WithDiagnosticMetadataRenderingFeatures), LifetimeKind.Singleton);

      //defaultServiceLocator.RegisterSingle<ResourceTheme>(() => new ResourceTheme.NovaGray());

      ServiceLocator.SetLocatorProvider(() => defaultServiceLocator);

      Assertion.IsTrue(
          defaultServiceLocator.GetInstance<IPrincipalProvider>() is SecurityManagerPrincipalProvider,
          "Wrong IPrincipalProvider is configured");

#if DEBUG
      const string configuration = "Debug";
#else
      const string configuration = "Release";
#endif

      _resourceVirtualPathProvider = new ResourceVirtualPathProvider(
          new[]
          {
              new ResourcePathMapping("Remotion.Web/Html", @$"..\..\Remotion\Web\ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping("Remotion.Web/Image", @"..\..\Remotion\Web\Core\res\Image"),
              new ResourcePathMapping("Remotion.Web/Themes", @"..\..\Remotion\Web\Core\res\Themes"),
              new ResourcePathMapping("Remotion.Web/UI", @"..\..\Remotion\Web\Core\res\UI"),
              new ResourcePathMapping("Remotion.ObjectBinding.Web/Html", @$"..\..\Remotion\ObjectBinding\Web.ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping("Remotion.ObjectBinding.Web/Themes", @"..\..\Remotion\ObjectBinding\Web\res\Themes"),
              new ResourcePathMapping("Remotion.Web.Legacy", @"..\..\Remotion\Web\Legacy\Res"),
              new ResourcePathMapping("Remotion.ObjectBinding.Web.Legacy", @"..\..\Remotion\ObjectBinding\Web.Legacy\Res"),
              new ResourcePathMapping("Remotion.SecurityManager.Clients.Web/Html", @"..\Clients.Web\res\Html"),
              new ResourcePathMapping("Remotion.SecurityManager.Clients.Web/Themes", @"..\Clients.Web\res\Themes"),
              new ResourcePathMapping("Remotion.SecurityManager.Clients.Web/UI", @"..\Clients.Web\UI"),
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register();
    }

    protected void Application_End (object sender, EventArgs e)
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
  }
}
