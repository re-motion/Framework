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
using System.ComponentModel;
using System.Data;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class SimpleDataParameterDefinitionTest
{
  [Test]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation_WithDateTime ()
  {
    GetParameterValue_ConvertsGivenValueViaStorageTypeInformation(new DateTime(2024, 04, 22, 14, 15, 42));
  }

  [Test]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation_WithDecimal ()
  {
    GetParameterValue_ConvertsGivenValueViaStorageTypeInformation(17.04m);
  }

  [Test]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation_WitGuidBasedObjectID ()
  {
    StandardConfiguration.EnsureInitialized();
    var domainObjectIDs = StandardConfiguration.Instance.GetDomainObjectIDs();

    GetParameterValue_ConvertsGivenValueViaStorageTypeInformation(domainObjectIDs.Location1);
  }

  [Test]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation_WithObjectIDFromCustomStorageProvider ()
  {
    StandardConfiguration.EnsureInitialized();
    var domainObjectIDs = StandardConfiguration.Instance.GetDomainObjectIDs();

    GetParameterValue_ConvertsGivenValueViaStorageTypeInformation(domainObjectIDs.Official1);
  }

  [Test]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation_WithGuid ()
  {
    GetParameterValue_ConvertsGivenValueViaStorageTypeInformation(new Guid("7ACD972D-37BA-4BB0-BA11-8DFA0763DB9F"));
  }

  [Test]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation_WithExtensibleEnum ()
  {
    GetParameterValue_ConvertsGivenValueViaStorageTypeInformation(TestExtensibleEnum.Values.Value2());
  }

  [Test]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation_WithBinary ()
  {
    GetParameterValue_ConvertsGivenValueViaStorageTypeInformation(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
  }

  [Test]
  [TestCase("StringValue")]
  [TestCase(42)]
  [TestCase((long)42)]
  [TestCase((short)42)]
  [TestCase((byte)42)]
  [TestCase(17.04d)]
  [TestCase(17.04f)]
  [TestCase(ClassWithAllDataTypes.EnumType.Value1)]
  public void GetParameterValue_ConvertsGivenValueViaStorageTypeInformation (object dummyValue)
  {
    var storageTypeInformationStub = new Mock<IStorageTypeInformation>(MockBehavior.Strict);

    storageTypeInformationStub
        .Setup(stub => stub.ConvertToStorageType(dummyValue))
        .Returns("It worked");

    storageTypeInformationStub
        .Setup(stub => stub.DotNetType)
        .Returns(dummyValue.GetType());

    var simpleDataParameterDefinition = new SimpleDataParameterDefinition(storageTypeInformationStub.Object);

    var result = simpleDataParameterDefinition.GetParameterValue(dummyValue);

    Assert.That(result, Is.EqualTo("It worked"));
  }

  [Test]
  public void GetParameterValue_WithMismatchedType_ThrowsArgumentException ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateIntStorageTypeInformation();

    var simpleDataParameterDefinition = new SimpleDataParameterDefinition(storageTypeInformation);

    Assert.That(
        () => simpleDataParameterDefinition.GetParameterValue("dummyValue"),
        Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
            $"Parameter 'value' has type '{typeof(string)}' when type '{typeof(int)}' was expected.",
            "value"));
  }

  [CLSCompliant(false)]
  [Test]
  [TestCase(typeof(int), DbType.Int32, false, 42)]
  [TestCase(typeof(double), DbType.Double, false, 17.04d)]
  [TestCase(typeof(DateTime?), DbType.DateTime2, false, null)]
  [TestCase(typeof(DateOnly), DbType.Date, false, null)]
  [TestCase(typeof(string), DbType.String, true, "TestValue")]
  [TestCase(typeof(byte[]), DbType.Binary, true, null)]
  public void CreateDataParameter_SetsNameValueTypeSize (Type dataType, DbType dbType, bool hasSize, [CanBeNull] object testValue)
  {
    testValue ??= DBNull.Value;
    var commandMock = new Mock<IDbCommand>(MockBehavior.Strict);
    var dataParameterStub = new Mock<IDbDataParameter>();

    var storageTypeInformation = new StorageTypeInformation(
        dataType,
        "test",
        dbType,
        false,
        hasSize ? -1 : null,
        dataType,
        Mock.Of<TypeConverter>());

    commandMock
        .Setup(_ => _.CreateParameter())
        .Returns(dataParameterStub.Object);

    dataParameterStub.SetupProperty(_ => _.ParameterName);
    dataParameterStub.SetupProperty(_ => _.Value);
    dataParameterStub.SetupProperty(_ => _.DbType);
    dataParameterStub.SetupProperty(_ => _.Size);

    var simpleDataParameterDefinition = new SimpleDataParameterDefinition(storageTypeInformation);

    var result = simpleDataParameterDefinition.CreateDataParameter(commandMock.Object, "dummy", testValue);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.ParameterName, Is.EqualTo("dummy"));
    Assert.That(result.Value, Is.EqualTo(testValue));
    Assert.That(result.DbType, Is.EqualTo(dbType));
    Assert.That(result.Size, Is.EqualTo(hasSize ? -1 : default));
  }
}
