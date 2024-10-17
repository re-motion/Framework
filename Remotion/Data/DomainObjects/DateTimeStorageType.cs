// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

namespace Remotion.Data.DomainObjects;

/// <summary>
/// Declares the storage types available when setting the storage type of a <see cref="DateTime" /> property.
/// </summary>
/// <seealso cref="DateTimeStorageTypeAttribute"/>
/// <seealso cref="IDateTimeDefaultStorageTypeProvider"/>
public enum DateTimeStorageType
{
  /// <summary>Declares the storage type as a <see cref="DbType"/>.<see cref="DbType.DateTime"/>.</summary>
  DateTime,
  /// <summary>Declares the storage type as a <see cref="DbType"/>.<see cref="DbType.DateTime2"/>.</summary>
  DateTime2
}
