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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ScopeControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<ScopeSelector, ScopeControlObject>))]
    [TestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<ScopeSelector, ScopeControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<ScopeSelector, ScopeControlObject> testAction)
    {
      testAction (Helper, e => e.Scopes(), "scope");
    }

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Does.Contain ("DoNotFindMe"));

      var scope = home.Scopes().GetByID ("body_MyScope");
      Assert.That (scope.GetHtmlID(), Is.EqualTo ("body_MyScope"));
      Assert.That (scope.Scope.Text, Does.Contain ("Content"));
      Assert.That (scope.Scope.Text, Does.Not.Contains ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Does.Contain ("DoNotFindMe"));

      var scope = home.Scopes().GetByLocalID ("MyScope");
      Assert.That (scope.GetHtmlID(), Is.EqualTo ("body_MyScope"));
      Assert.That (scope.Scope.Text, Does.Contain ("Content"));
      Assert.That (scope.Scope.Text, Does.Not.Contains ("DoNotFindMe"));
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("ScopeTest.aspx");
    }
  }
}