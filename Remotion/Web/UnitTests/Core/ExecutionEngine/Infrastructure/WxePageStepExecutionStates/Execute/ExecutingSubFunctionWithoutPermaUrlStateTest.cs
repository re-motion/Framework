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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class ExecutionSubFunctionWithoutPermaUrlStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new ExecutingSubFunctionWithoutPermaUrlState (ExecutionStateContextMock, new ExecutionStateParameters (SubFunction, PostBackCollection));
    }

    protected override OtherTestFunction CreateSubFunction ()
    {
      return new Mock<OtherTestFunction> (MockBehavior.Strict).Object;
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That (_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_GoesToPostProcessingSubFunction ()
    {
      var sequence = new MockSequence();
      SubFunction.Setup (mock => mock.Execute (WxeContext)).Verifiable();
      ExecutionStateContextMock.Setup (mock => mock.SetExecutionState (It.IsNotNull<PostProcessingSubFunctionState>()))
            .Callback ((IExecutionState executionState) => CheckExecutionState ((PostProcessingSubFunctionState) invocation.Arguments[0]))
            .Verifiable();

      _executionState.ExecuteSubFunction (WxeContext);
    }

    [Test]
    public void ExecuteSubFunction_ReEntrancy_GoesToPostProcessingSubFunction ()
    {
      var sequence = new MockSequence();
      SubFunction.Setup (mock => mock.Execute (WxeContext)).Callback ((WxeContext context) => Thread.CurrentThread.Abort ()).Verifiable();
      SubFunction.Setup (mock => mock.Execute (WxeContext)).Verifiable();
      ExecutionStateContextMock.Setup (mock => mock.SetExecutionState (It.IsNotNull<IExecutionState>())).Verifiable();

      try
      {
        _executionState.ExecuteSubFunction (WxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      _executionState.ExecuteSubFunction (WxeContext);
    }
  }
}
