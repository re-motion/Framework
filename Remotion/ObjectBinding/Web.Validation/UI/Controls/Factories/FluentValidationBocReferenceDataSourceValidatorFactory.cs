﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using FluentValidation.Results;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories
{
  /// <summary>
  /// Implements various <see cref="IBusinessObjectReferenceDataSourceControlValidatorFactory"/> intefaces and creates validators 
  /// that can apply the fluent validation <see cref="ValidationFailure"/> results to the respective control.
  /// </summary>
  /// <seealso cref="IBusinessObjectReferenceDataSourceControlValidatorFactory"/>
  [ImplementationFor (typeof (IBusinessObjectReferenceDataSourceControlValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class FluentValidationBocReferenceDataSourceValidatorFactory : IBusinessObjectReferenceDataSourceControlValidatorFactory
  {
    public const int Position = 0;

    public FluentValidationBocReferenceDataSourceValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (BusinessObjectReferenceDataSourceControl control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      yield return CreateBocReferenceDataSourceValidator (control);
    }

    private BocReferenceDataSourceValidationFailureDisptachingValidator CreateBocReferenceDataSourceValidator (BusinessObjectReferenceDataSourceControl control)
    {
      var bocValidator = new BocReferenceDataSourceValidationFailureDisptachingValidator();
      bocValidator.ControlToValidate = control.ID;
      bocValidator.ID = control.ID + "_BocReferenceDataSourceValidator";
      bocValidator.EnableViewState = false;

      return bocValidator;
    }
  }
}