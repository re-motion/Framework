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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
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

    private MockRepository _mockRepository;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _commandFactoryMock;
    private IDbConnection _connectionStub;
    private IDbTransaction _transactionStub;
    private IDbCommand _commandMock;

    private IConnectionCreator _connectionCreatorMock;
    private TestableRdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();

      _commandFactoryMock = _mockRepository.StrictMock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();

      _connectionStub = MockRepository.GenerateStub<IDbConnection> ();
      _connectionStub.Stub (stub => stub.State).Return (ConnectionState.Open);
      _transactionStub = _mockRepository.Stub<IDbTransaction> ();
      _commandMock = MockRepository.GenerateStrictMock<IDbCommand> ();

      _connectionCreatorMock = _mockRepository.StrictMock<IConnectionCreator>();

      _provider = new TestableRdbmsProvider (
          TestDomainStorageProviderDefinition,
          NullPersistenceExtension.Instance,
          _commandFactoryMock,
          () => _connectionCreatorMock.CreateConnection());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot call BeginTransaction when a transaction is already in progress.")]
    public void BeginTransaction_Twice ()
    {
      _connectionCreatorMock.Expect (mock => mock.CreateConnection ()).Return (_connectionStub);
      _connectionStub.Stub (stub => stub.BeginTransaction (IsolationLevel.Serializable)).Return (_transactionStub);
      _mockRepository.ReplayAll ();

      _provider.BeginTransaction ();
      _provider.BeginTransaction ();
    }

    [Test]
    public void UpdateTimestamps ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order3);
      var timestamp1 = new object();
      var timestamp2 = new object();

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext>>();

      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForMultiTimestampLookup (
                Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 })))
            .Return (commandMock);
        commandMock
            .Expect (stub => stub.Execute (_provider))
            .Return (
                new[]
                {
                    new ObjectLookupResult<object> (DomainObjectIDs.Order1, timestamp1),
                    new ObjectLookupResult<object> (DomainObjectIDs.Order3, timestamp2)
                });
      }

      _mockRepository.ReplayAll();

      _provider.UpdateTimestamps (new DataContainerCollection (new[] { dataContainer1, dataContainer2 }, true));

      _mockRepository.VerifyAll ();
      Assert.That (dataContainer1.Timestamp, Is.SameAs (timestamp1));
      Assert.That (dataContainer2.Timestamp, Is.SameAs (timestamp2));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "No timestamp found for object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void UpdateTimestamps_ObjectIDCannotBeFound ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var timestamp2 = new object ();

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext>> ();

      _connectionCreatorMock.Expect (mock => mock.CreateConnection ()).Return (_connectionStub);
      _commandFactoryMock
          .Expect (mock => mock.CreateForMultiTimestampLookup (
              Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1 })))
          .Return (commandMock);
      commandMock
          .Expect (stub => stub.Execute (_provider))
          .Return (new[] { new ObjectLookupResult<object> (DomainObjectIDs.Order3, timestamp2) });

      _mockRepository.ReplayAll ();

      _provider.UpdateTimestamps (new DataContainerCollection (new[] { dataContainer1 }, true));
    }

    [Test]
    public void ExecuteCollectionQuery ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order3);

      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      queryStub.Stub (stub => stub.QueryType).Return (QueryType.Collection);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();

      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForDataContainerQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { dataContainer1, dataContainer2 });
      }
      _mockRepository.ReplayAll();

      var result = _provider.ExecuteCollectionQuery (queryStub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { dataContainer1, dataContainer2 }));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "A database query returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed.")]
    public void ExecuteCollectionQuery_DuplicatedIDs ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order1);

      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      queryStub.Stub (stub => stub.QueryType).Return (QueryType.Collection);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>> ();

      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForDataContainerQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { dataContainer1, dataContainer2 });
      }
      _mockRepository.ReplayAll();

      _provider.ExecuteCollectionQuery (queryStub);
    }

    [Test]
    public void ExecuteCollectionQuery_DuplicateNullValues ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      queryStub.Stub (stub => stub.QueryType).Return (QueryType.Collection);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>> ();

      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForDataContainerQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new DataContainer[] { null, null });
      }
      _mockRepository.ReplayAll();

      var result = _provider.ExecuteCollectionQuery (queryStub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new DataContainer[] { null, null }));
    }

    [Test]
    public void ExecuteCustomQuery ()
    {
      var fakeResult = MockRepository.GenerateStrictMock<IEnumerable<IQueryResultRow>>();

      var queryStub = MockRepository.GenerateStub<IQuery> ();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      queryStub.Stub (stub => stub.QueryType).Return (QueryType.Custom);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<IQueryResultRow>, IRdbmsProviderCommandExecutionContext>> ();

      using (_mockRepository.Ordered ())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection ()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForCustomQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (fakeResult);
      }
      _mockRepository.ReplayAll ();

      var result = _provider.ExecuteCustomQuery (queryStub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs(fakeResult));
      fakeResult.AssertWasNotCalled (mock => mock.GetEnumerator());
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var fakeResult = new object();

      var queryStub = MockRepository.GenerateStub<IQuery> ();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      queryStub.Stub (stub => stub.QueryType).Return (QueryType.Scalar);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<object, IRdbmsProviderCommandExecutionContext>> ();

      using (_mockRepository.Ordered ())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection ()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForScalarQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (fakeResult);
      }
      _mockRepository.ReplayAll ();

      var result = _provider.ExecuteScalarQuery (queryStub);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void LoadDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var fakeResult = new ObjectLookupResult<DataContainer> (objectID, DataContainer.CreateNew (objectID));

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<ObjectLookupResult<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForSingleIDLookup (objectID))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (fakeResult);
      }
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainer (objectID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void LoadDataContainer_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;
      _mockRepository.ReplayAll();

      Assert.That (
          () => _provider.LoadDataContainer (objectID),
          Throws.ArgumentException.With.Message.EqualTo (
              "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider's ID 'TestDomain'.\r\nParameter name: id"));
    }

    [Test]
    public void LoadDataContainer_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.LoadDataContainer (objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainers ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order3;
      var objectID3 = DomainObjectIDs.Order4;

      var lookupResult1 = new ObjectLookupResult<DataContainer> (objectID1, DataContainer.CreateNew (objectID1));
      var lookupResult2 = new ObjectLookupResult<DataContainer> (objectID2, DataContainer.CreateNew (objectID2));
      var lookupResult3 = new ObjectLookupResult<DataContainer> (objectID3, DataContainer.CreateNew (objectID3));

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForSortedMultiIDLookup (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { objectID1, objectID2, objectID3 })))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { lookupResult1, lookupResult2, lookupResult3 });
      }
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainers (new[] { objectID1, objectID2, objectID3 }).ToArray();

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { lookupResult1, lookupResult2, lookupResult3 }));
    }

    [Test]
    public void LoadDataContainers_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.LoadDataContainers (new[] { objectID }),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainers_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;
      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext>>();
      _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
      _commandFactoryMock
          .Expect (mock => mock.CreateForSortedMultiIDLookup (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { objectID })))
          .Return (commandMock);
      _mockRepository.ReplayAll();

      Assert.That (
          () => _provider.LoadDataContainers (new[] { objectID }),
          Throws.ArgumentException.With.Message.EqualTo (
              "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider's ID 'TestDomain'.\r\nParameter name: ids"));
    }

    [Test]
    public void LoadDataContainersByRelatedID ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      var sortExpression = new SortExpressionDefinition (
          new[] { new SortedPropertySpecification (GetPropertyDefinition (typeof (Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew (objectID);

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForRelationLookup (relationEndPointDefinition, objectID, sortExpression))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { fakeResult });
      }
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, sortExpression, objectID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { fakeResult }));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A relation lookup returned a NULL ID, which is not allowed.")]
    public void LoadDataContainersByRelatedID_NullContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      var sortExpression = new SortExpressionDefinition (
          new[] { new SortedPropertySpecification (GetPropertyDefinition (typeof (Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew (objectID);

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForRelationLookup (relationEndPointDefinition, objectID, sortExpression))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { fakeResult, null });
      }
      _mockRepository.ReplayAll();

      _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, sortExpression, objectID);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "A relation lookup returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed.")]
    public void LoadDataContainersByRelatedID_Duplicates ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      var sortExpression = new SortExpressionDefinition (
          new[] { new SortedPropertySpecification (GetPropertyDefinition (typeof (Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew (objectID);

      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
      _commandFactoryMock
          .Expect (mock => mock.CreateForRelationLookup (relationEndPointDefinition, objectID, sortExpression))
          .Return (commandMock);
      commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { fakeResult, fakeResult });

      _mockRepository.ReplayAll();

      _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, sortExpression, objectID);
    }

    [Test]
    public void LoadDataContainersByRelatedID_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainersByRelatedID_ClassDefinitionWithDifferentStorageProviderDefinition ()
    {
      var providerWithDifferentID = new RdbmsProvider (
          new RdbmsProviderDefinition ("Test", new SqlStorageObjectFactory(), TestDomainConnectionString),
          NullPersistenceExtension.Instance,
          _commandFactoryMock,
          () => new SqlConnection());
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");

      _mockRepository.ReplayAll();

      Assert.That (
          () => providerWithDifferentID.LoadDataContainersByRelatedID (relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ArgumentException>().With.Message.EqualTo (
              "The StorageProviderID 'TestDomain' of the provided ClassDefinition does not match with this StorageProvider's ID 'Test'.\r\nParameter name: classDefinition"));
    }

    [Test]
    public void LoadDataContainersByRelatedID_StorageClassTransaction ()
    {
      var objectID = DomainObjectIDs.Order1;
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (objectID.ClassDefinition, "Test", StorageClass.Transaction);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, true);

      _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, null, objectID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Save ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order3);

      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForSave (Arg<DataContainer[]>.Is.Equal (new[] { dataContainer1, dataContainer2 })))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider));
      }
      _mockRepository.ReplayAll();

      _provider.Save (new DataContainerCollection (new[] { dataContainer1, dataContainer2 }, true));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Save_Disposed ()
    {
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.Save (new DataContainerCollection()),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void CreateDbCommand_WithTransaction ()
    {
      _connectionCreatorMock.Stub (mock => mock.CreateConnection()).Return (_connectionStub);
      _connectionCreatorMock.Replay();

      _connectionStub.Stub (stub => stub.BeginTransaction (_provider.IsolationLevel)).Return (_transactionStub);

      _provider.Connect();
      _provider.BeginTransaction();

      _connectionStub.Stub (stub => stub.CreateCommand()).Return (_commandMock);

      _commandMock.Expect (mock => mock.Connection = _connectionStub);
      _commandMock.Expect (mock => mock.Transaction = _transactionStub);
      _commandMock.Replay();

      var result = _provider.CreateDbCommand ();

      _commandMock.VerifyAllExpectations();
      Assert.That (result.WrappedInstance, Is.SameAs (_commandMock));
    }

    [Test]
    public void CreateDbCommand_NoTransaction ()
    {
      _connectionCreatorMock.Stub (mock => mock.CreateConnection ()).Return (_connectionStub);
      _connectionCreatorMock.Replay ();

      _provider.Connect ();

      _connectionStub.Stub (stub => stub.CreateCommand ()).Return (_commandMock);

      _commandMock.Expect (mock => mock.Connection = _connectionStub);
      _commandMock.Expect (mock => mock.Transaction = null);
      _commandMock.Replay ();

      var result = _provider.CreateDbCommand ();

      _commandMock.VerifyAllExpectations ();
      Assert.That (result.WrappedInstance, Is.SameAs (_commandMock));
    }

    [Test]
    public void CreateDbCommand_NoConnection ()
    {
      Assert.That (
          () => _provider.CreateDbCommand (),
          Throws.InvalidOperationException.With.Message.EqualTo ("Connect must be called before a command can be created."));
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenConnectionSetFails ()
    {
      _connectionCreatorMock.Stub (mock => mock.CreateConnection ()).Return (_connectionStub);
      _connectionCreatorMock.Replay ();

      _provider.Connect ();

      _connectionStub.Stub (stub => stub.CreateCommand ()).Return (_commandMock);

      var exception = new Exception();
      _commandMock.Expect (mock => mock.Connection = _connectionStub).Throw (exception);
      _commandMock.Expect (mock => mock.Dispose());
      _commandMock.Replay ();

      Assert.That (() => _provider.CreateDbCommand(), Throws.Exception.SameAs (exception));

      _commandMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenTransactionSetFails ()
    {
      _connectionCreatorMock.Stub (mock => mock.CreateConnection ()).Return (_connectionStub);
      _connectionCreatorMock.Replay ();

      _provider.Connect ();

      _connectionStub.Stub (stub => stub.CreateCommand ()).Return (_commandMock);

      var exception = new Exception ();
      _commandMock.Expect (mock => mock.Connection = _connectionStub);
      _commandMock.Expect (mock => mock.Transaction = null).Throw (exception);
      _commandMock.Expect (mock => mock.Dispose ());
      _commandMock.Replay ();

      Assert.That (() => _provider.CreateDbCommand (), Throws.Exception.SameAs (exception));

      _commandMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void CreateDbCommand_ChecksDisposed ()
    {
      _provider.Dispose ();
      _provider.CreateDbCommand ();
    }

    [Test]
    public void ExecuteReader ()
    {
      var dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _commandMock.Expect (mock => mock.ExecuteReader (CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo)).Return (dataReaderStub);
      _commandMock.Replay();

      var result = _provider.ExecuteReader (_commandMock, CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo);

      _commandMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (dataReaderStub));
    }

    [Test]
    public void ExecuteReader_Exception ()
    {
      var exception = new Exception("Test");
      _commandMock.Expect (mock => mock.ExecuteReader (CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo)).Throw (exception);
      _commandMock.Replay ();

      Assert.That (() => _provider.ExecuteReader (_commandMock, CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo), 
          Throws.TypeOf<RdbmsProviderException>()
              .With.Message.EqualTo ("Error while executing SQL command: Test")
              .And.InnerException.SameAs (exception));
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void ExecuteReader_ChecksDisposed ()
    {
      _provider.Dispose ();
      _provider.ExecuteReader (_commandMock, CommandBehavior.SequentialAccess | CommandBehavior.KeyInfo);
    }

    [Test]
    public void ExecuteScalar ()
    {
      var fakeScalar = new object();
      _commandMock.Expect (mock => mock.ExecuteScalar ()).Return (fakeScalar);
      _commandMock.Replay ();

      var result = _provider.ExecuteScalar (_commandMock);

      _commandMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeScalar));
    }

    [Test]
    public void ExecuteScalar_Exception ()
    {
      var exception = new Exception ("Test");
      _commandMock.Expect (mock => mock.ExecuteScalar ()).Throw (exception);
      _commandMock.Replay ();

      Assert.That (() => _provider.ExecuteScalar (_commandMock),
          Throws.TypeOf<RdbmsProviderException> ()
              .With.Message.EqualTo ("Error while executing SQL command: Test")
              .And.InnerException.SameAs (exception));
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void ExecuteScalar_ChecksDisposed ()
    {
      _provider.Dispose ();
      _provider.ExecuteScalar (_commandMock);
    }

    [Test]
    public void ExecuteNonQuery ()
    {
      _commandMock.Expect (mock => mock.ExecuteNonQuery ()).Return (12);
      _commandMock.Replay ();

      var result = _provider.ExecuteNonQuery (_commandMock);

      _commandMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (12));
    }

    [Test]
    public void ExecuteNonQuery_Exception ()
    {
      var exception = new Exception ("Test");
      _commandMock.Expect (mock => mock.ExecuteNonQuery ()).Throw (exception);
      _commandMock.Replay ();

      Assert.That (() => _provider.ExecuteNonQuery (_commandMock),
          Throws.TypeOf<RdbmsProviderException> ()
              .With.Message.EqualTo ("Error while executing SQL command: Test")
              .And.InnerException.SameAs (exception));
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void ExecuteNonQuery_ChecksDisposed ()
    {
      _provider.Dispose ();
      _provider.ExecuteNonQuery (_commandMock);
    }
  }
}