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
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Provides a common interface for classes defining the specifics of a SQL dialect. Used by <see cref="RdbmsProvider"/>.
  /// </summary>
  public interface ISqlDialect
  {
    /// <summary> A delimiter to end a SQL statement if the database requires one, an empty string otherwise. </summary>
    string StatementDelimiter { get; }

    /// <summary>Gets the dialect-conforming variant of the given parameter <paramref name="name"/>.</summary>
    string GetParameterName (string name);

    /// <summary> Surrounds an identifier with delimiters according to the database's syntax. </summary>
    string DelimitIdentifier (string identifier);

    /// <summary>
    /// Creates an <see cref="IDbDataParameter"/> for the given <paramref name="value"/>, <paramref name="parameterName"/>, <see cref="IStorageTypeInformation"/> and <see cref="IDbCommand"/>.
    /// </summary>
    /// <param name="command">The command to create the <see cref="IDbDataParameter"/> with.</param>
    /// <param name="storageTypeInformation">The <see cref="IStorageTypeInformation"/> for the column that matches the created parameter.</param>
    /// <param name="parameterName">The dialect-conforming name for the parameter; use <see cref="GetParameterName"/> to obtain a valid name.</param>
    /// <param name="value">The value to create the parameter for.</param>
    /// <returns>A parameter holding the given <paramref name="value"/> (possibly converted), for the given <paramref name="command"/>.</returns>
    /// <remarks>
    /// <para>
    /// The returned parameter's <see cref="IDataParameter.Value"/> is set to (a possibly converted version of) <paramref name="value"/>.
    /// </para>
    /// <para>
    /// The parameter's <see cref="IDataParameter.ParameterName"/> is set to <paramref name="parameterName"/> and ensured to conform to the
    /// <see cref="ISqlDialect"/>'s specifications.
    /// The parameter is not added to the command's <see cref="IDbCommand.Parameters"/> collection.
    /// </para>
    /// </remarks>
    /// <exception cref="NotSupportedException">The <paramref name="value"/> cannot be converted to the <see cref="IStorageTypeInformation.StorageType"/>.</exception>
    IDbDataParameter CreateDataParameter (IDbCommand command, IStorageTypeInformation storageTypeInformation, string parameterName, object? value);
  }
}
