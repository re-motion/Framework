﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class PlatformTest : IntegrationTest
  {
    [Test]
    public void ServerPlatformIsSameAsTestPlatform ()
    {
      var home = Start();

      var currentServerPlatform = home.Labels().GetByID("body_CurrentPlatform");

      var testPlatform = Environment.Is64BitProcess ? "x64" : "x86";

      Assert.That(currentServerPlatform.GetText(), Is.EqualTo(testPlatform));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("PlatformTest.wxe");
    }
  }
}
