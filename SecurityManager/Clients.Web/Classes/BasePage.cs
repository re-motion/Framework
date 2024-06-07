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
using Remotion.Globalization;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public abstract class BasePage : WxePage, IObjectWithResources
  {
    // types
    private const string c_globalStyleFileUrl = "Style.css";
    private const string c_globalStyleFileKey = "SecurityManagerGlobalStyle";

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    public new BaseTransactedFunction CurrentFunction
    {
      get { return (BaseTransactedFunction)base.CurrentFunction; }
    }

    protected virtual IFocusableControl? InitialFocusControl
    {
      get { return null; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      RegisterStyleSheets();
    }

    protected override void OnPreRender (EventArgs e)
    {
      ResourceDispatcher.Dispatch(this, ResourceManagerUtility.GetResourceManager(this));

      if (!IsPostBack && InitialFocusControl != null)
        SetFocus(InitialFocusControl);

      base.OnPreRender(e);
    }

    private void RegisterStyleSheets ()
    {
      var globalStyleFileUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(BasePage), ResourceType.Html, c_globalStyleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink(c_globalStyleFileKey, globalStyleFileUrl, HtmlHeadAppender.Priority.Library);

      HtmlHeadAppender.Current.RegisterPageStylesheetLink();
    }

    IResourceManager IObjectWithResources.GetResourceManager ()
    {
      return this.GetResourceManager();
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType();

      return GlobalizationService.GetResourceManager(type);
    }

    protected IResourceUrlFactory ResourceUrlFactory
    {
      get { return ServiceLocator.GetInstance<IResourceUrlFactory>(); }
    }

    protected IGlobalizationService GlobalizationService
    {
      get { return SafeServiceLocator.Current.GetInstance<IGlobalizationService>(); }
    }
  }
}
