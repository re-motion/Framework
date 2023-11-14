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
using System.ComponentModel;
using System.Data;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Model
{
  [TestFixture]
  public class SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecoratorTest
  {
    private Mock<TypeConverter> _typeConverterStub;

    [SetUp]
    public void SetUp ()
    {
      _typeConverterStub = new Mock<TypeConverter>();
    }

    [Test]
    public void Initialization_DelegatesToInnerStorageTypeInformation ()
    {
      var innerStorageTypeInformation = new StorageTypeInformation(
          typeof(bool),
          "test",
          BooleanObjectMother.GetRandomBoolean() ? DbType.Double : DbType.Int32,
          BooleanObjectMother.GetRandomBoolean(),
          new Random().Next(),
          typeof(int),
          _typeConverterStub.Object);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformation, 4000);

      Assert.That(storageTypeInformation.StorageType, Is.EqualTo(innerStorageTypeInformation.StorageType));
      Assert.That(storageTypeInformation.StorageTypeName, Is.EqualTo(innerStorageTypeInformation.StorageTypeName));
      Assert.That(storageTypeInformation.StorageDbType, Is.EqualTo(innerStorageTypeInformation.StorageDbType));
      Assert.That(storageTypeInformation.IsStorageTypeNullable, Is.EqualTo(innerStorageTypeInformation.IsStorageTypeNullable));
      Assert.That(storageTypeInformation.StorageTypeLength, Is.EqualTo(innerStorageTypeInformation.StorageTypeLength));
      Assert.That(storageTypeInformation.DotNetType, Is.EqualTo(innerStorageTypeInformation.DotNetType));
    }

    [Test]
    public void ConvertFromStorageType_DelegatesToInnerStorageTypeInformation ()
    {
      object originalValue = new object();
      object expectedValue = new object();

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.ConvertFromStorageType(originalValue)).Returns(expectedValue);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, 4000);
      var actualValue = storageTypeInformation.ConvertFromStorageType(originalValue);
      Assert.That(actualValue, Is.SameAs(expectedValue));
    }

    [Test]
    public void ConvertToStorageType_DelegatesToInnerStorageTypeInformation ()
    {
      object originalValue = new object();
      object expectedValue = new object();

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.ConvertToStorageType(originalValue)).Returns(expectedValue);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, 4000);
      var actualValue = storageTypeInformation.ConvertToStorageType(originalValue);
      Assert.That(actualValue, Is.SameAs(expectedValue));
    }

    [Test]
    public void Read_DelegatesToInnerStorageTypeInformation ()
    {
      object expectedValue = new object();
      var dataReaderStub = new Mock<IDataReader>();

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.Read(dataReaderStub.Object, 1)).Returns(expectedValue);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, 4000);
      var actualValue = storageTypeInformation.Read(dataReaderStub.Object, 1);
      Assert.That(actualValue, Is.SameAs(expectedValue));
    }

    [Test]
    public void CreateDataParameter_WithStringValue_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = new Mock<IDbCommand>();
      var dbParameterMock = new Mock<IDbDataParameter>(MockBehavior.Strict);
      bool isValueRead = false;

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, originalValue)).Returns(dbParameterMock.Object);
      dbParameterMock.Setup(_ => _.Value).Returns(new string('a', upperLimit)).Callback(()=> isValueRead = true);
      dbParameterMock.SetupSet(_ => _.Size = upperLimit).Callback(()=> Assert.That(isValueRead)).Verifiable();

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub.Object, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock.Object));
      dbParameterMock.Verify();
    }

    [Test]
    public void CreateDataParameter_WithStringValueExceedingUpperLimitForFulltext_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      // This case cannot e handled gracefully. If the fulltext parameter value exceeds the upper limit, a DB-error cannot be avoided.

      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = new Mock<IDbCommand>();
      var dbParameterMock = new Mock<IDbDataParameter>(MockBehavior.Strict);

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, originalValue)).Returns(dbParameterMock.Object);
      dbParameterMock.Setup(_ => _.Value).Returns(new string('a', upperLimit + 1));

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub.Object, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithCharArrayValue_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = new Mock<IDbCommand>();
      var dbParameterMock = new Mock<IDbDataParameter>(MockBehavior.Strict);
      bool isValueRead = false;

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, originalValue)).Returns(dbParameterMock.Object);
      dbParameterMock.Setup(_ => _.Value).Returns(new char[upperLimit]).Callback(()=> isValueRead = true);
      dbParameterMock.SetupSet(_ => _.Size = upperLimit).Callback(()=> Assert.That(isValueRead)).Verifiable();

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub.Object, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock.Object));
      dbParameterMock.Verify();
    }

    [Test]
    public void CreateDataParameter_WithCharArrayValueExceedingUpperLimitForFulltext_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      // This case cannot e handled gracefully. If the fulltext parameter value exceeds the upper limit, a DB-error cannot be avoided.

      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = new Mock<IDbCommand>();
      var dbParameterMock = new Mock<IDbDataParameter>(MockBehavior.Strict);

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, originalValue)).Returns(dbParameterMock.Object);
      dbParameterMock.Setup(_ => _.Value).Returns(new char[upperLimit + 1]);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub.Object, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock.Object));
    }

    [Test]
    public void CreateDataParameter_WithValueIsNull_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = new Mock<IDbCommand>();
      var dbParameterMock = new Mock<IDbDataParameter>(MockBehavior.Strict);
      bool isValueRead = false;

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, originalValue)).Returns(dbParameterMock.Object);
      dbParameterMock.Setup(_ => _.Value).Returns((object)null).Callback(()=> isValueRead = true);
      dbParameterMock.SetupSet(_ => _.Size = upperLimit).Callback(()=> Assert.That(isValueRead)).Verifiable();

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub.Object, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock.Object));
      dbParameterMock.Verify();
    }

    [Test]
    public void CreateDataParameter_WitValueIsDBNull_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = new Mock<IDbCommand>();
      var dbParameterMock = new Mock<IDbDataParameter>(MockBehavior.Strict);
      bool isValueRead = false;

      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Setup(_ => _.CreateDataParameter(dbCommandStub.Object, originalValue)).Returns(dbParameterMock.Object);
      dbParameterMock.Setup(_ => _.Value).Returns(DBNull.Value).Callback(()=> isValueRead = true);
      dbParameterMock.SetupSet(_ => _.Size = upperLimit).Callback(()=> Assert.That(isValueRead)).Verifiable();

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub.Object, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock.Object));
      dbParameterMock.Verify();
    }

    [Test]
    public void UnifyForEquivalentProperties_DelegatesToInnerStorageTypeInformation ()
    {
      var fulltextCompatibleMaxLength = 4242;
      var innerStorageTypeInformationStub1 = new Mock<IStorageTypeInformation>();
      var innerStorageTypeInformationStub2 = new Mock<IStorageTypeInformation>();
      var innerStorageTypeInformationStubUnified = new Mock<IStorageTypeInformation>();

      var innerStorageTypeInformationStubSource = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStubSource
          .Setup(_ => _.UnifyForEquivalentProperties(new[] { innerStorageTypeInformationStub1.Object, innerStorageTypeInformationStub2.Object }))
          .Returns(innerStorageTypeInformationStubUnified.Object);

      IStorageTypeInformation[] equivalentStorageTypes =
      {
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub1.Object, fulltextCompatibleMaxLength),
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub2.Object, fulltextCompatibleMaxLength)
      };

      var source = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(
          innerStorageTypeInformationStubSource.Object,
          fulltextCompatibleMaxLength);

      var unifiedResult = source.UnifyForEquivalentProperties(equivalentStorageTypes);

      Assert.That(unifiedResult, Is.Not.SameAs(source));
      Assert.That(unifiedResult, Is.InstanceOf<SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator>());
      Assert.That(
          ((SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator)unifiedResult).InnerStorageTypeInformation,
          Is.SameAs(innerStorageTypeInformationStubUnified.Object));
      Assert.That(
          ((SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator)unifiedResult).FulltextCompatibleMaxLength,
          Is.EqualTo(fulltextCompatibleMaxLength));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentFulltextCompatibleMaxLength ()
    {
      var innerStorageTypeInformationStub = new Mock<IStorageTypeInformation>();
      var innerStorageTypeInformationStubUnified = new Mock<IStorageTypeInformation>();

      var innerStorageTypeInformationStubSource = new Mock<IStorageTypeInformation>();
      innerStorageTypeInformationStubSource
          .Setup(_ => _.UnifyForEquivalentProperties(new[] { innerStorageTypeInformationStub.Object }))
          .Returns(innerStorageTypeInformationStubUnified.Object);

      IStorageTypeInformation[] equivalentStorageTypes =
      {
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub.Object, 42),
      };

      var source = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStubSource.Object, 13);

      Assert.That(
          () => source.UnifyForEquivalentProperties(equivalentStorageTypes),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has fulltext compatible max-length '13', and "
              + "the given property has fulltext compatible max-length '42'.", "equivalentStorageTypes"));
    }
  }
}
