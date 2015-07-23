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
using System.Web.UI;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  public class TestPageHolder
  {
    private readonly PageMock _page;
    private readonly ControlInvoker _pageInvoker;
    private readonly ReplaceableControlMock _namingContainer;
    private readonly ControlMock _parent;
    private readonly ControlMock _child;
    private readonly Control _child2;
    private readonly ControlMock _otherControl;
    private readonly NamingContainerMock _otherNamingContainer;

    public TestPageHolder (bool initializeState, RequestMode requestMode)
    {
      _page = new PageMock ();
      if (requestMode == RequestMode.PostBack)
        _page.SetRequestValueCollection (new NameValueCollection ());

      _otherNamingContainer = new NamingContainerMock ();
      _otherNamingContainer.ID = "OtherNamingContainer";
      _page.Controls.Add (_otherNamingContainer);

      _otherControl = new ControlMock ();
      _otherControl.ID = "OtherControl";
      _otherNamingContainer.Controls.Add (_otherControl);

      _namingContainer = new ReplaceableControlMock();
      _namingContainer.ID = "NamingContainer";
      _page.Controls.Add (_namingContainer);

      _parent = new ControlMock ();
      _parent.ID = "Parent";
      _namingContainer.Controls.Add (_parent);

      _child = new ControlMock ();
      _child.ID = "Child";
      _parent.Controls.Add (_child);

      _child2 = new Control ();
      _child2.ID = "Child2";
      _parent.Controls.Add (_child2);

      _pageInvoker = new ControlInvoker (_page);

      if (initializeState)
      {
        _parent.ValueInViewState = "ParentValue";
        _parent.ValueInControlState = "ParentValue";

        _namingContainer.ValueInViewState = "NamingContainerValue";
        _namingContainer.ValueInControlState = "NamingContainerValue";

        _otherControl.ValueInViewState = "OtherValue";
        _otherControl.ValueInControlState = "OtherValue";
      }

      Page.RegisterViewStateHandler ();
    }

    public PageMock Page
    {
      get { return _page; }
    }

    public ControlInvoker PageInvoker
    {
      get { return _pageInvoker; }
    }

    public ReplaceableControlMock NamingContainer
    {
      get { return _namingContainer; }
    }

    public ControlMock Parent
    {
      get { return _parent; }
    }

    public ControlMock Child
    {
      get { return _child; }
    }

    public Control Child2
    {
      get { return _child2; }
    }

    public ControlMock OtherControl
    {
      get { return _otherControl; }
    }

    public NamingContainerMock OtherNamingContainer
    {
      get { return _otherNamingContainer; }
    }
  }
}
