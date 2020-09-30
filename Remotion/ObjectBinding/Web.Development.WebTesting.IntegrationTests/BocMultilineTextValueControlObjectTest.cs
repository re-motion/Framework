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

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocMultilineTextValueControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource (typeof (DisabledTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (ReadOnlyTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (LabelTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (ValidationErrorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocMultilineTextValueSelector, BocMultilineTextValueControlObject> testAction)
    {
      testAction (Helper, e => e.MultilineTextValues(), "multilineText");
    }

    [Test]
    [TestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    [TestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocMultilineTextValueSelector, BocMultilineTextValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocMultilineTextValueSelector, BocMultilineTextValueControlObject> testAction)
    {
      testAction (Helper, e => e.MultilineTextValues(), "multilineText");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.MultilineTextValues().GetByLocalID ("CVField_Disabled");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("FillWith").Message));
      Assert.That (
          () => control.FillWith ("text", FinishInput.Promptly),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("FillWith").Message));
      Assert.That (
          () => control.FillWith (new[] { "text" }),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("FillWith").Message));
      Assert.That (
          () => control.FillWith (new[] { "text" }, FinishInput.Promptly),
          Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("FillWith").Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.MultilineTextValues().GetByLocalID ("CVField_ReadOnly");

      Assert.That (control.IsReadOnly(), Is.True);
      Assert.That (() => control.FillWith ("text"), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
      Assert.That (() => control.FillWith ("text", FinishInput.Promptly), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
      Assert.That (() => control.FillWith (new[] { "text" }), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
      Assert.That (() => control.FillWith (new[] { "text" }, FinishInput.Promptly), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlReadOnlyException().Message));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var shouldBeText = "<Test 1> " + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3";
      //TODO: Remove after implementation of RM-7062
      var shouldBeTextWithTrim = "<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3";

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo (shouldBeText));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_ReadOnly");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo (shouldBeTextWithTrim));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Disabled");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo (shouldBeText));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_NoAutoPostBack");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo (shouldBeText));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      {
        var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper (bocMultilineText);
        bocMultilineText.FillWith ("Blubba");
        Assert.That (completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Blubba"));
      }

      {
        var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper (bocMultilineText);
        bocMultilineText.FillWith ("Blubba"); // no auto post back
        Assert.That (completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("<Test 1> NL Test 2 NL Test 3"));
      }

      {
        var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper (bocMultilineText);
        bocMultilineText.FillWith ("Blubba", Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That (completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("<Test 1> NL Test 2 NL Test 3"));
      }

      {
        var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper (bocMultilineText);
        bocMultilineText.FillWith ("Doe" + Environment.NewLine + "SecondLineDoe");
        Assert.That (completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Doe NL SecondLineDoe"));
        Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Blubba"));
      }
    }

    [Test]
    public void TestFillWithLineFeed ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("Doe" + "\n" + "SecondLineDoe");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Doe NL SecondLineDoe"));

      bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("DoeAgain" + "\r\n" + "SecondLineDoeAgain");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("DoeAgain NL SecondLineDoeAgain"));
    }

    [Test]
    public void TestFillWithMixedKeysAndCharsThrowsException ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");

      Assert.That (
          () => bocMultilineText.FillWith ("a" + Keys.Enter + "a"),
          Throws.ArgumentException.With.Message.EqualTo ("Value may not contain both text and keys at the same time.\r\nParameter name: value"));
    }

    [Test]
    public void TestFillWithLongText ()
    {
      var home = Start();

      var builder = new StringBuilder();
      foreach (var _ in Enumerable.Range (0, 211))
      {
        // IE sometimes changes the first character of a word to upper case; usually for the last word of a sentence.
        // To prevent intermittent test failures, we are using upper case for the first character of each word.
        builder.Append ("This String Is 29 Chars Long.");
      }

      var input = builder.ToString();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith (input);
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo (input));
      Assert.That (bocMultilineText.GetText(), Is.EqualTo (input));
    }

    [Test]
    public void TestFillWithLines ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith (new[] { "Line1", "Line2", "Line3", "Line4", "Line5" });
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Line1 NL Line2 NL Line3 NL Line4 NL Line5"));
    }

    [Test]
    public void TestFillWithEmpty ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo (""));
    }

    [Test]
    public void TestFillWithClearDoesNotTriggerPostback ()
    {
      var home = Start();

      var bocMultilineText = home.MultilineTextValues().GetByLocalID ("CVField_Normal");
      Assert.That (bocMultilineText.GetText(), Is.Not.Empty); // Make sure there is something to clear
      var postBackCountBeforeFillWith = int.Parse (home.Context.Scope.FindId ("dmaWxePostBackSequenceNumberField").Value);

      bocMultilineText.FillWith ("Blubba");

      var postBackCountAfterFillWith = int.Parse (home.Context.Scope.FindId ("dmaWxePostBackSequenceNumberField").Value);
      Assert.That (postBackCountAfterFillWith, Is.EqualTo (postBackCountBeforeFillWith + 1));
    }

    private WxePageObject Start ()
    {
      return Start ("BocMultilineTextValue");
    }
  }
}