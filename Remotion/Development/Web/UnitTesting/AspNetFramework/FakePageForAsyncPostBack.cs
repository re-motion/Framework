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
using System.Web;
using System.Web.UI;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Utilities;

namespace Remotion.Web.UnitTests.Core.UI.SmartPageImplementation
{
  /// <summary>
  /// An implementation of <see cref="Page"/> that simulates an asynchronous request.
  /// </summary>
  public class FakePageForAsyncPostBack : Page
  {
    private readonly FakeScriptManager _scriptManager;

    private class FakeScriptManager : ScriptManager
    {
      protected override void OnInit (EventArgs e)
      {
        Assertion.IsFalse(DesignMode);
        Assertion.IsTrue(Page?.IsPostBack ?? false);

        var pageRequestManagerType = typeof(ScriptManager).Assembly.GetType("System.Web.UI.PageRequestManager", true, false)!;
        var isAsyncPostBackRequest = (bool)PrivateInvoke.InvokeNonPublicStaticMethod(
            pageRequestManagerType,
            "IsAsyncPostBackRequest",
            new HttpRequestWrapper(Context.Request))!;
        Assertion.IsTrue(isAsyncPostBackRequest);

        base.OnInit(e);

        Assertion.IsTrue(IsInAsyncPostBack);
      }
    }

    public FakePageForAsyncPostBack ()
    {
      PrivateInvoke.SetNonPublicStaticProperty(typeof(ScriptManager), "DefaultAjaxFrameworkAssembly", typeof(ScriptManager).Assembly);
      _scriptManager = new FakeScriptManager();
    }

    public void ProcessRequest ()
    {
      ProcessRequest(HttpContextHelper.CreateHttpContext("POST", "page.aspx", ""));
    }

    public override void ProcessRequest (HttpContext context)
    {
      HttpContextHelper.SetForm(
          context,
          new NameValueCollection(context.Request.Form)
          {
              { "__ASYNCPOST", "true" }
          });

      base.ProcessRequest(context);
    }

    protected override NameValueCollection? DeterminePostBackMode ()
    {
      return new NameValueCollection();
    }

    protected override void OnPreInit (EventArgs e)
    {
      base.OnPreInit(e);
      EnsureChildControls();
    }

    protected override void CreateChildControls ()
    {
      base.CreateChildControls();
      Controls.Add(_scriptManager);
    }

    public ScriptManager ScriptManager
    {
      get { return _scriptManager; }
    }

    public new HttpContext Context
    {
      get { return base.Context; }
    }
  }
}
