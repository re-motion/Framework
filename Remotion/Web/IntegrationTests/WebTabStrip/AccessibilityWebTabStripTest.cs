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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests;

namespace Remotion.Web.IntegrationTests.WebTabStrip
{
  [TestFixture]
  public class AccessibilityWebTabStripTest : IntegrationTest
  {
    [Test]
    public void TabStrip ()
    {
      var home = Start();
      var webTabStrip = home.WebTabStrips().GetByLocalID ("MyTabStrip1");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = analyzer.Analyze (webTabStrip);

      Assert.That (result.Violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("WebTabStripTest.wxe");
    }
  }
}