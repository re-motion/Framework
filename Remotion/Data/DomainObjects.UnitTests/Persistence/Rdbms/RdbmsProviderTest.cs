// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using SortOrder = Remotion.Data.DomainObjects.Mapping.SortExpressions.SortOrder;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderTest : StandardMappingTest
  {
    public interface IConnectionCreator
    {
      IDbConnection CreateConnection ();
    }
    private Mock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>> _commandFactoryMock;
    private Mock<IDbConnection> _connectionStub;
    private Mock<IDbTransaction> _transactionStub;
    private Mock<IDbCommand> _commandMock;

    private Mock<IConnectionCreator> _connectionCreatorMock;
    private TestableRdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();

      _commandFactoryMock = new Mock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      _connectionStub = new Mock<IDbConnection>();
      _connectionStub.Setup(stub => stub.State).Returns(ConnectionState.Open);
      _transactionStub = new Mock<IDbTransaction>();
      _commandMock = new Mock<IDbCommand>(MockBehavior.Strict);

      _connectionCreatorMock = new Mock<IConnectionCreator>(MockBehavior.Strict);

      _provider = new TestableRdbmsProvider(
          TestDomainStorageProviderDefinition,
          NullPersistenceExtension.Instance,
          _commandFactoryMock.Object,
          () => _connectionCreatorMock.Object.CreateConnection());
    }

    [Test]
    public void BeginTransaction_Twice ()
    {
      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _connectionStub.Setup(stub => stub.BeginTransaction(IsolationLevel.Serializable)).Returns(_transactionStub.Object);

      _provider.BeginTransaction();
      Assert.That(
          () => _provider.BeginTransaction(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot call BeginTransaction when a transaction is already in progress."));
    }

    [Test]
    public void UpdateTimestamps ()
    {
      var dataContainer1 = DataContainer.CreateNew(DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew(DomainObjectIDs.Order3);
      var timestamp1 = new object();
      var timestamp2 = new object();

      var commandMock =
          new Mock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateForMultiTimestampLookup(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.Execute(_provider))
          .Returns(
              new[]
              {
                  new ObjectLookupResult<object>(DomainObjectIDs.Order1, timestamp1),
                  new ObjectLookupResult<object>(DomainObjectIDs.Order3, timestamp2)
              })
          .Verifiable();

      _provider.UpdateTimestamps(new DataContainerCollection(new[] { dataContainer1, dataContainer2 }, true));

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(dataContainer1.Timestamp, Is.SameAs(timestamp1));
      Assert.That(dataContainer2.Timestamp, Is.SameAs(timestamp2));
    }

    [Test]
    public void UpdateTimestamps_ObjectIDCannotBeFound ()
    {
      var dataContainer1 = DataContainer.CreateNew(DomainObjectIDs.Order1);
      var timestamp2 = new object();

      var commandMock =
          new Mock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .Setup(mock => mock.CreateForMultiTimestampLookup(new[] { DomainObjectIDs.Order1 }))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock
          .Setup(stub => stub.Execute(_provider))
          .Returns(new[] { new ObjectLookupResult<object>(DomainObjectIDs.Order3, timestamp2) })
          .Verifiable();

      Assert.That(
          () => _provider.UpdateTimestamps(new DataContainerCollection(new[] { dataContainer1 }, true)),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("No timestamp found for object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'."));
    }

    [Test]
    public void ExecuteCollectionQuery ()
    {
      var dataContainer1 = DataContainer.CreateNew(DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew(DomainObjectIDs.Order3);

      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.StorageProviderDefinition).Returns(TestDomainStorageProviderDefinition);
      queryStub.Setup(stub => stub.QueryType).Returns(QueryType.Collection);
      var commandMock = new Mock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();

      _commandFactoryMock
            .Setup(mock => mock.CreateForDataContainerQuery(queryStub.Object))
            .Returns(commandMock.Object)
            .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(new[] { dataContainer1, dataContainer2 }).Verifiable();

      var result = _provider.ExecuteCollectionQuery(queryStub.Object);

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(new[] { dataContainer1, dataContainer2 }));
    }

    [Test]
    public void ExecuteCollectionQuery_DuplicatedIDs ()
    {
      var dataContainer1 = DataContainer.CreateNew(DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew(DomainObjectIDs.Order1);

      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.StorageProviderDefinition).Returns(TestDomainStorageProviderDefinition);
      queryStub.Setup(stub => stub.QueryType).Returns(QueryType.Collection);
      var commandMock = new Mock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();

      _commandFactoryMock
            .Setup(mock => mock.CreateForDataContainerQuery(queryStub.Object))
            .Returns(commandMock.Object)
            .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(new[] { dataContainer1, dataContainer2 }).Verifiable();
      Assert.That(
          () => _provider.ExecuteCollectionQuery(queryStub.Object),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "A database query returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed."));
      sequence.Verify();
    }

    [Test]
    public void ExecuteCollectionQuery_DuplicateNullValues ()
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.StorageProviderDefinition).Returns(TestDomainStorageProviderDefinition);
      queryStub.Setup(stub => stub.QueryType).Returns(QueryType.Collection);
      var commandMock = new Mock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();

      _commandFactoryMock
          .InVerifiableSequence(sequence)
            .Setup(mock => mock.CreateForDataContainerQuery(queryStub.Object))
            .Returns(commandMock.Object)
            .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(new DataContainer[] { null, null }).Verifiable();

      var result = _provider.ExecuteCollectionQuery(queryStub.Object);

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(new DataContainer[] { null, null }));
    }

    [Test]
    public void ExecuteCustomQuery ()
    {
      var fakeResult = new Mock<IEnumerable<IQueryResultRow>>(MockBehavior.Strict);

      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.StorageProviderDefinition).Returns(TestDomainStorageProviderDefinition);
      queryStub.Setup(stub => stub.QueryType).Returns(QueryType.Custom);
      var commandMock = new Mock<IStorageProviderCommand<IEnumerable<IQueryResultRow>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();

      _commandFactoryMock
          .InVerifiableSequence(sequence)
            .Setup(mock => mock.CreateForCustomQuery(queryStub.Object))
            .Returns(commandMock.Object)
            .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(fakeResult.Object).Verifiable();

      var result = _provider.ExecuteCustomQuery(queryStub.Object);

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));
      fakeResult.Verify(mock => mock.GetEnumerator(), Times.Never());
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var fakeResult = new object();

      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.StorageProviderDefinition).Returns(TestDomainStorageProviderDefinition);
      queryStub.Setup(stub => stub.QueryType).Returns(QueryType.Scalar);
      var commandMock = new Mock<IStorageProviderCommand<object, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();

      _commandFactoryMock
          .InVerifiableSequence(sequence)
            .Setup(mock => mock.CreateForScalarQuery(queryStub.Object))
            .Returns(commandMock.Object)
            .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(fakeResult).Verifiable();

      var result = _provider.ExecuteScalarQuery(queryStub.Object);

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void LoadDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var fakeResult = new ObjectLookupResult<DataContainer>(objectID, DataContainer.CreateNew(objectID));

      var commandMock =
          new Mock<IStorageProviderCommand<ObjectLookupResult<DataContainer>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);
      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateForSingleIDLookup(objectID))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(fakeResult).Verifiable();

      var result = _provider.LoadDataContainer(objectID);

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(fakeResult));
    }

    [Test]
    public void LoadDataContainer_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;

      Assert.That(
          () => _provider.LoadDataContainer(objectID),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The StorageProvider 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider 'TestDomain'.", "id"));
    }

    [Test]
    public void LoadDataContainer_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;

      _provider.Dispose();

      Assert.That(
          () => _provider.LoadDataContainer(objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainers ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order3;
      var objectID3 = DomainObjectIDs.Order4;

      var lookupResult1 = new ObjectLookupResult<DataContainer>(objectID1, DataContainer.CreateNew(objectID1));
      var lookupResult2 = new ObjectLookupResult<DataContainer>(objectID2, DataContainer.CreateNew(objectID2));
      var lookupResult3 = new ObjectLookupResult<DataContainer>(objectID3, DataContainer.CreateNew(objectID3));

      var commandMock =
          new Mock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);
      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateForSortedMultiIDLookup(new[] { objectID1, objectID2, objectID3 }))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(new[] { lookupResult1, lookupResult2, lookupResult3 }).Verifiable();

      var result = _provider.LoadDataContainers(new[] { objectID1, objectID2, objectID3 }).ToArray();

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(new[] { lookupResult1, lookupResult2, lookupResult3 }));
    }

    [Test]
    public void LoadDataContainers_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;

      _provider.Dispose();

      Assert.That(
          () => _provider.LoadDataContainers(new[] { objectID }),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainers_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;
      var commandMock =
          new Mock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);
      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .Setup(mock => mock.CreateForSortedMultiIDLookup(new[] { objectID }))
          .Returns(commandMock.Object)
          .Verifiable();

      Assert.That(
          () => _provider.LoadDataContainers(new[] { objectID }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The StorageProvider 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider 'TestDomain'.", "ids"));
    }

    [Test]
    public void LoadDataContainersByRelatedID ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(Order), "Official");
      var sortExpression = new SortExpressionDefinition(
          new[] { new SortedPropertySpecification(GetPropertyDefinition(typeof(Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew(objectID);

      var commandMock =
          new Mock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);
      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateForRelationLookup(relationEndPointDefinition, objectID, sortExpression))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(new[] { fakeResult }).Verifiable();

      var result = _provider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpression, objectID);

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(new[] { fakeResult }));
    }

    [Test]
    public void LoadDataContainersByRelatedID_NullContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(Order), "Official");
      var sortExpression = new SortExpressionDefinition(
          new[] { new SortedPropertySpecification(GetPropertyDefinition(typeof(Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew(objectID);

      var commandMock =
          new Mock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);
      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateForRelationLookup(relationEndPointDefinition, objectID, sortExpression))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Returns(new[] { fakeResult, null }).Verifiable();
      Assert.That(
          () => _provider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpression, objectID),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "A relation lookup returned a NULL ID, which is not allowed."));
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainersByRelatedID_Duplicates ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(Order), "Official");
      var sortExpression = new SortExpressionDefinition(
          new[] { new SortedPropertySpecification(GetPropertyDefinition(typeof(Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew(objectID);

      var commandMock = new Mock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);
      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .Setup(mock => mock.CreateForRelationLookup(relationEndPointDefinition, objectID, sortExpression))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock.Setup(mock => mock.Execute(_provider)).Returns(new[] { fakeResult, fakeResult }).Verifiable();

      Assert.That(
          () => _provider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpression, objectID),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "A relation lookup returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed."));
    }

    [Test]
    public void LoadDataContainersByRelatedID_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(Order), "Official");

      _provider.Dispose();

      Assert.That(
          () => _provider.LoadDataContainersByRelatedID(relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainersByRelatedID_ClassDefinitionWithDifferentStorageProviderDefinition ()
    {
      var sqlStorageObjectFactory = new SqlStorageObjectFactory(
          StorageSettings,
          ServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          ServiceLocator.Current.GetInstance<IDataContainerValidator>());

      var providerWithDifferentID = new RdbmsProvider(
          new RdbmsProviderDefinition("Test", sqlStorageObjectFactory, TestDomainConnectionString),
          NullPersistenceExtension.Instance,
          _commandFactoryMock.Object,
          () => new SqlConnection());
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(Order), "Official");

      Assert.That(
          () => providerWithDifferentID.LoadDataContainersByRelatedID(relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
              "The StorageProvider 'TestDomain' of the provided ClassDefinition does not match with this StorageProvider 'Test'.", "classDefinition"));
    }

    [Test]
    public void LoadDataContainersByRelatedID_StorageClassTransaction ()
    {
      var objectID = DomainObjectIDs.Order1;
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(objectID.ClassDefinition, "Test", StorageClass.Transaction);
      var relationEndPointDefinition = new RelationEndPointDefinition(propertyDefinition, true);

      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();

      var result = _provider.LoadDataContainersByRelatedID(relationEndPointDefinition, null, objectID);

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Save ()
    {
      var dataContainer1 = DataContainer.CreateNew(DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew(DomainObjectIDs.Order3);

      var commandMock = new Mock<IStorageProviderCommand<IRdbmsProviderCommandExecutionContext>>(MockBehavior.Strict);
      var sequence = new VerifiableSequence();
      _connectionCreatorMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object).Verifiable();
      _commandFactoryMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CreateForSave(new[] { dataContainer1, dataContainer2 }))
          .Returns(commandMock.Object)
          .Verifiable();
      commandMock.InVerifiableSequence(sequence).Setup(mock => mock.Execute(_provider)).Verifiable();

      _provider.Save(new DataContainerCollection(new[] { dataContainer1, dataContainer2 }, true));

      _commandFactoryMock.Verify();
      _connectionCreatorMock.Verify();
      commandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Save_Disposed ()
    {
      _provider.Dispose();

      Assert.That(
          () => _provider.Save(new DataContainerCollection()),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo(
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void CreateDbCommand_WithTransaction ()
    {
      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object);

      _connectionStub.Setup(stub => stub.BeginTransaction(_provider.IsolationLevel)).Returns(_transactionStub.Object);

      _provider.Connect();
      _provider.BeginTransaction();

      _connectionStub.Setup(stub => stub.CreateCommand()).Returns(_commandMock.Object);

      _commandMock.SetupSet(mock => mock.Connection = _connectionStub.Object).Verifiable();
      _commandMock.SetupSet(mock => mock.Transaction = _transactionStub.Object).Verifiable();

      var result = _provider.CreateDbCommand();

      _commandMock.Verify();
      Assert.That(result.WrappedInstance, Is.SameAs(_commandMock.Object));
    }

    [Test]
    public void CreateDbCommand_NoTransaction ()
    {
      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object);

      _provider.Connect();

      _connectionStub.Setup(stub => stub.CreateCommand()).Returns(_commandMock.Object);

      _commandMock.SetupSet(mock => mock.Connection = _connectionStub.Object).Verifiable();
      _commandMock.SetupSet(mock => mock.Transaction = null).Verifiable();

      var result = _provider.CreateDbCommand();

      _commandMock.Verify();
      Assert.That(result.WrappedInstance, Is.SameAs(_commandMock.Object));
    }

    [Test]
    public void CreateDbCommand_NoConnection ()
    {
      Assert.That(
          () => _provider.CreateDbCommand(),
          Throws.InvalidOperationException.With.Message.EqualTo("Connect must be called before a command can be created."));
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenConnectionSetFails ()
    {
      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object);

      _provider.Connect();

      _connectionStub.Setup(stub => stub.CreateCommand()).Returns(_commandMock.Object);

      var exception = new Exception();
      _commandMock.SetupSet(mock => mock.Connection = _connectionStub.Object).Throws(exception).Verifiable();
      _commandMock.Setup(mock => mock.Dispose()).Verifiable();

      Assert.That(() => _provider.CreateDbCommand(), Throws.Exception.SameAs(exception));

      _commandMock.Verify();
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenTransactionSetFails ()
    {
      _connectionCreatorMock.Setup(mock => mock.CreateConnection()).Returns(_connectionStub.Object);

      _provider.Connect();

      _connectionStub.Setup(stub => stub.CreateCommand()).Returns(_commandMock.Object);

      var exception = new Exception();
      _commandMock.SetupSet(mock => mock.Connection = _connectionStub.Object).Verifiable();
      _commandMock.SetupSet(mock => mock.Transaction = null).Throws(exception).Verifiable();
      _commandMock.Setup(mock => mock.Dispose()).Verifiable();

      Assert.That(() => _provider.CreateDbCommand(), Throws.Exception.SameAs(exception));

      _commandMock.Verify();
    }

    [Test]
    public void CreateDbCommand_ChecksDisposed ()
    {
      _provider.Dispose();
      Assert.That(
          () => _provider.CreateDbCommand(),
          Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void ExecuteReader ()
    {
      var dataReaderStub = new Mock<IDataReader>();
      _commandMock.Setup(mock => mock.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo)).Returns(dataReaderStub.Object).Verifiable();

      var result = _provider.ExecuteReader(_commandMock.Object, CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo);

      _commandMock.Verify();
      Assert.That(result, Is.SameAs(dataReaderStub.Object));
    }

    [Test]
    public void ExecuteReader_Exception ()
    {
      var exception = new Exception("Test");
      _commandMock.Setup(mock => mock.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo)).Throws(exception).Verifiable();

      Assert.That(() => _provider.ExecuteReader(_commandMock.Object, CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo),
          Throws.TypeOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while executing SQL command: Test")
              .And.InnerException.SameAs(exception));
    }

    [Test]
    public void ExecuteReader_ChecksDisposed ()
    {
      _provider.Dispose();
      Assert.That(
          () => _provider.ExecuteReader(_commandMock.Object, CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo),
          Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void ExecuteScalar ()
    {
      var fakeScalar = new object();
      _commandMock.Setup(mock => mock.ExecuteScalar()).Returns(fakeScalar).Verifiable();

      var result = _provider.ExecuteScalar(_commandMock.Object);

      _commandMock.Verify();
      Assert.That(result, Is.SameAs(fakeScalar));
    }

    [Test]
    public void ExecuteScalar_Exception ()
    {
      var exception = new Exception("Test");
      _commandMock.Setup(mock => mock.ExecuteScalar()).Throws(exception).Verifiable();

      Assert.That(() => _provider.ExecuteScalar(_commandMock.Object),
          Throws.TypeOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while executing SQL command: Test")
              .And.InnerException.SameAs(exception));
    }

    [Test]
    public void ExecuteScalar_ChecksDisposed ()
    {
      _provider.Dispose();
      Assert.That(
          () => _provider.ExecuteScalar(_commandMock.Object),
          Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void ExecuteNonQuery ()
    {
      _commandMock.Setup(mock => mock.ExecuteNonQuery()).Returns(12).Verifiable();

      var result = _provider.ExecuteNonQuery(_commandMock.Object);

      _commandMock.Verify();
      Assert.That(result, Is.EqualTo(12));
    }

    [Test]
    public void ExecuteNonQuery_Exception ()
    {
      var exception = new Exception("Test");
      _commandMock.Setup(mock => mock.ExecuteNonQuery()).Throws(exception).Verifiable();

      Assert.That(() => _provider.ExecuteNonQuery(_commandMock.Object),
          Throws.TypeOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while executing SQL command: Test")
              .And.InnerException.SameAs(exception));
    }

    [Test]
    public void ExecuteNonQuery_ChecksDisposed ()
    {
      _provider.Dispose();
      Assert.That(
          () => _provider.ExecuteNonQuery(_commandMock.Object),
          Throws.InstanceOf<ObjectDisposedException>());
    }
  }
}
