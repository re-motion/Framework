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

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Provides a method to create an <see cref="IResourceManager"/> object for a given <see cref="Type"/>.
  /// </summary>
  /// <seealso cref="CompoundResourceManagerFactory"/>
  /// <seealso cref="ResourceAttributeBasedResourceManagerFactory"/>
  /// <threadsafety static="true" instance="true"/>
  public interface IResourceManagerFactory
  {
    /// <summary>
    /// Creates an implementation of the <see cref="IResourceManager"/> interface if the <paramref name="type"/> as resource information associated.
    /// </summary>
    /// <remarks>Only the actual <paramref name="type"/> should be considered, not the type hierarchy.</remarks>
    [NotNull]
    IResourceManager CreateResourceManager ([NotNull] Type type);
  }
}