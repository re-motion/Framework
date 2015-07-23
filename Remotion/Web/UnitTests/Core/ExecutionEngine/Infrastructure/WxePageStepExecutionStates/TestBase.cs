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
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  public class TestBase
  {
    private MockRepository _mockRepository;
    private IExecutionStateContext _executionStateContextMock;
    private TestFunction _rootFunction;
    private OtherTestFunction _subFunction;
    private HttpContextBase _httpContextMock;
    private WxeFunctionState _functionState;
    private WxeContext _wxeContext;
    private HttpResponseBase _responseMock;
    private HttpRequestBase _requestMock;
    private NameValueCollection _postBackCollection;
    private WxeFunctionStateManager _functionStateManager;

    [SetUp]
    public virtual void SetUp ()
    {
      _mockRepository = new MockRepository();
      _executionStateContextMock = MockRepository.StrictMock<IExecutionStateContext>();

      _rootFunction = new TestFunction ("Value");
      _subFunction = CreateSubFunction();

      _httpContextMock = MockRepository.DynamicMock<HttpContextBase>();
      _functionState = new WxeFunctionState (_rootFunction, true);

      _responseMock = MockRepository.StrictMock<HttpResponseBase>();
      _httpContextMock.Stub (stub => stub.Response).Return (_responseMock).Repeat.Any();

      _requestMock = MockRepository.StrictMock<HttpRequestBase>();
      _httpContextMock.Stub (stub => stub.Request).Return (_requestMock).Repeat.Any();

      _postBackCollection = new NameValueCollection();

      _functionStateManager = new WxeFunctionStateManager (new FakeHttpSessionStateBase());
      _wxeContext = new WxeContext (_httpContextMock, _functionStateManager, _functionState, new NameValueCollection ());
    }

    protected virtual OtherTestFunction CreateSubFunction ()
    {
      return new OtherTestFunction ("OtherValue");
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent (null);
      UrlMappingConfiguration.SetCurrent (null);
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    protected WxeFunctionState FunctionState
    {
      get { return _functionState; }
    }

    protected IExecutionStateContext ExecutionStateContextMock
    {
      get { return _executionStateContextMock; }
    }

    protected TestFunction RootFunction
    {
      get { return _rootFunction; }
    }

    protected OtherTestFunction SubFunction
    {
      get { return _subFunction; }
    }

    protected WxeContext WxeContext
    {
      get { return _wxeContext; }
    }

    protected HttpContextBase HttpContextMock
    {
      get { return _httpContextMock; }
    }

    protected HttpRequestBase RequestMock
    {
      get { return _requestMock; }
    }

    protected HttpResponseBase ResponseMock
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
      Assert.That (executionState, Is.Not.Null);
      Assert.That (executionState.ExecutionStateContext, Is.SameAs (ExecutionStateContextMock));
      Assert.That (executionState.Parameters.SubFunction, Is.SameAs (SubFunction));

      return executionState;
    }
  }
}
