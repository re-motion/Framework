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
using JetBrains.Annotations;
using Remotion.Validation.Results;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Defines a rule associated with a property which can have multiple validators.
  /// </summary>
  public interface IValidationRule
  {
    /// <summary>
    /// Performs validation using a validation context and returns a collection of Validation Failures.
    /// </summary>
    /// <param name="context">Validation Context. Must not be <see langword="null" />.</param>
    /// <returns>A collection of validation failures</returns>
    IEnumerable<ValidationFailure> Validate ([NotNull] ValidationContext context);

    /// <summary>
    /// Evaluates if the validation rule is active for the specified <paramref name="context"/>.
    /// </summary>
    /// <param name="context">Validation Context. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the <see cref="IValidationRule"/> is active.</returns>
    bool IsActive ([NotNull] ValidationContext context);
  }
}
