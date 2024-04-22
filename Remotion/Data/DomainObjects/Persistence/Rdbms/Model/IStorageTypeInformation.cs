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
using System.Collections.Generic;
using System.Data;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Defines an API for classes providing information about the storage type of a value in a relational database.
  /// In addition, it can create an unnamed <see cref="IDbDataParameter"/> for a given value (convertible to the storage format) or read and 
  /// convert a value from an <see cref="IDataReader"/>.
  /// </summary>
  public interface IStorageTypeInformation
  {
    /// <summary>
    /// Gets the storage type as a CLR <see cref="Type"/>; this is the <see cref="Type"/> used to represent stored values in memory before conversion.
    /// For example, for <see cref="Enum"/> values, this could be the <see cref="int"/> type.
    /// </summary>
    /// <value>The storage type as a CLR <see cref="Type"/>.</value>
    Type StorageType { get; }

    /// <summary>
    /// Gets the name of the storage type as understood by the underlying database.
    /// </summary>
    /// <value>The name of the storage type.</value>
    string StorageTypeName { get; }

    /// <summary>
    /// Gets a value indicating whether the storage type is nullable in the underlying database.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if the storage type is nullable; otherwise, <see langword="false"/>.
    /// </value>
    bool IsStorageTypeNullable { get; }

    /// <summary>
    /// Gets the storage type after conversion. This is the <see cref="Type"/> of values returned by <see cref="Read"/>.
    /// For example, for <see cref="Enum"/> values, this would be the respective <see cref="Enum"/> type.
    /// </summary>
    /// <value>The converted type.</value>
    Type DotNetType { get; }

    /// <summary>
    /// Gets the <see cref="DbType"/> value corresponding to the storage type.
    /// </summary>
    /// <value>The <see cref="DbType"/> of the storage type.</value>
    DbType StorageDbType { get; }

    /// <summary>
    /// Gets the length of the storage type as used by the underlying database.
    /// </summary>
    /// <value>
    /// The length of the storage type. <see langword="null" /> is used if the storage type does not specify an explicit length, 
    /// <b>-1</b> typically represents the <b>max</b> value.
    /// </value>
    int? StorageTypeLength { get; }

    /// <summary>
    /// Reads a value from the specified <see cref="IDataReader"/> at the given <paramref name="ordinal"/>, returning it as an instance of 
    /// <see cref="DotNetType"/> (or <see langword="null" />).
    /// </summary>
    /// <param name="dataReader">The <see cref="IDataReader"/> to read from.</param>
    /// <param name="ordinal">The ordinal identifying the value to be read.</param>
    /// <returns>A (possibly converted) value read from the given <paramref name="dataReader"/>.</returns>
    /// <exception cref="NotSupportedException">The read value cannot be converted to the <see cref="DotNetType"/>.</exception>
    object? Read (IDataReader dataReader, int ordinal);

    /// <summary>
    /// Converts a value to the <see cref="StorageTypeInformation.StorageType"/>, so that it can be stored by the database.
    /// </summary>
    /// <param name="dotNetValue">The value to be converted. Can be <see langword="null" />.</param>
    /// <returns>The converted value as it can be stored by the database.</returns>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    object ConvertToStorageType (object? dotNetValue);

    /// <summary>
    /// Converts a value read from the database to the <see cref="StorageTypeInformation.DotNetType"/>.
    /// </summary>
    /// <param name="storageValue">The value to be converted. Can be <see langword="null" />.</param>
    /// <returns>The converted value as it can be used in .NET code.</returns>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    object? ConvertFromStorageType (object? storageValue);

    IStorageTypeInformation UnifyForEquivalentProperties (IEnumerable<IStorageTypeInformation> equivalentStorageTypes);
  }
}
