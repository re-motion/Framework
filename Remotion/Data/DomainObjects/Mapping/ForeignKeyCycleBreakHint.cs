// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;

namespace Remotion.Data.DomainObjects.Mapping;

/// <summary>
/// Provides hints for resolving foreign key cycles during persistence operations.
/// These hints influence how foreign key relationships may be temporarily broken
/// to allow successful insert/update and delete ordering.
/// </summary>
public enum ForeignKeyCycleBreakHint
{
  /// <summary>
  /// Marks the property/foreign key to be allowed to be broken automatically.
  /// </summary>
  Automatic = 0,

  /// <summary>
  /// Marks the property/foreign key to be never broken.
  /// This can lead to situations where inserting can fail with foreign key violation exceptions
  /// if applied to multiple properties within the same cycle.
  /// </summary>
  NeverBreak = 1,

  /// <summary>t
  /// Marks the property/foreign key to be broken if necessary.
  /// Foreign keys marked with <see cref="PreferredBreak"/> will be broken before <see cref="Automatic"/> breaks.
  /// </summary>
  PreferredBreak = 2,

  /// <summary>
  /// Forces the foreign key to always be broken during SQL command generation.
  /// Inserts will use null values, followed by an update to set the actual value.
  /// For ordering, these are broken before <see cref="PreferredBreak"/> and <see cref="Automatic"/>.
  /// </summary>
  AlwaysBreak = 3
}
