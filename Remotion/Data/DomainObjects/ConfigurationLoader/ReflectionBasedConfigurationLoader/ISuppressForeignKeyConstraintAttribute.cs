// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

/// <summary>
///   When applied to a property the creation of a foreign key constraint in the database is omitted
/// </summary>
public interface ISuppressForeignKeyConstraintAttribute : IMappingAttribute
{
  /// <summary>
  ///   Gets a flag which causes the creation of a foreign key constraint in the database to be omitted.
  /// </summary>
  bool IsForeignKeyConstraintSuppressed { get; }
}
