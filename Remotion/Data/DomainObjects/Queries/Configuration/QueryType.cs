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

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Indicates the type of a <see cref="QueryDefinition"/>.
  /// </summary>
  public enum QueryType
  {
    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a collection of <see cref="DomainObject"/>s.
    /// </summary>
    Collection = 0,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return only a single value.
    /// </summary>
    Scalar = 1,

    /// <summary>
    /// Instances of a <see cref="QueryDefinition"/> return a sequence of arbitrary objects.
    /// </summary>
    Custom = 2
  }
}
