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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderExecuteCollectionQueryTest : SqlProviderBaseTest
  {
    [Test]
    public void ExecuteCollectionQuery ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1.Value);

      var orderContainerIDs = Provider.ExecuteCollectionQuery(query).Select(dc => dc.ID).ToArray();

      Assert.That(orderContainerIDs.Contains(DomainObjectIDs.Order1), Is.True);
      Assert.That(orderContainerIDs.Contains(DomainObjectIDs.Order2), Is.True);
    }

    [Test]
    public void ExecuteCollectionQuery_WithNullIDs ()
    {
      var query = QueryFactory.CreateCollectionQuery(
          "test",
          Provider.StorageProviderDefinition,
          "SELECT NULL AS [ID], NULL As [ClassID] FROM [Order] WHERE [Order].[OrderNo] IN (1, 2)",
          new QueryParameterCollection(),
          typeof(DomainObjectCollection));

      var orderContainers = Provider.ExecuteCollectionQuery(query);
      Assert.That(orderContainers, Is.EqualTo(new DataContainer[] { null, null }));
    }

    [Test]
    public void AllDataTypes ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("QueryWithAllDataTypes"));
      query.Parameters.Add("@boolean", false);
      query.Parameters.Add("@byte", (byte)85);
      query.Parameters.Add("@date", new DateOnly(2005, 1, 1));
      query.Parameters.Add("@dateTime", new DateTime(2005, 1, 1, 17, 0, 0));
      query.Parameters.Add("@decimal", (decimal)123456.789);
      query.Parameters.Add("@doubleLowerBound", 987654D);
      query.Parameters.Add("@doubleUpperBound", 987655D);
      query.Parameters.Add("@enum", ClassWithAllDataTypes.EnumType.Value1);
      query.Parameters.Add("@flags", ClassWithAllDataTypes.FlagsType.Flag0 | ClassWithAllDataTypes.FlagsType.Flag2);
      query.Parameters.Add("@extensibleEnum", Color.Values.Red());
      query.Parameters.Add("@guid", new Guid("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"));
      query.Parameters.Add("@int16", (short)32767);
      query.Parameters.Add("@int32", 2147483647);
      query.Parameters.Add("@int64", 9223372036854775807L);
      query.Parameters.Add("@singleLowerBound", (float)6789);
      query.Parameters.Add("@singleUpperBound", (float)6790);
      query.Parameters.Add("@string", "abcdeföäü");
      query.Parameters.Add("@stringWithoutMaxLength",
          "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890");
      query.Parameters.Add("@binary", ResourceManager.GetImage1());

      query.Parameters.Add("@naBoolean", true);
      query.Parameters.Add("@naByte", (byte)78);
      query.Parameters.Add("@naDate", new DateOnly(2005, 2, 1));
      query.Parameters.Add("@naDateTime", new DateTime(2005, 2, 1, 5, 0, 0));
      query.Parameters.Add("@naDecimal", 765.098m);
      query.Parameters.Add("@naDoubleLowerBound", 654321D);
      query.Parameters.Add("@naDoubleUpperBound", 654322D);
      query.Parameters.Add("@naEnum", ClassWithAllDataTypes.EnumType.Value2);
      query.Parameters.Add("@naFlags", ClassWithAllDataTypes.FlagsType.Flag1 | ClassWithAllDataTypes.FlagsType.Flag2);
      query.Parameters.Add("@naGuid", new Guid("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
      query.Parameters.Add("@naInt16", (short)12000);
      query.Parameters.Add("@naInt32", -2147483647);
      query.Parameters.Add("@naInt64", 3147483647L);
      query.Parameters.Add("@naSingleLowerBound", 12F);
      query.Parameters.Add("@naSingleUpperBound", 13F);

      query.Parameters.Add("@extensibleEnumWithNullValue", null);
      query.Parameters.Add("@naBooleanWithNullValue", null);
      query.Parameters.Add("@naByteWithNullValue", null);
      query.Parameters.Add("@naDateWithNullValue", null);
      query.Parameters.Add("@naDateTimeWithNullValue", null);
      query.Parameters.Add("@naDecimalWithNullValue", null);
      query.Parameters.Add("@naEnumWithNullValue", null);
      query.Parameters.Add("@naFlagsWithNullValue", null);
      query.Parameters.Add("@naDoubleWithNullValue", null);
      query.Parameters.Add("@naGuidWithNullValue", null);
      query.Parameters.Add("@naInt16WithNullValue", null);
      query.Parameters.Add("@naInt32WithNullValue", null);
      query.Parameters.Add("@naInt64WithNullValue", null);
      query.Parameters.Add("@naSingleWithNullValue", null);
      query.Parameters.Add("@stringWithNullValue", null);
      query.Parameters.Add("@nullableBinaryWithNullValue", null);

      var actualContainers = Provider.ExecuteCollectionQuery(query).ToArray();

      Assert.That(actualContainers, Is.Not.Null);
      Assert.That(actualContainers.Length, Is.EqualTo(1));

      DataContainer expectedContainer = TestDataContainerObjectMother.CreateClassWithAllDataTypes1DataContainer();
      var checker = new DataContainerChecker();
      checker.Check(expectedContainer, actualContainers[0]);
    }

    [Test]
    public void ExecuteCollectionQuery_WithLargeString ()
    {
      var queryParameterCollection = new QueryParameterCollection();
      queryParameterCollection.Add("@parameter", new string('c', 4001));
      var query = QueryFactory.CreateCollectionQuery(
          "test",
          Provider.StorageProviderDefinition,
          "SELECT NULL AS [ID], NULL As [ClassID] FROM [TableWithAllDataTypes] WHERE LEN (@parameter) > 0",
          queryParameterCollection,
          typeof(DomainObjectCollection));

      var orderContainers = Provider.ExecuteCollectionQuery(query);
      Assert.That(orderContainers, Is.EqualTo(new DataContainer[] { null, null }));
    }

    [Test]
    public void DifferentStorageProviderID ()
    {
      var definition = new QueryDefinition(
          "QueryWithDifferentStorageProviderID",
          UnitTestStorageProviderDefinition,
          "select 42",
          QueryType.CollectionReadOnly);
      Assert.That(
          () => Provider.ExecuteCollectionQuery(QueryFactory.CreateQuery(definition)),
          Throws.ArgumentException);
    }

    [Test]
    public void ObjectIDParameter ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      query.Parameters.Add("@customerID", DomainObjectIDs.Customer1);

      var orderContainerIDs = Provider.ExecuteCollectionQuery(query).Select(dc => dc.ID);

      Assert.That(orderContainerIDs.ToArray(), Is.EquivalentTo(new[] {DomainObjectIDs.Order1, DomainObjectIDs.Order2}));
    }

    [Test]
    public void ObjectIDOfDifferentStorageProvider ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("OrderByOfficialQuery"));
      query.Parameters.Add("@officialID", DomainObjectIDs.Official1);

      var orderContainerIDs = Provider.ExecuteCollectionQuery(query).Select(dc => dc.ID);

      Assert.That(orderContainerIDs.ToArray(), Is.EquivalentTo(
          new[] {
              DomainObjectIDs.Order1,
              DomainObjectIDs.Order3,
              DomainObjectIDs.Order4,
              DomainObjectIDs.Order5,
              DomainObjectIDs.Order2}));

    }
  }
}
