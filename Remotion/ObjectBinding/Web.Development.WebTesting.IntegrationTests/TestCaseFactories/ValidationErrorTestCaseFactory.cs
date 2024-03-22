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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestPageParameters;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories
{
  /// <summary>
  /// Contains tests for accessing the validation errors of a <see cref="BocControlObject"/> in a form grid.
  /// </summary>
  public class ValidationErrorTestCaseFactory <TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, ValidationErrorTestPageParameters>
      where TControlSelector : IHtmlIDControlSelector<TControl>
      where TControl : BocControlObject, ISupportsValidationErrors, ISupportsValidationErrorsForReadOnly
  {
    protected override string TestPrefix
    {
      get { return "Validation Errors"; }
    }

    [GenericPageTestMethod(PageType = GenericTestPageType.EnabledValidation)]
    public void GetValidationErrors_OneCustomValidator ()
    {
      var control = Selector.GetByID(Parameter.CustomValidatedControlHtmlId);

      ClickValidationButton(control);

      Assert.That(control.GetValidationErrors(), Is.EqualTo(new List<string> { "Always false." }));
    }

    [GenericPageTestMethod(PageType = GenericTestPageType.EnabledValidation)]
    public void GetValidationErrors_MultipleValidators ()
    {
      var control = Selector.GetByID(Parameter.MultipleValidatorsControlHtmlId);

      ClickValidationButton(control);

      Assert.That(control.GetValidationErrors(), Is.EqualTo(new List<string> { "Always false.", "Always false. The second." }));
    }

    [GenericPageTestMethod(PageType = GenericTestPageType.EnabledValidation)]
    public void NotInFormGrid ()
    {
      var control = Selector.GetByID(Parameter.ControlNotInFormGridHtmlId);

      Assert.That(control.GetValidationErrors(), Is.EqualTo(new List<string>()));
    }

    [GenericPageTestMethod(PageType = GenericTestPageType.EnabledValidation)]
    public void NoValidationErrors ()
    {
      var control = Selector.GetByID(Parameter.ControlWithoutValidationHtmlId);

      Assert.That(control.GetValidationErrors(), Is.EqualTo(new List<string>()));
    }

    [GenericPageTestMethod(PageType = GenericTestPageType.EnabledValidation, SearchTimeout = SearchTimeout.UseShortTimeout)]
    public void GetValidationErrorsForReadOnlyControl_Throws ()
    {
      var control = Selector.GetByID(Parameter.ReadOnlyControlHtmlId);

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(
          () => control.GetValidationErrors(),
          Throws.Exception.Message.EqualTo(
              AssertionExceptionUtility.CreateControlReadOnlyException(control.Context.Browser.Driver).Message));
    }

    [GenericPageTestMethod(PageType = GenericTestPageType.EnabledValidation)]
    public void GetValidationErrorsForReadOnly_ReadOnlyControl ()
    {
      var control = Selector.GetByID(Parameter.CustomValidatedReadOnlyControlHtmlId);

      ClickValidationButton(control);

      Assert.That(control.IsReadOnly(), Is.True);
      Assert.That(control.GetValidationErrorsForReadOnly(), Is.EqualTo(new List<string> { "Always false." }));
    }

    [GenericPageTestMethod(PageType = GenericTestPageType.EnabledValidation)]
    public void GetValidationErrorsForReadOnly_EditableControl_Throws ()
    {
      var control = Selector.GetByID(Parameter.ControlWithoutValidationHtmlId);

      Assert.That(control.IsReadOnly(), Is.False);
      Assert.That(
          () => control.GetValidationErrorsForReadOnly(),
          Throws.Exception.Message.EqualTo(
              AssertionExceptionUtility.CreateControlNotReadOnlyException(control.Context.Browser.Driver).Message));
    }

    private void ClickValidationButton (TControl control)
    {
      var validateButtonScope = control.Context.RootScope.FindId(Parameter.ValidateButtonId);
      var validateButton = new WebButtonControlObject(control.Context.CloneForControl(validateButtonScope));
      validateButton.Click(Opt.ContinueWhen(Wxe.PostBackCompleted));
    }
  }
}
