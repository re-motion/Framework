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
using Remotion.Web.Configuration;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  public class TestBase
  {
    private Mock<IExecutionStateContext> _executionStateContextMock;
    private TestFunction _rootFunction;
    private Mock<OtherTestFunction> _subFunction;
    private Mock<HttpContextBase> _httpContextMock;
    private WxeFunctionState _functionState;
    private WxeContext _wxeContext;
    private Mock<HttpResponseBase> _responseMock;
    private Mock<HttpRequestBase> _requestMock;
    private NameValueCollection _postBackCollection;
    private WxeFunctionStateManager _functionStateManager;

    [SetUp]
    public virtual void SetUp ()
    {
      _executionStateContextMock = new Mock<IExecutionStateContext>(MockBehavior.Strict);

      _rootFunction = new TestFunction("Value");
      _subFunction = CreateSubFunction();

      _httpContextMock = new Mock<HttpContextBase>();
      _functionState = new WxeFunctionState(_rootFunction, 20, true);

      _responseMock = new Mock<HttpResponseBase>(MockBehavior.Strict);
      _httpContextMock.Setup(stub => stub.Response).Returns(_responseMock.Object);

      _requestMock = new Mock<HttpRequestBase>(MockBehavior.Strict);
      _httpContextMock.Setup(stub => stub.Request).Returns(_requestMock.Object);

      _postBackCollection = new NameValueCollection();

      _functionStateManager = new WxeFunctionStateManager(new FakeHttpSessionStateBase());
      _wxeContext = new WxeContext(
          _httpContextMock.Object,
          _functionStateManager,
          _functionState,
          new NameValueCollection(),
          new WxeUrlSettings(),
          new WxeLifetimeManagementSettings());
    }

    protected virtual Mock<OtherTestFunction> CreateSubFunction ()
    {
      return new Mock<OtherTestFunction>("OtherValue");
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent(null);
      UrlMappingConfiguration.SetCurrent(null);
    }

    protected WxeFunctionState FunctionState
    {
      get { return _functionState; }
    }

    protected Mock<IExecutionStateContext> ExecutionStateContextMock
    {
      get { return _executionStateContextMock; }
    }

    protected TestFunction RootFunction
    {
      get { return _rootFunction; }
    }

    protected Mock<OtherTestFunction> SubFunction
    {
      get { return _subFunction; }
    }

    protected WxeContext WxeContext
    {
      get { return _wxeContext; }
    }

    protected Mock<HttpContextBase> HttpContextMock
    {
      get { return _httpContextMock; }
    }

    protected Mock<HttpRequestBase> RequestMock
    {
      get { return _requestMock; }
    }

    protected Mock<HttpResponseBase> ResponseMock
    {
      get { return _responseMock; }
    }

    public NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
    }

    protected T CheckExecutionState<T> (T executionState)
        where T: IExecutionState
    {
      Assert.That(executionState, Is.Not.Null);
      Assert.That(executionState.ExecutionStateContext, Is.SameAs(ExecutionStateContextMock.Object));
      Assert.That(executionState.Parameters.SubFunction, Is.SameAs(SubFunction.Object));

      return executionState;
    }

    protected void VerifyAll ()
    {
      _executionStateContextMock.Verify();
      _httpContextMock.Verify();
      _responseMock.Verify();
      _requestMock.Verify();
      _subFunction.Verify();
    }
  }
}
