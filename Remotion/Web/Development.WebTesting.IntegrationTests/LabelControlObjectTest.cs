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
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class LabelControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource (typeof(HtmlIDControlSelectorTestCaseFactory<LabelSelector, LabelControlObject>))]
    [TestCaseSource (typeof(LocalIDControlSelectorTestCaseFactory<LabelSelector, LabelControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<LabelSelector, LabelControlObject> testAction)
    {
      testAction(Helper, e => e.Labels(), "label");
    }

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var smartLabel = home.Labels().GetByID("body_MySmartLabel");
      Assert.That(smartLabel.Scope.Id, Is.EqualTo("body_MySmartLabel"));

      var formGridLabel = home.Labels().GetByID("body_MyFormGridLabel");
      Assert.That(formGridLabel.Scope.Id, Is.EqualTo("body_MyFormGridLabel"));

      var aspLabel = home.Labels().GetByID("body_MyAspLabel");
      Assert.That(aspLabel.Scope.Id, Is.EqualTo("body_MyAspLabel"));

      var htmlLabel = home.Labels().GetByID("body_MyHtmlLabel");
      Assert.That(htmlLabel.Scope.Id, Is.EqualTo("body_MyHtmlLabel"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var smartLabel = home.Labels().GetByLocalID("MySmartLabel");
      Assert.That(smartLabel.Scope.Id, Is.EqualTo("body_MySmartLabel"));

      var formGridLabel = home.Labels().GetByLocalID("MyFormGridLabel");
      Assert.That(formGridLabel.Scope.Id, Is.EqualTo("body_MyFormGridLabel"));

      var aspLabel = home.Labels().GetByLocalID("MyAspLabel");
      Assert.That(aspLabel.Scope.Id, Is.EqualTo("body_MyAspLabel"));

      var htmlLabel = home.Labels().GetByLocalID("MyHtmlLabel");
      Assert.That(htmlLabel.Scope.Id, Is.EqualTo("body_MyHtmlLabel"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var smartLabel = home.Labels().GetByLocalID("MySmartLabel");
      Assert.That(smartLabel.GetText(), Is.EqualTo("MySmartLabelContent"));

      var formGridLabel = home.Labels().GetByLocalID("MyFormGridLabel");
      Assert.That(formGridLabel.GetText(), Is.EqualTo("MyFormGridLabelContent"));

      var aspLabel = home.Labels().GetByLocalID("MyAspLabel");
      Assert.That(aspLabel.GetText(), Is.EqualTo("MyAspLabelContent"));

      var htmlLabel = home.Labels().GetByLocalID("MyHtmlLabel");
      Assert.That(htmlLabel.GetText(), Is.EqualTo("MyHtmlLabelContent"));
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject>("LabelTest.aspx");
    }
  }
}