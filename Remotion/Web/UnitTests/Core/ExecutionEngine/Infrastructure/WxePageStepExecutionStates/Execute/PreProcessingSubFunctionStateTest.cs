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
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class PreProcessingSubFunctionStateTest : TestBase
  {
    private const string c_senderUniqueID = "TheUnqiueID";
    private WxeStep _parentStep;
    private Mock<IWxePage> _pageMock;

    public override void SetUp ()
    {
      base.SetUp();

      _parentStep = new WxePageStep ("page.aspx");
      ExecutionStateContextMock.Setup (stub => stub.CurrentStep).Returns (_parentStep);

      _pageMock = new Mock<IWxePage> (MockBehavior.Strict);

      PostBackCollection.Add ("Key", "Value");
      PostBackCollection.Add (c_senderUniqueID, "Value");
      PostBackCollection.Add (ControlHelper.PostEventSourceID, "TheEventSource");
      PostBackCollection.Add (ControlHelper.PostEventArgumentID, "TheEventArgument");
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionStateForDoRepost (null, WxePermaUrlOptions.Null);
      Assert.That (executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_WithoutPermaUrl ()
    {
      IExecutionState executionState = CreateExecutionStateForDoRepost (null, WxePermaUrlOptions.Null);

      var sequence = new MockSequence();

      _pageMock.Setup (mock => mock.GetPostBackCollection()).Returns (PostBackCollection).Verifiable();
      _pageMock.Setup (mock => mock.SaveAllState()).Verifiable();

      ExecutionStateContextMock.InSequence (sequence)
          .Setup (mock => mock.SetExecutionState (It.IsNotNull<ExecutingSubFunctionWithoutPermaUrlState>()))
          .Callback (
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) executionState);
                Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
                Assert.That (
                    nextState.Parameters.PostBackCollection.AllKeys,
                    Is.EquivalentTo (new[] { "Key", c_senderUniqueID, ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
                Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
              })
          .Verifiable();

      executionState.ExecuteSubFunction (WxeContext);

      _pageMock.Verify();
      VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IExecutionState executionState = CreateExecutionStateForDoRepost (null, permaUrlOptions);

      var sequence = new MockSequence();

      _pageMock.Setup (mock => mock.GetPostBackCollection()).Returns (PostBackCollection).Verifiable();
      _pageMock.Setup (mock => mock.SaveAllState()).Verifiable();

      ExecutionStateContextMock.InSequence (sequence)
          .Setup (mock => mock.SetExecutionState (It.IsNotNull<PreparingRedirectToSubFunctionState>()))
          .Callback (
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState ((PreparingRedirectToSubFunctionState) executionState);
                Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
                Assert.That (
                    nextState.Parameters.PostBackCollection.AllKeys,
                    Is.EquivalentTo (new[] { "Key", c_senderUniqueID, ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
                Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
                Assert.That (nextState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
              })
          .Verifiable();

      executionState.ExecuteSubFunction (WxeContext);

      _pageMock.Verify();
      VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_SuppressSender_IPostBackEventHandler ()
    {
      var senderMock = new Mock<Control> (MockBehavior.Strict);
      senderMock.Setup (stub => stub.UniqueID).Returns (c_senderUniqueID);

      IExecutionState executionState = CreateExecutionStateForSupressRepost ((Control) senderMock.As<IPostBackDataHandler>().Object);

      var sequence = new MockSequence();

      _pageMock.Setup (mock => mock.GetPostBackCollection()).Returns (PostBackCollection).Verifiable();
      _pageMock.Setup (mock => mock.SaveAllState()).Verifiable();

      ExecutionStateContextMock.InSequence (sequence)
          .Setup (mock => mock.SetExecutionState (It.IsNotNull<ExecutingSubFunctionWithoutPermaUrlState>()))
          .Callback (
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) executionState);
                Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
                Assert.That (
                    nextState.Parameters.PostBackCollection.AllKeys,
                    Is.EquivalentTo (new[] { "Key", ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
              })
          .Verifiable();

      executionState.ExecuteSubFunction (WxeContext);

      _pageMock.Verify();
      VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_SuppressSender_IPostBackDataHandler ()
    {
      var senderMock = new Mock<Control> (MockBehavior.Strict);
      senderMock.Setup (stub => stub.UniqueID).Returns (c_senderUniqueID);

      IExecutionState executionState = CreateExecutionStateForSupressRepost ((Control) senderMock.As<IPostBackDataHandler>().Object);

      var sequence = new MockSequence();

      _pageMock.Setup (mock => mock.GetPostBackCollection()).Returns (PostBackCollection).Verifiable();
      _pageMock.Setup (mock => mock.SaveAllState()).Verifiable();

      ExecutionStateContextMock.InSequence (sequence)
          .Setup (mock => mock.SetExecutionState (It.IsNotNull<ExecutingSubFunctionWithoutPermaUrlState>()))
          .Callback (
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) executionState);
                Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
                Assert.That (
                    nextState.Parameters.PostBackCollection.AllKeys,
                    Is.EquivalentTo (new[] { "Key", ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
              })
          .Verifiable();

      executionState.ExecuteSubFunction (WxeContext);

      _pageMock.Verify();
      VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_UsesEventTarget ()
    {
      IExecutionState executionState = CreateExecutionStateForSupressRepost (true);

      var sequence = new MockSequence();

      _pageMock.Setup (mock => mock.GetPostBackCollection()).Returns (PostBackCollection).Verifiable();
      _pageMock.Setup (mock => mock.SaveAllState()).Verifiable();

      ExecutionStateContextMock.InSequence (sequence)
          .Setup (mock => mock.SetExecutionState (It.IsNotNull<ExecutingSubFunctionWithoutPermaUrlState>()))
          .Callback (
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) executionState);
                Assert.That (nextState.Parameters.PostBackCollection.AllKeys, Is.EquivalentTo (new[] { "Key", c_senderUniqueID }));
              })
          .Verifiable();

      executionState.ExecuteSubFunction (WxeContext);

      _pageMock.Verify();
      VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_UsesEventTarget_SuppressSender_SenderRemains ()
    {
      var senderMock = new Mock<Control> (MockBehavior.Strict);
      senderMock.Setup (stub => stub.UniqueID).Returns (c_senderUniqueID);

      IExecutionState executionState = new PreProcessingSubFunctionState (
          ExecutionStateContextMock.Object,
          new PreProcessingSubFunctionStateParameters (_pageMock.Object, SubFunction.Object, WxePermaUrlOptions.Null),
          WxeRepostOptions.SuppressRepost (senderMock.Object, true));

      var sequence = new MockSequence();

      _pageMock.Setup (mock => mock.GetPostBackCollection()).Returns (PostBackCollection).Verifiable();
      _pageMock.Setup (mock => mock.SaveAllState()).Verifiable();

      ExecutionStateContextMock.InSequence (sequence)
          .Setup (mock => mock.SetExecutionState (It.IsNotNull<ExecutingSubFunctionWithoutPermaUrlState>()))
          .Callback (
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) executionState);
                Assert.That (nextState.Parameters.PostBackCollection.AllKeys, Is.EquivalentTo (new[] { "Key", c_senderUniqueID }));
              })
          .Verifiable();

      executionState.ExecuteSubFunction (WxeContext);

      _pageMock.Verify();
      VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_SenderRequiresRegistrationForPostBack ()
    {
      var senderMock = new Mock<Control> (MockBehavior.Strict);
      senderMock.Setup (stub => stub.UniqueID).Returns (c_senderUniqueID);
      PostBackCollection.Remove (c_senderUniqueID);

      IExecutionState executionState = CreateExecutionStateForDoRepost ((Control) senderMock.As<IPostBackDataHandler>().Object, WxePermaUrlOptions.Null);

      var sequence = new MockSequence();

      _pageMock.Setup (mock => mock.GetPostBackCollection()).Returns (PostBackCollection).Verifiable();

      _pageMock.InSequence (sequence).Setup (mock => mock.RegisterRequiresPostBack (senderMock.Object)).Verifiable();
      _pageMock.InSequence (sequence).Setup (mock => mock.SaveAllState()).Verifiable();

      ExecutionStateContextMock.InSequence (sequence)
                               .Setup (mock => mock.SetExecutionState (It.IsNotNull<ExecutingSubFunctionWithoutPermaUrlState>()))
                               .Callback ((IExecutionState executionState) => CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) executionState))
                               .Verifiable();

      executionState.ExecuteSubFunction (WxeContext);

      _pageMock.Verify();
      VerifyAll();
    }

    private PreProcessingSubFunctionState CreateExecutionStateForDoRepost (Control sender, WxePermaUrlOptions permaUrlOptions)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock.Object,
          new PreProcessingSubFunctionStateParameters (_pageMock.Object, SubFunction.Object, permaUrlOptions),
          WxeRepostOptions.DoRepost (sender));
    }

    private PreProcessingSubFunctionState CreateExecutionStateForSupressRepost (Control sender)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock.Object,
          new PreProcessingSubFunctionStateParameters (_pageMock.Object, SubFunction.Object, WxePermaUrlOptions.Null),
          WxeRepostOptions.SuppressRepost (sender, false));
    }

    private PreProcessingSubFunctionState CreateExecutionStateForSupressRepost (bool usesEventTarget)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock.Object,
          new PreProcessingSubFunctionStateParameters (_pageMock.Object, SubFunction.Object, WxePermaUrlOptions.Null),
          WxeRepostOptions.SuppressRepost (new Mock<Control>().Object, usesEventTarget));
    }
  }
}
