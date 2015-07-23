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
using System.Threading;
using System.Web;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxePageStepIntegrationTests
{
  [TestFixture]
  public class ExecuteFunctionExternalByRedirect : TestBase
  {
    private MockRepository _mockRepository;
    private WxePageStep _pageStep;
    private HttpContextBase _httpContextMock;
    private WxeContext _wxeContext;
    private IWxePageExecutor _pageExecutorMock;
    private IWxePage _pageMock;
    private TestFunction _subFunction;
    private TestFunction _rootFunction;
    private NameValueCollection _postBackCollection;
    private WxeHandler _wxeHandler;
    private WxeFunctionState _functionState;
    private WxeFunctionStateManager _functionStateManager;
    private HttpResponseBase _responseMock;
    private HttpRequestBase _requestMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _rootFunction = new TestFunction();
      PrivateInvoke.InvokeNonPublicMethod (_rootFunction, "SetFunctionToken", "RootFunction");

      _subFunction = _mockRepository.PartialMock<TestFunction>();

      _httpContextMock = _mockRepository.DynamicMock<HttpContextBase>();
      _pageExecutorMock = _mockRepository.StrictMock<IWxePageExecutor>();
      _functionState = new WxeFunctionState (_rootFunction, true);

      _pageStep = _mockRepository.PartialMock<WxePageStep> ("ThePage");
      _pageStep.SetPageExecutor (_pageExecutorMock);
      _rootFunction.Add (_pageStep);

      _pageMock = _mockRepository.DynamicMock<IWxePage>();
      _postBackCollection = new NameValueCollection { { "Key", "Value" } };
      _wxeHandler = new WxeHandler();

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_rootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_subFunction.GetType(), "~/sub.wxe"));

      _functionStateManager = new WxeFunctionStateManager (new FakeHttpSessionStateBase());

      Uri uri = new Uri ("http://localhost/root.wxe");

      _responseMock = _mockRepository.StrictMock<HttpResponseBase>();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("~/sub.wxe")).Return ("/session/sub.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/sub.wxe")).Return ("/session/sub.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("~/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      _responseMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();
      _httpContextMock.Stub (stub => stub.Response).Return (_responseMock).Repeat.Any();

      _requestMock = _mockRepository.StrictMock<HttpRequestBase>();
      _requestMock.Stub (stub => stub.Url).Return (uri).Repeat.Any();
      _httpContextMock.Stub (stub => stub.Request).Return (_requestMock).Repeat.Any();

      _wxeContext = new WxeContext (_httpContextMock, _functionStateManager, _functionState, new NameValueCollection ());
      WxeContextMock.SetCurrent (_wxeContext);
    }

    [Test]
    public void Test_ReturnToCaller ()
    {
      NameValueCollection callerUrlParameters = new NameValueCollection { { "CallerKey", "CallerValue" } };

      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }

        //Redirect to external subfunction
        _responseMock
            .Expect (mock => mock.Redirect (Arg<string>.Matches (arg => arg == "/session/sub.wxe?WxeFunctionToken=" + _subFunction.FunctionToken)))
            .WhenCalled (invocation => Thread.CurrentThread.Abort ());

        //Show external sub function
        _subFunction.Expect (mock => mock.Execute (_wxeContext)).WhenCalled (
            invocation =>
            {
              PrivateInvoke.SetNonPublicField (_functionState, "_postBackID", 100);
              _pageStep.SetPostBackCollection (new NameValueCollection ());
              Thread.CurrentThread.Abort ();
            });

        //Return from external sub function
        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Throw (new WxeExecuteNextStepException());

        _requestMock.Expect (mock => mock.HttpMethod).Return ("GET");
        _pageExecutorMock.Expect (mock => mock.ExecutePage (_wxeContext, "~/ThePage", true)).WhenCalled (
            invocation =>
            {
              Assert.That (((IExecutionStateContext) _pageStep).ExecutionState, Is.SameAs (NullExecutionState.Null));
              Assert.That (_pageStep.PostBackCollection[WxePageInfo.PostBackSequenceNumberID], Is.EqualTo ("100"));
              Assert.That (_pageStep.PostBackCollection.AllKeys, Has.Member("Key"));
              Assert.That (_pageStep.ReturningFunction, Is.SameAs (_subFunction));
              Assert.That (_pageStep.IsReturningPostBack, Is.True);
            });
      }

      _mockRepository.ReplayAll();

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      try
      {
        //Redirect to external subfunction
        WxeReturnOptions returnOptions = new WxeReturnOptions (callerUrlParameters);
        _pageStep.ExecuteFunctionExternalByRedirect (new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), returnOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      try
      {
        //Show external sub function
        _subFunction.Execute();
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      try
      {
        //Return from external sub function
        _subFunction.Execute();
        Assert.Fail();
      }
      catch (WxeExecuteNextStepException)
      {
      }

      Assert.That (_subFunction.ReturnUrl, Is.EqualTo ("/session/root.wxe?CallerKey=CallerValue&WxeFunctionToken=" + _rootFunction.FunctionToken));

      //Show current page
      _pageStep.Execute();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_DoNotReturnToCaller ()
    {
      using (_mockRepository.Ordered())
      {
        using (_mockRepository.Unordered())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (_postBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
          _pageMock.Expect (mock => mock.WxeHandler).Return (_wxeHandler);
        }
        //Redirect to external subfunction
        _responseMock
            .Expect (mock => mock.Redirect (Arg<string>.Matches (arg => arg == "/session/sub.wxe?WxeFunctionToken=" + _subFunction.FunctionToken)))
            .WhenCalled (invocation => Thread.CurrentThread.Abort ());

        //Show external sub function
        _subFunction.Expect (mock => mock.Execute (_wxeContext)).WhenCalled (invocation => Thread.CurrentThread.Abort ());

        //Return from external sub function
        _subFunction.Expect (mock => mock.Execute (_wxeContext)).Throw (new WxeExecuteNextStepException());
      }

      _mockRepository.ReplayAll();

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      try
      {
        //Redirect to external subfunction
        WxeReturnOptions returnOptions = WxeReturnOptions.Null;
        _pageStep.ExecuteFunctionExternalByRedirect (new PreProcessingSubFunctionStateParameters (_pageMock, _subFunction, permaUrlOptions), returnOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      try
      {
        //Show external sub function
        _subFunction.Execute();
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      try
      {
        //Return from external sub function
        _subFunction.Execute();
        Assert.Fail();
      }
      catch (WxeExecuteNextStepException)
      {
      }

      Assert.That (_subFunction.ReturnUrl, Is.EqualTo ("DefaultReturn.html"));

      _mockRepository.VerifyAll();
    }
  }
}
