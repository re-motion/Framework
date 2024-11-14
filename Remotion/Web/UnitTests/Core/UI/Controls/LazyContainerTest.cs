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
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{

  [TestFixture]
  public class LazyContainerTest : WebControlTest
  {
    // types

    // static members and constants

    // member fields

    private HttpContext _currentHttpContext;

    private StringCollection _actualEvents;

    private LazyContainer _lazyContainer;
    private ControlInvoker _lazyContainerInvoker;

    private ControlMock _parent;
    private ControlInvoker _parentInvoker;
    private ControlMock _child;
    private ControlMock _childSecond;
    private ControlInvoker _childInvoker;
    private ControlInvoker _childSecondInvoker;

    // construction and disposing

    public LazyContainerTest ()
    {
    }

    // methods and properties

    protected override void SetUpContext ()
    {
      base.SetUpContext();

      _currentHttpContext = HttpContextHelper.CreateHttpContext("GET", "default.html", null);
      HttpContextHelper.SetCurrent(_currentHttpContext);
    }

    protected override void SetUpPage ()
    {
      base.SetUpPage();

      _actualEvents = new StringCollection();

      _lazyContainer = new LazyContainer();
      _lazyContainer.ID = "LazyContainer";
      _lazyContainer.Init += new EventHandler(LazyContainer_Init);
      _lazyContainer.Load += new EventHandler(LazyContainer_Load);
      NamingContainer.Controls.Add(_lazyContainer);

      _lazyContainerInvoker = new ControlInvoker(_lazyContainer);

      _parent = new ControlMock();
      _parent.ID = "Parent";
      _parent.Init += new EventHandler(Parent_Init);
      _parent.Load += new EventHandler(Parent_Load);
      _parentInvoker = new ControlInvoker(_parent);

      _child = new ControlMock();
      _child.ID = "Child";
      _child.Init += new EventHandler(Child_Init);
      _child.Load += new EventHandler(Child_Load);
      _childInvoker = new ControlInvoker(_child);
      _parent.Controls.Add(_child);

      _childSecond = new ControlMock();
      _childSecond.ID = "ChildSecond";
      _childSecond.Init += new EventHandler(ChildSecond_Init);
      _childSecond.Load += new EventHandler(ChildSecond_Load);
      _childSecondInvoker = new ControlInvoker(_childSecond);
      _parent.Controls.Add(_childSecond);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_lazyContainer.Controls is EmptyControlCollection, Is.True);

      Assert.That(_lazyContainer.RealControls, Is.Not.Null);
      Assert.That(_lazyContainer.RealControls is EmptyControlCollection, Is.False);
    }

    [Test]
    public void Ensure ()
    {
      Assert.That(_lazyContainer.Controls is EmptyControlCollection, Is.True);

      _lazyContainer.Ensure();

      Assert.That(_lazyContainer.Controls, Is.Not.Null);
      Assert.That(_lazyContainer.Controls is EmptyControlCollection, Is.False);
    }


    [Test]
    public void Control_Add_Init_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatInitEvent(_lazyContainer));

      _lazyContainer.RealControls.Add(_parent);
      NamingContainerInvoker.InitRecursive();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));

      expectedEvents.Add(FormatInitEvent(_child));
      expectedEvents.Add(FormatInitEvent(_childSecond));
      expectedEvents.Add(FormatInitEvent(_parent));

      _lazyContainer.Ensure();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));
    }

    [Test]
    public void Control_Init_Ensure_Add ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatInitEvent(_lazyContainer));

      NamingContainerInvoker.InitRecursive();
      _lazyContainer.Ensure();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));

      expectedEvents.Add(FormatInitEvent(_child));
      expectedEvents.Add(FormatInitEvent(_childSecond));
      expectedEvents.Add(FormatInitEvent(_parent));

      _lazyContainer.RealControls.Add(_parent);

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));
    }


    [Test]
    public void Control_Add_Init_Load_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatInitEvent(_lazyContainer));
      expectedEvents.Add(FormatLoadEvent(_lazyContainer));

      _lazyContainer.RealControls.Add(_parent);
      NamingContainerInvoker.InitRecursive();
      NamingContainerInvoker.LoadRecursive();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));

      expectedEvents.Add(FormatInitEvent(_child));
      expectedEvents.Add(FormatInitEvent(_childSecond));
      expectedEvents.Add(FormatInitEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_child));
      expectedEvents.Add(FormatLoadEvent(_childSecond));

      _lazyContainer.Ensure();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));
    }

    [Test]
    public void Control_Init_Add_Load_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatInitEvent(_lazyContainer));
      expectedEvents.Add(FormatLoadEvent(_lazyContainer));

      NamingContainerInvoker.InitRecursive();
      _lazyContainer.RealControls.Add(_parent);
      NamingContainerInvoker.LoadRecursive();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));

      expectedEvents.Add(FormatInitEvent(_child));
      expectedEvents.Add(FormatInitEvent(_childSecond));
      expectedEvents.Add(FormatInitEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_child));
      expectedEvents.Add(FormatLoadEvent(_childSecond));

      _lazyContainer.Ensure();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));
    }

    [Test]
    public void Control_Init_Load_Add_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatInitEvent(_lazyContainer));
      expectedEvents.Add(FormatLoadEvent(_lazyContainer));

      NamingContainerInvoker.InitRecursive();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));

      expectedEvents.Add(FormatInitEvent(_child));
      expectedEvents.Add(FormatInitEvent(_childSecond));
      expectedEvents.Add(FormatInitEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_child));
      expectedEvents.Add(FormatLoadEvent(_childSecond));

      _lazyContainer.Ensure();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));
    }

    [Test]
    public void Control_Init_Load_Ensure_Add ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add(FormatInitEvent(_lazyContainer));
      expectedEvents.Add(FormatLoadEvent(_lazyContainer));

      NamingContainerInvoker.InitRecursive();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));

      expectedEvents.Add(FormatInitEvent(_child));
      expectedEvents.Add(FormatInitEvent(_childSecond));
      expectedEvents.Add(FormatInitEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_parent));
      expectedEvents.Add(FormatLoadEvent(_child));
      expectedEvents.Add(FormatLoadEvent(_childSecond));

      _lazyContainer.Ensure();

      Assert.That(_actualEvents, Is.EqualTo(expectedEvents));
    }

    [Test]
    public void Control_Init_Load_Add_SaveViewState ()
    {
      NamingContainerInvoker.InitRecursive();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);

      _parent.ValueInViewState = "Parent Value";
      _child.ValueInViewState = "Child Value";

      object viewState = _lazyContainerInvoker.SaveViewState();

      Assert.That(viewState, Is.Not.Null);
      Assert.That(viewState is Pair, Is.True);
      Pair values = (Pair)viewState;
      Assert.That(values.Second, Is.Null);
    }

    [Test]
    public void Control_Init_LoadViewState ()
    {
      NamingContainerInvoker.InitRecursive();

      _lazyContainerInvoker.LoadViewState(new Pair(null, null));
    }

    [Test]
    public void Control_Init_LoadViewStateWithNull ()
    {
      NamingContainerInvoker.InitRecursive();

      _lazyContainerInvoker.LoadViewState(null);
    }

    [Test]
    public void Control_Init_Load_Add_Ensure_SaveViewStateRecursive ()
    {
      NamingContainerInvoker.InitRecursive();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);
      _lazyContainer.Ensure();

      _parent.ValueInViewState = "Parent Value";
      _child.ValueInViewState = "Child Value";

      object viewState = _lazyContainerInvoker.SaveViewState();

      Assert.That(viewState, Is.Not.Null);
      Assert.That(viewState is Pair, Is.True);
      Pair values = (Pair)viewState;
      Assert.That(values.Second, Is.Not.Null);
    }


    [Test]
    public void Control_PostBack_Init_Add_Ensure ()
    {
      Page.SetRequestValueCollection(new NameValueCollection());
      NamingContainerInvoker.InitRecursive();
      _lazyContainer.RealControls.Add(_parent);
      Assert.That(
          () => _lazyContainer.Ensure(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot ensure LazyContainer 'LazyContainer' before its state has been loaded."));
    }

    [Test]
    public void Control_Init_Add_LoadAllState_Ensure ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister();

      TearDownPage();
      SetUpPage();

      Page.SetRequestValueCollection(new NameValueCollection());
      NamingContainerInvoker.InitRecursive();
      Page.SetPageStatePersister(pageStatePersisterBackup);
      Page.LoadAllState();
      _lazyContainer.RealControls.Add(_parent);

      Assert.That(_parent.ValueInControlState, Is.Null);
      Assert.That(_child.ValueInControlState, Is.Null);
      Assert.That(_childSecond.ValueInControlState, Is.Null);

      _lazyContainer.Ensure();

      Assert.That(_parent.ValueInControlState, Is.EqualTo("Parent Value"));
      Assert.That(_child.ValueInControlState, Is.EqualTo("Child Value"));
      Assert.That(_childSecond.ValueInControlState, Is.Null);
    }

    [Test]
    public void Control_BackUpChildControlState ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister();

      Dictionary<string, object> expectedControlStates = new Dictionary<string, object>();
      expectedControlStates[_parent.UniqueID] = _parentInvoker.SaveControlStateInternal();
      expectedControlStates[_child.UniqueID] = _childInvoker.SaveControlStateInternal();

      TearDownPage();
      SetUpPage();

      Page.SetRequestValueCollection(new NameValueCollection());
      NamingContainerInvoker.InitRecursive();
      Page.SetPageStatePersister(pageStatePersisterBackup);
      Page.LoadAllState();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);

      object controlState = _lazyContainerInvoker.SaveControlState();

      Assert.That(controlState is Triplet, Is.True);
      Triplet values = (Triplet)controlState;
      Assert.That(values.Third, Is.InstanceOf((typeof(HybridDictionary))));
      IDictionary actualControlStates = (IDictionary)values.Third;
      Assert.That(actualControlStates.Count, Is.EqualTo(2));

      foreach (string expectedKey in expectedControlStates.Keys)
      {
        Pair expectedValues = (Pair)expectedControlStates[expectedKey];

        object actualControlState = actualControlStates[expectedKey];
        Assert.That(actualControlState is Pair, Is.True);
        Pair actualValues = (Pair)actualControlState;
        Assert.That(actualValues.First, Is.EqualTo(expectedValues.First), expectedKey);
        Assert.That(actualValues.Second, Is.EqualTo(expectedValues.Second), expectedKey);
      }
    }

    [Test]
    public void Control_RestoreChildControlState_EnsureAfterLoadAllState ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister();

      TearDownPage();
      SetUpPage();

      Page.SetRequestValueCollection(new NameValueCollection());
      Page.RegisterViewStateHandler();
      NamingContainerInvoker.InitRecursive();
      Page.SetPageStatePersister(pageStatePersisterBackup);
      Page.LoadAllState();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);
      Page.SaveAllState();

      pageStatePersisterBackup = Page.GetPageStatePersister();

      Assert.That(pageStatePersisterBackup.ControlState is IDictionary, Is.True);
      IDictionary controlStates = (IDictionary)pageStatePersisterBackup.ControlState;
      Assert.That(controlStates.Count, Is.EqualTo(1));
      Assert.That(controlStates.Contains(_lazyContainer.UniqueID), Is.True);

      TearDownPage();
      SetUpPage();

      Page.SetRequestValueCollection(new NameValueCollection());
      Page.RegisterViewStateHandler();
      NamingContainerInvoker.InitRecursive();
      Page.SetPageStatePersister(pageStatePersisterBackup);
      Page.LoadAllState();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);

      Assert.That(_parent.ValueInControlState, Is.Null);
      Assert.That(_child.ValueInControlState, Is.Null);
      Assert.That(_childSecond.ValueInControlState, Is.Null);

      _lazyContainer.Ensure();

      Assert.That(_parent.ValueInControlState, Is.EqualTo("Parent Value"));
      Assert.That(_child.ValueInControlState, Is.EqualTo("Child Value"));
      Assert.That(_childSecond.ValueInControlState, Is.Null);
    }

    [Test]
    public void Control_RestoreChildControlState_EnsureBeforeLoadAllState ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister();

      TearDownPage();
      SetUpPage();

      Page.RegisterViewStateHandler();
      Page.SetRequestValueCollection(new NameValueCollection());
      NamingContainerInvoker.InitRecursive();
      Page.SetPageStatePersister(pageStatePersisterBackup);
      Page.LoadAllState();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);
      Page.SaveAllState();

      pageStatePersisterBackup = Page.GetPageStatePersister();

      Assert.That(pageStatePersisterBackup.ControlState is IDictionary, Is.True);
      IDictionary controlStates = (IDictionary)pageStatePersisterBackup.ControlState;
      Assert.That(controlStates.Count, Is.EqualTo(1));
      Assert.That(controlStates.Contains(_lazyContainer.UniqueID), Is.True);

      TearDownPage();
      SetUpPage();

      Page.SetRequestValueCollection(new NameValueCollection());
      Assert.That(
          () => _lazyContainer.Ensure(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot ensure LazyContainer 'LazyContainer' before its state has been loaded."));
    }

    private void Page_Init_Load_Add_Ensure_SaveAllState (string parentControlState, string childControlState, string childSecondControlState)
    {
      Page.RegisterViewStateHandler();
      NamingContainerInvoker.InitRecursive();
      NamingContainerInvoker.LoadRecursive();
      _lazyContainer.RealControls.Add(_parent);
      _lazyContainer.Ensure();

      _parent.ValueInControlState = parentControlState;
      _child.ValueInControlState = childControlState;
      _childSecond.ValueInControlState = childSecondControlState;

      Page.SaveAllState();
    }

    private void LazyContainer_Init (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatInitEvent(_lazyContainer));
    }

    private void LazyContainer_Load (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatLoadEvent(_lazyContainer));
    }

    private void Parent_Init (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatInitEvent(_parent));
    }

    private void Parent_Load (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatLoadEvent(_parent));
    }

    private void Child_Load (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatLoadEvent(_child));
    }

    private void Child_Init (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatInitEvent(_child));
    }

    private void ChildSecond_Load (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatLoadEvent(_childSecond));
    }

    private void ChildSecond_Init (object sender, EventArgs e)
    {
      _actualEvents.Add(FormatInitEvent(_childSecond));
    }


    private string FormatInitEvent (Control sender)
    {
      return FormatEvent(sender, "Init");
    }

    private string FormatLoadEvent (Control sender)
    {
      return FormatEvent(sender, "Load");
    }

    private string FormatEvent (Control sender, string eventName)
    {
      ArgumentUtility.CheckNotNull("sender", sender);
      ArgumentUtility.CheckNotNullOrEmpty("eventName", eventName);

      return sender.ID + " " + eventName;
    }
  }

}
