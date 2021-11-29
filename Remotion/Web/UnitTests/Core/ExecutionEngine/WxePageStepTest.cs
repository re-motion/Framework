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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using PreProcessingSubFunctionState_WithRedirect =
    Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect.PreProcessingSubFunctionState;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxePageStepTest
  {
    private Mock<WxePageStep> _pageStep;
    private Mock<HttpContextBase> _httpContextMock;
    private WxeContext _wxeContext;
    private Mock<IWxePage> _pageMock;
    private Mock<TestFunction> _subFunction;
    private TestFunction _rootFunction;
    private WxeHandler _wxeHandler;
    private WxeFunctionState _functionState;
    private WxeFunctionStateManager _functionStateManager;

    [SetUp]
    public void SetUp ()
    {
      _rootFunction = new TestFunction();
      _subFunction = new Mock<TestFunction>() { CallBase = true };

      _httpContextMock = new Mock<HttpContextBase>();
      _functionState = new WxeFunctionState(_rootFunction, true);

      _pageStep = new Mock<WxePageStep>("ThePage") { CallBase = true };

      _pageMock = new Mock<IWxePage>();
      _wxeHandler = new WxeHandler();

      _functionStateManager = new WxeFunctionStateManager(new FakeHttpSessionStateBase());
      _wxeContext = new WxeContext(_httpContextMock.Object, _functionStateManager, _functionState, new NameValueCollection());
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WxeContext.SetCurrent(null);
      UrlMappingConfiguration.SetCurrent(null);
    }

    [Test]
    public void Initialize_WithPath ()
    {
      WxePageStep step = new WxePageStep("page.aspx");
      Assert.That(step.Page, Is.EqualTo("~/page.aspx"));
    }

    [Test]
    public void Initialize_WithVariableReference ()
    {
      WxeFunction function = new TestFunction();
      WxePageStep step = new WxePageStep(new WxeVariableReference("ThePage"));
      function.Add(step);
      function.Variables["ThePage"] = "page.aspx";

      Assert.That(step.Page, Is.EqualTo("~/page.aspx"));
    }

    [Test]
    public void ExecuteFunction ()
    {
      WxeContextMock.SetCurrent(_wxeContext);

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeRepostOptions repostOptions = WxeRepostOptions.SuppressRepost(new Mock<Control>().Object, true);

      var sequence = new MockSequence();
      _pageMock.InSequence(sequence).Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();
      _pageStep.InSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback(
          (WxeContext context) =>
          {
            var executionState = (PreProcessingSubFunctionState) ((IExecutionStateContext) _pageStep.Object).ExecutionState;
            Assert.That(executionState.Parameters.SubFunction, Is.SameAs(_subFunction.Object));
            Assert.That(executionState.Parameters.PermaUrlOptions, Is.SameAs(permaUrlOptions));
            Assert.That(executionState.RepostOptions, Is.SameAs(repostOptions));
            Assert.That(PrivateInvoke.GetNonPublicField(_pageStep.Object, "_wxeHandler"), Is.SameAs(_wxeHandler));
          }).Verifiable();

      _pageStep.Object.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), repostOptions);

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
    }

    [Test]
    public void ExecuteFunction_IsAlreadyExecutingSubFunction ()
    {
      WxeContextMock.SetCurrent(_wxeContext);

      _pageMock.Setup(stub => stub.GetPostBackCollection()).Returns(new NameValueCollection());
      _pageMock.Setup(stub => stub.SaveAllState());
      _pageMock.Setup(stub => stub.WxeHandler).Returns(_wxeHandler);

      _subFunction.Setup(mock => mock.Execute(_wxeContext)).Throws(new ApplicationException()).Verifiable();

      try
      {
        _pageStep.Object.ExecuteFunction(
            new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, WxePermaUrlOptions.Null), WxeRepostOptions.DoRepost(null));
      }
      catch (ApplicationException)
      {
      }
      Assert.That(
          () => _pageStep.Object.ExecuteFunction(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, WxePermaUrlOptions.Null), WxeRepostOptions.DoRepost(null)),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot execute function while another function executes."));
    }

    [Test]
    public void ExecuteFunctionExternalByRedirect ()
    {
      WxeContextMock.SetCurrent(_wxeContext);

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeReturnOptions returnOptions = new WxeReturnOptions();

      var sequence = new MockSequence();
      _pageMock.InSequence(sequence).Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();
      _pageStep.InSequence(sequence).Setup(mock => mock.Execute(_wxeContext)).Callback(
          (WxeContext context) =>
          {
            var executionState = (PreProcessingSubFunctionState_WithRedirect) ((IExecutionStateContext) _pageStep.Object).ExecutionState;
            Assert.That(executionState.Parameters.SubFunction, Is.SameAs(_subFunction.Object));
            Assert.That(executionState.Parameters.PermaUrlOptions, Is.SameAs(permaUrlOptions));
            Assert.That(executionState.ReturnOptions, Is.SameAs(returnOptions));
            Assert.That(PrivateInvoke.GetNonPublicField(_pageStep.Object, "_wxeHandler"), Is.SameAs(_wxeHandler));
          }).Verifiable();

      _pageStep.Object.ExecuteFunctionExternalByRedirect(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), returnOptions);

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
    }

    [Test]
    public void ExecuteFunctionExternalByRedirect_IsAlreadyExecutingSubFunction ()
    {
      WxeContextMock.SetCurrent(_wxeContext);

      _pageMock.Setup(stub => stub.GetPostBackCollection()).Returns(new NameValueCollection());
      _pageMock.Setup(stub => stub.SaveAllState());
      _pageMock.Setup(stub => stub.WxeHandler).Returns(_wxeHandler);

      _pageStep.Setup(mock => mock.Execute(_wxeContext)).Throws(new ApplicationException()).Verifiable();

      try
      {
        _pageStep.Object.ExecuteFunctionExternalByRedirect(
            new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, WxePermaUrlOptions.Null), WxeReturnOptions.Null);
      }
      catch (ApplicationException)
      {
      }
      Assert.That(
          () => _pageStep.Object.ExecuteFunctionExternalByRedirect(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, WxePermaUrlOptions.Null), WxeReturnOptions.Null),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot execute function while another function executes."));
    }

    [Test]
    public void IExecutionStateContext_GetAndSetExecutionState ()
    {
      var executionStateContext = (IExecutionStateContext) _pageStep.Object;
      var newExecutionState = new Mock<IExecutionState>();

      Assert.That(executionStateContext.ExecutionState, Is.SameAs(NullExecutionState.Null));

      executionStateContext.SetExecutionState(newExecutionState.Object);

      Assert.That(executionStateContext.ExecutionState, Is.SameAs(newExecutionState.Object));
    }

    [Test]
    public void IExecutionStateContext_GetCurrentStep ()
    {
      IExecutionStateContext executionStateContext = _pageStep.Object;
      Assert.That(executionStateContext.CurrentStep, Is.SameAs(_pageStep.Object));
    }

    [Test]
    public void IExecutionStateContext_GetCurrentFunction ()
    {
      _pageStep.Object.SetParentStep(_rootFunction);
      IExecutionStateContext executionStateContext = _pageStep.Object;

      Assert.That(executionStateContext.CurrentFunction, Is.SameAs(_rootFunction));
    }

    [Test]
    public void IExecutionStateContext_GetCurrentFunction_WithNullParentFunction_ThrowsInvalidOperationException ()
    {
      IExecutionStateContext executionStateContext = _pageStep.Object;

      Assert.That(
          () => executionStateContext.CurrentFunction,
          Throws.Exception.TypeOf<WxeException>().With.Message.EqualTo("There must be a function associated to the current step while executing."));
    }
  }
}
