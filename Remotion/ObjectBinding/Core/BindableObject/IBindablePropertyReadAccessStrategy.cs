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
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Defines an interface to check whether the value of a <see cref="PropertyBase"/> can be read from.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public interface IBindablePropertyReadAccessStrategy
  {
    /// <summary>
    /// Evaluates if getting the <paramref name="bindableProperty"/> is supported for the <paramref name="businessObject"/>.
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the check will be performed or <see langword="null" />.</param>
    /// <param name="bindableProperty">The <see cref="PropertyBase"/> for which the check will be performed. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the <paramref name="bindableProperty"/> can be gotten.</returns>
    /// <remarks>If getting the property is not supported, the property is hidden in the UI.</remarks>
    bool CanRead ([CanBeNull] IBusinessObject businessObject, [NotNull] PropertyBase bindableProperty);

    /// <summary>
    /// Checks if the <paramref name="exception"/> that has occured while accessing the <paramref name="bindableProperty"/> 
    /// indicates an expected error condition that can be handled by the infrastructure.
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the property was accessed. Must not be <see langword="null" />.</param>
    /// <param name="bindableProperty">The <see cref="PropertyBase"/> that was accessed. Must not be <see langword="null" />.</param>
    /// <param name="exception">
    ///   The <see cref="Exception"/> that has occurred while accessing the <paramref name="bindableProperty"/>. Must not be <see langword="null" />.
    /// </param>
    /// <param name="propertyAccessException">
    ///   The resulting <see cref="BusinessObjectPropertyAccessException"/> that should be re-thrown if the <see cref="IsPropertyAccessException"/> method
    ///   returns <see langword="true" />. This new exception may contain additional information about the property access error.
    /// </param>
    /// <returns>
    ///   <see langword="true" /> if the <paramref name="exception"/> represents an expected domain constraint during property access, 
    ///   <see langword="false" /> if the original <paramref name="exception"/> should be re-thrown.
    /// </returns>
    [ContractAnnotation ("=>true, propertyAccessException:notnull; =>false, propertyAccessException:null")]
    bool IsPropertyAccessException (
        [NotNull] IBusinessObject businessObject,
        [NotNull] PropertyBase bindableProperty,
        [NotNull] Exception exception,
        [CanBeNull] out BusinessObjectPropertyAccessException propertyAccessException);
  }
}