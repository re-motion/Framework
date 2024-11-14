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
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Defines an API for a structured type in a relational database.
  /// </summary>
  public interface IRdbmsStructuredTypeDefinition
  {
    /// <summary>
    /// The name of the structured type.
    /// </summary>
    EntityNameDefinition TypeName { get; }

    /// <summary>
    /// The <see cref="IRdbmsStoragePropertyDefinition"/>s that correspond to the attributes of the structured type. 
    /// </summary>
    IReadOnlyCollection<IRdbmsStoragePropertyDefinition> Properties { get; }

    /// <summary>
    /// Calls the method on the given <paramref name="visitor"/> that corresponds to the <see cref="IRdbmsStructuredTypeDefinition"/> implementation.  
    /// </summary>
    void Accept (IRdbmsStructuredTypeDefinitionVisitor visitor);
  }
}
