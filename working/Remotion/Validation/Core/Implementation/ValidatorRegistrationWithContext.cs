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
using Remotion.Utilities;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Extends a <see cref="Rules.ValidatorRegistration"/> with information about the property for which it applies.
  /// </summary>
  public class ValidatorRegistrationWithContext
  {
    private readonly ValidatorRegistration _validatorRegistration;
    private readonly IRemovingComponentPropertyRule _removingComponentPropertyRule;

    public ValidatorRegistrationWithContext (
        ValidatorRegistration validatorRegistration,
        IRemovingComponentPropertyRule removingComponentPropertyRule)
    {
      ArgumentUtility.CheckNotNull ("validatorRegistration", validatorRegistration);
      ArgumentUtility.CheckNotNull ("removingComponentPropertyRule", removingComponentPropertyRule);

      _validatorRegistration = validatorRegistration;
      _removingComponentPropertyRule = removingComponentPropertyRule;
    }

    public ValidatorRegistration ValidatorRegistration
    {
      get { return _validatorRegistration; }
    }

    public IRemovingComponentPropertyRule RemovingComponentPropertyRule
    {
      get { return _removingComponentPropertyRule; }
    }
  }
}