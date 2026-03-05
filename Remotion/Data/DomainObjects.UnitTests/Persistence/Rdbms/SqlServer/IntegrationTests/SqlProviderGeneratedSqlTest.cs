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
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient.Server;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderGeneratedSqlTest : StandardMappingTest
  {
    private SqlProviderGeneratedSqlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new SqlProviderGeneratedSqlTestHelper(StorageSettings, TestDomainStorageProviderDefinition);
    }

    public override void TearDown ()
    {
      _testHelper.Dispose();
      base.TearDown();
    }

    [Test]
    public void LoadDataContainer ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleRow,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] WHERE [ID] = @ID;",
          Tuple.Create("@ID", DbType.Guid, DomainObjectIDs.Computer1.Value));

      _testHelper.Provider.LoadDataContainer(DomainObjectIDs.Computer1);

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainers_SingleID ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] WHERE [ID] = @ID;",
          Tuple.Create("@ID", DbType.Guid, DomainObjectIDs.Computer1.Value));

      _testHelper.Provider.LoadDataContainers(new[] { DomainObjectIDs.Computer1 }).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainers_EmptyIDCollection_DoesNotExecuteAnySqlCommand ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.Provider.LoadDataContainers(Array.Empty<ObjectID>()).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainers_MultiIDs_SameTable ()
    {
      var expectedTvpValue1 = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue1.AddRecord(DomainObjectIDs.Computer1.Value);
      expectedTvpValue1.AddRecord(DomainObjectIDs.Computer2.Value);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] "
          + "WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create("@ID", DbType.Object, (object)expectedTvpValue1));

      _testHelper.Provider.LoadDataContainers(new[] { DomainObjectIDs.Computer1, DomainObjectIDs.Computer2 }).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainers_MultiIDs_DifferentTables ()
    {
      var expectedTvpValue1 = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue1.AddRecord(DomainObjectIDs.Computer1.Value);
      expectedTvpValue1.AddRecord(DomainObjectIDs.Computer2.Value);
      var expectedTvpValue2 = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue2.AddRecord(DomainObjectIDs.Employee1.Value);
      expectedTvpValue2.AddRecord(DomainObjectIDs.Employee2.Value);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] "
          + "WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create("@ID", DbType.Object, (object)expectedTvpValue1));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID] FROM [Employee] "
          + "WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create("@ID", DbType.Object, (object)expectedTvpValue2));

      _testHelper.Provider.LoadDataContainers(
          new[] { DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Employee1, DomainObjectIDs.Employee2 }).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainersByRelatedID_NoSortExpression ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops], [SupplierQuality], [CustomerSince], [CustomerType] "
          + "FROM [Company] WHERE [IndustrialSectorID] = @IndustrialSectorID;",
          Tuple.Create("@IndustrialSectorID", DbType.Guid, DomainObjectIDs.IndustrialSector1.Value));

      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(Company), "IndustrialSector");
      _testHelper.Provider.LoadDataContainersByRelatedID(relationEndPointDefinition, null, DomainObjectIDs.IndustrialSector1).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithSortExpression ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [ContactPersonID], [NumberOfShops], [SupplierQuality], [CustomerSince], [CustomerType] "
          + "FROM [Company] WHERE [IndustrialSectorID] = @IndustrialSectorID ORDER BY [CustomerSince] DESC, [Name] ASC;",
          Tuple.Create("@IndustrialSectorID", DbType.Guid, DomainObjectIDs.IndustrialSector1.Value));

      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(Company), "IndustrialSector");
      var sortExpression = new SortExpressionDefinition(
          new[]
          {
              new SortedPropertySpecification(GetPropertyDefinition(typeof(Customer), "CustomerSince"), SortOrder.Descending),
              new SortedPropertySpecification(GetPropertyDefinition(typeof(Company), "Name"), SortOrder.Ascending)
          });
      _testHelper.Provider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpression, DomainObjectIDs.IndustrialSector1).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void ExecuteCollectionQuery ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT * FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
          Tuple.Create("@p1", DbType.Int32, (object)1),
          Tuple.Create("@p2", DbType.Guid, DomainObjectIDs.Order3.Value),
          Tuple.Create("@p3", DbType.AnsiString, (object)DomainObjectIDs.Official1.ToString()),
          Tuple.Create("@p4", DbType.String, (object)DBNull.Value)
          );

      var query =
          new Query(
              new QueryDefinition(
                  "id",
                  TestDomainStorageProviderDefinition,
                  "SELECT * FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
                  QueryType.CollectionReadOnly),
              new QueryParameterCollection
              {
                  { "@p1", 1 },
                  { "@p2", DomainObjectIDs.Order3 },
                  { "@p3", DomainObjectIDs.Official1 },
                  { "@p4", null }
              });
      _testHelper.Provider.ExecuteCollectionQuery(query).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteScalar(
          sequence,
          "SELECT COUNT(*) FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
          Tuple.Create("@p1", DbType.Int32, (object)1),
          Tuple.Create("@p2", DbType.Guid, DomainObjectIDs.Order3.Value),
          Tuple.Create("@p3", DbType.AnsiString, (object)DomainObjectIDs.Official1.ToString()),
          Tuple.Create("@p4", DbType.String, (object)DBNull.Value)
          );

      var query =
          new Query(
              new QueryDefinition(
                  "id",
                  TestDomainStorageProviderDefinition,
                  "SELECT COUNT(*) FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
                  QueryType.ScalarReadOnly),
              new QueryParameterCollection
              {
                  { "@p1", 1 },
                  { "@p2", DomainObjectIDs.Order3 },
                  { "@p3", DomainObjectIDs.Official1 },
                  { "@p4", null }
              });
      _testHelper.Provider.ExecuteScalarQuery(query);

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void Save ()
    {
      var newGuid = new Guid("322D1DCB-19E4-49BA-90AB-7F5C9C8126E8");
      var newDataContainer = DataContainer.CreateNew(new ObjectID(Configuration.GetTypeDefinition(typeof(Employee)), newGuid));
      SetPropertyValue(newDataContainer, typeof(Employee), "Name", "");
      var changedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee1);
      SetPropertyValue(changedDataContainer, typeof(Employee), "Name", "George");
      var markedAsChangedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee2);
      markedAsChangedDataContainer.MarkAsChanged();
      var unchangedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee3);
      var deletedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee7);
      SetPropertyValue(deletedDataContainer, typeof(Employee), "Supervisor", null);
      deletedDataContainer.Delete();

      var expectedLockTvpValue = new SqlTableValuedParameterValue("TVP_AllTables_Lock", new[] { new SqlMetaData("ID", SqlDbType.UniqueIdentifier), new SqlMetaData("Timestamp", SqlDbType.VarBinary, 8) });
      expectedLockTvpValue.AddRecord([changedDataContainer.ID.Value, changedDataContainer.Timestamp]);
      expectedLockTvpValue.AddRecord([deletedDataContainer.ID.Value, deletedDataContainer.Timestamp]);
      expectedLockTvpValue.AddRecord([markedAsChangedDataContainer.ID.Value, markedAsChangedDataContainer.Timestamp]);


      var expectedInsertTvpValue = new SqlTableValuedParameterValue("TVP_Employee_Insert", new[]
                                                                                           { new SqlMetaData("ID", SqlDbType.UniqueIdentifier),
                                                                                             new SqlMetaData("ClassID", SqlDbType.VarChar, 100),
                                                                                             new SqlMetaData("Name", SqlDbType.NVarChar, 100),
                                                                                             new SqlMetaData("SupervisorID", SqlDbType.UniqueIdentifier)
                                                                                           });
      expectedInsertTvpValue.AddRecord([newDataContainer.ID.Value,newDataContainer.ID.ClassID, "", DBNull.Value]);

      var expectedUpdateTvpValue = new SqlTableValuedParameterValue("TVP_Employee_Update", new[]
                                                                                           {
                                                                                               new SqlMetaData("ID", SqlDbType.UniqueIdentifier),
                                                                                               new SqlMetaData("ClassID", SqlDbType.VarChar, 100),
                                                                                               new SqlMetaData("Name", SqlDbType.NVarChar, 100),
                                                                                               new SqlMetaData("Name__IsSet", SqlDbType.Bit),
                                                                                               new SqlMetaData("SupervisorID", SqlDbType.UniqueIdentifier)
                                                                                           });
      expectedUpdateTvpValue.AddRecord([changedDataContainer.ID.Value, changedDataContainer.ID.ClassID, "George", true, DBNull.Value]);
      expectedUpdateTvpValue.AddRecord([newDataContainer.ID.Value, newDataContainer.ID.ClassID, "", false, DBNull.Value]);
      expectedUpdateTvpValue.AddRecord([deletedDataContainer.ID.Value, deletedDataContainer.ID.ClassID, "", false, DBNull.Value]);
      expectedUpdateTvpValue.AddRecord([markedAsChangedDataContainer.ID.Value, markedAsChangedDataContainer.ID.ClassID, "", false, DBNull.Value]);

      var expectedDeleteTvpValue = new SqlTableValuedParameterValue("TVP_AllTables_Delete", new[]
                                                                                            {
                                                                                                new SqlMetaData("ID", SqlDbType.UniqueIdentifier)
                                                                                            });
      expectedDeleteTvpValue.AddRecord([deletedDataContainer.ID.Value]);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          expectedCommandBehavior:CommandBehavior.Default,
          "DECLARE @TransactionIsolationLevel int;\r\nDECLARE @IsReadCommittedSnapshotOn bit;\r\nSET @TransactionIsolationLevel = (SELECT [transaction_isolation_level] FROM [sys].[dm_exec_sessions] WHERE [session_id] = @@SPID);\r\nSET @IsReadCommittedSnapshotOn = (SELECT [is_read_committed_snapshot_on] FROM [sys].[databases] WHERE [database_id] = DB_ID());\r\nIF (@TransactionIsolationLevel = 2 AND @IsReadCommittedSnapshotOn = 1)\r\nBEGIN\r\nSELECT [P].[ID], [P].[Timestamp] FROM [Employee] [T] WITH(ROWLOCK, XLOCK, READPAST)\r\nRIGHT JOIN @TVP_Lock_Employee [P] ON [P].[ID] = [T].[ID] AND [P].[Timestamp] = [T].[Timestamp]\r\nWHERE [T].[ID] IS NULL;\r\nEND\r\nELSE\r\nBEGIN\r\nSELECT [P].[ID], [P].[Timestamp] FROM [Employee] [T] WITH(ROWLOCK, XLOCK)\r\nRIGHT JOIN @TVP_Lock_Employee [P] ON [P].[ID] = [T].[ID] AND [P].[Timestamp] = [T].[Timestamp]\r\nWHERE [T].[ID] IS NULL;\r\nEND",
          Tuple.Create("@TVP_Lock_Employee", DbType.Object, (object)expectedLockTvpValue));

      _testHelper.ExpectExecuteNonQuery(
          sequence,
          """
          INSERT INTO [Employee] ([ID], [ClassID], [Name], [SupervisorID])
          SELECT [ID], [ClassID], [Name], [SupervisorID] FROM @TVP_Insert_Employee;
          """,
          Tuple.Create("@TVP_Insert_Employee", DbType.Object, (object)expectedInsertTvpValue));

      _testHelper.ExpectExecuteNonQuery(
          sequence,
          """
          UPDATE [T]
          SET
          [T].[ID] = [P].[ID],
          [T].[ClassID] = [P].[ClassID],
          [T].[SupervisorID] = [P].[SupervisorID],
          [T].[Name] = CASE WHEN [P].[Name__IsSet] = 1 THEN [P].[Name] ELSE [T].[Name] END
          FROM [Employee] [T]
          INNER JOIN @TVP_Update_Employee [P] ON [P].[ID] = [T].[ID];
          """,
          Tuple.Create("@TVP_Update_Employee", DbType.Object, (object)expectedUpdateTvpValue)
          );

      _testHelper.ExpectExecuteNonQuery(
          sequence,
          """
          DELETE [T]
          FROM [Employee] [T]
          INNER JOIN @TVP_Delete_Employee [P] ON [P].[ID] = [T].[ID];
          """,
          Tuple.Create("@TVP_Delete_Employee", DbType.Object, (object)expectedDeleteTvpValue));

      _testHelper.Provider.Save(new[] { changedDataContainer, newDataContainer, deletedDataContainer, markedAsChangedDataContainer, unchangedDataContainer});

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void UpdateTimestamps_SingleID ()
    {
      var dataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee1);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] = @ID;",
          Tuple.Create("@ID", DbType.Guid, dataContainer.ID.Value));

      _testHelper.Provider.UpdateTimestamps(new[] { dataContainer });

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void UpdateTimestamps_MultipleIDs_SameTable ()
    {
      var dataContainer1 = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee1);
      var dataContainer2 = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee2);

      var expectedTvpValue1 = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue1.AddRecord(DomainObjectIDs.Employee1.Value);
      expectedTvpValue1.AddRecord(DomainObjectIDs.Employee2.Value);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create("@ID", DbType.Object, (object)expectedTvpValue1));

      _testHelper.Provider.UpdateTimestamps(new[] { dataContainer1, dataContainer2 });

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void UpdateTimestamps_MultipleIDs_MultipleTables ()
    {
      var dataContainer1 = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee1);
      var dataContainer2 = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee2);
      var dataContainer3 = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Customer1);
      var dataContainer4 = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Partner1);

      var expectedTvpValue1 = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue1.AddRecord(DomainObjectIDs.Employee1.Value);
      expectedTvpValue1.AddRecord(DomainObjectIDs.Employee2.Value);
      var expectedTvpValue2 = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue2.AddRecord(DomainObjectIDs.Customer1.Value);
      expectedTvpValue2.AddRecord(DomainObjectIDs.Partner1.Value);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create("@ID", DbType.Object, (object)expectedTvpValue1));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Company] WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create("@ID", DbType.Object, (object)expectedTvpValue2));

      _testHelper.Provider.UpdateTimestamps(new[] { dataContainer1, dataContainer2, dataContainer3, dataContainer4 });

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }
  }
}
