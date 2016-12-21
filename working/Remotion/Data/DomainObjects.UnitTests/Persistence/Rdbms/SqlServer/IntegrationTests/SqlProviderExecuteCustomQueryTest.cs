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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderExecuteCustomQueryTest : SqlProviderBaseTest
  {
    private IQuery _query;

    public override void SetUp ()
    {
      base.SetUp();

      _query = QueryFactory.CreateCustomQuery (
          "CustomQuery",
          TestDomainStorageProviderDefinition,
          "SELECT String, Int16, Boolean, Enum, ExtensibleEnum FROM [TableWithAllDataTypes]",
          new QueryParameterCollection());
    }

    [Test]
    public void ExecuteCustomQuery_RawValues ()
    {
      var result = Provider.ExecuteCustomQuery (_query);

      var rawValues =
          result
            .Select (qrr => new[] { qrr.GetRawValue (0), qrr.GetRawValue (1), qrr.GetRawValue (2), qrr.GetRawValue (3), qrr.GetRawValue (4) })
            .ToArray ();
      var expected =
          new object[]
          {
              new object[] { "üäöfedcba", -32767, true, 0, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ColorExtensions.Blue" },
              new object[] { "abcdeföäü", 32767, false, 1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ColorExtensions.Red" }
          };

      Assert.That (rawValues, Is.EquivalentTo (expected));
    }

    [Test]
    public void ExecuteCustomQuery_ConvertedValues ()
    {
      var result = Provider.ExecuteCustomQuery (_query);

      var convertedValues = result.Select (
          qrr =>
          new object[]
          {
              qrr.GetConvertedValue<string> (0), 
              qrr.GetConvertedValue<Int16> (1), 
              qrr.GetConvertedValue<bool> (2),
              qrr.GetConvertedValue<ClassWithAllDataTypes.EnumType> (3), 
              qrr.GetConvertedValue<Color> (4)
          }).ToArray();

      var expected =
          new[]
          {
              new object[]
              {
                  "üäöfedcba",
                  -32767,
                  true,
                  ClassWithAllDataTypes.EnumType.Value0,
                  Color.Values.Blue()
              },
              new object[]
              {
                  "abcdeföäü",
                  32767,
                  false,
                  ClassWithAllDataTypes.EnumType.Value1,
                  Color.Values.Red()
              },
          };

      Assert.That (convertedValues, Is.EquivalentTo (expected));
    }
  }
}