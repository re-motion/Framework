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
      get { return (BaseTransactedFunction) base.CurrentFunction; }
    }

    protected virtual IFocusableControl InitialFocusControl
    {
      get { return null; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      RegisterStyleSheets ();
    }

    protected override void OnPreRender (EventArgs e)
    {
      ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

      if (!IsPostBack && InitialFocusControl != null)
        SetFocus (InitialFocusControl);

      base.OnPreRender (e);
    }

    private void RegisterStyleSheets ()
    {
      var globalStyleFileUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BasePage), ResourceType.Html, c_globalStyleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (c_globalStyleFileKey, globalStyleFileUrl, HtmlHeadAppender.Priority.Library);

      HtmlHeadAppender.Current.RegisterPageStylesheetLink ();
    }

    IResourceManager IObjectWithResources.GetResourceManager ()
    {
      return this.GetResourceManager ();
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      Type type = this.GetType ();

      return GlobalizationService.GetResourceManager (type);
    }

    protected IResourceUrlFactory ResourceUrlFactory
    {
      get { return ServiceLocator.GetInstance<IResourceUrlFactory> (); }
    }

    protected IGlobalizationService GlobalizationService
    {
      get { return SafeServiceLocator.Current.GetInstance<IGlobalizationService>(); }
    }
  }
}
