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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation
{
  /// <summary>
  /// Implements the <see cref="IBocReferenceValueValidatorFactory"/> inteface and creates all validators required to ensure a valid property value (i.e. nullability and formatting).
  /// </summary>
  /// <seealso cref="IBocReferenceValueValidatorFactory"/>
  [ImplementationFor (typeof (IBocReferenceValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocReferenceValueValidatorFactory: IBocReferenceValueValidatorFactory
  {
    public const int Position = 0;

    public BocReferenceValueValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocReferenceValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly || !control.IsRequired)
        yield break;

      var resourceManager = control.GetResourceManager ();
      yield return CreateRequiredFieldValidator (control, resourceManager);
    }

    private RequiredFieldValidator CreateRequiredFieldValidator (IBocReferenceValue control, IResourceManager resourceManage)
    {
      var requiredFieldValidator = new RequiredFieldValidator ();
      requiredFieldValidator.ID = control.ID + "_ValidatorNotNullItem";
      requiredFieldValidator.ControlToValidate = control.ID;
      requiredFieldValidator.ErrorMessage = resourceManage.GetString (BocReferenceValue.ResourceIdentifier.NullItemErrorMessage);
      return requiredFieldValidator;
    }
  }
}