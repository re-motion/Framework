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
using System.Linq;
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.MetaValidation
{
  /// <summary>
  /// Base class for implementations of the <see cref="IPropertyMetaValidationRule"/> interface which are constrained to a specific <see cref="IPropertyValidator"/> type.
  /// </summary>
  /// <typeparam name="TValidator">The type of the <see cref="IPropertyValidator"/> validated by this meta validator.</typeparam>
  public abstract class PropertyMetaValidationRuleBase<TValidator> : IPropertyMetaValidationRule
      where TValidator: IPropertyValidator
  {
    public abstract IEnumerable<MetaValidationRuleValidationResult> Validate (IEnumerable<TValidator> validationRules);

    IEnumerable<MetaValidationRuleValidationResult> IPropertyMetaValidationRule.Validate (IEnumerable<IPropertyValidator> validationRules)
    {
      ArgumentUtility.CheckNotNull("validationRules", validationRules);

      return Validate(validationRules.OfType<TValidator>());
    }
  }
}
