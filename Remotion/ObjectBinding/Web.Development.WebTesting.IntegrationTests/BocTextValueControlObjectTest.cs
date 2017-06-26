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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.Factories;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocTextValueControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>), "GetTests")]
    [TestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>), "GetTests")]
    [TestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>), "GetTests")]
    [TestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>), "GetTests")]
    [TestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>), "GetTests")]
    [TestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>), "GetTests")]
    [TestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>), "GetTests")]
    public void TestControlSelectors (TestCaseFactoryBase.TestSetupAction<BocTextValueSelector, BocTextValueControlObject> testAction)
    {
      testAction (Helper, e => e.TextValues(), "textValue");
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocText = home.TextValues().GetByLocalID ("LastNameField_Normal");
      Assert.That (bocText.IsReadOnly(), Is.False);

      bocText = home.TextValues().GetByLocalID ("LastNameField_ReadOnly");
      Assert.That (bocText.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocText = home.TextValues().GetByLocalID ("LastNameField_Normal");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.TextValues().GetByLocalID ("LastNameField_ReadOnly");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.TextValues().GetByLocalID ("LastNameField_Disabled");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.TextValues().GetByLocalID ("LastNameField_NoAutoPostBack");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.TextValues().GetByLocalID ("LastNameField_PasswordNoRender");
      Assert.That (bocText.GetText(), Is.Empty);

      bocText = home.TextValues().GetByLocalID ("LastNameField_PasswordRenderMasked");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      var bocText = home.TextValues().GetByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Blubba");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Blubba"));

      bocText = home.TextValues().GetByLocalID ("LastNameField_NoAutoPostBack");
      bocText.FillWith ("Blubba"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Doe"));

      bocText = home.TextValues().GetByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Blubba", Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Doe"));

      bocText = home.TextValues().GetByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Doe");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Doe"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Blubba"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocTextValue");
    }
  }
}