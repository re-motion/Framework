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
using System.Threading;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class ReturningFromSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new ReturningFromSubFunctionState(
          ExecutionStateContextMock.Object, new RedirectingToSubFunctionStateParameters(SubFunction.Object, PostBackCollection, "dummy", "/resumeUrl.wxe"));
    }

    protected override Mock<OtherTestFunction> CreateSubFunction ()
    {
      return new Mock<OtherTestFunction>(MockBehavior.Strict);
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That(_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction ()
    {
      var sequence = new VerifiableSequence();
      ResponseMock.InVerifiableSequence(sequence).Setup(mock => mock.Redirect("/resumeUrl.wxe")).Callback((string url) => WxeThreadAbortHelper.Abort()).Verifiable();
      ExecutionStateContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetExecutionState(It.IsNotNull<PostProcessingSubFunctionState>()))
          .Callback((IExecutionState executionState) => CheckExecutionState((PostProcessingSubFunctionState)executionState))
          .Verifiable();

      try
      {
        _executionState.ExecuteSubFunction(WxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }

      VerifyAll();
      sequence.Verify();
    }

    [Test]
    public void ExecuteSubFunction_WithFailedRedirect ()
    {
      var sequence = new VerifiableSequence();
      ResponseMock.InVerifiableSequence(sequence).Setup(mock => mock.Redirect("/resumeUrl.wxe")).Verifiable();

      Assert.That(
          () => _executionState.ExecuteSubFunction(WxeContext),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Redirect to '/resumeUrl.wxe' failed."));
      sequence.Verify();
    }
  }
}
