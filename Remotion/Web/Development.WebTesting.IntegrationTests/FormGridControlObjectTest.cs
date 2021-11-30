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
  public class FormGridControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [TestCaseSource(typeof(TitleControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<FormGridSelector, FormGridControlObject> testAction)
    {
      testAction(Helper, e => e.FormGrids(), "formGrid");
    }

    [Test]
    public void TestSelectByTitle_WithSingleQuote ()
    {
      var home = Start();

      var formGrid = home.FormGrids().GetByTitle("With'SingleQuote");

      Assert.That(formGrid.GetHtmlID(), Is.EqualTo("body_My3FormGrid"));
    }

    [Test]
    public void TestSelectByTitle_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var formGrid = home.FormGrids().GetByTitle("With'SingleQuoteAndDouble\"Quote");

      Assert.That(formGrid.GetHtmlID(), Is.EqualTo("body_My4FormGrid"));
    }


    [Test]
    public void TestSelectByTitleOrNull_WithSingleQuote ()
    {
      var home = Start();

      var formGrid = home.FormGrids().GetByTitleOrNull("With'SingleQuote");

      Assert.That(formGrid.GetHtmlID(), Is.EqualTo("body_My3FormGrid"));
    }

    [Test]
    public void TestSelectByTitleOrNull_WithSingleQuoteAndDoubleQuote ()
    {
      var home = Start();

      var formGrid = home.FormGrids().GetByTitleOrNull("With'SingleQuoteAndDouble\"Quote");

      Assert.That(formGrid.GetHtmlID(), Is.EqualTo("body_My4FormGrid"));
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject>("FormGridTest.aspx");
    }
  }
}
