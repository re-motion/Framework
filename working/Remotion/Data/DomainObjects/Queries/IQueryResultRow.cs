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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Represents a result row returned by a <see cref="QueryType.Custom"/> query.
  /// </summary>
  public interface IQueryResultRow
  {
    /// <summary>
    /// Returns the count of values in the current row.
    /// </summary>
    int ValueCount { get; }

    /// <summary>
    /// Returns a value stored within the current row at the given <paramref name="position"/> in the format returned by the 
    /// <see cref="StorageProvider"/>.
    /// </summary>
    object GetRawValue (int position);
    
    /// <summary>
    /// Returns a value stored within the current row at the given <paramref name="position"/>, asking the <see cref="StorageProvider"/> 
    /// to convert the value into the given <paramref name="type"/>.
    /// </summary>
    object GetConvertedValue (int position, Type type);

    /// <summary>
    /// Returns a value stored within the current row at the given <paramref name="position"/>, asking the <see cref="StorageProvider"/> to 
    /// convert the value into the given type <typeparamref name="T"/>.
    /// </summary>
    T GetConvertedValue<T> (int position);


  }
}