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
  public class BocCheckBoxControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocCheckBoxSelector, BocCheckBoxControlObject> testAction)
    {
      testAction(Helper, e => e.CheckBoxes(), "checkBox");
    }

    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocCheckBoxSelector, BocCheckBoxControlObject> testAction)
    {
      testAction(Helper, e => e.CheckBoxes(), "checkBox");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.CheckBoxes().GetByLocalID("DeceasedField_Disabled");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.SetTo(false),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SetTo").Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.CheckBoxes().GetByLocalID("DeceasedField_ReadOnly");

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(() => control.SetTo(false), Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
    }

    [Test]
    public void TestGetState ()
    {
      var home = Start();

      var bocCheckBox = home.CheckBoxes().GetByLocalID("DeceasedField_Normal");
      Assert.That(bocCheckBox.GetState(), Is.EqualTo(false));

      bocCheckBox = home.CheckBoxes().GetByLocalID("DeceasedField_ReadOnly");
      Assert.That(bocCheckBox.GetState(), Is.EqualTo(false));

      bocCheckBox = home.CheckBoxes().GetByLocalID("DeceasedField_Disabled");
      Assert.That(bocCheckBox.GetState(), Is.EqualTo(false));

      bocCheckBox = home.CheckBoxes().GetByLocalID("DeceasedField_NoAutoPostBack");
      Assert.That(bocCheckBox.GetState(), Is.EqualTo(false));
    }

    [Test]
    public void TestSetTo ()
    {
      var home = Start();

      {
        var normalBocBooleanValue = home.CheckBoxes().GetByLocalID("DeceasedField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(normalBocBooleanValue);
        normalBocBooleanValue.SetTo(true);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text, Is.EqualTo("True"));
      }

      {
        var noAutoPostBackBocBooleanValue = home.CheckBoxes().GetByLocalID("DeceasedField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(noAutoPostBackBocBooleanValue);
        noAutoPostBackBocBooleanValue.SetTo(true);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("False"));
      }

      {
        var normalBocBooleanValue = home.CheckBoxes().GetByLocalID("DeceasedField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(normalBocBooleanValue);
        normalBocBooleanValue.SetTo(true, Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.Null);
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("False"));
      }

      {
        var normalBocBooleanValue = home.CheckBoxes().GetByLocalID("DeceasedField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(normalBocBooleanValue);
        normalBocBooleanValue.SetTo(false);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text, Is.EqualTo("False"));
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("True"));
      }
    }

    private WxePageObject Start ()
    {
      return Start("BocCheckBox");
    }
  }
}
