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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories
{
  /// <summary>
  /// Implements various <see cref="IBocValidatorFactory{T}"/> interfaces and creates validators 
  /// that can apply the <see cref="IBusinessObjectValidationResult"/> object to the respective control.
  /// </summary>
  /// <seealso cref="IBocTextValueValidatorFactory"/>
  /// <seealso cref="IBocReferenceValueValidatorFactory"/>
  /// <seealso cref="IBocAutoCompleteReferenceValueValidatorFactory"/>
  /// <seealso cref="IBocBooleanValueValidatorFactory"/>
  /// <seealso cref="IBocCheckBoxValidatorFactory"/>
  /// <seealso cref="IBocDateTimeValueValidatorFactory"/>
  /// <seealso cref="IBocEnumValueValidatorFactory"/>
  /// <seealso cref="IBocMultilineTextValueValidatorFactory"/>
  [ImplementationFor(
      typeof(IBocReferenceValueValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocReferenceValueValidatorFactory)]
  [ImplementationFor(
      typeof(IBocAutoCompleteReferenceValueValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocAutoCompleteReferenceValueValidatorFactory)]
  [ImplementationFor(
      typeof(IBocTextValueValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocTextValueValidatorFactory)]
  [ImplementationFor(
      typeof(IBocBooleanValueValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocBooleanValueValidatorFactory)]
  [ImplementationFor(
      typeof(IBocCheckBoxValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocCheckBoxValidatorFactory)]
  [ImplementationFor(
      typeof(IBocDateTimeValueValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocDateTimeValueValidatorFactory)]
  [ImplementationFor(
      typeof(IBocEnumValueValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocEnumValueValidatorFactory)]
  [ImplementationFor(
      typeof(IBocMultilineTextValueValidatorFactory),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position_BocMultilineTextValueValidatorFactory)]
  public class ValidationBusinessObjectBoundEditableWebControlValidatorFactory
      : IBocTextValueValidatorFactory,
          IBocReferenceValueValidatorFactory,
          IBocAutoCompleteReferenceValueValidatorFactory,
          IBocBooleanValueValidatorFactory,
          IBocCheckBoxValidatorFactory,
          IBocDateTimeValueValidatorFactory,
          IBocEnumValueValidatorFactory,
          IBocMultilineTextValueValidatorFactory
  {
    public const int Position_BocTextValueValidatorFactory = BocTextValueValidatorFactory.Position + 1;
    public const int Position_BocReferenceValueValidatorFactory = BocReferenceValueValidatorFactory.Position + 1;
    public const int Position_BocAutoCompleteReferenceValueValidatorFactory = BocAutoCompleteReferenceValueValidatorFactory.Position + 1;
    public const int Position_BocBooleanValueValidatorFactory = BocBooleanValueValidatorFactory.Position + 1;
    public const int Position_BocCheckBoxValidatorFactory = 0;
    public const int Position_BocDateTimeValueValidatorFactory = BocDateTimeValueValidatorFactory.Position + 1;
    public const int Position_BocEnumValueValidatorFactory = BocEnumValueValidatorFactory.Position + 1;
    public const int Position_BocMultilineTextValueValidatorFactory = BocMultilineTextValueValidatorFactory.Position + 1;

    public ValidationBusinessObjectBoundEditableWebControlValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocTextValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocReferenceValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocAutoCompleteReferenceValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocBooleanValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocCheckBox control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocDateTimeValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocEnumValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocMultilineTextValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return CreateBocValidator(control.ID);
    }

    private IEnumerable<BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator> CreateBocValidator (string? id)
    {
      var bocValidator = new BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator();
      bocValidator.ControlToValidate = id;
      bocValidator.ID = id + "_BocValidator";
      bocValidator.EnableViewState = false;

      yield return bocValidator;
    }
  }
}
