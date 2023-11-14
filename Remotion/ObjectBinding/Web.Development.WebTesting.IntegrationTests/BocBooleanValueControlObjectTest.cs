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
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
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
  public class BocBooleanValueControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(ReadOnlyTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(LabelTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(ValidationErrorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocBooleanValueSelector, BocBooleanValueControlObject> testAction)
    {
      testAction(Helper, e => e.BooleanValues(), "booleanValue");
    }

    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(DomainPropertyControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [TestCaseSource(typeof(DisplayNameControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocBooleanValueSelector, BocBooleanValueControlObject> testAction)
    {
      testAction(Helper, e => e.BooleanValues(), "booleanValue");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.BooleanValues().GetByLocalID("DeceasedField_Disabled");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(
          () => control.SetTo(false),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "SetTo").Message));
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.BooleanValues().GetByLocalID("DeceasedField_ReadOnly");

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(() => control.SetTo(false), Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlReadOnlyException(Driver).Message));
    }

    [Test]
    public void TestGetState ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_Normal");
      Assert.That(bocBooleanValue.GetState(), Is.EqualTo(false));

      bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_ReadOnly");
      Assert.That(bocBooleanValue.GetState(), Is.EqualTo(false));

      bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_Disabled");
      Assert.That(bocBooleanValue.GetState(), Is.EqualTo(false));

      bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_NoAutoPostBack");
      Assert.That(bocBooleanValue.GetState(), Is.EqualTo(false));

      bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_TriState");
      Assert.That(bocBooleanValue.GetState(), Is.EqualTo(false));
    }

    [Test]
    public void TestIsTriState ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_Normal");
      Assert.That(bocBooleanValue.IsTriState(), Is.EqualTo(false));

      bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_TriState");
      Assert.That(bocBooleanValue.IsTriState(), Is.EqualTo(true));
    }

    [Test]
    public void TestSetTo_WithRequiredValue ()
    {
      var home = Start();

      {
        var normalBocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(normalBocBooleanValue);
        normalBocBooleanValue.SetTo(true);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text, Is.EqualTo("True"));
      }

      {
        var noAutoPostBackBocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_NoAutoPostBack");
        var completionDetection = new CompletionDetectionStrategyTestHelper(noAutoPostBackBocBooleanValue);
        noAutoPostBackBocBooleanValue.SetTo(true);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<NullCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("False"));
      }

      {
        var normalBocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(normalBocBooleanValue);
        normalBocBooleanValue.SetTo(true, Opt.ContinueImmediately()); // same value, does not trigger post back
        Assert.That(completionDetection.GetAndReset(), Is.Null);
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("False"));
      }

      {
        var normalBocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_Normal");
        var completionDetection = new CompletionDetectionStrategyTestHelper(normalBocBooleanValue);
        normalBocBooleanValue.SetTo(false);
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindIdEndingWith("NormalCurrentValueLabel").Text, Is.EqualTo("False"));
        Assert.That(home.Scope.FindIdEndingWith("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo("True"));
      }
    }

    [Test]
    public void TestSetTo_WithNullableValue ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_TriState");

      bocBooleanValue.SetTo(null);
      Assert.That(home.Scope.FindIdEndingWith("TriStateCurrentValueLabel").Text, Is.Empty);

      bocBooleanValue.SetTo(false);
      Assert.That(home.Scope.FindIdEndingWith("TriStateCurrentValueLabel").Text, Is.EqualTo("False"));

      bocBooleanValue.SetTo(true);
      Assert.That(home.Scope.FindIdEndingWith("TriStateCurrentValueLabel").Text, Is.EqualTo("True"));
    }

    [Test]
    public void TestSetTo_True_WithRequiredValueAndUnitialized ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_NormalAndUnitialized");

      bocBooleanValue.SetTo(true);
      Assert.That(home.Scope.FindIdEndingWith("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo("True"));

      bocBooleanValue.SetTo(false);
      Assert.That(home.Scope.FindIdEndingWith("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo("False"));

      bocBooleanValue.SetTo(true);
      Assert.That(home.Scope.FindIdEndingWith("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo("True"));
    }

    [Test]
    public void TestSetTo_False_WithRequiredValueAndUnitialized ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID("DeceasedField_NormalAndUnitialized");

      bocBooleanValue.SetTo(false);
      Assert.That(home.Scope.FindIdEndingWith("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo("False"));

      bocBooleanValue.SetTo(true);
      Assert.That(home.Scope.FindIdEndingWith("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo("True"));

      bocBooleanValue.SetTo(false);
      Assert.That(home.Scope.FindIdEndingWith("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo("False"));
    }

    [Test]
    public void TestSelectOptionWithUmlaut ()
    {
      var home = Start();

      Assert.That(home.BooleanValues().GetByDisplayName("IstVolljährig"), Is.Not.Null);
    }

    private WxePageObject Start ()
    {
      return Start("BocBooleanValue");
    }
  }
}
