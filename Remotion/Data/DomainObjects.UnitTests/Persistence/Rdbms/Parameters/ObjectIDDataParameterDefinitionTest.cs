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
using System.ComponentModel;
using System.Data;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class ObjectIDDataParameterDefinitionTest : StandardMappingTest
{
  [Test]
  public void GetParameterValue_WithObjectID_ReturnsObjectIDValue ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation();
    var dummyValue = DomainObjectIDs.Order1;

    var objectIDDataParameterDefinition = new ObjectIDDataParameterDefinition(storageTypeInformation);

    var result = objectIDDataParameterDefinition.GetParameterValue(dummyValue);

    Assert.That(result, Is.EqualTo(dummyValue.Value));
  }

  [Test]
  public void GetParameterValue_WithNull_ReturnsDBNull ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation(true);

    var objectIDDataParameterDefinition = new ObjectIDDataParameterDefinition(storageTypeInformation);

    Assert.That(objectIDDataParameterDefinition.GetParameterValue(null), Is.EqualTo(DBNull.Value));
  }

  [Test]
  public void GetParameterValue_WithTypeMismatchValue_ThrowsArgumentException ()
  {
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateIntStorageTypeInformation();
    var dummyValue = new ObjectID(typeof(Customer), Guid.NewGuid());

    var objectIDDataParameterDefinition = new ObjectIDDataParameterDefinition(storageTypeInformation);

    Assert.That(
        () => objectIDDataParameterDefinition.GetParameterValue(dummyValue),
        Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo(
            $"Parameter 'objectID.Value' has type '{typeof(Guid)}' when type '{typeof(int)}' was expected.",
            "objectID.Value"));
  }

  [Test]
  public void CreateDataParameter_WithIntValue_SetsNameValueType ()
  {
    var parameterValue = 42;
    var commandStub = new Mock<IDbCommand>();
    var dataParameterStub = new Mock<IDbDataParameter>();

    var storageTypeInformation = StorageTypeInformationObjectMother.CreateIntStorageTypeInformation();

    commandStub
        .Setup(_ => _.CreateParameter())
        .Returns(dataParameterStub.Object);

    dataParameterStub.SetupProperty(_ => _.ParameterName);
    dataParameterStub.SetupProperty(_ => _.DbType);
    dataParameterStub.SetupProperty(_ => _.Value);

    var objectIDDataParameterDefinition = new ObjectIDDataParameterDefinition(storageTypeInformation);

    var result = objectIDDataParameterDefinition.CreateDataParameter(commandStub.Object, "dummy", parameterValue);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.ParameterName, Is.EqualTo("dummy"));
    Assert.That(result.Value, Is.EqualTo(42));
    Assert.That(result.DbType, Is.EqualTo(DbType.Int32));
  }

  [Test]
  public void CreateDataParameter_WithGuidValue_SetsNameValueType ()
  {
    var parameterValue = Guid.NewGuid();
    var commandStub = new Mock<IDbCommand>();
    var dataParameterStub = new Mock<IDbDataParameter>();

    var storageTypeInformation = new StorageTypeInformation(
        typeof(Guid),
        "test",
        DbType.Guid,
        false,
        null,
        typeof(Guid),
        new DefaultConverter(typeof(Guid)));

    commandStub
        .Setup(_ => _.CreateParameter())
        .Returns(dataParameterStub.Object);

    dataParameterStub.SetupProperty(_ => _.ParameterName);
    dataParameterStub.SetupProperty(_ => _.DbType);
    dataParameterStub.SetupProperty(_ => _.Value);

    var objectIDDataParameterDefinition = new ObjectIDDataParameterDefinition(storageTypeInformation);

    var result = objectIDDataParameterDefinition.CreateDataParameter(commandStub.Object, "dummy", parameterValue);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.ParameterName, Is.EqualTo("dummy"));
    Assert.That(result.Value, Is.EqualTo(parameterValue));
    Assert.That(result.DbType, Is.EqualTo(DbType.Guid));
  }

  [Test]
  public void CreateDataParameter_WithDBNullValue_SetsNameValueType ()
  {
    var commandStub = new Mock<IDbCommand>();
    var dataParameterStub = new Mock<IDbDataParameter>();

    commandStub
        .Setup(_ => _.CreateParameter())
        .Returns(dataParameterStub.Object);

    var storageTypeInformation = new StorageTypeInformation(
        typeof(Guid),
        "test",
        DbType.Guid,
        true,
        null,
        typeof(Guid),
        Mock.Of<TypeConverter>()
    );

    dataParameterStub.SetupProperty(_ => _.ParameterName);
    dataParameterStub.SetupProperty(_ => _.DbType);
    dataParameterStub.SetupProperty(_ => _.Value);

    var objectIDDataParameterDefinition = new ObjectIDDataParameterDefinition(storageTypeInformation);

    var result = objectIDDataParameterDefinition.CreateDataParameter(commandStub.Object, "dummy", DBNull.Value);

    Assert.That(result, Is.SameAs(dataParameterStub.Object));
    Assert.That(result.ParameterName, Is.EqualTo("dummy"));
    Assert.That(result.Value, Is.EqualTo(DBNull.Value));
    Assert.That(result.DbType, Is.EqualTo(storageTypeInformation.StorageDbType));
  }
}
