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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Defines a strategy to use when traversing a <see cref="DomainObject"/> graph using a <see cref="DomainObjectGraphTraverser"/>.
  /// </summary>
  public interface IGraphTraversalStrategy
  {
    /// <summary>
    /// Determines whether to process the given object in the result set when traversing a <see cref="DomainObject"/> graph.
    /// </summary>
    /// <param name="domainObject">The domain object to decide about.</param>
    /// <returns>True if the object should be processed; otherwise, false.</returns>
    /// <remarks>The question of processing has no effect on the question whether the object's links should be followed.</remarks>
    bool ShouldProcessObject (IDomainObject domainObject);

    /// <summary>
    /// Determines whether to follow a relation link when traversing a <see cref="DomainObject"/> graph.
    /// </summary>
    /// <param name="root">The root domain object from which the traversal was started.</param>
    /// <param name="currentObject">The current domain object defining the relation link.</param>
    /// <param name="currentDepth">The number of links that were traversed from the root to the current object. Note that this value is not
    ///   necessarily the shortest path from the root to the current object; if an object can be reached in more than one way, it is not defined
    ///   which way is taken by the traverser.</param>
    /// <param name="linkProperty">The link property. Note that when the property's <see cref="PropertyAccessor.GetValue{T}"/> methods are
    ///   accessed, this can cause the related objects to be loaded from the database.</param>
    /// <returns>True if the traverser should follow the link; otherwise, if traversal should stop at the <paramref name="currentObject"/>.</returns>
    bool ShouldFollowLink (IDomainObject root, IDomainObject currentObject, int currentDepth, PropertyAccessor linkProperty);
  }
}
