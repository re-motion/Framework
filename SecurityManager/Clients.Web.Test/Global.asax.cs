// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using Microsoft.Practices.ServiceLocation;
using Remotion.Development.Web.ResourceHosting;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Test
{
  public class Global : SecurityManagerHttpApplication
  {
    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    protected void Application_Start (object sender, EventArgs e)
    {
      var defaultServiceLocator = DefaultServiceLocator.Create();

      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.IClientTransactionExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);
      //defaultServiceLocator.Register (typeof (Remotion.Data.DomainObjects.Tracing.IPersistenceExtensionFactory), typeof (Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory), LifetimeKind.Singleton);
      //defaultServiceLocator.RegisterMultiple<IOrganizationalStructureEditControlFormGridRowProvider<EditUserControl>> (() => new EditUserControlFormGridRowProvider());

      ServiceLocator.SetLocatorProvider (() => defaultServiceLocator);

      Assertion.IsTrue (
          defaultServiceLocator.GetInstance<IPrincipalProvider>() is SecurityManagerPrincipalProvider,
          "Wrong IPrincipalProvider is configured");

      _resourceVirtualPathProvider = new ResourceVirtualPathProvider (
          new[]
          {
              new ResourcePathMapping ("Remotion.Web", @"..\..\Remotion\Web\Core\res"),
              new ResourcePathMapping ("Remotion.Web.Legacy", @"..\..\Remotion\Web\Legacy\Res"),
              new ResourcePathMapping ("Remotion.ObjectBinding.Web", @"..\..\Remotion\ObjectBinding\Web\res"),
              new ResourcePathMapping ("Remotion.ObjectBinding.Web.Legacy", @"..\..\Remotion\ObjectBinding\Web.Legacy\Res"),
              new ResourcePathMapping ("Remotion.SecurityManager.Clients.Web/Html", @"..\Clients.Web\res\Html"),
              new ResourcePathMapping ("Remotion.SecurityManager.Clients.Web/Themes", @"..\Clients.Web\res\Themes"),
              new ResourcePathMapping ("Remotion.SecurityManager.Clients.Web/UI", @"..\Clients.Web\UI"),
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
  }
}