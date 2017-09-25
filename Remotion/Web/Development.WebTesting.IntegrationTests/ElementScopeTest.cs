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
using Coypu;
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ElementScopeTest : IntegrationTest
  {

    [Test]
    public void TestFocus_IsDisabled ()
    {
      var home = Start();

      var disabledButton = home.Scope.FindId ("DisabledButton");

      Assert.That (disabledButton.Disabled, Is.True);
      Assert.That (() => disabledButton.Focus(), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestFocus_IsNotDisabled ()
    {
      var home = Start();

      var normalButton = home.Scope.FindId ("NormalButton");

      Assert.That (normalButton.Disabled, Is.False);
      Assert.That (() => normalButton.Focus(), Throws.Nothing);
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("ElementScopeTest.aspx");
    }
  }
}