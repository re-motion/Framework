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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

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
      RequestMock.Setup (stub => stub.HttpMethod).Returns ("GET");

      var sequence = new MockSequence();

      ExecutionStateContextMock.Setup (mock => mock.SetReturnState (SubFunction, true, PostBackCollection)).Verifiable();

      ExecutionStateContextMock.Setup (mock => mock.SetExecutionState (NullExecutionState.Null)).Verifiable();

      _executionState.ExecuteSubFunction (WxeContext);

      Assert.That (PostBackCollection[WxePageInfo.PostBackSequenceNumberID], Is.EqualTo ("100"));
    }

    [Test]
    public void ExecuteSubFunction_WithPostRequest ()
    {
      PrivateInvoke.SetNonPublicField (FunctionState, "_postBackID", 100);
      RequestMock.Setup (stub => stub.HttpMethod).Returns ("POST");

      var sequence = new MockSequence();

      ExecutionStateContextMock.Setup (mock => mock.SetReturnState (SubFunction, false, null)).Verifiable();

      ExecutionStateContextMock.Setup (mock => mock.SetExecutionState (NullExecutionState.Null)).Verifiable();

      _executionState.ExecuteSubFunction (WxeContext);
    }
  }
}
