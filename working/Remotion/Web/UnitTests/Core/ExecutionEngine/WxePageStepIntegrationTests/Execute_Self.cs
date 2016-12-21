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
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxePageStepIntegrationTests
{
  [TestFixture]
  public class Execute_Self : TestBase
  {
    private MockRepository _mockRepository;
    private WxePageStep _pageStep;
    private HttpContextBase _httpContextMock;
    private WxeContext _wxeContext;
    private IWxePageExecutor _pageExecutorMock;
    private TestFunction _subFunction;
    private TestFunction _rootFunction;
    private WxeFunctionState _functionState;
    private WxeFunctionStateManager _functionStateManager;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _rootFunction = new TestFunction();
      _subFunction = _mockRepository.PartialMock<TestFunction>();

      _httpContextMock = _mockRepository.DynamicMock<HttpContextBase>();
      _pageExecutorMock = _mockRepository.StrictMock<IWxePageExecutor>();
      _functionState = new WxeFunctionState (_rootFunction, true);

      _pageStep = _mockRepository.PartialMock<WxePageStep> ("ThePage");
      _pageStep.SetPageExecutor (_pageExecutorMock);

      _functionStateManager = new WxeFunctionStateManager (new FakeHttpSessionStateBase());
      _wxeContext = new WxeContext (_httpContextMock, _functionStateManager, _functionState, new NameValueCollection ());
    }

    [Test]
    public void Execute ()
    {
      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "~/ThePage", false)).WhenCalled (
          invocation =>
          {
            Assert.That (_pageStep.PostBackCollection, Is.Null);
            Assert.That (_pageStep.IsReturningPostBack, Is.False);
          });

      _mockRepository.ReplayAll();

      _pageStep.Execute (_wxeContext);
      _mockRepository.VerifyAll();
      Assert.That (_pageStep.IsPostBack, Is.False);
    }

    [Test]
    public void Execute_WithPostBack ()
    {
      _pageExecutorMock.Stub (stub => stub.ExecutePage (_wxeContext, "~/ThePage", false));
      _mockRepository.ReplayAll ();
      _pageStep.Execute (_wxeContext);
      _mockRepository.BackToRecordAll();

      _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "~/ThePage", true)).WhenCalled (
          invocation =>
          {
            Assert.That (_pageStep.PostBackCollection, Is.Null);
            Assert.That (_pageStep.IsReturningPostBack, Is.False);
          });

      _mockRepository.ReplayAll ();

      _pageStep.Execute (_wxeContext);

      _mockRepository.VerifyAll ();
      Assert.That (_pageStep.IsPostBack, Is.True);
    }
  }
}
