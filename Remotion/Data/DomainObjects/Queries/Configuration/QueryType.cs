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
using Remotion.Obsolete;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Indicates the type of a <see cref="QueryDefinition"/>.
  /// </summary>
  public enum QueryType
  {
    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a collection of <see cref="DomainObject"/>s and can write to the domain.
    /// </summary>
    CollectionReadWrite = 0,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return only a single value and can write to the domain.
    /// </summary>
    ScalarReadWrite = 1,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a sequence of arbitrary objects and can write to the domain.
    /// </summary>
    CustomReadWrite = 2,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a collection of <see cref="DomainObject"/>s and cannot write to the domain.
    /// </summary>
    CollectionReadOnly = 3,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return only a single value and cannot write to the domain.
    /// </summary>
    ScalarReadOnly = 4,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a sequence of arbitrary objects and cannot write to the domain.
    /// </summary>
    CustomReadOnly = 5,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a collection of <see cref="DomainObject"/>s.
    /// </summary>
    [Obsolete("'QueryType.Collection' has been replaced with 'QueryType.CollectionReadWrite'. (Version 7.0.0)", false, DiagnosticId = ObsoleteDiagnosticIDs.QueryTypeValue)]
    Collection = CollectionReadWrite,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return only a single value.
    /// </summary>
    [Obsolete("'QueryType.Scalar' has been replaced with 'QueryType.ScalarReadWrite'. (Version 7.0.0)", false, DiagnosticId = ObsoleteDiagnosticIDs.QueryTypeValue)]
    Scalar = ScalarReadWrite,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a sequence of arbitrary objects.
    /// </summary>
    [Obsolete("'QueryType.Custom' has been replaced with 'QueryType.CustomReadWrite'. (Version 7.0.0)", false, DiagnosticId = ObsoleteDiagnosticIDs.QueryTypeValue)]
    Custom = CustomReadWrite,
  }
}
