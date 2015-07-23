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
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ScopeControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var scope = home.GetScope().ByID ("body_MyScope");
      Assert.That (scope.GetHtmlID(), Is.EqualTo ("body_MyScope"));
      Assert.That (scope.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (scope.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var scope = home.GetScope().ByLocalID ("MyScope");
      Assert.That (scope.GetHtmlID(), Is.EqualTo ("body_MyScope"));
      Assert.That (scope.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (scope.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("ScopeTest.aspx");
    }
  }
}