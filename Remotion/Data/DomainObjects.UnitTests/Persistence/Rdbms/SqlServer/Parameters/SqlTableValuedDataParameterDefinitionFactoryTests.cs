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
#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters;

[TestFixture]
public class SqlTableValuedDataParameterDefinitionFactoryTests
{
  private static IEnumerable<(object, Type, bool, IStorageTypeInformation)> GetTestCasesForCreateParameterDefinition ()
  {
    yield return (Array.Empty<int>(), typeof(int), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (Array.AsReadOnly(Array.Empty<int>()), typeof(int), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (new ArrayList(), typeof(object), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (new List<int>(), typeof(int), false, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
    yield return (Mock.Of<ICollection>(), typeof(object), false, StorageTypeInformationObjectMother.CreateStorageTypeInformation());
    yield return (Mock.Of<ICollection<DateTime>>(), typeof(DateTime), false, StorageTypeInformationObjectMother.CreateDateTimeStorageTypeInformation());
    yield return (Mock.Of<IReadOnlyCollection<Guid>>(), typeof(Guid), false, StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation());
    yield return (Mock.Of<ISet<string>>(), typeof(string), true, StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation());
#if NET5_0_OR_GREATER
    yield return (Mock.Of<IReadOnlySet<int>>(), typeof(int), true, StorageTypeInformationObjectMother.CreateIntStorageTypeInformation());
#endif
  }

  [Test]
  [TestCaseSource(nameof(GetTestCasesForCreateParameterDefinition))]
  public void CreateParameterDefinition ((object, Type, bool, IStorageTypeInformation) testCase)
  {
    var value = testCase.Item1;
    var dotNetType = testCase.Item2;
    var isDistinct = testCase.Item3;
    var storageTypeInformation = testCase.Item4;

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub.Setup(_ => _.GetStorageType(dotNetType)).Returns(storageTypeInformation);

    var nextFactoryStub = new Mock<IDataParameterDefinitionFactory>();
    var factory = new SqlTableValuedDataParameterDefinitionFactory(storageTypeInformationProviderStub.Object, nextFactoryStub.Object);

    var queryParameter = new QueryParameter("Dummy", value);
    var result = factory.CreateDataParameterDefinition(queryParameter);

    Assert.That(result, Is.InstanceOf<SqlTableValuedDataParameterDefinition>());
    Assert.That(result.As<SqlTableValuedDataParameterDefinition>().IsDistinct, Is.EqualTo(isDistinct));
    Assert.That(result.As<SqlTableValuedDataParameterDefinition>().StorageTypeInformation, Is.SameAs(storageTypeInformation));
  }

  private static IEnumerable<object?> GetNonCollectionValues ()
  {
    yield return null;
    yield return "StringValue";
    yield return "CharArray".ToCharArray();
    yield return new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
    yield return Mock.Of<IEnumerable>();
  }

  [Test]
  [TestCaseSource(nameof(GetNonCollectionValues))]
  public void CreateParameterDefinition_WithNonCollection_ReturnsResultOfNextFactory (object value)
  {
    var queryParameter = new QueryParameter("SingleValue", value);

    var definitionStub = Mock.Of<IDataParameterDefinition>();

    var nextFactoryStub = new Mock<IDataParameterDefinitionFactory>();
    nextFactoryStub.Setup(_ => _.CreateDataParameterDefinition(queryParameter)).Returns(definitionStub);

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();

    var factory = new SqlTableValuedDataParameterDefinitionFactory(storageTypeInformationProviderStub.Object, nextFactoryStub.Object);
    var result = factory.CreateDataParameterDefinition(queryParameter);
    Assert.That(result, Is.SameAs(definitionStub));
  }
}
