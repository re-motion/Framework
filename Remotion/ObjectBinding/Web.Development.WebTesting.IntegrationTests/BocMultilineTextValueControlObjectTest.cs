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
  public class BocMultilineTextValueControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    public void GenericTests (GenericSelectorTestSetupAction<BocMultilineTextValueSelector, BocMultilineTextValueControlObject> testAction)
    {
      testAction (Helper, e => e.MultilineTextValues(), "multilineText");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestSetupAction<BocMultilineTextValueSelector, BocMultilineTextValueControlObject> testAction)
    {
      testAction (Helper, e => e.MultilineTextValues(), "multilineText");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.MultilineTextValues().GetByLocalID ("CVField_Disabled");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith (new[] { "text" }), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith (new[] { "text" }, FinishInput.Promptly), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.MultilineTextValues().GetByLocalID ("CVField_ReadOnly");

      Assert.That (control.IsReadOnly(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith (new[] { "text" }), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith (new[] { "text" }, FinishInput.Promptly), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_ReadOnly");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Disabled");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_NoAutoPostBack");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("Blubba");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Blubba"));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_NoAutoPostBack");
      bocMultilineText.FillWith ("Blubba"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("<Test 1> NL Test 2 NL Test 3"));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("Blubba", Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("<Test 1> NL Test 2 NL Test 3"));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("Doe" + Environment.NewLine + "SecondLineDoe");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Doe NL SecondLineDoe"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Blubba"));
    }

    [Test]
    public void TestFillWithLines ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith (new[] { "Line1", "Line2", "Line3", "Line4", "Line5" });
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Line1 NL Line2 NL Line3 NL Line4 NL Line5"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocMultilineTextValue");
    }
  }
}