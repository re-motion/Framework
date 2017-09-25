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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TextBoxControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (GeneralTestCaseFactory<TextBoxSelector, TextBoxControlObject>))]
    public void GenericTests (GenericSelectorTestSetupAction<TextBoxSelector, TextBoxControlObject> testSetupAction)
    {
      testSetupAction (Helper, e => e.TextBoxes(), "textBox");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<TextBoxSelector, TextBoxControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<TextBoxSelector, TextBoxControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<TextBoxSelector, TextBoxControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<TextBoxSelector, TextBoxControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<TextBoxSelector, TextBoxControlObject>))]
    public void TestControlSelectors (GenericSelectorTestSetupAction<TextBoxSelector, TextBoxControlObject> testSetupAction)
    {
      testSetupAction (Helper, e => e.TextBoxes(), "textBox");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.TextBoxes().GetByLocalID ("TextBoxDisabled");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.InstanceOf<MissingHtmlException>());
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var editableTextBox = home.TextBoxes().GetByID ("body_MyEditableTextBox");
      Assert.That (editableTextBox.Scope.Id, Is.EqualTo ("body_MyEditableTextBox"));

      var aspTextBox = home.TextBoxes().GetByID ("body_MyAspTextBox");
      Assert.That (aspTextBox.Scope.Id, Is.EqualTo ("body_MyAspTextBox"));

      var htmlTextBox = home.TextBoxes().GetByID ("body_MyHtmlTextBox");
      Assert.That (htmlTextBox.Scope.Id, Is.EqualTo ("body_MyHtmlTextBox"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var editableTextBox = home.TextBoxes().GetByLocalID ("MyEditableTextBox");
      Assert.That (editableTextBox.Scope.Id, Is.EqualTo ("body_MyEditableTextBox"));

      var aspTextBox = home.TextBoxes().GetByLocalID ("MyAspTextBox");
      Assert.That (aspTextBox.Scope.Id, Is.EqualTo ("body_MyAspTextBox"));

      var htmlTextBox = home.TextBoxes().GetByLocalID ("MyHtmlTextBox");
      Assert.That (htmlTextBox.Scope.Id, Is.EqualTo ("body_MyHtmlTextBox"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var editableTextBox = home.TextBoxes().GetByLocalID ("MyEditableTextBox");
      Assert.That (editableTextBox.GetText(), Is.EqualTo ("MyEditableTextBoxValue"));

      var aspTextBox = home.TextBoxes().GetByLocalID ("MyAspTextBox");
      Assert.That (aspTextBox.GetText(), Is.EqualTo ("MyAspTextBoxValue"));

      var htmlTextBox = home.TextBoxes().GetByLocalID ("MyHtmlTextBox");
      Assert.That (htmlTextBox.GetText(), Is.EqualTo ("MyHtmlTextBoxValue"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      // Check WaitFor.Nothing once before default behavior usage...

      var aspTextBoxNoAutoPostback = home.TextBoxes().GetByLocalID ("MyAspTextBoxNoAutoPostBack");
      aspTextBoxNoAutoPostback.FillWith ("Blubba4", Opt.ContinueImmediately());
      Assert.That (aspTextBoxNoAutoPostback.GetText(), Is.EqualTo ("Blubba4"));

      var editableTextBox = home.TextBoxes().GetByLocalID ("MyEditableTextBox");
      editableTextBox.FillWith ("Blubba1");
      Assert.That (editableTextBox.GetText(), Is.EqualTo ("Blubba1"));

      // ...and once afterwards

      var aspTextBox = home.TextBoxes().GetByLocalID ("MyAspTextBox");
      aspTextBox.FillWith ("Blubba2");
      Assert.That (aspTextBox.GetText(), Is.EqualTo ("Blubba2"));

      var htmlTextBox = home.TextBoxes().GetByLocalID ("MyHtmlTextBox");
      htmlTextBox.FillWith ("Blubba3", Opt.ContinueImmediately());
      Assert.That (htmlTextBox.GetText(), Is.EqualTo ("Blubba3"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("TextBoxTest.wxe");
    }
  }
}