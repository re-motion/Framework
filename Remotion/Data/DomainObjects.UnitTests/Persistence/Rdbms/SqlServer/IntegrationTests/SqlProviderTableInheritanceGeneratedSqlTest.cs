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
using Microsoft.SqlServer.Server;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderTableInheritanceGeneratedSqlTest : TableInheritanceMappingTest
  {
    private SqlProviderGeneratedSqlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new SqlProviderGeneratedSqlTestHelper(StorageSettings, TableInheritanceTestDomainStorageProviderDefinition);
    }

    public override void TearDown ()
    {
      _testHelper.Dispose();
      base.TearDown();
    }

    [Test]
    public void LoadDataContainersByRelatedID_NoSortExpression ()
    {
      var expectedTvpValue = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue.AddRecord(DomainObjectIDs.PersonForUnidirectionalRelationTest.Value);
      expectedTvpValue.AddRecord(DomainObjectIDs.Customer.Value);
      expectedTvpValue.AddRecord(DomainObjectIDs.Person.Value);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID] FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID "
          + "UNION ALL SELECT [ID], [ClassID] FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID;",
          Tuple.Create("@ClientID", DbType.Guid, DomainObjectIDs.Client.Value));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], "
          + "[CustomerType], [CustomerSince], [RegionID] FROM [TableInheritance_Person] "
          + "WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create(
              "@ID",
              DbType.Object,
              (object)expectedTvpValue));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [Name] FROM [TableInheritance_OrganizationalUnit] WHERE [ID] = @ID;",
          Tuple.Create("@ID", DbType.Guid, DomainObjectIDs.OrganizationalUnit.Value));

      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(TIDomainBase), "Client");
      _testHelper.Provider.LoadDataContainersByRelatedID(relationEndPointDefinition, null, DomainObjectIDs.Client).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithSortExpression ()
    {
      var expectedTvpValue = new SqlTableValuedParameterValue("TVP_Guid", new[] { new SqlMetaData("Value", SqlDbType.UniqueIdentifier) });
      expectedTvpValue.AddRecord(DomainObjectIDs.Customer.Value);
      expectedTvpValue.AddRecord(DomainObjectIDs.PersonForUnidirectionalRelationTest.Value);
      expectedTvpValue.AddRecord(DomainObjectIDs.Person.Value);

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [CreatedAt], [LastName] FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID "
          + "UNION ALL SELECT [ID], [ClassID], [CreatedAt], NULL FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID "
          + "ORDER BY [CreatedAt] DESC, [LastName] ASC;",
          Tuple.Create("@ClientID", DbType.Guid, DomainObjectIDs.Client.Value));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [FirstName], [LastName], [DateOfBirth], [Photo], "
          + "[CustomerType], [CustomerSince], [RegionID] FROM [TableInheritance_Person] "
          + "WHERE [ID] IN (SELECT [Value] FROM @ID);",
          Tuple.Create(
              "@ID",
              DbType.Object,
              (object)expectedTvpValue));
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [CreatedBy], [CreatedAt], [ClientID], [Name] FROM [TableInheritance_OrganizationalUnit] WHERE [ID] = @ID;",
          Tuple.Create("@ID", DbType.Guid, DomainObjectIDs.OrganizationalUnit.Value));

      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(TIDomainBase), "Client");
      var sortExpression = new SortExpressionDefinition(
          new[]
          {
              new SortedPropertySpecification(GetPropertyDefinition(typeof(TIDomainBase), "CreatedAt"), SortOrder.Descending),
              new SortedPropertySpecification(GetPropertyDefinition(typeof(TIPerson), "LastName"), SortOrder.Ascending)
          });
      _testHelper.Provider.LoadDataContainersByRelatedID(relationEndPointDefinition, sortExpression, DomainObjectIDs.Client).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithEmptyResult ()
    {
      var newObjectID = _testHelper.Provider.CreateNewObjectID(Configuration.GetTypeDefinition(typeof(TIClient)));

      var sequence = new VerifiableSequence();
      _testHelper.ExpectExecuteReader(
          sequence,
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID] FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID "
          + "UNION ALL SELECT [ID], [ClassID] FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID;",
          Tuple.Create("@ClientID", DbType.Guid, newObjectID.Value));

      var relationEndPointDefinition = (RelationEndPointDefinition)GetEndPointDefinition(typeof(TIDomainBase), "Client");
      _testHelper.Provider.LoadDataContainersByRelatedID(relationEndPointDefinition, null, newObjectID).ToArray();

      _testHelper.VerifyAllExpectations();
      sequence.Verify();
    }
  }
}
