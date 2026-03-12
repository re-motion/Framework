// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
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
public class BatchedObjectsRdbmsProviderCommandTest
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
  public void Execute_ThrowsConcurrencyViolation_ContainingAllObjectIDs ()
  {
    var dataContainer1 = DataContainer.CreateNew(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), pd => pd.DefaultValue);
    var dataContainer2 = DataContainer.CreateNew(new ObjectID("Computer", new Guid("FBB1F2FA-A546-4DB8-B3F0-8847FFC39B1C")), pd => pd.DefaultValue);
    var dataContainer3 = DataContainer.CreateNew(new ObjectID("Computer", new Guid("7044662C-AC8E-435B-8457-D1B8B20EA7FE")), pd => pd.DefaultValue);

    var commandBuilderMock = new Mock<IDbCommandBuilder>();
    var dbCommandMock = new Mock<DbCommand>();

    List<ObjectID> failedIDs = [dataContainer1.ID, dataContainer2.ID, dataContainer3.ID];
    commandBuilderMock.Setup(stub => stub.Create(_executionContextMock.Object)).Returns(dbCommandMock.Object);

    _executionContextMock.Setup(stub => stub.ExecuteNonQuery(dbCommandMock.Object))
        .Returns(1);

    var commandContext = new BatchedObjectsRdbmsProviderCommand(commandBuilderMock.Object, [dataContainer1, dataContainer2, dataContainer3]);

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
  public void Execute_ThrowsConcurrencyViolation_AndAddsMaximal10ObjectIDsToErrorMessage ()
  {
    var dataContainers = new List<DataContainer>();
    for (int i = 0; i < 11; i++)
    {
      var dataContainer = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
      dataContainers.Add(dataContainer);
    }

    var commandBuilderMock = new Mock<IDbCommandBuilder>();
    var dbCommandMock = new Mock<DbCommand>();

    commandBuilderMock.Setup(stub => stub.Create(_executionContextMock.Object)).Returns(dbCommandMock.Object);

    _executionContextMock.Setup(stub => stub.ExecuteNonQuery(dbCommandMock.Object)).Returns(1);

    var commandContext = new BatchedObjectsRdbmsProviderCommand(commandBuilderMock.Object, dataContainers);

    try
    {
      commandContext.Execute(_executionContextMock.Object);
      Assert.Fail($"Expected was a {nameof(ConcurrencyViolationException)} but no exception occured.");
    }
    catch (ConcurrencyViolationException cve)
    {
      Assert.That(cve.Message, Is.EqualTo($"Concurrency violation encountered. One or more object(s) have already been changed by someone else: {string.Join(", ", dataContainers.Take(10).Select(d => "'" + d.ID + "'"))}, ..."));
      Assert.That(cve.IDs, Is.EquivalentTo(dataContainers.Select(d => d.ID)));
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

    _executionContextMock.Setup(stub => stub.ExecuteNonQuery(dbCommandMock.Object)).Throws(new RdbmsProviderException("Original Message"));

    var commandContext = new BatchedObjectsRdbmsProviderCommand(commandBuilderMock.Object, [dataContainer1]);

    try
    {
      commandContext.Execute(_executionContextMock.Object);
      Assert.Fail($"Expected was a {nameof(RdbmsProviderException)} but no exception occured.");
    }
    catch (RdbmsProviderException rdbms)
    {
      Assert.That(rdbms.InnerException, Is.TypeOf<RdbmsProviderException>());
      Assert.That(rdbms.InnerException!.Message, Is.EqualTo("Original Message"));
      Assert.That(rdbms.Message, Is.EqualTo($"Error while saving objects '{dataContainer1.ID}'. Original Message"));
    }
    catch (Exception ex)
    {
      Assert.Fail($"Expected was {nameof(RdbmsProviderException)} but got '{ex}'");
    }
  }

  [Test]
  public void Execute_RdbmsProviderException_AndAddsMaximal10ObjectIDsToErrorMessage ()
  {
    var dataContainers = new List<DataContainer>();
    for (int i = 0; i < 11; i++)
    {
      var dataContainer = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
      dataContainers.Add(dataContainer);
    }

    var commandBuilderMock = new Mock<IDbCommandBuilder>();
    var dbCommandMock = new Mock<DbCommand>();

    commandBuilderMock.Setup(stub => stub.Create(_executionContextMock.Object)).Returns(dbCommandMock.Object);

    _executionContextMock.Setup(stub => stub.ExecuteNonQuery(dbCommandMock.Object)).Throws(new RdbmsProviderException("Original Message"));

    var commandContext = new BatchedObjectsRdbmsProviderCommand(commandBuilderMock.Object, dataContainers);

    try
    {
      commandContext.Execute(_executionContextMock.Object);
      Assert.Fail($"Expected was a {nameof(RdbmsProviderException)} but no exception occured.");
    }
    catch (RdbmsProviderException rdbms)
    {
      Assert.That(rdbms.InnerException, Is.TypeOf<RdbmsProviderException>());
      Assert.That(rdbms.InnerException!.Message, Is.EqualTo("Original Message"));
      Assert.That(rdbms.Message, Is.EqualTo($"Error while saving objects '{string.Join(", ", dataContainers.Take(10).Select(d => d.ID))}, ...'. Original Message"));
      Assert.That(rdbms.Message, Is.Not.EqualTo($"Error while saving objects '{string.Join(", ", dataContainers.Select(d => d.ID))}, ...'. Original Message"));
    }
    catch (Exception ex)
    {
      Assert.Fail($"Expected was {nameof(RdbmsProviderException)} but got '{ex}'");
    }
  }
}
