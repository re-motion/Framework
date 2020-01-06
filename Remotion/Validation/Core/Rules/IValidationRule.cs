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
using Remotion.Reflection;
using Remotion.Validation.Results;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Defines a rule associated with a property which can have multiple validators.
  /// </summary>
  public interface IValidationRule
  {
    // TODO RM-5960: remove member
    IPropertyInformation Property { get; }

    // TODO RM-5960: Switch to type-irreverent form (property and object validators)
    /// <summary>The validators that are grouped under this rule.</summary>
    IReadOnlyCollection<IPropertyValidator> Validators { get; }

    /// <summary>
    /// Performs validation using a validation context and returns a collection of Validation Failures.
    /// </summary>
    /// <param name="context">Validation Context</param>
    /// <returns>A collection of validation failures</returns>
    IEnumerable<ValidationFailure> Validate (ValidationContext context);
  }
}