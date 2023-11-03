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
using System.Web;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  public class WxeTest
  {
    private HttpContext _currentHttpContext;
    private WxeContextMock _currentWxeContext;

    public HttpContext CurrentHttpContext
    {
      get { return _currentHttpContext; }
    }

    public WxeContextMock CurrentWxeContext
    {
      get { return _currentWxeContext; }
    }

    [SetUp]
    public virtual void SetUp ()
    {
      _currentHttpContext = HttpContextHelper.CreateHttpContext("GET", "default.html", null);
      HttpContextHelper.SetCurrent(_currentHttpContext);

      _currentWxeContext = new WxeContextMock(_currentHttpContext, new WxeUrlSettings(), new WxeLifetimeManagementSettings());
      WxeContext.SetCurrent(_currentWxeContext);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent(null);
      WxeContext.SetCurrent(null);
    }
  }
}
