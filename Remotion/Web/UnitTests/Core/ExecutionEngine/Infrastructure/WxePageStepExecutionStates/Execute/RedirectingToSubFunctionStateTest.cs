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

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class RedirectingToSubFunctionStateTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();

      _executionState = new RedirectingToSubFunctionState(
          ExecutionStateContextMock.Object,
          new RedirectingToSubFunctionStateParameters(SubFunction.Object, PostBackCollection, "~/destination.wxe", "~/resume.wxe"));
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That(_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_GoesToExecutingSubFunction ()
    {
      var sequence = new VerifiableSequence();
      ResponseMock.InVerifiableSequence(sequence).Setup(mock => mock.Redirect("~/destination.wxe")).Callback((string url) => WxeThreadAbortHelper.Abort()).Verifiable();
      ExecutionStateContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetExecutionState(It.IsNotNull<ExecutingSubFunctionWithPermaUrlState>()))
          .Callback(
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState((ExecutingSubFunctionWithPermaUrlState)executionState);
                Assert.That(nextState.Parameters.ResumeUrl, Is.EqualTo("~/resume.wxe"));
              })
          .Verifiable();

      Assert.That(() => _executionState.ExecuteSubFunction(WxeContext), Throws.TypeOf<ThreadAbortException>());

      VerifyAll();
      sequence.Verify();
    }

    [Test]
    public void ExecuteSubFunction_WithFailedRedirect ()
    {
      ResponseMock.Setup(mock => mock.Redirect("~/destination.wxe")).Verifiable();

      Assert.That(
          () => _executionState.ExecuteSubFunction(WxeContext),
          Throws.InvalidOperationException
              .With.Message.Contains("Redirect to '~/destination.wxe' failed."));
    }
  }
}
