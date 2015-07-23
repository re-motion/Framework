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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class InternalControlMemberCallerTest
  {
    private InternalControlMemberCaller _memberCaller;

    private HttpContext _httpContext;
    private PageMock _page;
    private NamingContainerMock _namingContainer;
    private ControlInvoker _pageInvoker;
    private ControlMock _parent;
    private ControlMock _child;
    private Control _child2;
    private ControlMock _otherControl;

    [SetUp]
    public void SetUp ()
    {
      _httpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      _httpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
      HttpContextHelper.SetCurrent (_httpContext);

      _page = new PageMock ();
      _page.SetRequestValueCollection (new NameValueCollection ());
      _httpContext.Handler = _page;

      _namingContainer = new NamingContainerMock ();
      _namingContainer.ID = "NamingContainer";
      _page.Controls.Add (_namingContainer);

      _parent = new ControlMock ();
      _parent.ID = "Parent";
      _namingContainer.ValueInViewState = "NamingContainerValue";
      _namingContainer.ValueInControlState = "NamingContainerValue";
      _namingContainer.Controls.Add (_parent);

      _child = new ControlMock ();
      _child.ID = "Child";
      _parent.Controls.Add (_child);

      _child2 = new Control ();
      _child2.ID = "Child2";
      _parent.Controls.Add (_child2);

      NamingContainerMock otherNamingContainer = new NamingContainerMock ();
      otherNamingContainer.ID = "OtherNamingContainer";
      _page.Controls.Add (otherNamingContainer);

      _otherControl = new ControlMock ();
      _otherControl.ID = "OtherControl";
      _otherControl.ValueInViewState = "OtherValue";
      _otherControl.ValueInControlState = "OtherValue";
      otherNamingContainer.Controls.Add (_otherControl);

      _pageInvoker = new ControlInvoker (_page);

      _memberCaller = new InternalControlMemberCaller ();

    }

    [TearDown]
    public void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
    }

    [Test]
    public void InitRecursive ()
    {
      MockRepository mockRepository = new MockRepository();
      Page namingContainer = new Page();
      Control parentControlMock = mockRepository.PartialMock<Control> ();
      Control childControlMock = mockRepository.PartialMock<Control> ();

      using (mockRepository.Ordered())
      {
        childControlMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnInit", EventArgs.Empty));
        parentControlMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnInit", EventArgs.Empty));
      }

      mockRepository.ReplayAll();

      namingContainer.Controls.Add (parentControlMock);
      parentControlMock.Controls.Add (childControlMock);
      _memberCaller.InitRecursive (parentControlMock, namingContainer);

      mockRepository.VerifyAll();
    }

    [Test]
    public void GetControlState ()
    {
      Control control = new Control();
      Assert.That (_memberCaller.GetControlState (control), Is.EqualTo (ControlState.Constructed));
    }

    [Test]
    public void SetControlState ()
    {
      Control control = new Control ();
      _memberCaller.SetControlState (control, ControlState.Initialized);
      Assert.That (_memberCaller.GetControlState (control), Is.EqualTo (ControlState.Initialized));
    }

    [Test]
    public void ControlStateNames_AreEquvialentToInternalControlStateNames ()
    {
      Assert.That (Enum.GetNames (InternalControlMemberCaller.InternalControlStateType), Is.EqualTo (Enum.GetNames (typeof (ControlState))));
    }

    [Test]
    public void ControlStateValues_AreEquvialentToInternalControlStateValues ()
    {
      Assert.That (Enum.GetValues (InternalControlMemberCaller.InternalControlStateType).Cast<int>().ToArray(),
          Is.EqualTo (Enum.GetValues (typeof (ControlState)).Cast<int> ().ToArray ()));
    }

    [Test]
    public void LoadViewStateRecursive ()
    {
      object viewState = new Pair ("ParentValue", new ArrayList { 0, new Pair ("ChildValue", null) });

      _memberCaller.LoadViewStateRecursive (_parent, viewState);

      Assert.That (_parent.ValueInViewState, Is.EqualTo ("ParentValue"));
      Assert.That (_child.ValueInViewState, Is.EqualTo ("ChildValue"));
    }

    [Test]
    public void SaveViewStateRecursive_NamingContainerViewStateModeIsEnabled_SavesViewState ()
    {
      _namingContainer.ViewStateMode = ViewStateMode.Enabled;
      _parent.ValueInViewState = "ParentValue";
      _child.ValueInViewState = "ChildValue";

      object viewState = _memberCaller.SaveViewStateRecursive (_parent);

      Assert.That (viewState, Is.InstanceOf (typeof (Pair)));
      var parentViewState = (Pair) viewState;
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
      Assert.That (parentViewState.Second, Is.InstanceOf (typeof (ArrayList)));
      var childViewStates = (IList) parentViewState.Second;
      Assert.That (childViewStates.Count, Is.EqualTo (2));
      Assert.That (childViewStates[0], Is.EqualTo (0));
      Assert.That (childViewStates[1], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void SaveViewStateRecursive_ParentViewStateModeIsAlwaysInterited_FallsBackToSaveViewState ()
    {
      _page.ViewStateMode = ViewStateMode.Inherit;
      _namingContainer.ViewStateMode = ViewStateMode.Inherit;
      _parent.ValueInViewState = "ParentValue";
      _child.ValueInViewState = "ChildValue";

      object viewState = _memberCaller.SaveViewStateRecursive (_parent);

      Assert.That (viewState, Is.InstanceOf (typeof (Pair)));
      var parentViewState = (Pair) viewState;
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
      Assert.That (parentViewState.Second, Is.InstanceOf (typeof (ArrayList)));
      var childViewStates = (IList) parentViewState.Second;
      Assert.That (childViewStates.Count, Is.EqualTo (2));
      Assert.That (childViewStates[0], Is.EqualTo (0));
      Assert.That (childViewStates[1], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void SaveViewStateRecursive_ParentViewStateModeIsAlwaysDisabled_DoesNotSaveViewState ()
    {
      _namingContainer.ViewStateMode = ViewStateMode.Disabled;
      _parent.ValueInViewState = "ParentValue";
      _child.ValueInViewState = "ChildValue";

      object viewState = _memberCaller.SaveViewStateRecursive (_parent);

      Assert.That (viewState, Is.Null);
    }

    [Test]
    public void SaveViewStateRecursive_ParentViewStateModeIsDisabledAndOwnViewStateModeIsEnabled_SavesViewState ()
    {
      _namingContainer.ViewStateMode = ViewStateMode.Disabled;
      _parent.ViewStateMode = ViewStateMode.Enabled;
      _parent.ValueInViewState = "ParentValue";
      _child.ValueInViewState = "ChildValue";

      object viewState = _memberCaller.SaveViewStateRecursive (_parent);

      Assert.That (viewState, Is.InstanceOf (typeof (Pair)));
      var parentViewState = (Pair) viewState;
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
      Assert.That (parentViewState.Second, Is.InstanceOf (typeof (ArrayList)));
      var childViewStates = (IList) parentViewState.Second;
      Assert.That (childViewStates.Count, Is.EqualTo (2));
      Assert.That (childViewStates[0], Is.EqualTo (0));
      Assert.That (childViewStates[1], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void SaveViewStateRecursive_ParentViewStateModeIsEnabledAndOwnViewStateModeIsDisabled_DoesNotSaveViewState ()
    {
      _namingContainer.ViewStateMode = ViewStateMode.Enabled;
      _parent.ViewStateMode = ViewStateMode.Disabled;
      _parent.ValueInViewState = "ParentValue";
      _child.ValueInViewState = "ChildValue";

      object viewState = _memberCaller.SaveViewStateRecursive (_parent);

      Assert.That (viewState, Is.Null);
    }

    [Test]
    public void GetPageStatePersister ()
    {
      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      Assert.That (_memberCaller.GetPageStatePersister (_page), Is.SameAs (pageStatePersister));
    }

    [Test]
    public void SaveControlStateInternal ()
    {
      _parent.ValueInControlState = "ParentValue";
      Assert.That (_memberCaller.SaveControlStateInternal (_parent), new PairConstraint (new Pair ("ParentValue", null)));
    }

    [Test]
    public void SaveChildControlState ()
    {
      _parent.ValueInControlState = "ParentValue";
      _child.ValueInControlState = "ChildValue";

      _pageInvoker.InitRecursive ();

      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      IDictionary childControlState = _memberCaller.SaveChildControlState (_namingContainer);

      Assert.That (childControlState, Is.InstanceOf (typeof (HybridDictionary)));
      Assert.That (childControlState.Count, Is.EqualTo (2));
      Assert.That (childControlState[_parent.UniqueID], new PairConstraint (new Pair ("ParentValue", null)));
      Assert.That (childControlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void SaveChildControlState_NoControlsRegistered ()
    {
      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      Assert.That (_memberCaller.SaveChildControlState (_namingContainer), Is.Null);
    }

    [Test]
    public void SaveChildControlState_NoControlStateFromRegisteredControl ()
    {
      _parent.ValueInControlState = null;
      _child.ValueInControlState = "ChildValue";
      _pageInvoker.InitRecursive ();

      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      IDictionary childControlState = _memberCaller.SaveChildControlState (_namingContainer);

      Assert.That (childControlState, Is.InstanceOf (typeof (HybridDictionary)));
      Assert.That (childControlState.Count, Is.EqualTo (1));
      Assert.That (childControlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void SaveChildControlState_ControlRegisteredTwice ()
    {
      _parent.ValueInControlState = null;
      _child.ValueInControlState = "ChildValue";

      _pageInvoker.InitRecursive ();
      _page.RegisterRequiresControlState (_child);

      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      IDictionary childControlState = _memberCaller.SaveChildControlState (_namingContainer);

      Assert.That (childControlState, Is.InstanceOf (typeof (HybridDictionary)));
      Assert.That (childControlState.Count, Is.EqualTo (1));
      Assert.That (childControlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void GetChildControlState ()
    {
      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      var controlState = new Dictionary<string, object> ();
      pageStatePersister.ControlState = controlState;

      controlState[_parent.UniqueID] = "ParentValue";
      controlState[_child.UniqueID] = "ChildValue";
      controlState[_otherControl.UniqueID] = "OtherValue";
      controlState[_namingContainer.UniqueID + "1"] = "Parent1Value";

      IDictionary childControlState = _memberCaller.GetChildControlState (_namingContainer);

      Assert.That (childControlState, Is.InstanceOf (typeof (HybridDictionary)));
      Assert.That (childControlState.Count, Is.EqualTo (2));
      Assert.That (childControlState[_parent.UniqueID], Is.EqualTo ("ParentValue"));
      Assert.That (childControlState[_child.UniqueID], Is.EqualTo ("ChildValue"));
    }

    [Test]
    public void SetChildControlState ()
    {
      _pageInvoker.InitRecursive ();

      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      var controlState = new Dictionary<string, object> ();
      pageStatePersister.ControlState = controlState;

      IDictionary childControlState = new Dictionary<string, object> ();
      childControlState[_parent.UniqueID] = new Pair ("ParentValue", null);
      childControlState[_child.UniqueID] = new Pair ("ChildValue", null);

      _memberCaller.SetChildControlState (_namingContainer, childControlState);

      Assert.That (controlState.Count, Is.EqualTo (2));
      Assert.That (controlState[_parent.UniqueID], new PairConstraint (new Pair ("ParentValue", null)));
      Assert.That (controlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));

      _page.LoadAllState ();

      Assert.That (_parent.ValueInControlState, Is.EqualTo ("ParentValue"));
      Assert.That (_child.ValueInControlState, Is.EqualTo ("ChildValue"));
    }

    [Test]
    public void SetCollectionReadOnly_SetReadOnly ()
    {
      ControlCollection controlCollection = new ControlCollection (new ControlMock ());
      Assert.That (controlCollection.IsReadOnly, Is.False);

      string exceptionMessage = _memberCaller.SetCollectionReadOnly (controlCollection, "error");

      Assert.That (exceptionMessage, Is.Null);
      Assert.That (controlCollection.IsReadOnly, Is.True);
    }

    [Test]
    public void SetCollectionReadOnly_SetModifiable ()
    {
      ControlCollection controlCollection = new ControlCollection (new ControlMock ());
      _memberCaller.SetCollectionReadOnly (controlCollection, "error");
      Assert.That (controlCollection.IsReadOnly, Is.True);

      string exceptionMessage = _memberCaller.SetCollectionReadOnly (controlCollection, null);

      Assert.That (exceptionMessage, Is.EqualTo ("error"));
      Assert.That (controlCollection.IsReadOnly, Is.False);
    }

    [Test]
    public void SaveAllState ()
    {
      _parent.ValueInViewState = "ParentValue";
      _child.ValueInViewState = "ChildValue";
      _page.RegisterViewStateHandler();

      _memberCaller.SaveAllState (_page);
      var viewState = _page.GetPageStatePersister().ViewState;

      Assert.That (viewState, Is.InstanceOf (typeof (Pair)));
      var pageViewState = (Pair) viewState;
      var namingContainerViewState = pageViewState.Second;
      Assert.That (namingContainerViewState, Is.InstanceOf (typeof (Pair)));
    }

    [Test]
    public void SetControlState_CanInvoke ()
    {
      _memberCaller.SetControlState (_parent, ControlState.Initialized);
    }

    [Test]
    public void GetControlState_CanInvoke ()
    {
      _memberCaller.GetControlState (_parent);
    }
    
    [Test]
    public void InitRecursive_CanInvoke ()
    {
      _memberCaller.InitRecursive (_child, _namingContainer);
    }

    [Test]
    public void LoadViewStateRecursive_CanInvoke ()
    {
      _memberCaller.LoadViewStateRecursive (_parent, null);
    }

    [Test]
    public void SaveViewStateRecursive_CanInvoke ()
    {
      _memberCaller.SaveViewStateRecursive (_parent);
    }

    [Test]
    public void SaveAllState_CanInvoke ()
    {
      _memberCaller.SaveAllState (_page);
    }

    [Test]
    public void ClearChildControlState_CanInvoke ()
    {
      _memberCaller.ClearChildControlState (_namingContainer);
    }

    [Test]
    public void GetPageStatePersister_CanInvoke ()
    {
      _memberCaller.GetPageStatePersister (_page);
    }

    [Test]
    public void SetCollectionReadOnly_CanInvoke ()
    {
      _memberCaller.SetCollectionReadOnly (_parent.Controls, "Message");
    }

    [Test]
    public void RenderChildrenInternal_CanInvoke ()
    {
      _memberCaller.RenderChildrenInternal (_parent, new HtmlTextWriter(new StringWriter()), _parent.Controls);
    }
  }
}
