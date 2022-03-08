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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Builder
{
  /// <summary>
  /// Represents <see cref="TypeDefinition"/> node in a tree constructed by <see cref="TypeDefinitionFactory"/>.
  /// Supports child navigation and correctly setting up child relationships in a resulting <see cref="TypeDefinition"/>.
  /// </summary>
  public abstract class TypeDefinitionBuilderNode
  {
    public Type Type { get; }

    protected TypeDefinitionBuilderNode (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      Type = type;
    }

    /// <summary>
    /// Returns <see langword="true"/> if this node has no child nodes.
    /// </summary>
    public abstract bool IsLeafNode { get; }

    /// <summary>
    /// Creates a <see cref="TypeDefinition"/> for this node and its ancestors if it is a leaf node.
    /// The created <see cref="TypeDefinition"/> can be retrieved using <see cref="GetBuiltTypeDefinition"/> after calling <see cref="EndBuildTypeDefinition"/>.
    /// </summary>
    public void BeginBuildTypeDefinition (IMappingObjectFactory mappingObjectFactory)
    {
      if (!IsLeafNode)
        return;

      BuildTypeDefinition(mappingObjectFactory);
    }

    /// <summary>
    /// Creates a <see cref="TypeDefinition"/> for this node and links it to its parents.
    /// </summary>
    protected abstract void BuildTypeDefinition (IMappingObjectFactory mappingObjectFactory);

    /// <summary>
    /// Completes the building of a <see cref="TypeDefinition"/> by linking it to its children.
    /// Implicitly calls <see cref="EndBuildTypeDefinition"/> on other nodes in the same hierarchy.
    /// </summary>
    public abstract void EndBuildTypeDefinition ();

    /// <summary>
    /// Returns the built <see cref="TypeDefinition"/> that has the correct parent/child relationships.
    /// Can only be called after <see cref="EndBuildTypeDefinition"/> has been called.
    /// </summary>
    public abstract TypeDefinition? GetBuiltTypeDefinition ();
  }
}
