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
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class RedirectingToSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();

      _executionState = new RedirectingToSubFunctionState (
          ExecutionStateContextMock,
          new RedirectingToSubFunctionStateParameters (SubFunction, PostBackCollection, "~/destination.wxe", "~/resume.wxe"));
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That (_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_GoesToExecutingSubFunction ()
    {
      using (MockRepository.Ordered())
      {
        ResponseMock.Expect (mock => mock.Redirect ("~/destination.wxe")).WhenCalled (invocation => Thread.CurrentThread.Abort ());
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionWithPermaUrlState>.Is.NotNull))
            .WhenCalled (
            invocation =>
            {
              var nextState = CheckExecutionState ((ExecutingSubFunctionWithPermaUrlState) invocation.Arguments[0]);
              Assert.That (nextState.Parameters.ResumeUrl, Is.EqualTo ("~/resume.wxe"));
            });
      }

      MockRepository.ReplayAll();

      try
      {
        _executionState.ExecuteSubFunction (WxeContext);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      MockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Redirect to '~/destination.wxe' failed.",
        MatchType = MessageMatch.Contains)]
    public void ExecuteSubFunction_WithFailedRedirect ()
    {
      ResponseMock.Expect (mock => mock.Redirect ("~/destination.wxe"));

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);
    }
  }
}
