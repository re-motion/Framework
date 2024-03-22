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
using Remotion.Web.UI;
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
      _functionState = new WxeFunctionState(_rootFunction, 20, true);

      _pageStep = new Mock<WxePageStep>("ThePage") { CallBase = true };

      _pageMock = new Mock<IWxePage>();
      _wxeHandler = new WxeHandler();

      _functionStateManager = new WxeFunctionStateManager(new FakeHttpSessionStateBase());
      _wxeContext = new WxeContext(
          _httpContextMock.Object,
          _functionStateManager,
          _functionState,
          new NameValueCollection(),
          new WxeUrlSettings(),
          new WxeLifetimeManagementSettings());
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
      WxeContext.SetCurrent(_wxeContext);

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeRepostOptions repostOptions = WxeRepostOptions.SuppressRepost(new Mock<Control>().Object, true);

      var sequence = new VerifiableSequence();
      _pageMock.InVerifiableSequence(sequence).Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();
      _pageStep
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Execute(_wxeContext))
          .Callback(
              (WxeContext context) =>
              {
                var executionState = (PreProcessingSubFunctionState)((IExecutionStateContext)_pageStep.Object).ExecutionState;
                Assert.That(executionState.Parameters.SubFunction, Is.SameAs(_subFunction.Object));
                Assert.That(executionState.Parameters.PermaUrlOptions, Is.SameAs(permaUrlOptions));
                Assert.That(executionState.RepostOptions, Is.SameAs(repostOptions));
                Assert.That(PrivateInvoke.GetNonPublicField(_pageStep.Object, "_wxeHandler"), Is.SameAs(_wxeHandler));
              })
          .Verifiable();

      _pageStep.Object.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), repostOptions);

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteFunction_IsAlreadyExecutingSubFunction ()
    {
      WxeContext.SetCurrent(_wxeContext);

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
    public void ExecuteFunction_CopiesDirtyStateFromPageBeforeInnerExecuteWhenPageIsDirty ()
    {
      WxeContext.SetCurrent(_wxeContext);

      _pageMock.Setup(_=> _.GetDirtyStates(new[] { SmartPageDirtyStates.CurrentPage })).Returns(new[] { SmartPageDirtyStates.CurrentPage });
      _pageStep
          .Setup(mock => mock.Execute(_wxeContext))
          .Callback(
              (WxeContext _) =>
              {
                Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
              })
          .Verifiable();

      _pageStep.Object.ExecuteFunction(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, new WxePermaUrlOptions()), WxeRepostOptions.DoRepost(null));

      _pageStep.Verify();

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void ExecuteFunction_CopiesDirtyStateFromPageBeforeInnerExecuteWhenPageStepWhenPageIsNotDirty ()
    {
      WxeContext.SetCurrent(_wxeContext);

      _pageStep.Object.SetDirtyStateForCurrentPage(true);

      _pageMock.Setup(_=> _.GetDirtyStates(new[] { SmartPageDirtyStates.CurrentPage })).Returns(new[] { WxePageDirtyStates.CurrentFunction });
      _pageStep
          .Setup(mock => mock.Execute(_wxeContext))
          .Callback(
              (WxeContext _) =>
              {
                Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
              })
          .Verifiable();

      _pageStep.Object.ExecuteFunction(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, new WxePermaUrlOptions()), WxeRepostOptions.DoRepost(null));

      _pageStep.Verify();

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void ExecuteFunctionExternalByRedirect ()
    {
      WxeContext.SetCurrent(_wxeContext);

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeReturnOptions returnOptions = new WxeReturnOptions();

      var sequence = new VerifiableSequence();
      _pageMock.InVerifiableSequence(sequence).Setup(mock => mock.WxeHandler).Returns(_wxeHandler).Verifiable();
      _pageStep
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Execute(_wxeContext))
          .Callback(
              (WxeContext context) =>
              {
                var executionState = (PreProcessingSubFunctionState_WithRedirect)((IExecutionStateContext)_pageStep.Object).ExecutionState;
                Assert.That(executionState.Parameters.SubFunction, Is.SameAs(_subFunction.Object));
                Assert.That(executionState.Parameters.PermaUrlOptions, Is.SameAs(permaUrlOptions));
                Assert.That(executionState.ReturnOptions, Is.SameAs(returnOptions));
                Assert.That(PrivateInvoke.GetNonPublicField(_pageStep.Object, "_wxeHandler"), Is.SameAs(_wxeHandler));
              })
          .Verifiable();

      _pageStep.Object.ExecuteFunctionExternalByRedirect(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, permaUrlOptions), returnOptions);

      _subFunction.Verify();
      _httpContextMock.Verify();
      _pageStep.Verify();
      _pageMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteFunctionExternalByRedirect_IsAlreadyExecutingSubFunction ()
    {
      WxeContext.SetCurrent(_wxeContext);

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
    public void ExecuteFunctionExternalByRedirect_CopiesDirtyStateFromPageBeforeInnerExecuteWhenPageIsDirty ()
    {
      WxeContext.SetCurrent(_wxeContext);

      _pageMock.Setup(_=> _.GetDirtyStates(new[] { SmartPageDirtyStates.CurrentPage })).Returns(new[] { SmartPageDirtyStates.CurrentPage });
      _pageStep
          .Setup(mock => mock.Execute(_wxeContext))
          .Callback(
              (WxeContext _) =>
              {
                Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
              })
          .Verifiable();

      _pageStep.Object.ExecuteFunctionExternalByRedirect(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, new WxePermaUrlOptions()), new WxeReturnOptions());

      _pageStep.Verify();

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void ExecuteFunctionExternalByRedirect_CopiesDirtyStateFromPageBeforeInnerExecuteWhenPageStepWhenPageIsNotDirty ()
    {
      WxeContext.SetCurrent(_wxeContext);

      _pageStep.Object.SetDirtyStateForCurrentPage(true);

      _pageMock.Setup(_=> _.GetDirtyStates(new[] { SmartPageDirtyStates.CurrentPage })).Returns(new[] { WxePageDirtyStates.CurrentFunction });
      _pageStep
          .Setup(mock => mock.Execute(_wxeContext))
          .Callback(
              (WxeContext _) =>
              {
                Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
              })
          .Verifiable();

      _pageStep.Object.ExecuteFunctionExternalByRedirect(
          new PreProcessingSubFunctionStateParameters(_pageMock.Object, _subFunction.Object, new WxePermaUrlOptions()), new WxeReturnOptions());

      _pageStep.Verify();

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void IExecutionStateContext_GetAndSetExecutionState ()
    {
      var executionStateContext = (IExecutionStateContext)_pageStep.Object;
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

    [Test]
    public void SetReturnState_SetsReturningFunction ()
    {
      var returningFunction = new TestFunction2();

      _pageStep.Object.SetReturnState(returningFunction, true, new NameValueCollection());

      Assert.That(_pageStep.Object.ReturningFunction, Is.SameAs(returningFunction));
    }

    [Test]
    public void SetReturnState_SetsIsReturningPostback ()
    {
      Assert.That(_pageStep.Object.IsReturningPostBack, Is.False);

      _pageStep.Object.SetReturnState(new TestFunction2(), true, new NameValueCollection());

      Assert.That(_pageStep.Object.IsReturningPostBack, Is.True);
    }

    [Test]
    public void SetReturnState_SetsPostbackCollection ()
    {
      var previousPostBackCollection = new NameValueCollection();

      _pageStep.Object.SetReturnState(new TestFunction2(), true, previousPostBackCollection);

      Assert.That(_pageStep.Object.PostBackCollection, Is.SameAs(previousPostBackCollection));
    }

    [Test]
    public void SetReturnState_CopiesDirtyStateFromReturningFunctionWhenReturningFunctionIsDirty ()
    {
      var returningFunction = new TestFunction2();
      returningFunction.IsDirty = true;
      _pageStep.Object.SetReturnState(returningFunction, true, new NameValueCollection());

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void SetReturnState_IgnoresDirtyStateFromReturningFunctionWhenReturningFunctionHasAnException ()
    {
      var returningFunction = new TestFunction2();
      returningFunction.ExceptionHandler.SetCatchExceptionTypes(typeof(Exception));
      returningFunction.ExceptionHandler.Catch(new Exception("Test Exception"));
      Assert.That(returningFunction.ExceptionHandler.Exception, Is.Not.Null);
      returningFunction.IsDirty = true;
      _pageStep.Object.SetReturnState(returningFunction, true, new NameValueCollection());

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void SetReturnState_AggregatesDirtyStateFromReturningFunctionWhenPreviousReturningFunctionWasAlreadyDirty ()
    {
      var returningFunction1 = new TestFunction2();
      returningFunction1.IsDirty = true;
      _pageStep.Object.SetReturnState(returningFunction1, true, new NameValueCollection());

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);

      var returningFunction2 = new Mock<TestFunction2>();

      _pageStep.Object.SetReturnState(returningFunction2.Object, true, new NameValueCollection());

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
      returningFunction2.Verify(_=>_.EvaluateDirtyState(), Times.Never());
    }

    [Test]
    public void Execute_ClearsReturnStateBySettingReturningFunctionToNull ()
    {
      _pageStep.Object.SetReturnState(new TestFunction2(), true, new NameValueCollection());
      _pageStep.Object.SetPageExecutor(Mock.Of<IWxePageExecutor>());

      _pageStep.Object.Execute(_wxeContext);

      Assert.That(_pageStep.Object.ReturningFunction, Is.Null);
    }

    [Test]
    public void Execute_ClearsReturnStateBySettingIsReturningPostbackToFalse ()
    {
      _pageStep.Object.SetReturnState(new TestFunction2(), true, new NameValueCollection());
      _pageStep.Object.SetPageExecutor(Mock.Of<IWxePageExecutor>());

      _pageStep.Object.Execute(_wxeContext);

      Assert.That(_pageStep.Object.IsReturningPostBack, Is.False);
    }

    [Test]
    public void Execute_ClearsReturnStateBySettingPostbackCollectionToNull ()
    {
      _pageStep.Object.SetReturnState(new TestFunction2(), true, new NameValueCollection());
      _pageStep.Object.SetPageExecutor(Mock.Of<IWxePageExecutor>());

      _pageStep.Object.Execute(_wxeContext);

      Assert.That(_pageStep.Object.PostBackCollection, Is.Null);
    }

    [Test]
    public void Execute_PreservesDirtyStateFromPreviousReturningFunction ()
    {
      var returningFunction = new TestFunction2();
      returningFunction.IsDirty = true;
      _pageStep.Object.SetReturnState(returningFunction, true, new NameValueCollection());
      _pageStep.Object.SetPageExecutor(Mock.Of<IWxePageExecutor>());

      _pageStep.Object.Execute(_wxeContext);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void SetDirtyStateForCurrentPage_WithTrue_SetsPageStepDirty ()
    {
      _pageStep.Object.SetDirtyStateForCurrentPage(true);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void SetDirtyStateForCurrentPage_WithFalse_SetsPageStepNotDirty ()
    {
      _pageStep.Object.SetDirtyStateForCurrentPage(true);
      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);

      _pageStep.Object.SetDirtyStateForCurrentPage(false);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void ResetDirtyStateForExecutedSteps_ResetsDirtyStateFromCurrentPage ()
    {
      _pageStep.Object.SetDirtyStateForCurrentPage(true);

      _pageStep.Object.ResetDirtyStateForExecutedSteps();

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void ResetDirtyStateForExecutedSteps_ResetsDirtyStateFromReturningFunction ()
    {
      var returningFunction = new TestFunction2();
      returningFunction.IsDirty = true;
      _pageStep.Object.SetReturnState(returningFunction, true, new NameValueCollection());
      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);

      _pageStep.Object.ResetDirtyStateForExecutedSteps();

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void EvaluateDirtyState_WithoutAnyInformation_ReturnsFalse ()
    {
      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void EvaluateDirtyState_ReturnsDirtyStateFromReturningFunction ()
    {
      var returningFunction = new TestFunction2();
      returningFunction.IsDirty = true;
      _pageStep.Object.SetReturnState(returningFunction, true, new NameValueCollection());
      _pageStep.Object.SetPageExecutor(Mock.Of<IWxePageExecutor>());

      _pageStep.Object.Execute(_wxeContext);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void EvaluateDirtyState_ReturnsDirtyStateFromCurrentPage ()
    {
      _pageStep.Object.SetDirtyStateForCurrentPage(true);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void EvaluateDirtyState_EvaluatesDirtyStateForExecutingSubFunction ()
    {
      var dirtyFunction = new TestFunction2();
      dirtyFunction.IsDirty = true;

      var parametersStub = new Mock<IExecutionStateParameters>();
      parametersStub.Setup(_ => _.SubFunction).Returns(dirtyFunction);

      var executionStateStub = new Mock<IExecutionState>();
      executionStateStub.Setup(_ => _.IsExecuting).Returns(true);
      executionStateStub.Setup(_ => _.Parameters).Returns(parametersStub.Object);
      ((IExecutionStateContext)_pageStep.Object).SetExecutionState(executionStateStub.Object);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void EvaluateDirtyState_EvaluatesDirtyStateForExecutingStepWhenDirtyFromReturningFunction ()
    {
      var returningFunction = new TestFunction2();
      returningFunction.IsDirty = true;
      _pageStep.Object.SetReturnState(returningFunction, true, new NameValueCollection());
      _pageStep.Object.SetPageExecutor(Mock.Of<IWxePageExecutor>());

      var dirtyStepStub = new Mock<WxeStep>();
      _pageStep.Setup(_ => _.ExecutingStep).Returns(dirtyStepStub.Object);

      _pageStep.Object.Execute(_wxeContext);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
      dirtyStepStub.Verify(_ => _.EvaluateDirtyState(), Times.Never());
    }

    [Test]
    public void EvaluateDirtyState_EvaluatesDirtyStateForExecutingStepWhenDirtyFromCurrentPage ()
    {
      _pageStep.Object.SetDirtyStateForCurrentPage(true);

      var dirtyStepStub = new Mock<WxeStep>();
      _pageStep.Setup(_ => _.ExecutingStep).Returns(dirtyStepStub.Object);

      Assert.That(_pageStep.Object.EvaluateDirtyState(), Is.True);
      dirtyStepStub.Verify(_ => _.EvaluateDirtyState(), Times.Never());
    }

    [Test]
    public void IsDirtyStateEnabled_PassesCallToBaseImplementation_WithParentFunctionHasDirtyStateEnabled_ReturnsTrue ()
    {
      _pageStep.Object.SetParentStep(_rootFunction);
      Assert.That(_rootFunction.IsDirtyStateEnabled, Is.True);

      Assert.That(_pageStep.Object.IsDirtyStateEnabled, Is.True);
    }

    [Test]
    public void IsDirtyStateEnabled_PassesCallToBaseImplementation_WithParentFunctionHasDirtyStateDisabled_ReturnsFalse ()
    {
      _rootFunction.DisableDirtyState();
      _pageStep.Object.SetParentStep(_rootFunction);
      Assert.That(_rootFunction.IsDirtyStateEnabled, Is.False);

      Assert.That(_pageStep.Object.IsDirtyStateEnabled, Is.False);
    }
  }
}
