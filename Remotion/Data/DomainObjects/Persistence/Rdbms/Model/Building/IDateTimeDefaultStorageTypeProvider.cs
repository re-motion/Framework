// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Data;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

/// <summary>
/// Provides an interface for defining the default rdbms storage type for <see cref="DateTime"/> objects.
/// </summary>
/// <threadsafety static="true" instance="true" />
/// <seealso cref="DateTimeDefaultStorageTypeProvider" />
/// <seealso cref="DateTime2DefaultStorageTypeProvider" />
public interface IDateTimeDefaultStorageTypeProvider
{
  /// <summary>
  /// Gets the StorageTypeName of the default rdbms storage type.
  /// </summary>
  public string StorageTypeName { get; }

  /// <summary>
  /// Gets the <see cref="DbType"/> of the default rdbms storage type.
  /// </summary>
  public DbType DbType { get; }
}
