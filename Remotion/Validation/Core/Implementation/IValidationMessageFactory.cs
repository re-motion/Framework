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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Implementation
{
  public interface IValidationMessageFactory
  {
    /// <summary>
    /// Returns the <see cref="ValidationMessage"/> for a given <see cref="Type"/> that implements the <see cref="IPropertyValidator"/> interface.
    /// </summary>
    /// <param name="validatorType">The <see cref="Type"/> of the <see cref="IPropertyValidator"/> to get the <see cref="ValidationMessage"/> for.</param>
    /// <param name="validatedProperty">The <see cref="IPropertyInformation"/> for which the <see cref="ValidationMessage"/> is created.</param>
    /// <returns>
    /// The <see cref="ValidationMessage"/> for the <see cref="IPropertyValidator"/>.
    /// Please note that the method can return <see langword="null"/> if no <see cref="ValidationMessage"/> is defined for the <paramref name="validatorType"/>.
    /// </returns>
    [CanBeNull]
    ValidationMessage CreateValidationMessageForPropertyValidator ([NotNull] Type validatorType, [NotNull] IPropertyInformation validatedProperty);

    /// <summary>
    /// Returns the <see cref="ValidationMessage"/> for a given <see cref="Type"/> that implements the <see cref="IObjectValidator"/> interface.
    /// </summary>
    /// <param name="validatorType">The <see cref="Type"/> of the <see cref="IObjectValidator"/> to get the <see cref="ValidationMessage"/> for.</param>
    /// <param name="validatedType">The <see cref="ITypeInformation"/> for which the <see cref="ValidationMessage"/> is created.</param>
    /// <returns>
    /// The <see cref="ValidationMessage"/> for the <see cref="IObjectValidator"/>.
    /// Please note that the method can return <see langword="null"/> if no <see cref="ValidationMessage"/> is defined for the <paramref name="validatorType"/>.
    /// </returns>
    [CanBeNull]
    ValidationMessage CreateValidationMessageForObjectValidator ([NotNull] Type validatorType, [NotNull] ITypeInformation validatedType);
  }
}