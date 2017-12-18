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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocTextValueControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocTextValueSelector, BocTextValueControlObject> testAction)
    {
      testAction (Helper, e => e.TextValues(), "textValue");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocTextValueSelector, BocTextValueControlObject> testAction)
    {
      testAction (Helper, e => e.TextValues(), "textValue");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.TextValues().GetByLocalID ("LastNameField_Disabled");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.TextValues().GetByLocalID ("LastNameField_ReadOnly");

      Assert.That (control.IsReadOnly(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.InstanceOf<MissingHtmlException>());
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