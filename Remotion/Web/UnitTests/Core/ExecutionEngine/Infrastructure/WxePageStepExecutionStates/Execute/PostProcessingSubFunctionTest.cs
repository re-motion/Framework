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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class PostProcessingSubFunctionTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new PostProcessingSubFunctionState(ExecutionStateContextMock.Object, new ExecutionStateParameters(SubFunction.Object, PostBackCollection));
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That(_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction ()
    {
      PrivateInvoke.SetNonPublicField(FunctionState, "_postBackID", 100);

      var sequence = new MockSequence();
      ExecutionStateContextMock.InSequence(sequence).Setup(mock => mock.SetReturnState(SubFunction.Object, true, PostBackCollection)).Verifiable();
      ExecutionStateContextMock.InSequence(sequence).Setup(mock => mock.SetExecutionState(NullExecutionState.Null)).Verifiable();

      _executionState.ExecuteSubFunction(WxeContext);

      VerifyAll();

      Assert.That(PostBackCollection[WxePageInfo.PostBackSequenceNumberID], Is.EqualTo("100"));
    }
  }
}
