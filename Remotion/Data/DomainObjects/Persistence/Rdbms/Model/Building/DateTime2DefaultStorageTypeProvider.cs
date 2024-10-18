// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Data;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

/// <summary>
/// Implements the <see cref="IDateTimeDefaultStorageTypeProvider"/> to provide a default storage type
/// of <see cref="DbType.DateTime2"/>.
/// </summary>
/// <threadsafety static="true" instance="true" />
public class DateTime2DefaultStorageTypeProvider : IDateTimeDefaultStorageTypeProvider
{
  /// <inheritdoc cref="IDateTimeDefaultStorageTypeProvider.StorageTypeName"/>
  public string StorageTypeName => "datetime2";

  /// <inheritdoc cref="IDateTimeDefaultStorageTypeProvider.DbType"/>
  public DbType DbType => DbType.DateTime2;
}
