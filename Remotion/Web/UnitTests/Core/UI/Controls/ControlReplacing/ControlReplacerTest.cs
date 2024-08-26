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
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class ControlReplacerTest : TestBase
  {
    [Test]
    public void SaveAllState_ViewState ()
    {
      var testPageHolder = new TestPageHolder(true, RequestMode.Get);
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest(testPageHolder.NamingContainer, new StateLoadingStrategy());
      testPageHolder.PageInvoker.InitRecursive();

#pragma warning disable CFW0001
      var formatter = new LosFormatter();
#pragma warning restore CFW0001
      var state = (Pair)formatter.Deserialize(replacer.SaveAllState());

      Pair replacerViewState = (Pair)state.Second;
      Assert.That(replacerViewState.First, Is.EqualTo("value"));
      var namingContainerViewState = (Pair)((IList)(replacerViewState).Second)[1];
      Assert.That(namingContainerViewState.First, Is.EqualTo("NamingContainerValue"));
      var parentViewState = (Pair)((IList)(namingContainerViewState).Second)[1];
      Assert.That(parentViewState.First, Is.EqualTo("ParentValue"));
    }

    [Test]
    public void SaveViewStateRecursive ()
    {
      var testPageHolder = new TestPageHolder(true, RequestMode.Get);
      SetupControlReplacerForIntegrationTest(testPageHolder.NamingContainer, new StateLoadingStrategy());

      testPageHolder.PageInvoker.InitRecursive();
      object viewState = testPageHolder.PageInvoker.SaveViewStateRecursive(ViewStateMode.Enabled);

      Assert.That(viewState, Is.InstanceOf(typeof(Pair)));
      var replacerViewState = (Pair)((IList)((Pair)viewState).Second)[3];
      Assert.That(replacerViewState.First, Is.EqualTo("value"));
      var namingContainerViewState = (Pair)((IList)(replacerViewState).Second)[1];
      Assert.That(namingContainerViewState.First, Is.EqualTo("NamingContainerValue"));
      var parentViewState = (Pair)((IList)(namingContainerViewState).Second)[1];
      Assert.That(parentViewState.First, Is.EqualTo("ParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_RegularPostBack ()
    {
      object viewState = CreateViewState();
      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateLoadingStrategy());

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive(viewState);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo("NamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo("ParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_ReplaceViewState ()
    {
      object originalViewState = CreateViewState();

      var testPageHolderWithChangedState = new TestPageHolder(false, RequestMode.Get);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest(testPageHolderWithChangedState.NamingContainer, new StateLoadingStrategy());
      testPageHolderWithChangedState.PageInvoker.InitRecursive();
      testPageHolderWithChangedState.Parent.ValueInViewState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInViewState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateReplacingStrategy(backedUpState));

      testPageHolderWithoutState.Page.SetRequestValueCollection(new NameValueCollection());
      testPageHolderWithoutState.PageInvoker.InitRecursive();
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive(originalViewState);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo("NewNamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo("NewParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_ClearViewState ()
    {
      object originalViewState = CreateViewState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateClearingStrategy());

      testPageHolderWithoutState.Page.SetRequestValueCollection(new NameValueCollection());
      testPageHolderWithoutState.PageInvoker.InitRecursive();
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive(originalViewState);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);
    }


    [Test]
    public void LoadViewStateRecursive_RegularPostBack_InitializedAfterLoadViewState ()
    {
      object viewState = CreateViewState();
      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateLoadingStrategy());
      testPageHolderWithoutState.Page.Controls.Remove(testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive(viewState);
      testPageHolderWithoutState.Page.Controls.Add(testPageHolderWithoutState.NamingContainer);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo("NamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo("ParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_ReplaceViewState_InitializedAfterLoadViewState ()
    {
      object originalViewState = CreateViewState();

      var testPageHolderWithChangedState = new TestPageHolder(false, RequestMode.Get);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest(testPageHolderWithChangedState.NamingContainer, new StateLoadingStrategy());
      testPageHolderWithChangedState.PageInvoker.InitRecursive();
      testPageHolderWithChangedState.Parent.ValueInViewState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInViewState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateReplacingStrategy(backedUpState));
      testPageHolderWithoutState.Page.Controls.Remove(testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.Page.SetRequestValueCollection(new NameValueCollection());
      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive(originalViewState);
      testPageHolderWithoutState.Page.Controls.Add(testPageHolderWithoutState.NamingContainer);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo("NewNamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo("NewParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_ClearViewState_InitializedAfterLoadViewState ()
    {
      object originalViewState = CreateViewState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateClearingStrategy());
      testPageHolderWithoutState.Page.Controls.Remove(testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.Page.SetRequestValueCollection(new NameValueCollection());
      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive(originalViewState);
      testPageHolderWithoutState.Page.Controls.Add(testPageHolderWithoutState.NamingContainer);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);
    }


    [Test]
    public void SaveAllState_ControlState ()
    {
      var testPageHolder = new TestPageHolder(true, RequestMode.Get);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolder.NamingContainer, new StateLoadingStrategy());
      testPageHolder.PageInvoker.InitRecursive();

#pragma warning disable CFW0001
      var formatter = new LosFormatter();
#pragma warning restore CFW0001
      var state = (Pair)formatter.Deserialize(replacer.SaveAllState());

      IDictionary controlState = (IDictionary)state.First;
      Assert.That(controlState[replacer.UniqueID], Is.Null);
      Assert.That(controlState[testPageHolder.NamingContainer.UniqueID], new PairConstraint(new Pair("NamingContainerValue", null)));
      Assert.That(controlState[testPageHolder.Parent.UniqueID], new PairConstraint(new Pair("ParentValue", null)));
    }

    [Test]
    public void SaveControlStateRecursive ()
    {
      var testPageHolder = new TestPageHolder(true, RequestMode.Get);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolder.NamingContainer, new StateLoadingStrategy());

      testPageHolder.PageInvoker.InitRecursive();
      testPageHolder.Page.SaveAllState();

      var controlStateObject = testPageHolder.Page.GetPageStatePersister().ControlState;
      Assert.That(controlStateObject, Is.InstanceOf(typeof(IDictionary)));
      IDictionary controlState = (IDictionary)controlStateObject;
      Assert.That(controlState[replacer.UniqueID], new PairConstraint(new Pair("value", null)));
      Assert.That(controlState[testPageHolder.NamingContainer.UniqueID], new PairConstraint(new Pair("NamingContainerValue", null)));
      Assert.That(controlState[testPageHolder.Parent.UniqueID], new PairConstraint(new Pair("ParentValue", null)));
    }

    [Test]
    public void LoadControlStateRecursive_RegularPostBack ()
    {
      object controlState = CreateControlState();
      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateLoadingStrategy());

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.Page.SetPageStatePersister(
          new HiddenFieldPageStatePersister(testPageHolderWithoutState.Page) { ControlState = controlState });
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      testPageHolderWithoutState.Page.LoadAllState();
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo("NamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo("ParentValue"));
    }

    [Test]
    public void LoadControlStateRecursive_ReplaceControlState ()
    {
      object originalControlState = CreateControlState();

      var testPageHolderWithChangedState = new TestPageHolder(false, RequestMode.Get);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest(testPageHolderWithChangedState.NamingContainer, new StateLoadingStrategy());
      testPageHolderWithChangedState.PageInvoker.InitRecursive();
      testPageHolderWithChangedState.Parent.ValueInControlState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInControlState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateReplacingStrategy(backedUpState));

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.Page.SetPageStatePersister(
          new HiddenFieldPageStatePersister(testPageHolderWithoutState.Page) { ControlState = originalControlState });
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      testPageHolderWithoutState.Page.LoadAllState();
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo("NewNamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo("NewParentValue"));
    }

    [Test]
    public void LoadControlStateRecursive_ClearControlState ()
    {
      object originalControlState = CreateControlState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateClearingStrategy());

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.Page.SetPageStatePersister(
          new HiddenFieldPageStatePersister(testPageHolderWithoutState.Page) { ControlState = originalControlState });
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      testPageHolderWithoutState.Page.LoadAllState();
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);
    }

    [Test]
    public void LoadControlStateRecursive_RegularPostBack_InitializedAfterLoadControlState ()
    {
      object controlState = CreateControlState();
      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateLoadingStrategy());
      testPageHolderWithoutState.Page.Controls.Remove(testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.Page.SetPageStatePersister(
          new HiddenFieldPageStatePersister(testPageHolderWithoutState.Page) { ControlState = controlState });
      testPageHolderWithoutState.Page.LoadAllState();
      testPageHolderWithoutState.Page.Controls.Add(testPageHolderWithoutState.NamingContainer);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo("NamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo("ParentValue"));
    }

    [Test]
    public void LoadControlStateRecursive_ReplaceControlState_InitializedAfterLoadControlState ()
    {
      object originalControlState = CreateControlState();

      var testPageHolderWithChangedState = new TestPageHolder(false, RequestMode.Get);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest(testPageHolderWithChangedState.NamingContainer, new StateLoadingStrategy());
      testPageHolderWithChangedState.PageInvoker.InitRecursive();
      testPageHolderWithChangedState.Parent.ValueInControlState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInControlState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateReplacingStrategy(backedUpState));
      testPageHolderWithoutState.Page.Controls.Remove(testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.Page.SetPageStatePersister(
          new HiddenFieldPageStatePersister(testPageHolderWithoutState.Page) { ControlState = originalControlState });
      testPageHolderWithoutState.Page.LoadAllState();
      testPageHolderWithoutState.Page.Controls.Add(testPageHolderWithoutState.NamingContainer);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo("NewNamingContainerValue"));
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo("NewParentValue"));
    }

    [Test]
    public void LoadControlStateRecursive_ClearControlState_InitializedAfterLoadControlState ()
    {
      object originalControlState = CreateControlState();

      var testPageHolderWithoutState = new TestPageHolder(false, RequestMode.PostBack);
      var replacer = SetupControlReplacerForIntegrationTest(testPageHolderWithoutState.NamingContainer, new StateClearingStrategy());
      testPageHolderWithoutState.Page.Controls.Remove(testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.Page.SetPageStatePersister(
          new HiddenFieldPageStatePersister(testPageHolderWithoutState.Page) { ControlState = originalControlState });
      testPageHolderWithoutState.Page.LoadAllState();
      testPageHolderWithoutState.Page.Controls.Add(testPageHolderWithoutState.NamingContainer);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      replacer.LoadPostData(null, null);
      Assert.That(testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo("OtherValue"));
      Assert.That(testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That(testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);
    }


    [Test]
    public void WrapControlWithParentContainer_ReplacesControl_WithGetRequest ()
    {
      var testPageHolder = new TestPageHolder(true, RequestMode.Get);
      ControlReplacer replacer = new ControlReplacer(MemberCallerMock.Object) { ID = "TheReplacer" };
      var controlToReplace = new ReplaceableControlMock();
      var controlToWrap = new ReplaceableControlMock();
      MemberCallerMock.Setup(stub => stub.GetControlState(controlToReplace)).Returns(ControlState.ChildrenInitialized);

      var sequence = new VerifiableSequence();
      MemberCallerMock.InVerifiableSequence(sequence).Setup(mock => mock.SetCollectionReadOnly(testPageHolder.Page.Controls, null)).Returns("error").Verifiable();
      MemberCallerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetCollectionReadOnly(testPageHolder.Page.Controls, "error"))
          .Returns((string)null)
          .Callback(
              (ControlCollection collection, string exceptionMessage) =>
                  Assert.That(testPageHolder.Page.Controls, Is.EqualTo(new Control[] { testPageHolder.OtherNamingContainer, testPageHolder.NamingContainer, replacer })))
          .Verifiable();
      MemberCallerMock.InVerifiableSequence(sequence).Setup(mock => mock.InitRecursive(replacer, testPageHolder.Page)).Verifiable();

      Assert.That(replacer.Controls, Is.Empty);

      testPageHolder.Page.Controls.Add(controlToReplace);
      replacer.ReplaceAndWrap(controlToReplace, controlToWrap, new StateLoadingStrategy());

      MemberCallerMock.Verify();
      sequence.Verify();

      Assert.That(
          testPageHolder.Page.Controls,
          Is.EqualTo(new Control[] { testPageHolder.OtherNamingContainer, testPageHolder.NamingContainer, replacer }));
      Assert.That(replacer.Controls, Is.EqualTo(new[] { controlToWrap }));
      Assert.That(controlToReplace.Replacer, Is.Null);
      Assert.That(controlToWrap.Replacer, Is.SameAs(replacer));
      Assert.That(replacer.WrappedControl, Is.SameAs(controlToWrap));
    }

    [Test]
    public void WrapControlWithParentContainer_ReplacesControl_WithPostRequest ()
    {
      var testPageHolder = new TestPageHolder(true, RequestMode.PostBack);
      ControlReplacer replacer = new ControlReplacer(MemberCallerMock.Object) { ID = "TheReplacer" };
      var controlToReplace = new ReplaceableControlMock();
      var controlToWrap = new ReplaceableControlMock();
      MemberCallerMock.Setup(stub => stub.GetControlState(controlToReplace)).Returns(ControlState.ChildrenInitialized);

      var sequence = new VerifiableSequence();
      MemberCallerMock.InVerifiableSequence(sequence).Setup(mock => mock.SetCollectionReadOnly(testPageHolder.Page.Controls, null)).Returns("error").Verifiable();
      MemberCallerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.SetCollectionReadOnly(testPageHolder.Page.Controls, "error"))
          .Returns((string)null)
          .Callback(
              (ControlCollection collection, string exceptionMessage) =>
                  Assert.That(testPageHolder.Page.Controls, Is.EqualTo(new Control[] { testPageHolder.OtherNamingContainer, testPageHolder.NamingContainer, replacer })))
          .Verifiable();
      MemberCallerMock.InVerifiableSequence(sequence).Setup(mock => mock.InitRecursive(replacer, testPageHolder.Page)).Verifiable();

      Assert.That(replacer.Controls, Is.Empty);

      testPageHolder.Page.Controls.Add(controlToReplace);
      replacer.ReplaceAndWrap(controlToReplace, controlToWrap, new StateLoadingStrategy());
      MemberCallerMock.Verify();
      sequence.Verify();
      Assert.That(
          testPageHolder.Page.Controls,
          Is.EqualTo(new Control[] { testPageHolder.OtherNamingContainer, testPageHolder.NamingContainer, replacer }));
      Assert.That(replacer.Controls, Is.Empty);

      MemberCallerMock.Reset();
      MemberCallerMock.Setup(stub => stub.SetControlState(controlToWrap, ControlState.Constructed));

      replacer.LoadPostData(null, null);

      MemberCallerMock.Verify();

      Assert.That(
          testPageHolder.Page.Controls,
          Is.EqualTo(new Control[] { testPageHolder.OtherNamingContainer, testPageHolder.NamingContainer, replacer }));
      Assert.That(replacer.Controls, Is.EqualTo(new[] { controlToWrap }));
      Assert.That(controlToReplace.Replacer, Is.Null);
      Assert.That(controlToWrap.Replacer, Is.SameAs(replacer));
      Assert.That(replacer.WrappedControl, Is.SameAs(controlToWrap));
    }

    [Test]
    public void WrapControlWithParentContainer_ThrowsIfNotInOnInit ()
    {
      ControlReplacer replacer = new ControlReplacer(MemberCallerMock.Object) { ID = "TheReplacer" };
      var control = new ReplaceableControlMock();
      MemberCallerMock.Setup(stub => stub.GetControlState(control)).Returns(ControlState.Initialized);
      Assert.That(
          () => replacer.ReplaceAndWrap(control, control, new StateLoadingStrategy()),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Controls can only be wrapped during OnInit phase."));
    }

    [Test]
    public void WrapControlWithParentContainer_ThrowsIfAlreadyInitialized ()
    {
      ControlReplacer replacer = new ControlReplacer(MemberCallerMock.Object) { ID = "TheReplacer" };
      var control = new ReplaceableControlMock();
      MemberCallerMock.Setup(stub => stub.GetControlState(control)).Returns(ControlState.ChildrenInitialized);
      control.EnsureLazyInitializationContainer();

      Assert.That(
          () => replacer.ReplaceAndWrap(control, control, new StateLoadingStrategy()),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Controls can only be wrapped before they are initialized."));
    }

    [Test]
    [Ignore("Reason is missing")]
    public void GetWrappedControl_BeforeReplaceAndWrap ()
    {
      Assert.That(
          () => { },
          Throws.InvalidOperationException
              .With.Message.EqualTo("The WrappedControl property can only be accessed after ReplaceAndWrap was invoked."));
    }

    [Test]
    public void GetWrappedControl ()
    {
      ControlReplacer replacer = new ControlReplacer(MemberCallerMock.Object) { ID = "TheReplacer" };
      Control control = new Control();

      Assert.That(replacer.WrappedControl, Is.Null);

      replacer.Controls.Add(control);

      Assert.That(replacer.WrappedControl, Is.SameAs(control));
    }
  }
}
