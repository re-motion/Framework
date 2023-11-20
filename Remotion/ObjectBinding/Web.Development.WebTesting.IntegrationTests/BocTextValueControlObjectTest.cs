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
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocTextValueControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocTextValueSelector, BocTextValueControlObject> testAction)
    {
      testAction(Helper, e => e.TextValues(), "textValue");
    }

    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocTextValueSelector, BocTextValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocTextValueSelector, BocTextValueControlObject> testAction)
    {
      testAction(Helper, e => e.TextValues(), "textValue");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.TextValues().GetByLocalID("LastNameField_Disabled");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.FillWith("text"),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "FillWith").Message));
      Assert.That(
          () => control.FillWith("text", FinishInput.Promptly),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "FillWith").Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.TextValues().GetByLocalID("LastNameField_ReadOnly");

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(() => control.FillWith("text"), Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
      Assert.That(
          () => control.FillWith("text", FinishInput.Promptly),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocText = home.TextValues().GetByLocalID("LastNameField_Normal");
      Assert.That(bocText.GetText(), Is.EqualTo("Doe"));

      bocText = home.TextValues().GetByLocalID("LastNameField_ReadOnly");
      Assert.That(bocText.GetText(), Is.EqualTo("Doe"));

      bocText = home.TextValues().GetByLocalID("LastNameField_Disabled");
      Assert.That(bocText.GetText(), Is.EqualTo("Doe"));

      bocText = home.TextValues().GetByLocalID("LastNameField_NoAutoPostBack");
      Assert.That(bocText.GetText(), Is.EqualTo("Doe"));

      bocText = home.TextValues().GetByLocalID("LastNameField_PasswordNoRender");
      Assert.That(bocText.GetText(), Is.Empty);

      bocText = home.TextValues().GetByLocalID("LastNameField_PasswordRenderMasked");
      Assert.That(bocText.GetText(), Is.EqualTo("Doe"));
    }

    [Test]
    public void TestGetText_WithTrailingWhitespaceBetweenLines ()
    {
      var home = Start();

      var shouldBeText = "<Test 1> " + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3";

      var bocText = home.TextValues().GetByLocalID("CVString_Normal");
      Assert.That(bocText.GetText(), Is.EqualTo(shouldBeText));

      bocText = home.TextValues().GetByLocalID("CVString_ReadOnly");
      Assert.That(bocText.GetText(), Is.EqualTo(shouldBeText));

      bocText = home.TextValues().GetByLocalID("CVString_Disabled");
      Assert.That(bocText.GetText(), Is.EqualTo(shouldBeText));

      bocText = home.TextValues().GetByLocalID("CVString_NoAutoPostBack");
      Assert.That(bocText.GetText(), Is.EqualTo(shouldBeText));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      {
        var bocText = home.TextValues().GetByLocalID("LastNameField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocText);
        bocText.FillWith("Blubba");
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text, Is.EqualTo("Blubba"));
      }

      {
        var bocText = home.TextValues().GetByLocalID("LastNameField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocText);
        bocText.FillWith("Blubba"); // no auto post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("Doe"));
      }

      {
        var bocText = home.TextValues().GetByLocalID("LastNameField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocText);
        bocText.FillWith("Blubba", Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("Doe"));
      }

      {
        var bocText = home.TextValues().GetByLocalID("LastNameField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(bocText);
        bocText.FillWith("Doe");
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text, Is.EqualTo("Doe"));
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("Blubba"));
      }
    }

    [Test]
    public void TestFillWithEmpty ()
    {
      var home = Start();

      var bocText = home.TextValues().GetByLocalID("LastNameField_Normal");
      bocText.FillWith("");
      Assert.That(bocText.GetText(), Is.EqualTo(""));
    }

    [Test]
    public void TestFillWithMixedKeysAndCharsThrowsException ()
    {
      var home = Start();

      var bocMultilineText = home.TextValues().GetByLocalID("LastNameField_Normal");

      Assert.That(
          () => bocMultilineText.FillWith("a" + Keys.Enter + "a"),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Value may not contain both text and keys at the same time.", "value"));
    }

    [Test]
    public void TestFillWithLongText ()
    {
      var home = Start();

      var builder = new StringBuilder();
      foreach (var _ in Enumerable.Range(0, 211))
      {
        // IE sometimes changes the first character of a word to upper case; usually for the last word of a sentence.
        // To prevent intermittent test failures, we are using upper case for the first character of each word.
        builder.Append("This String Is 29 Chars Long.");
      }

      var input = builder.ToString();

      var bocText = home.TextValues().GetByLocalID("UnboundText");
      bocText.FillWith(input);
      Assert.That(home.Scope.FindIdEndingWith("UnboundText_Value").Value, Is.EqualTo(input));
      Assert.That(bocText.GetText(), Is.EqualTo(input));
    }

    [Test]
    public void TestFillWithClearDoesNotTriggerPostback ()
    {
      var home = Start();

      var bocText = home.TextValues().GetByLocalID("LastNameField_Normal");
      Assert.That(bocText.GetText(), Is.Not.Empty); // Make sure there is something to clear
      var postBackCountBeforeFillWith = int.Parse(home.Context.Scope.FindId("dmaWxePostBackSequenceNumberField").Value);

      bocText.FillWith("Blubba");

      var postBackCountAfterFillWith = int.Parse(home.Context.Scope.FindId("dmaWxePostBackSequenceNumberField").Value);
      Assert.That(postBackCountAfterFillWith, Is.EqualTo(postBackCountBeforeFillWith + 1));
    }

    private WxePageObject Start ()
    {
      return Start("BocTextValue");
    }
  }
}
