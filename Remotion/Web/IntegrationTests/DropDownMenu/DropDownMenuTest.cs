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
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.IntegrationTests.DropDownMenu
{
  [TestFixture]
  public class DropDownMenuTest : IntegrationTest
  {
    [Test]
    public void Open_ShouldNotAppendFragmentHashToUrl ()
    {
      var home = Start();
      var dropDownMenu = home.DropDownMenus().GetByLocalID("MyDropDownMenu");
      dropDownMenu.Open();

      var url = JavaScriptExecutor.ExecuteStatement<string>((IJavaScriptExecutor)Driver.Native, "return window.location.href;");

      Assert.That(url, Does.Not.Contain("#"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("DropDownMenuTest.wxe");
    }
  }
}
