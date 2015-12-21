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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Implements the <see cref="IUserControlBindingValidatorFactory"/> inteface and creates all validators required to ensure a valid property value (i.e. nullability and formatting).
  /// </summary>
  /// <seealso cref="IUserControlBindingValidatorFactory"/>
  [ImplementationFor (typeof (IUserControlBindingValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple,
      Position = Position)]
  public class UserControlBindingValidatorFactory : IUserControlBindingValidatorFactory
  {
    public const int Position = 0;

    public IEnumerable<BaseValidator> CreateValidators (UserControlBinding control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      yield return CreateUserControlBindingValidator (control);
    }

    private BaseValidator CreateUserControlBindingValidator (UserControlBinding control)
    {
      var userControlBindingValidator = new UserControlBindingValidator();
      userControlBindingValidator.ID = control.ID + "_UserControlBindingValidator";
      userControlBindingValidator.ControlToValidate = control.ID;
      return userControlBindingValidator;
    }
  }
}