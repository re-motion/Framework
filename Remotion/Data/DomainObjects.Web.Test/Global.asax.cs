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
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.Web.ResourceHosting;
using Remotion.Security;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Web.Test
{
  /// <summary>
  /// Summary description for Global.
  /// </summary>
  public class Global : HttpApplication
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    private static ResourceVirtualPathProvider _resourceVirtualPathProvider;

    public Global ()
    {
      InitializeComponent();
    }

    protected void Application_Start (Object sender, EventArgs e)
    {
#if DEBUG
      const string configuration = "Debug";
#else
      const string configuration = "Release";
#endif

      _resourceVirtualPathProvider = new ResourceVirtualPathProvider(
          new[]
          {
              new ResourcePathMapping("Remotion.Web/Html", @$"..\..\Web\ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping("Remotion.Web/Image", @"..\..\Web\Core\res\Image"),
              new ResourcePathMapping("Remotion.Web/Themes", @"..\..\Web\Core\res\Themes"),
              new ResourcePathMapping("Remotion.Web/UI", @"..\..\Web\Core\res\UI"),
              new ResourcePathMapping("Remotion.ObjectBinding.Web/Html", @$"..\..\ObjectBinding\Web.ClientScript\bin\{configuration}\dist"),
              new ResourcePathMapping("Remotion.ObjectBinding.Web/Themes", @"..\..\ObjectBinding\Web\res\Themes"),
              new ResourcePathMapping("Remotion.Web.Legacy", @"..\..\Web\Legacy\Res"),
              new ResourcePathMapping("Remotion.ObjectBinding.Web.Legacy", @"..\..\ObjectBinding\Web.Legacy\Res")
          },
          FileExtensionHandlerMapping.Default);
      _resourceVirtualPathProvider.Register();

      var serviceLocator = DefaultServiceLocator.Create();

      var storageSettingsFactory = StorageSettingsFactory.CreateForSqlServer("Integrated Security=SSPI;Initial Catalog=RpaTest;Data Source=localhost");
      serviceLocator.RegisterSingle(() => storageSettingsFactory);

      serviceLocator.RegisterSingle<ISecurityProvider>(() => new StubSecurityProvider());
      ServiceLocator.SetLocatorProvider(()=> serviceLocator);
    }

    protected void Session_Start (Object sender, EventArgs e)
    {
    }

    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
      _resourceVirtualPathProvider.HandleBeginRequest();
    }

    protected void Application_EndRequest (Object sender, EventArgs e)
    {
    }

    protected void Application_AuthenticateRequest (Object sender, EventArgs e)
    {
    }

    protected virtual void Application_PreRequestHandlerExecute (Object sender, EventArgs e)
    {
    }

    protected void Application_PostRequestHandlerExecute (Object sender, EventArgs e)
    {
    }

    protected void Application_Error (Object sender, EventArgs e)
    {
    }

    protected void Session_End (Object sender, EventArgs e)
    {
    }

    protected void Application_End (Object sender, EventArgs e)
    {
    }

    #region Web Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.components = new System.ComponentModel.Container();
    }

    #endregion
  }
}
