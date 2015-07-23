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
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Development.Web.UnitTesting.AspNetFramework
{
  /// <summary> 
  ///   Provides helper methods for initalizing an <see cref="HttpContext"/> object when simulating ASP.NET request
  ///   cycles. 
  /// </summary>
  public class HttpContextHelper
  {
    public static readonly string s_appVirtualDir = "/";
    public static readonly string s_appPhysicalDir = @"c:\";
    public static readonly string s_serverPath = "http://127.0.0.1";

    public static void SetCurrent (HttpContext context)
    {
      HttpContext.Current = context;
    }

    public static HttpContext CreateHttpContext (string httpMethod, string page, string query)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("httpMethod", httpMethod);
      ArgumentUtility.CheckNotNullOrEmpty ("page", page);


      SimpleWorkerRequest workerRequest =
          new SimpleWorkerRequest (s_appVirtualDir, s_appPhysicalDir, page, query, new System.IO.StringWriter());

      object httpRuntime = PrivateInvoke.GetNonPublicStaticField (typeof (HttpRuntime), "_theRuntime");
      PrivateInvoke.SetNonPublicField (httpRuntime, "_appDomainAppPath", s_appPhysicalDir);
      string assemblyName = typeof (HttpApplication).Assembly.FullName;
      Type virtualPathType = Type.GetType ("System.Web.VirtualPath, " + assemblyName, true);
      object virtualPath = PrivateInvoke.InvokePublicStaticMethod (virtualPathType, "Create", s_appVirtualDir);
      PrivateInvoke.SetNonPublicField (httpRuntime, "_appDomainAppVPath", virtualPath);
      PrivateInvoke.SetNonPublicField (httpRuntime, "_appDomainAppId", "Remotion.Web.UnitTests");
      Type buildManagerType = typeof (System.Web.Compilation.BuildManager);
      PrivateInvoke.SetNonPublicStaticProperty (buildManagerType, "SkipTopLevelCompilationExceptions", true);
      HttpContext context = new HttpContext (workerRequest);
      PrivateInvoke.SetNonPublicField (context.Request, "_httpMethod", httpMethod);

      HttpSessionState sessionState = CreateSession();
      SetSession (context, sessionState);

      context.Request.Browser = new HttpBrowserCapabilities
                                {
                                    Capabilities =
                                        new Hashtable
                                        {
                                            { "w3cdomversion", "4.0" },
                                            { "ecmascriptversion", "5.0" },
                                            { "supportsCallback", "true" }
                                        }
                                };
      Assertion.IsTrue (context.Request.Browser.W3CDomVersion == new Version (4, 0));
      Assertion.IsTrue (context.Request.Browser.EcmaScriptVersion == new Version (5, 0));
      Assertion.IsTrue (context.Request.Browser.SupportsCallback);

      return context;
    }

    public static void SetQueryString (HttpContext context, NameValueCollection queryString)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("queryString", queryString);

      PrivateInvoke.InvokeNonPublicMethod (context.Request.QueryString, "MakeReadWrite", new object[0]);
      context.Request.QueryString.Clear();
      foreach (string key in queryString)
        context.Request.QueryString.Set (key, queryString[key]);
      PrivateInvoke.InvokeNonPublicMethod (context.Request.QueryString, "MakeReadOnly", new object[0]);

      PrivateInvoke.SetNonPublicField (context.Request, "_params", null);
    }

    public static void SetForm (HttpContext context, NameValueCollection form)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("form", form);

      PrivateInvoke.InvokeNonPublicMethod (context.Request.Form, "MakeReadWrite", new object[0]);
      context.Request.Form.Clear();
      foreach (string key in form)
        context.Request.Form.Set (key, form[key]);
      PrivateInvoke.InvokeNonPublicMethod (context.Request.Form, "MakeReadOnly", new object[0]);

      PrivateInvoke.SetNonPublicField (context.Request, "_params", null);
    }
 
    protected static HttpSessionState CreateSession ()
    {
      HttpSessionState sessionState;
      string id = Guid.NewGuid().ToString();
      HttpStaticObjectsCollection staticObjects = new HttpStaticObjectsCollection();
      int timeout = 20;
      bool newSession = true;
      SessionStateMode mode = SessionStateMode.InProc;
      bool isReadOnly = false;
      SessionStateItemCollection sessionItems = new SessionStateItemCollection ();
      HttpSessionStateContainer httpSessionStateContainer = new HttpSessionStateContainer (
          id, sessionItems, staticObjects, timeout, newSession, HttpCookieMode.UseUri, mode, isReadOnly);

      sessionState = (HttpSessionState) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (HttpSessionState), httpSessionStateContainer);
      return sessionState;
    }

    protected static void SetSession (HttpContext context, HttpSessionState sessionState)
    {
      context.Items["AspSession"] = sessionState;
    }

    private HttpContextHelper()
    {
    }
  }
}
