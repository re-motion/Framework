// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

/// <summary>
/// Provides the <see cref="RecordDefinition"/>s for TVPs that can be used to efficiently batch table manipulations.
/// </summary>
public interface ITableManipulationRecordDefinitionProvider
{
  /// <summary>
  /// Gets the Delete TVP <see cref="RecordDefinition"/> for the specified <paramref name="classDefinition"/>.
  /// </summary>
  RecordDefinition GetDeleteRecordDefinition (ClassDefinition classDefinition);

  /// <summary>
  /// Gets the Insert TVP <see cref="RecordDefinition"/> for the specified <paramref name="classDefinition"/>.
  /// </summary>
  RecordDefinition GetInsertRecordDefinition (ClassDefinition classDefinition);

  /// <summary>
  /// Gets the Lock TVP <see cref="RecordDefinition"/> for the specified <paramref name="classDefinition"/>.
  /// </summary>
  RecordDefinition GetLockRecordDefinition (ClassDefinition classDefinition);

  /// <summary>
  /// Gets the Update TVP <see cref="RecordDefinition"/> for the specified <paramref name="classDefinition"/>.
  /// </summary>
  RecordDefinition GetUpdateRecordDefinition (ClassDefinition classDefinition);
}
