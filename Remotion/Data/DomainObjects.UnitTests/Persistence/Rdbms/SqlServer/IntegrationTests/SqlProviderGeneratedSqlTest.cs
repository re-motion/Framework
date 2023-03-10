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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
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

      _testHelper = new SqlProviderGeneratedSqlTestHelper(TestDomainStorageProviderDefinition);
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
    public void LoadDataContainers_MultiIDs_SameTable ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] "
          + "WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create("@ID", DbType.Xml, (object)"<L><I>c7c26bf5-871d-48c7-822a-e9b05aac4e5a</I><I>176a0ff6-296d-4934-bd1a-23cf52c22411</I></L>"));

      _testHelper.Provider.LoadDataContainers(new[] { DomainObjectIDs.Computer1, DomainObjectIDs.Computer2 }).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainers_MultiIDs_DifferentTables ()
    {
      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] "
          + "WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create("@ID", DbType.Xml, (object)"<L><I>c7c26bf5-871d-48c7-822a-e9b05aac4e5a</I><I>176a0ff6-296d-4934-bd1a-23cf52c22411</I></L>"));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID] FROM [Employee] "
          + "WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create("@ID", DbType.Xml, (object)"<L><I>51ece39b-f040-45b0-8b72-ad8b45353990</I><I>c3b2bbc3-e083-4974-bac7-9cee1fb85a5e</I></L>"));

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
                  QueryType.Collection),
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
                  QueryType.Scalar),
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
      SetPropertyValue(newDataContainer, typeof(Employee), "Name", "Anonymous");
      var changedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee1);
      SetPropertyValue(changedDataContainer, typeof(Employee), "Name", "George");
      var markedAsChangedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee2);
      markedAsChangedDataContainer.MarkAsChanged();
      var unchangedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee3);
      var deletedDataContainer = _testHelper.LoadDataContainerInSeparateProvider(DomainObjectIDs.Employee7);
      SetPropertyValue(deletedDataContainer, typeof(Employee), "Supervisor", null);
      deletedDataContainer.Delete();

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteNonQuery(
          sequence,
          "INSERT INTO [Employee] ([ID], [ClassID], [Name]) VALUES (@ID, @ClassID, @Name);",
          Tuple.Create("@ID", DbType.Guid, newDataContainer.ID.Value),
          Tuple.Create("@ClassID", DbType.AnsiString, (object)"Employee"),
          Tuple.Create("@Name", DbType.String, (object)"Anonymous"));
      _testHelper.ExpectExecuteNonQuery(
          sequence,
          "UPDATE [Employee] SET [Name] = @Name WHERE [ID] = @ID AND [Timestamp] = @Timestamp;",
          Tuple.Create("@Name", DbType.String, (object)"George"),
          Tuple.Create("@ID", DbType.Guid, changedDataContainer.ID.Value),
          Tuple.Create("@Timestamp", DbType.Binary, changedDataContainer.Timestamp)
          );
      _testHelper.ExpectExecuteNonQuery(
          sequence,
          "UPDATE [Employee] SET [SupervisorID] = @SupervisorID WHERE [ID] = @ID;",
          Tuple.Create("@SupervisorID", DbType.Guid, (object)DBNull.Value),
          Tuple.Create("@ID", DbType.Guid, newDataContainer.ID.Value));
      _testHelper.ExpectExecuteNonQuery(
          sequence,
          "UPDATE [Employee] SET [SupervisorID] = @SupervisorID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;",
          Tuple.Create("@SupervisorID", DbType.Guid, (object)DBNull.Value),
          Tuple.Create("@ID", DbType.Guid, deletedDataContainer.ID.Value),
          Tuple.Create("@Timestamp", DbType.Binary, deletedDataContainer.Timestamp));
      _testHelper.ExpectExecuteNonQuery(
          sequence,
          "UPDATE [Employee] SET [ClassID] = @ClassID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;",
          Tuple.Create("@ClassID", DbType.AnsiString, (object)"Employee"),
          Tuple.Create("@ID", DbType.Guid, markedAsChangedDataContainer.ID.Value),
          Tuple.Create("@Timestamp", DbType.Binary, markedAsChangedDataContainer.Timestamp));
      _testHelper.ExpectExecuteNonQuery(
          sequence,
          "DELETE FROM [Employee] WHERE [ID] = @ID;",
          Tuple.Create("@ID", DbType.Guid, deletedDataContainer.ID.Value));

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

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create("@ID", DbType.Xml, (object)"<L><I>51ece39b-f040-45b0-8b72-ad8b45353990</I><I>c3b2bbc3-e083-4974-bac7-9cee1fb85a5e</I></L>"));

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

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create("@ID", DbType.Xml, (object)"<L><I>51ece39b-f040-45b0-8b72-ad8b45353990</I><I>c3b2bbc3-e083-4974-bac7-9cee1fb85a5e</I></L>"));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Company] WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create("@ID", DbType.Xml, (object)"<L><I>55b52e75-514b-4e82-a91b-8f0bb59b80ad</I><I>5587a9c0-be53-477d-8c0a-4803c7fae1a9</I></L>"));

      _testHelper.Provider.UpdateTimestamps(new[] { dataContainer1, dataContainer2, dataContainer3, dataContainer4 });

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }
  }
}
