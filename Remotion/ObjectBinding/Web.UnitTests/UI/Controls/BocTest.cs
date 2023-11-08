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
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.Development.Web.UnitTesting.UI;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{

public class BocTest
{
  private Page _page;
  private NamingContainerMock _namingContainer;
  private ControlInvoker _invoker;

  public BocTest ()
  {
  }

  [SetUp]
  public virtual void SetUp ()
  {

    _page = new Page();

    _namingContainer = new NamingContainerMock();
    _namingContainer.ID = "NamingContainer";
    _page.Controls.Add(_namingContainer);

    _invoker = new ControlInvoker(_namingContainer);

    var context = HttpContextHelper.CreateHttpContext("GET", "/", "");
    HttpContextHelper.SetCurrent(context);
    HttpBrowserCapabilities browser = new HttpBrowserCapabilities();
    browser.Capabilities = new Hashtable();
    browser.Capabilities.Add("browser", "IE");
    browser.Capabilities.Add("majorversion", "7");
    context.Request.Browser = browser;
  }

  [TearDown]
  public virtual void TearDown ()
  {
    HttpContextHelper.SetCurrent(null);
    WebConfigurationMock.Current = null;
  }

  public Page Page
  {
    get { return _page; }
  }

  public NamingContainerMock NamingContainer
  {
    get { return _namingContainer; }
  }

  public ControlInvoker Invoker
  {
    get { return _invoker; }
  }
}

}
