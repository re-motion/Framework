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
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Attributes.Validation
{
  /// <summary>
  /// Base class for validation attributes used to substitute the API-based <see cref="ValidationRuleCollectorBase{TValidatedType}"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property)]
  public abstract class AddingValidationAttributeBase : Attribute
  {
    /// <summary>
    /// Gets or sets a flag whether the constraint can be removed by an other component.
    /// </summary>
    public bool IsRemovable { get; set; }

    /// <summary>
    /// Gets or sets the error message displayed when the validation fails.
    /// </summary>
    public string? ErrorMessage { get; set; }

    protected abstract IEnumerable<IPropertyValidator> GetValidators (IPropertyInformation property, IValidationMessageFactory validationMessageFactory);

    public IEnumerable<IPropertyValidator> GetPropertyValidators (IPropertyInformation property, IValidationMessageFactory validationMessageFactory)
    {
      ArgumentUtility.CheckNotNull("property", property);
      ArgumentUtility.CheckNotNull("validationMessageFactory", validationMessageFactory);

      return GetValidators(property, validationMessageFactory);
    }
  }
}
