// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms;

public class TestableRdbmsProviderCommandFactory : RdbmsProviderCommandFactory
{
  private class ExceptionThrowingSaveCommandFactory : ISaveCommandFactory
  {
    public IRdbmsProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers)
    {
      Assert.Fail($"{nameof(TestableRdbmsProviderCommandFactory)} was configured to use {nameof(ExceptionThrowingSaveCommandFactory)} as {nameof(ISaveCommandFactory)}. Maybe you want to specify the behaviour and/or duplicate the tests so both variants are tested.");

      throw new UnreachableException();
    }
  }

  private class SaveCommandFactoryWrapper : ISaveCommandFactory
  {
    private readonly Lazy<ISaveCommandFactory> _realSaveCommandFactory;

    public SaveCommandFactoryWrapper (TestableRdbmsProviderCommandFactory providerCommandFactory)
    {
      ArgumentNullException.ThrowIfNull(providerCommandFactory);

      _realSaveCommandFactory = new Lazy<ISaveCommandFactory>(() => CreateSaveCommandFactory(providerCommandFactory));
    }

    private ISaveCommandFactory CreateSaveCommandFactory (TestableRdbmsProviderCommandFactory providerCommandFactory)
    {
      var dbCommandBuilderFactory = providerCommandFactory.DbCommandBuilderFactory;
      var rdbmsPersistenceModelProvider = providerCommandFactory.RdbmsPersistenceModelProvider;
      var tableDefinitionFinder = providerCommandFactory.TableDefinitionFinder;
      var tableManipulationRecordDefinitionProvider = providerCommandFactory.TableManipulationRecordDefinitionProvider;

      switch (providerCommandFactory.CreateSaveCommandFactoryBehaviour)
      {
        case CreateSaveCommandFactoryBehaviour.CreateBatchedSaveCommandFactory:
          return new BatchedSaveCommandFactory(dbCommandBuilderFactory, rdbmsPersistenceModelProvider, tableDefinitionFinder, tableManipulationRecordDefinitionProvider);
        case CreateSaveCommandFactoryBehaviour.CreateIndividualSaveCommandFactory:
          return new IndividualSaveCommandFactory(dbCommandBuilderFactory, rdbmsPersistenceModelProvider, tableDefinitionFinder);
        case CreateSaveCommandFactoryBehaviour.CreateExceptionThrowingSaveCommandFactory:
          return new ExceptionThrowingSaveCommandFactory();
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public IRdbmsProviderCommand CreateForSave (IEnumerable<DataContainer> dataContainers)
    {
      return _realSaveCommandFactory.Value.CreateForSave(dataContainers);
    }
  }

  public CreateSaveCommandFactoryBehaviour CreateSaveCommandFactoryBehaviour { get; }

  public TestableRdbmsProviderCommandFactory ([NotNull] RdbmsProviderDefinition storageProviderDefinition, [NotNull] IDbCommandBuilderFactory dbCommandBuilderFactory, [NotNull] IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider, [NotNull] IObjectReaderFactory objectReaderFactory, [NotNull] ITableDefinitionFinder tableDefinitionFinder, [NotNull] IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory, [NotNull] IDataParameterDefinitionFactory dataParameterDefinitionFactory, [NotNull] ITableManipulationRecordDefinitionProvider tableManipulationRecordDefinitionProvider, CreateSaveCommandFactoryBehaviour createSaveCommandFactoryBehaviour)
      : base(storageProviderDefinition, dbCommandBuilderFactory, rdbmsPersistenceModelProvider, objectReaderFactory, tableDefinitionFinder, dataStoragePropertyDefinitionFactory, dataParameterDefinitionFactory, tableManipulationRecordDefinitionProvider)
  {
    CreateSaveCommandFactoryBehaviour = createSaveCommandFactoryBehaviour;
  }

  protected override ISaveCommandFactory CreateSaveCommandFactory () => new SaveCommandFactoryWrapper(this);
}
