// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories;

/// <summary>
///   Provides a testable subclass of <see cref="BatchedSaveCommandFactory" /> that records the
///   <see cref="DataContainer" /> arguments passed to each overridden accessor-creation method,
///   enabling verification in unit tests.
/// </summary>
public class TestableBatchedSaveCommandFactory : BatchedSaveCommandFactory
{
  private readonly List<DataContainer> _createUpdateDataContainerAccessorCalledForDataContainers = new();
  private readonly List<DataContainer> _createDeleteDataContainerAccessorForDataContainers = new();
  private readonly List<DataContainer> _createInsertDataContainerAccessorCalledForDataContainers = new();
  private readonly List<DataContainer> _createLockDataContainerAccessorAccessorCalledForDataContainers = new();

  public TestableBatchedSaveCommandFactory ([NotNull] IDbCommandBuilderFactory dbCommandBuilderFactory, [NotNull] IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider, [NotNull] ITableDefinitionFinder tableDefinitionFinder, [NotNull] ITableManipulationRecordDefinitionProvider tableManipulationRecordDefinitionProvider)
      : base(dbCommandBuilderFactory, rdbmsPersistenceModelProvider, tableDefinitionFinder, tableManipulationRecordDefinitionProvider)
  {
  }

  public IReadOnlyCollection<DataContainer> CreateUpdateDataContainerAccessorCalledForDataContainers => _createUpdateDataContainerAccessorCalledForDataContainers;

  public IReadOnlyCollection<DataContainer> CreateDeleteDataContainerAccessorForDataContainers => _createDeleteDataContainerAccessorForDataContainers;

  public IReadOnlyCollection<DataContainer> CreateInsertDataContainerAccessorCalledForDataContainers => _createInsertDataContainerAccessorCalledForDataContainers;

  public IReadOnlyCollection<DataContainer> CreateLockDataContainerAccessorAccessorCalledForDataContainers => _createLockDataContainerAccessorAccessorCalledForDataContainers;

  protected override ITableManipulationDataContainerAccessor CreateUpdateDataContainerAccessor (DataContainer dataContainer, Dictionary<PropertyDefinition, ObjectID> objectIDValues)
  {
    _createUpdateDataContainerAccessorCalledForDataContainers.Add(dataContainer);
    return base.CreateUpdateDataContainerAccessor(dataContainer, objectIDValues);
  }

  protected override ITableManipulationDataContainerAccessor CreateDeleteDataContainerAccessor (DataContainer dataContainer)
  {
    _createDeleteDataContainerAccessorForDataContainers.Add(dataContainer);
    return base.CreateDeleteDataContainerAccessor(dataContainer);
  }

  protected override ITableManipulationDataContainerAccessor CreateInsertDataContainerAccessor (DataContainer dataContainer, Dictionary<PropertyDefinition, ObjectID> objectIDValues)
  {
    _createInsertDataContainerAccessorCalledForDataContainers.Add(dataContainer);
    return base.CreateInsertDataContainerAccessor(dataContainer, objectIDValues);
  }

  protected override ITableManipulationDataContainerAccessor CreateLockDataContainerAccessor (DataContainer dataContainer)
  {
    _createLockDataContainerAccessorAccessorCalledForDataContainers.Add(dataContainer);
    return base.CreateLockDataContainerAccessor(dataContainer);
  }
}
