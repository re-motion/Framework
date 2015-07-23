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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  [TestFixture]
  public class PreProcessingSubFunctionStateTest : TestBase
  {
    private const string c_senderUniqueID = "TheUnqiueID";
    private WxeStep _parentStep;
    private IWxePage _pageMock;

    public override void SetUp ()
    {
      base.SetUp();

      _parentStep = new WxePageStep ("page.aspx");
      ExecutionStateContextMock.Stub (stub => stub.CurrentStep).Return (_parentStep).Repeat.Any();

      _pageMock = MockRepository.StrictMock<IWxePage>();

      PostBackCollection.Add ("Key", "Value");
      PostBackCollection.Add (c_senderUniqueID, "Value");
      PostBackCollection.Add (ControlHelper.PostEventSourceID, "TheEventSource");
      PostBackCollection.Add (ControlHelper.PostEventArgumentID, "TheEventArgument");
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null, WxeReturnOptions.Null);
      Assert.That (executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeReturnOptions returnOptions = new WxeReturnOptions();
      IExecutionState executionState = CreateExecutionState (permaUrlOptions, returnOptions);

      using (MockRepository.Ordered ())
      {
        using (MockRepository.Unordered())
        {
          _pageMock.Expect (mock => mock.GetPostBackCollection()).Return (PostBackCollection);
          _pageMock.Expect (mock => mock.SaveAllState());
        }

        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<PreparingRedirectToSubFunctionState>.Is.NotNull))
            .WhenCalled (
            invocation =>
            {
              var nextState = CheckExecutionState ((PreparingRedirectToSubFunctionState) invocation.Arguments[0]);
              Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
              Assert.That (
                  nextState.Parameters.PostBackCollection.AllKeys,
                  Is.EquivalentTo (new[] { "Key", c_senderUniqueID, ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
              Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.Null);
              Assert.That (nextState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
              Assert.That (nextState.ReturnOptions, Is.SameAs (returnOptions));
            });
      }

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    private PreProcessingSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions, WxeReturnOptions returnOptions)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock, new PreProcessingSubFunctionStateParameters (_pageMock, SubFunction, permaUrlOptions), returnOptions);
    }
  }
}
