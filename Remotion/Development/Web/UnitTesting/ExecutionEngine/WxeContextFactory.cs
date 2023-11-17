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
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine.TestFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Development.Web.UnitTesting.ExecutionEngine
{
  /// <summary> Provides a <see cref="WxeContext"/> for simualating ASP.NET request life cycles. </summary>
  public class WxeContextFactory
  {
    public static HttpContext CreateHttpContext (NameValueCollection queryString)
    {
      HttpContext context = HttpContextHelper.CreateHttpContext("GET", "Other.wxe", null);
      context.Response.ContentEncoding = Encoding.UTF8;
      HttpContextHelper.SetQueryString(context, queryString);
      HttpContextHelper.SetCurrent(context);
      return context;
    }

    public static HttpContext CreateHttpContext ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add(WxeHandler.Parameters.ReturnUrl, "/Root.wxe");

      return CreateHttpContext(queryString);
    }

    public static WxeContext Create (HttpContext context, WxeUrlSettings wxeUrlSettings, IWxeLifetimeManagementSettings wxeLifetimeManagementSettings)
    {
      return new WxeContext(
          new HttpContextWrapper(context),
          new WxeFunctionStateManager(new HttpSessionStateWrapper(context.Session)),
          new WxeFunctionState(new TestFunction(), 20, false),
          null!,
          wxeUrlSettings,
          wxeLifetimeManagementSettings);
    }

    public static WxeContext Create (HttpContext context, NameValueCollection queryString, WxeUrlSettings wxeUrlSettings, IWxeLifetimeManagementSettings wxeLifetimeManagementSettings)
    {
      return new WxeContext(
          new HttpContextWrapper(context),
          new WxeFunctionStateManager(new HttpSessionStateWrapper(context.Session)),
          new WxeFunctionState(new TestFunction(), 20, false),
          queryString,
          wxeUrlSettings,
          wxeLifetimeManagementSettings);
    }

    public static WxeContext Create (WxeFunction rootFunction)
    {
      var httpContext = CreateHttpContext();
      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager(new HttpSessionStateWrapper(httpContext.Session));
      WxeFunctionState functionState = new WxeFunctionState(rootFunction, 20, false);
      NameValueCollection queryString = new NameValueCollection();

      return new WxeContext(new HttpContextWrapper(httpContext), functionStateManager, functionState, queryString, new WxeUrlSettings(), new WxeLifetimeManagementSettings());
    }
  }
}
