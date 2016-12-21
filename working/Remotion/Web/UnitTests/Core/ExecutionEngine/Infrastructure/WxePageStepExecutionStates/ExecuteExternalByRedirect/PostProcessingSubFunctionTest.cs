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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  [TestFixture]
  public class PostProcessingSubFunctionTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new PostProcessingSubFunctionState (ExecutionStateContextMock, new ExecutionStateParameters (SubFunction, PostBackCollection));
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That (_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_WithGetRequest ()
    {
      PrivateInvoke.SetNonPublicField (FunctionState, "_postBackID", 100);
      RequestMock.Stub (stub => stub.HttpMethod).Return ("GET").Repeat.Any();

      using (MockRepository.Ordered())
      {
        ExecutionStateContextMock.Expect (mock => mock.SetReturnState (SubFunction, true, PostBackCollection));
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (NullExecutionState.Null));
      }

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();

      Assert.That (PostBackCollection[WxePageInfo.PostBackSequenceNumberID], Is.EqualTo ("100"));
    }

    [Test]
    public void ExecuteSubFunction_WithPostRequest ()
    {
      PrivateInvoke.SetNonPublicField (FunctionState, "_postBackID", 100);
      RequestMock.Stub (stub => stub.HttpMethod).Return ("POST").Repeat.Any();

      using (MockRepository.Ordered ())
      {
        ExecutionStateContextMock.Expect (mock => mock.SetReturnState (SubFunction, false, null));
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (NullExecutionState.Null));
      }

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }
  }
}
