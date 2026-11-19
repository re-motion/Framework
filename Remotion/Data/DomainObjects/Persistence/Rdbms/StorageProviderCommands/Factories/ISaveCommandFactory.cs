// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;

/// <summary>
///   The <see cref="ISaveCommandFactory" /> is responsible to create save commands for a relational database.
/// </summary>
public interface ISaveCommandFactory
{
  IRdbmsProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers);
}
