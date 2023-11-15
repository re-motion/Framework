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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Web;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxePageStepIntegrationTests
{
  [TestFixture]
  public class ExecuteFunction : TestBase
  {
    private Mock<WxePageStep> _pageStep;
    private Mock<HttpContextBase> _httpContextMock;
    private WxeContext _wxeContext;
    private Mock<IWxePageExecutor> _pageExecutorMock;
    private Mock<IWxePage> _pageMock;
    private Mock<TestFunction> _subFunction;
    private TestFunction _rootFunction;
    private NameValueCollection _postBackCollection;
    private WxeHandler _wxeHandler;
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

      _pageMock = new Mock<IWxePage>();
      _postBackCollection = new NameValueCollection { { "Key", "Value" } };
      _wxeHandler = new WxeHandler();

      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(_rootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(_subFunction.Object.GetType(), "~/sub.wxe"));

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
    public void Test_SubFunction ()
    {
      WxeContextMock.SetCurrent(_wxeContext);

      var sequence = new VerifiableSequence();

      _pageMock.Setup(mock => mock.GetPostBackCollection()).Returns(_postBackCollection).Verifiable();
      _pageMock.Setup(mock => mock.SaveAllState()).Verifiable();
      _pageMock.Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();

      _subFunction
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Execute(_wxeContext))
          .Callback(
              (WxeContext context) =>
              {
                PrivateInvoke.SetNonPublicField(_functionState, "_postBackID", 100);
                _pageStep.Object.SetPostBackCollection(new NameValueCollection());
              })
          .Verifiable();
      _pageExecutorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ExecutePage(_wxeContext, "~/ThePage", false))
          .Callback(
              (WxeContext context, string page, bool isPostBack) =>
              {
                Assert.That(((IExecutionStateContext)_pageStep.Object).ExecutionState, Is.SameAs(NullExecutionState.Null));
                Assert.That(_pageStep.Object.PostBackCollection[WxePageInfo.PostBackSequenceNumberID], Is.EqualTo("100"));
                Assert.That(_pageStep.Object.PostBackCollection.AllKeys, Has.Member("Key"));
                Assert.That(_pageStep.Object.ReturningFunction, Is.SameAs(_subFunction.Object));
                Assert.That(_pageStep.Object.IsReturningPostBack, Is.True);
              })
          .Verifiable();

      WxePermaUrlOptions permaUrlOptions = WxePermaUrlOptions.Null;
      WxeRepostOptions repostOptions = WxeRepostOptions.DoRepost(null);
      _pageStep.Object.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), repostOptions);

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageExecutorMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Test_SubFunctionCompleted_ReEntrancy ()
    {
      WxeContextMock.SetCurrent(_wxeContext);

      var sequence = new VerifiableSequence();

      _pageMock.Setup(mock => mock.GetPostBackCollection()).Returns(_postBackCollection).Verifiable();
      _pageMock.Setup(mock => mock.SaveAllState()).Verifiable();
      _pageMock.Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();

      var executeCallbacks = new Queue<Action>();

      executeCallbacks.Enqueue(() => WxeThreadAbortHelper.Abort());
      _subFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();
      executeCallbacks.Enqueue(() => { /* NOP */ });
      _subFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();

      _pageExecutorMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecutePage(_wxeContext, "~/ThePage", true)).Verifiable();

      try
      {
        WxePermaUrlOptions permaUrlOptions = WxePermaUrlOptions.Null;
        WxeRepostOptions repostOptions = WxeRepostOptions.DoRepost(null);
        _pageStep.Object.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), repostOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }
      _pageStep.Object.Execute();

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageExecutorMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Test_SubFunction_ReEntrancy ()
    {
      WxeContextMock.SetCurrent(_wxeContext);

      var sequence = new VerifiableSequence();
      var executeCallbacks = new Queue<Action>();


      _pageMock.Setup(mock => mock.GetPostBackCollection()).Returns(_postBackCollection).Verifiable();
      _pageMock.Setup(mock => mock.SaveAllState()).Verifiable();
      _pageMock.Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();

      executeCallbacks.Enqueue(() => WxeThreadAbortHelper.Abort());
      _subFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();
      executeCallbacks.Enqueue(() => { /* NOP */ });
      _subFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();

      _pageExecutorMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecutePage(_wxeContext, "~/ThePage", true)).Verifiable();

      try
      {
        WxePermaUrlOptions permaUrlOptions = WxePermaUrlOptions.Null;
        WxeRepostOptions repostOptions = WxeRepostOptions.DoRepost(null);
        _pageStep.Object.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), repostOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }
      _pageStep.Object.Execute();

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageExecutorMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Test_SubFunction_RedirectToPermaUrl ()
    {
      WxeContextMock.SetCurrent(_wxeContext);
      Uri uri = new Uri("http://localhost/AppDir/root.wxe");

      var responseMock = new Mock<HttpResponseBase>(MockBehavior.Strict);
      responseMock.Setup(stub => stub.ContentEncoding).Returns(Encoding.UTF8);
      _httpContextMock.Setup(stub => stub.Response).Returns(responseMock.Object);

      var requestMock = new Mock<HttpRequestBase>(MockBehavior.Strict);
      requestMock.Setup(stub => stub.Url).Returns(uri);
      requestMock.Setup(stub => stub.ApplicationPath).Returns("/AppDir");
      _httpContextMock.Setup(stub => stub.Request).Returns(requestMock.Object);

      var sequence = new VerifiableSequence();
      var executeCallbacks = new Queue<Action>();


      _pageMock.Setup(mock => mock.GetPostBackCollection()).Returns(_postBackCollection).Verifiable();
      _pageMock.Setup(mock => mock.SaveAllState()).Verifiable();
      _pageMock.Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();

      //Redirect to subfunction
      responseMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Redirect("/AppDir/sub.wxe?WxeFunctionToken=" + _wxeContext.FunctionToken))
          .Callback((string url) => WxeThreadAbortHelper.Abort())
          .Verifiable();

      //Show sub function
      executeCallbacks.Enqueue(() => WxeThreadAbortHelper.Abort());
      _subFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();

      //Return from sub function part 1
      executeCallbacks.Enqueue(() => { throw new WxeExecuteNextStepException(); });
      _subFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();

      //Return from sub function part 2
      executeCallbacks.Enqueue(() => { /* NOP */ } );
      _subFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();

      //Return from sub function
      responseMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Redirect("/AppDir/root.wxe?WxeFunctionToken=" + _wxeContext.FunctionToken))
          .Callback(
              (string url) =>
              {
                PrivateInvoke.SetNonPublicField(_functionState, "_postBackID", 100);
                _pageStep.Object.SetPostBackCollection(new NameValueCollection());
                WxeThreadAbortHelper.Abort();
              })
          .Verifiable();

      _pageExecutorMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ExecutePage(_wxeContext, "~/ThePage", true))
          .Callback(
              (WxeContext context, string page, bool isPostBack) =>
              {
                Assert.That(((IExecutionStateContext)_pageStep.Object).ExecutionState, Is.SameAs(NullExecutionState.Null));
                Assert.That(_pageStep.Object.PostBackCollection[WxePageInfo.PostBackSequenceNumberID], Is.EqualTo("100"));
                Assert.That(_pageStep.Object.PostBackCollection.AllKeys, Has.Member("Key"));
                Assert.That(_pageStep.Object.ReturningFunction, Is.SameAs(_subFunction.Object));
                Assert.That(_pageStep.Object.IsReturningPostBack, Is.True);
              })
          .Verifiable();

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      try
      {
        //Redirect to subfunction
        WxeRepostOptions repostOptions = WxeRepostOptions.DoRepost(null);
        _pageStep.Object.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), repostOptions);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }

      try
      {
        //Show sub function
        _pageStep.Object.Execute();
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }

      try
      {
        //Return from sub function part 1
        _pageStep.Object.Execute();
        Assert.Fail();
      }
      catch (WxeExecuteNextStepException)
      {
      }

      try
      {
        //Return from sub function part 2
        _pageStep.Object.Execute();
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }

      //Show current page
      _pageStep.Object.Execute();

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageExecutorMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
      responseMock.Verify();
      requestMock.Verify();
      sequence.Verify();
    }
  }
}
