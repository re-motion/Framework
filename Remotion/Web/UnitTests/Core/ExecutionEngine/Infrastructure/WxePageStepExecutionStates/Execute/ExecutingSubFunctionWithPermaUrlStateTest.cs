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
using System.Threading;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class ExecutionSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new ExecutingSubFunctionWithPermaUrlState(
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
    public void ExecuteSubFunction_GoesToReturningFromSubFunction ()
    {
      var sequence = new VerifiableSequence();
      SubFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(WxeContext)).Verifiable();
      ExecutionStateContextMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.SetExecutionState(It.IsNotNull<IExecutionState>()))
          .Callback(
              (IExecutionState executionState) =>
              {
                Assert.That(executionState, Is.InstanceOf(typeof(ReturningFromSubFunctionState)));
                var nextState = (ReturningFromSubFunctionState)executionState;
                Assert.That(nextState.ExecutionStateContext, Is.SameAs(ExecutionStateContextMock.Object));
                Assert.That(nextState.Parameters.SubFunction, Is.SameAs(SubFunction.Object));
                Assert.That(nextState.Parameters.ResumeUrl, Is.EqualTo("/resumeUrl.wxe"));
              })
          .Verifiable();

      _executionState.ExecuteSubFunction(WxeContext);

      VerifyAll();
      sequence.Verify();
    }

    [Test]
    public void ExecuteSubFunction_ReEntrancy_GoesToReturningFromSubFunction ()
    {
      var sequence = new VerifiableSequence();
      var executeCallbacks = new Queue<Action>();
      executeCallbacks.Enqueue(() => WxeThreadAbortHelper.Abort());
      SubFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(WxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();
      executeCallbacks.Enqueue(() => { /* NOP */ });
      SubFunction.InVerifiableSequence(sequence).Setup(mock => mock.Execute(WxeContext)).Callback((WxeContext _) => executeCallbacks.Dequeue().Invoke()).Verifiable();
      ExecutionStateContextMock.InVerifiableSequence(sequence).Setup(mock => mock.SetExecutionState(It.IsNotNull<IExecutionState>())).Verifiable();

      try
      {
        _executionState.ExecuteSubFunction(WxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }

      _executionState.ExecuteSubFunction(WxeContext);

      VerifyAll();
      sequence.Verify();
    }
  }
}
