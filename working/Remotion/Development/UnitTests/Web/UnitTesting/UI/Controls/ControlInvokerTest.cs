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
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;

namespace Remotion.Development.UnitTests.Web.UnitTesting.UI.Controls
{
  [TestFixture]
  public class ControlInvokerTest
  {
    // types

    // static members and constants

    // member fields

    private HttpContext _currentHttpContext;
    
    private PageMock _page;
    private PlaceHolder _parent;
    private Literal _child;

    private PageMock _pageAfterPostBack;
    private PlaceHolder _parentAfterPostBack;
    private Literal _childAfterPostBack;
  
    private ControlInvoker _invoker;
    private ControlInvoker _invokerAfterPostBack;

    private string _events;

    // construction and disposing

    public ControlInvokerTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp()
    {
      _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      HttpContextHelper.SetCurrent (_currentHttpContext);

      _page = new PageMock ();
      _page.SetRequestValueCollection (new NameValueCollection ());
      _currentHttpContext.Handler = _page;

      _parent = new PlaceHolder();
      _parent.ID = "Parent";
      _parent.Init += new EventHandler (Control_Init);
      _parent.Load += new EventHandler (Control_Load);
      _parent.PreRender += new EventHandler (Control_PreRender);
      _page.Controls.Add (_parent);

      _child = new Literal();
      _child.ID = "Child";
      _child.Init += new EventHandler (Control_Init);
      _child.Load += new EventHandler (Control_Load);
      _child.PreRender += new EventHandler (Control_PreRender);

      _parent.Controls.Add (_child);

      _invoker = new ControlInvoker (_parent);


      _pageAfterPostBack = new PageMock ();
      _pageAfterPostBack.SetRequestValueCollection (new NameValueCollection ());
      _currentHttpContext.Handler = _pageAfterPostBack;

      _parentAfterPostBack = new PlaceHolder();
      _parentAfterPostBack.ID = "Parent";
      _parentAfterPostBack.Init += new EventHandler (Control_Init);
      _parentAfterPostBack.Load += new EventHandler (Control_Load);
      _parentAfterPostBack.PreRender += new EventHandler (Control_PreRender);
      _pageAfterPostBack.Controls.Add (_parentAfterPostBack);

      _childAfterPostBack = new Literal();
      _childAfterPostBack.ID = "Child";
      _childAfterPostBack.Init += new EventHandler (Control_Init);
      _childAfterPostBack.Load += new EventHandler (Control_Load);
      _childAfterPostBack.PreRender += new EventHandler (Control_PreRender);

      _parentAfterPostBack.Controls.Add (_childAfterPostBack);

      _invokerAfterPostBack = new ControlInvoker (_parentAfterPostBack);

      _events = string.Empty;
    }

    [TearDown]
    public void TearDown()
    {
      _parent.Init -= new EventHandler (Control_Init);
      _parent.Load -= new EventHandler (Control_Load);
      _parent.PreRender -= new EventHandler (Control_PreRender);
    
      _child.Init -= new EventHandler (Control_Init);
      _child.Load -= new EventHandler (Control_Load);
      _child.PreRender -= new EventHandler (Control_PreRender);

      HttpContextHelper.SetCurrent (null);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_parent, Is.SameAs (_invoker.Control));
    }

    [Test]
    public void InitRecursive ()
    {
      _invoker.InitRecursive ();

      Assert.That (_events, Is.EqualTo ("Child Init, Parent Init"));
    }

    [Test]
    public void TestViewState ()
    {
      _invoker.InitRecursive();
      _invoker.LoadRecursive();
      _child.Text = "Foo Bar";
      _invoker.PreRenderRecursive();

      object viewState = _invoker.SaveViewStateRecursive (ViewStateMode.Enabled);

      _invokerAfterPostBack.InitRecursive();
      Assert.That (_childAfterPostBack.Text, Is.EqualTo (string.Empty));
      _invokerAfterPostBack.LoadViewStateRecursive (viewState);
      Assert.That (_childAfterPostBack.Text, Is.EqualTo ("Foo Bar"));
    }

    [Test]
    public void LoadRecursive ()
    {
      _invoker.LoadRecursive ();

      Assert.That (_events, Is.EqualTo ("Parent Load, Child Load"));
    }

    [Test]
    public void PreRenderRecursive ()
    {
      _invoker.PreRenderRecursive ();

      Assert.That (_events, Is.EqualTo ("Parent PreRender, Child PreRender"));
    }

    private void Control_Init (object sender, EventArgs e)
    {
      _events = AppendEvents ((Control) sender, _events, "Init");
    }

    private void Control_Load (object sender, EventArgs e)
    {
      _events = AppendEvents ((Control) sender, _events, "Load");
    }

    private void Control_PreRender (object sender, EventArgs e)
    {
      _events = AppendEvents ((Control) sender, _events, "PreRender");
    }

    private string AppendEvents (Control control, string events, string eventName)
    {
      events = events ?? string.Empty;
      if (events.Length > 0)
        events += ", ";

      return events + control.ID + " " + eventName;
    }
  }
}
