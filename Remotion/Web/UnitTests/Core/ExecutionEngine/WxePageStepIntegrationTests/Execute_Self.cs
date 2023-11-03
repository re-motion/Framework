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
using Moq;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxePageStepIntegrationTests
{
  [TestFixture]
  public class Execute_Self : TestBase
  {
    private Mock<WxePageStep> _pageStep;
    private Mock<HttpContextBase> _httpContextMock;
    private WxeContext _wxeContext;
    private Mock<IWxePageExecutor> _pageExecutorMock;
    private Mock<TestFunction> _subFunction;
    private TestFunction _rootFunction;
    private WxeFunctionState _functionState;
    private WxeFunctionStateManager _functionStateManager;

    [SetUp]
    public void SetUp ()
    {
      _rootFunction = new TestFunction();
      _subFunction = new Mock<TestFunction>() { CallBase = true };

      _httpContextMock = new Mock<HttpContextBase>();
      _pageExecutorMock = new Mock<IWxePageExecutor>(MockBehavior.Strict);
      _functionState = new WxeFunctionState(_rootFunction, 20, true);

      _pageStep = new Mock<WxePageStep>("ThePage") { CallBase = true };
      _pageStep.Object.SetPageExecutor(_pageExecutorMock.Object);

      _functionStateManager = new WxeFunctionStateManager(new FakeHttpSessionStateBase());
      _wxeContext = new WxeContext(
          _httpContextMock.Object,
          _functionStateManager,
          _functionState,
          new NameValueCollection(),
          new WxeUrlSettings(),
          new WxeLifetimeManagementSettings());
    }

    [Test]
    public void Execute ()
    {
      _pageExecutorMock
          .Setup(mock => mock.ExecutePage(_wxeContext, "~/ThePage", false))
          .Callback(
              (WxeContext context, string page, bool isPostBack) =>
              {
                Assert.That(_pageStep.Object.PostBackCollection, Is.Null);
                Assert.That(_pageStep.Object.IsReturningPostBack, Is.False);
              })
          .Verifiable();

      _pageStep.Object.Execute(_wxeContext);
      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageExecutorMock.Verify();
      _pageStep.Verify();

      Assert.That(_pageStep.Object.IsPostBack, Is.False);
    }

    [Test]
    public void Execute_WithPostBack ()
    {
      _pageExecutorMock.Setup(stub => stub.ExecutePage(_wxeContext, "~/ThePage", false));
      _pageStep.Object.Execute(_wxeContext);
      _subFunction.Reset();
      _httpContextMock.Reset();
      _pageExecutorMock.Reset();
      _pageStep.Reset();

      _pageExecutorMock
          .Setup(mock => mock.ExecutePage(_wxeContext, "~/ThePage", true))
          .Callback(
              (WxeContext context, string page, bool isPostBack) =>
              {
                Assert.That(_pageStep.Object.PostBackCollection, Is.Null);
                Assert.That(_pageStep.Object.IsReturningPostBack, Is.False);
              })
          .Verifiable();

      _pageStep.Object.Execute(_wxeContext);

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageExecutorMock.Verify();
      _pageStep.Verify();

      Assert.That(_pageStep.Object.IsPostBack, Is.True);
    }
  }
}
