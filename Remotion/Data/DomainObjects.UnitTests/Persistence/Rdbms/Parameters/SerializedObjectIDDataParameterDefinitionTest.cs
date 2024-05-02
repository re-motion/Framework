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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class SerializedObjectIDDataParameterDefinitionTest : StandardMappingTest
{
  [Test]
  public void Initialize ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();

    var parameterDefinition = new SerializedObjectIDDataParameterDefinition(storageTypeInformation);

    Assert.That(parameterDefinition.StorageTypeInformation, Is.SameAs(storageTypeInformation));
  }

  [Test]
  public void Initialize_WithIncorrectStorageTypeInformation ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateIntStorageTypeInformation();

    Assert.That(
        () => new SerializedObjectIDDataParameterDefinition(storageTypeInformation),
        Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
            $"Parameter 'storageTypeInformation.DotNetType' is a '{typeof(int)}', which cannot be assigned to type '{typeof(string)}'.",
            "storageTypeInformation.DotNetType"));
  }

  [Test]
  public void GetParameterValue_WithObjectID_ReturnsSerializedObjectID ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation(storageDbType: DbType.AnsiString, storageTypeLength: 255);

    var objectID = DomainObjectIDs.Order1;
    var expectedValue = ObjectIDStringSerializer.Instance.Serialize(objectID);

    var serializedObjectIDDataParameterDefinition = new SerializedObjectIDDataParameterDefinition(storageTypeInformation);

    var result = serializedObjectIDDataParameterDefinition.GetParameterValue(objectID);

    Assert.That(result, Is.EqualTo(expectedValue));
  }

  [Test]
  public void GetParameterValue_WithNull_ReturnsDBNull ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();

    var serializedObjectIDDataParameterDefinition = new SerializedObjectIDDataParameterDefinition(storageTypeInformation);

    Assert.That(serializedObjectIDDataParameterDefinition.GetParameterValue(null), Is.EqualTo(DBNull.Value));
  }

  [Test]
  public void GetParameterValue_WithTypeMismatchedValue_ThrowsArgumentException ()
  {
    var dummyValue = "Not an ObjectID";
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();

    var serializedObjectIDDataParameterDefinition = new SerializedObjectIDDataParameterDefinition(storageTypeInformation);

    Assert.That(
        () => serializedObjectIDDataParameterDefinition.GetParameterValue(dummyValue),
        Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
            $"Parameter 'value' has type '{typeof(string)}' when type '{typeof(ObjectID)}' was expected.",
            "value"));
  }

  [Test]
  public void CreateDataParameter_SetsNameValueTypeSize_AnsiString255 ()
  {
    var commandStub = new Mock<IDbCommand>();
    var dataParameterStub = new Mock<IDbDataParameter>();

    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation(storageDbType: DbType.AnsiString, storageTypeLength: 255);

    var objectID = DomainObjectIDs.Order1;
    var testValue = ObjectIDStringSerializer.Instance.Serialize(objectID);

    commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterStub.Object);

    dataParameterStub.SetupProperty(_ => _.ParameterName);
    dataParameterStub.SetupProperty(_ => _.DbType);
    dataParameterStub.SetupProperty(_ => _.Value);
    dataParameterStub.SetupProperty(_ => _.Size);

    var serializedObjectIDDataParameterDefinition = new SerializedObjectIDDataParameterDefinition(storageTypeInformation);

    var result = serializedObjectIDDataParameterDefinition.CreateDataParameter(commandStub.Object, "dummy", testValue);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.ParameterName, Is.EqualTo("dummy"));
    Assert.That(result.Value, Is.EqualTo(testValue));
    Assert.That(result.DbType, Is.EqualTo(DbType.AnsiString));
    Assert.That(result.Size, Is.EqualTo(255));
  }

  [Test]
  public void CreateDataParameter_WithDBNullValue_SetsNameValueTypeSize_AnsiString255 ()
  {
    var commandStub = new Mock<IDbCommand>();
    var dataParameterStub = new Mock<IDbDataParameter>();

    commandStub.Setup(_ => _.CreateParameter()).Returns(dataParameterStub.Object);

    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation(storageDbType: DbType.AnsiString, storageTypeLength: 255);

    dataParameterStub.SetupProperty(_ => _.ParameterName);
    dataParameterStub.SetupProperty(_ => _.DbType);
    dataParameterStub.SetupProperty(_ => _.Value);
    dataParameterStub.SetupProperty(_ => _.Size);

    var serializedObjectIDDataParameterDefinition = new SerializedObjectIDDataParameterDefinition(storageTypeInformation);

    var result = serializedObjectIDDataParameterDefinition.CreateDataParameter(commandStub.Object, "dummy", DBNull.Value);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.ParameterName, Is.EqualTo("dummy"));
    Assert.That(result.Value, Is.EqualTo(DBNull.Value));
    Assert.That(result.DbType, Is.EqualTo(DbType.AnsiString));
    Assert.That(result.Size, Is.EqualTo(255));
  }
}
