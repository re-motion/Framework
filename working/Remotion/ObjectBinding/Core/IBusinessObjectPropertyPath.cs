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
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;

namespace Remotion.ObjectBinding
{
  /// <summary> A collection of business object properties that result in each other. </summary>
  /// <remarks>
  ///   <para>
  ///     A property path is comprised of zero or more <see cref="IBusinessObjectReferenceProperty"/> instances and 
  ///     a final <see cref="IBusinessObjectProperty"/>.
  ///   </para><para>
  ///     In its string representation, the property path uses the <see cref="char"/> returned by the 
  ///     <see cref="IBusinessObjectProvider.GetPropertyPathSeparator"/> method as the separator. The 
  ///     current property supplies the <see cref="IBusinessObjectProvider"/>.
  ///   </para>
  /// </remarks>
  public interface IBusinessObjectPropertyPath
  {
    /// <summary> Gets the string representation of this property path. </summary>
    [NotNull]
    string Identifier { get; }

    /// <summary> Gets the list of properties in this path. </summary>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="IsDynamic"/> evaluates <see langword="true" />.</exception>
    [NotNull]
    ReadOnlyCollection<IBusinessObjectProperty> Properties { get; }

    /// <summary> Get a flag that indicates whether the property path will be resolved anew for each call to <see cref="GetResult"/>. </summary>
    bool IsDynamic { get; }

    /// <summary>Evaluates the property path for the supplied <paramref name="root"/> object.</summary>
    /// <param name="root">The starting point for evaluating the property path. Must not be <see langword="null" />.</param>
    /// <param name="unreachableValueBehavior">Defines the behavior when the property path cannot be evaluated due to a <see langword="null" /> value.</param>
    /// <param name="listValueBehavior">Defines the behavior when the property path has to resolve a list-property. </param>
    /// <returns>The result object that can be used to get the actual value of the evaluated property path. Is never <see langword="null" />. </returns>
    [NotNull]
    IBusinessObjectPropertyPathResult GetResult (
        [NotNull] IBusinessObject root,
        BusinessObjectPropertyPath.UnreachableValueBehavior unreachableValueBehavior,
        BusinessObjectPropertyPath.ListValueBehavior listValueBehavior);
  }
}