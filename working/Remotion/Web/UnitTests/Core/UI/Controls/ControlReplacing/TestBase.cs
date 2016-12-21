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
using System.Text;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.ControlReplacing
{
  public class TestBase
  {
    private IInternalControlMemberCaller _memberCallerMock;
    private MockRepository _mockRepository;

    [SetUp]
    public virtual void SetUp ()
    {
      SetupHttpContext();
      _mockRepository = new MockRepository();
      _memberCallerMock = _mockRepository.StrictMultiMock<IInternalControlMemberCaller> ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
    }

    protected void SetupHttpContext ()
    {
      var httpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      httpContext.Response.ContentEncoding = Encoding.UTF8;
      HttpContextHelper.SetCurrent (httpContext);
    }

    protected ControlReplacer SetupControlReplacerForIntegrationTest (
        ReplaceableControlMock wrappedControl, IStateModificationStrategy stateModificationStrategy)
    {
      return SetupControlReplacer (new InternalControlMemberCaller(), wrappedControl, stateModificationStrategy);
    }

    protected ControlReplacer SetupControlReplacer (
        IInternalControlMemberCaller memberCaller, ReplaceableControlMock wrappedControl, IStateModificationStrategy stateModificationStrategy)
    {
      ControlReplacer replacer = new ControlReplacer (memberCaller) { ID = "TheReplacer" };
      wrappedControl.OnInitParameters = new Tuple<ControlReplacer, IStateModificationStrategy>  (replacer, stateModificationStrategy);
      return replacer;
    }

    protected object CreateViewState ()
    {
      return CreateViewState (new TestPageHolder (true, RequestMode.Get));
    }

    protected object CreateViewState (TestPageHolder testPageHolder)
    {
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new StateLoadingStrategy());
      testPageHolder.PageInvoker.InitRecursive();
      if (testPageHolder.Page.IsPostBack)
        new ControlInvoker (testPageHolder.NamingContainer).LoadControlState (null);
      Assertion.IsTrue (replacer.Controls.Count == 1);

      return testPageHolder.PageInvoker.SaveViewStateRecursive (ViewStateMode.Enabled);
    }

    protected object CreateControlState ()
    {
      return CreateControlState (new TestPageHolder (true, RequestMode.Get));
    }

    protected object CreateControlState (TestPageHolder testPageHolder)
    {
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new StateLoadingStrategy ());
      testPageHolder.PageInvoker.InitRecursive();
      if (testPageHolder.Page.IsPostBack)
        new ControlInvoker (testPageHolder.NamingContainer).LoadControlState (null);
      Assertion.IsTrue (replacer.Controls.Count == 1);
      
      testPageHolder.Page.SaveAllState();

      return testPageHolder.Page.GetPageStatePersister().ControlState;
    }

    protected IInternalControlMemberCaller MemberCallerMock
    {
      get { return _memberCallerMock; }
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
    }
  }
}
