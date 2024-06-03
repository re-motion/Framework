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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Queries
{
  [TestFixture]
  public class CustomQueryTest : QueryTestBase
  {
    private IQuery _query;

    public override void SetUp ()
    {
      base.SetUp();

      _query = QueryFactory.CreateCustomQuery(
          "CustomQuery",
          TestDomainStorageProviderDefinition,
          "SELECT String, Int16, Boolean, Enum, ExtensibleEnum FROM [TableWithAllDataTypes]",
          new QueryParameterCollection());
    }

    [Test]
    public void WithRawValues ()
    {
      var result = QueryManager.GetCustom(_query, QueryResultRowTestHelper.ExtractRawValues).ToList();

      var expected = new object[]
                            {
                                new object[] { "üäöfedcba", -32767, true, 0, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ColorExtensions.Blue" },
                                new object[] { "abcdeföäü", 32767, false, 1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ColorExtensions.Red" }
                            };

      Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void WithConvertedValues ()
    {
      var result = QueryManager.GetCustom(
         _query,
         queryResultRow => new
         {
           StringValue = queryResultRow.GetConvertedValue<string>(0),
           Int16Value = queryResultRow.GetConvertedValue<Int16>(1),
           BoolValue = queryResultRow.GetConvertedValue<bool>(2),
           EnumValue = queryResultRow.GetConvertedValue<ClassWithAllDataTypes.EnumType>(3),
           ExtensibleEnumValue = queryResultRow.GetConvertedValue<Color>(4)
         }).ToList();

      var expected =
          new[]
          {
              new
              {
                  StringValue = "üäöfedcba",
                  Int16Value = (Int16)(-32767),
                  BoolValue = true,
                  EnumValue = ClassWithAllDataTypes.EnumType.Value0,
                  ExtensibleEnumValue = Color.Values.Blue()
              },
              new
              {
                  StringValue = "abcdeföäü",
                  Int16Value = (Int16)32767,
                  BoolValue = false,
                  EnumValue = ClassWithAllDataTypes.EnumType.Value1,
                  ExtensibleEnumValue = Color.Values.Red()
              }
          };

      Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void InvokesFilterQueryResultEvent ()
    {
      var listenerMock = new Mock<IClientTransactionListener>();

      var fakeResult = new[] { new object[0] };
      listenerMock
          .Setup(mock => mock.FilterCustomQueryResult(TestableClientTransaction, _query, It.Is<IEnumerable<object[]>>(e => e.Count() == 2)))
          .Returns(fakeResult)
          .Verifiable();

      TestableClientTransaction.AddListener(listenerMock.Object);

      var result = QueryManager.GetCustom(_query, QueryResultRowTestHelper.ExtractRawValues);

      Assert.That(result, Is.SameAs(fakeResult));
    }

    [Test]
    public void FromXmlFile ()
    {
      var query = QueryFactory.CreateQuery(Queries.GetMandatory("CustomQueryReadOnly"));

      var result = QueryManager.GetCustom(query, QueryResultRowTestHelper.ExtractRawValues);

      Assert.That(result.Count(), Is.EqualTo(2));
    }
  }
}
