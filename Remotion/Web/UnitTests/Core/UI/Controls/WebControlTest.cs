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
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{

  public class WebControlTest
  {
    private PageMock _page;
    private NamingContainerMock _namingContainer;
    private ControlInvoker _namingContainerInvoker;

    [SetUp]
    public virtual void SetUp ()
    {
      SetUpContext();
      SetUpPage();
    }

    protected virtual void SetUpContext ()
    {
    }

    protected virtual void SetUpPage ()
    {
      _page = new PageMock();

      _namingContainer = new NamingContainerMock();
      _namingContainer.ID = "NamingContainer";
      _page.Controls.Add(_namingContainer);

      _namingContainerInvoker = new ControlInvoker(_namingContainer);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      TearDownPage();
      TearDownContext();
    }

    protected virtual void TearDownContext ()
    {
      HttpContextHelper.SetCurrent(null);
    }

    protected virtual void TearDownPage ()
    {
    }

    public PageMock Page
    {
      get { return _page; }
    }

    public NamingContainerMock NamingContainer
    {
      get { return _namingContainer; }
    }

    public ControlInvoker NamingContainerInvoker
    {
      get { return _namingContainerInvoker; }
    }
  }

}
