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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;

namespace Remotion.Globalization
{
  /// <summary>
  /// Defines an interface for resolving the <see cref="IResourceManager"/> for an <see cref="ITypeInformation"/>.
  /// </summary>
  /// <seealso cref="GlobalizationService"/>
  /// <seealso cref="T:Remotion.Globalization.Mixins.MixinGlobalizationService"/>
  /// <threadsafety static="true" instance="true" />
  public interface IGlobalizationService
  {
    /// <summary>
    /// Resolves the <see cref="IResourceManager"/> for the specified <paramref name="typeInformation"/>.
    /// </summary>
    /// <remarks>
    /// If multiple resource managers are defined for a type via the base-class hierarchy or mixins, the following rules are applied to the order 
    /// in which the resource managers are aggregated:
    /// <list type="number">
    ///   <item>The resource managers for the mixins of mixin.</item>
    ///   <item>The resource managers for the mixins.</item>
    ///   <item>The resource manager for the type.</item>
    ///   <item>The resource managers for the base-types.</item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// The <see cref="IResourceManager"/> for the speficied <paramref name="typeInformation"/>, 
    /// or a <see cref="NullResourceManager"/> if no resources are defined.
    /// </returns>
    [NotNull]
    IResourceManager GetResourceManager ([NotNull] ITypeInformation typeInformation);
  }
}