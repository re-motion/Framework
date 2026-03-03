// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands;

[TestFixture]
public class BatchedLockRdbmsProviderCommandTest
{
  private Mock<IRdbmsProviderReadWriteCommandExecutionContext> _executionContextMock;

  [SetUp]
  public void Setup ()
  {
    MappingConfiguration.SetCurrent(StandardConfiguration.Instance.GetMappingConfiguration());
    _executionContextMock = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>(MockBehavior.Strict);
  }

  [TearDown]
  public void TearDown ()
  {
    MappingConfiguration.SetCurrent(null!);
  }

  [Test]
  public void Execute_ThrowsConcurrencyViolation_ContainingFailedObjectIDs ()
  {
    var dataContainer1 = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
    var dataContainer2 = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("FBB1F2FA-A546-4DB8-B3F0-8847FFC39B1C")), new byte[8], pd => pd.DefaultValue);
    var dataContainer3 = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("7044662C-AC8E-435B-8457-D1B8B20EA7FE")), new byte[8], pd => pd.DefaultValue);

    var commandBuilderMock = new Mock<IDbCommandBuilder>();
    var dbCommandMock = new Mock<DbCommand>();
    var dataReaderMock = new Mock<DbDataReader>();

    List<ObjectID> failedIDs = [dataContainer1.ID, dataContainer2.ID];
    var dataReaderQueue = new Queue<Guid>(failedIDs.Select(i => (Guid)i.Value));

    dataReaderMock.Setup(stub => stub.Read()).Returns(() => dataReaderQueue.Count > 0);
    dataReaderMock.Setup(stub => stub.GetGuid(0)).Returns(() => dataReaderQueue.Dequeue());

    commandBuilderMock.Setup(stub => stub.Create(_executionContextMock.Object)).Returns(dbCommandMock.Object);

    _executionContextMock.Setup(stub => stub.ExecuteReader(dbCommandMock.Object, CommandBehavior.Default))
        .Returns(dataReaderMock.Object);

    var commandContext = new BatchedLockRdbmsProviderCommand(commandBuilderMock.Object, [dataContainer1, dataContainer2, dataContainer3]);

    try
    {
      commandContext.Execute(_executionContextMock.Object);
      Assert.Fail($"Expected was a {nameof(ConcurrencyViolationException)} but no exception occured.");
    }
    catch (ConcurrencyViolationException cve)
    {
      Assert.That(cve.IDs, Is.EqualTo(failedIDs));
    }
    catch(Exception ex)
    {
      Assert.Fail($"Expected was {nameof(ConcurrencyViolationException)} but got '{ex}'");
    }
  }

  [Test]
  public void Execute_WithDuplicateGuid_ThrowsConcurrencyViolation_ContainingFailedObjectIDs ()
  {
    var dataContainer1 = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
    var dataContainer2 = DataContainer.CreateForExisting(new ObjectID("Order", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
    var dataContainer3 = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("7044662C-AC8E-435B-8457-D1B8B20EA7FE")), new byte[8], pd => pd.DefaultValue);

    var commandBuilderMock = new Mock<IDbCommandBuilder>();
    var dbCommandMock = new Mock<DbCommand>();
    var dataReaderMock = new Mock<DbDataReader>();

    List<ObjectID> failedIDs = [dataContainer1.ID, dataContainer2.ID];
    var dataReaderQueue = new Queue<Guid>([new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")]);

    dataReaderMock.Setup(stub => stub.Read()).Returns(() => dataReaderQueue.Count > 0);
    dataReaderMock.Setup(stub => stub.GetGuid(0)).Returns(() => dataReaderQueue.Dequeue());

    commandBuilderMock.Setup(stub => stub.Create(_executionContextMock.Object)).Returns(dbCommandMock.Object);

    _executionContextMock.Setup(stub => stub.ExecuteReader(dbCommandMock.Object, CommandBehavior.Default))
        .Returns(dataReaderMock.Object);

    var commandContext = new BatchedLockRdbmsProviderCommand(commandBuilderMock.Object, [dataContainer1, dataContainer2, dataContainer3]);

    try
    {
      commandContext.Execute(_executionContextMock.Object);
      Assert.Fail($"Expected was a {nameof(ConcurrencyViolationException)} but no exception occured.");
    }
    catch (ConcurrencyViolationException cve)
    {
      Assert.That(cve.IDs, Is.EqualTo(failedIDs));
    }
    catch (Exception ex)
    {
      Assert.Fail($"Expected was {nameof(ConcurrencyViolationException)} but got '{ex}'");
    }
  }

  [Test]
  public void Execute_RdbmsProviderException_AndAddsObjectIDs ()
  {
    var dataContainer1 = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
    var commandBuilderMock = new Mock<IDbCommandBuilder>();
    var dbCommandMock = new Mock<DbCommand>();

    commandBuilderMock.Setup(stub => stub.Create(_executionContextMock.Object)).Returns(dbCommandMock.Object);

    _executionContextMock.Setup(stub => stub.ExecuteReader(dbCommandMock.Object, CommandBehavior.Default)).Throws(new RdbmsProviderException("Original Message"));

    var commandContext = new BatchedLockRdbmsProviderCommand(commandBuilderMock.Object, [dataContainer1]);

    try
    {
      commandContext.Execute(_executionContextMock.Object);
      Assert.Fail($"Expected was a {nameof(ConcurrencyViolationException)} but no exception occured.");
    }
    catch (RdbmsProviderException rdbms)
    {
      Assert.That(rdbms.InnerException, Is.TypeOf<RdbmsProviderException>());
      Assert.That(rdbms.InnerException!.Message, Is.EqualTo("Original Message"));
      Assert.That(rdbms.Message, Is.EqualTo($"Error while locking objects '{dataContainer1.ID}'. Original Message"));
    }
    catch (Exception ex)
    {
      Assert.Fail($"Expected was {nameof(ConcurrencyViolationException)} but got '{ex}'");
    }
  }

  [Test]
  public void Execute_RdbmsProviderException_AndAddsMaximal10ObjectIDsToErrorMessage ()
  {
    var dataContainers = new List<DataContainer>();
    for(int i = 0; i < 11; i++)
    {
      var dataContainer = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
      dataContainers.Add(dataContainer);
    }
    var commandBuilderMock = new Mock<IDbCommandBuilder>();
    var dbCommandMock = new Mock<DbCommand>();

    commandBuilderMock.Setup(stub => stub.Create(_executionContextMock.Object)).Returns(dbCommandMock.Object);

    _executionContextMock.Setup(stub => stub.ExecuteReader(dbCommandMock.Object, CommandBehavior.Default)).Throws(new RdbmsProviderException("Original Message"));

    var commandContext = new BatchedLockRdbmsProviderCommand(commandBuilderMock.Object, dataContainers);

    try
    {
      commandContext.Execute(_executionContextMock.Object);
      Assert.Fail($"Expected was a {nameof(ConcurrencyViolationException)} but no exception occured.");
    }
    catch (RdbmsProviderException rdbms)
    {
      Assert.That(rdbms.InnerException, Is.TypeOf<RdbmsProviderException>());
      Assert.That(rdbms.InnerException!.Message, Is.EqualTo("Original Message"));
      Assert.That(rdbms.Message, Is.EqualTo($"Error while locking objects '{string.Join(", ", dataContainers.Take(10).Select(d=>d.ID))}, ...'. Original Message"));
      Assert.That(rdbms.Message, Is.Not.EqualTo($"Error while locking objects '{string.Join(", ", dataContainers.Select(d=>d.ID))}, ...'. Original Message"));
    }
    catch (Exception ex)
    {
      Assert.Fail($"Expected was {nameof(ConcurrencyViolationException)} but got '{ex}'");
    }
  }
}
