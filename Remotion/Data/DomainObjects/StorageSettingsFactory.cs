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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Used to instantiate basic implementations of <see cref="IStorageObjectFactory"/>s.
  /// </summary>
  public static class StorageSettingsFactory
  {
    /// <summary>
    ///   Creates an <see cref="RdbmsStorageSettingsFactory"/> named <c>Default</c> and a <see cref="SqlStorageObjectFactory"/> with the given
    ///   <paramref name="connectionString"/> and <paramref name="readOnlyConnectionString"/>.
    /// </summary>
    /// <param name="connectionString">The connection string for the read/write connection to the database. Must not be <see langname="null"/> or empty.</param>
    /// <param name="readOnlyConnectionString">
    ///   The connection string for the readonly connection to the database.
    ///   If not specified, the <paramref name="connectionString"/> will be inferred as the value.
    ///   Must not be empty.
    /// </param>
    public static IStorageSettingsFactory CreateForSqlServer (string connectionString, string? readOnlyConnectionString = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);
      ArgumentUtility.CheckNotEmpty("readOnlyConnectionString", readOnlyConnectionString);

      return new RdbmsStorageSettingsFactory("Default", typeof(SqlStorageObjectFactory), connectionString, readOnlyConnectionString ?? connectionString);
    }

    /// <summary>
    ///   Creates an <see cref="RdbmsStorageSettingsFactory"/> named <c>Default</c>,
    ///   an <see cref="IStorageObjectFactory"/> of the type specified by <typeparamref name="TStorageObjectFactory"/>
    ///   and the given <paramref name="connectionString"/> and <paramref name="readOnlyConnectionString"/>.
    /// </summary>
    /// <param name="connectionString">The connection string for the read/write connection to the database. Must not be <see langname="null"/> or empty.</param>
    /// <param name="readOnlyConnectionString">
    ///   The connection string for the readonly connection to the database.
    ///   If not specified, the <paramref name="connectionString"/> will be inferred as the value.
    ///   Must not be empty.
    /// </param>
    public static IStorageSettingsFactory CreateForSqlServer<TStorageObjectFactory> (string connectionString, string? readOnlyConnectionString = null)
        where TStorageObjectFactory : IRdbmsStorageObjectFactory
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);
      ArgumentUtility.CheckNotEmpty("readOnlyConnectionString", readOnlyConnectionString);

      return new RdbmsStorageSettingsFactory("Default", typeof(TStorageObjectFactory), connectionString, readOnlyConnectionString ?? connectionString);
    }
  }
}
