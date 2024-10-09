// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Data;

namespace Remotion.Data.DomainObjects;

/// <summary>
/// Declares the storage types available when setting the storage type of a <see cref="DateTime" /> property.
/// </summary>
public enum DateTimeStorageType
{
  /// <summary>Declares the storage type as a <see cref="DbType"/>.<see cref="DbType.DateTime"/>.</summary>
  DateTime,
  /// <summary>Declares the storage type as a <see cref="DbType"/>.<see cref="DbType.DateTime2"/>.</summary>
  DateTime2
}
