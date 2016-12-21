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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class ControlHelperTest
  {
    [Test]
    public void GetViewStateFieldPrefixID_AspNetConstant ()
    {
      Assert.That (
          ControlHelper.ViewStateFieldPrefixID,
          Is.EqualTo (PrivateInvoke.GetNonPublicStaticField (typeof (Page), "ViewStateFieldPrefixID")));
    }

    [Test]
    public void GetAsyncPostBackErrorKey_AspNetConstant ()
    {
      var pageRequestManagerType = typeof (ScriptManager).Assembly.GetType ("System.Web.UI.PageRequestManager", true, false);
      Assert.That (
          ControlHelper.AsyncPostBackErrorKey,
          Is.EqualTo (PrivateInvoke.GetNonPublicStaticField (pageRequestManagerType, "AsyncPostBackErrorKey")));
    }

    [Test]
    public void GetAsyncPostBackErrorHttpCodeKey_AspNetConstant ()
    {
      var pageRequestManagerType = typeof (ScriptManager).Assembly.GetType ("System.Web.UI.PageRequestManager", true, false);
      Assert.That (
          ControlHelper.AsyncPostBackErrorHttpCodeKey,
          Is.EqualTo (PrivateInvoke.GetNonPublicStaticField (pageRequestManagerType, "AsyncPostBackErrorHttpCodeKey")));
    }

    [Test]
    public void GetAsyncPostBackErrorMessageKey_AspNetConstant ()
    {
      var pageRequestManagerType = typeof (ScriptManager).Assembly.GetType ("System.Web.UI.PageRequestManager", true, false);
      Assert.That (
          ControlHelper.AsyncPostBackErrorMessageKey,
          Is.EqualTo (PrivateInvoke.GetNonPublicStaticField (pageRequestManagerType, "AsyncPostBackErrorMessageKey")));
    }
  }
}