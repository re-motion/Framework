﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Model
{
  [TestFixture]
  public class SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecoratorTest
  {
    private TypeConverter _typeConverterStub;

    [SetUp]
    public void SetUp ()
    {
      _typeConverterStub = MockRepository.GenerateStub<TypeConverter>();
    }

    [Test]
    public void Initialization_DelegatesToInnerStorageTypeInformation ()
    {
      var innerStorageTypeInformation = new StorageTypeInformation(
          typeof (bool),
          "test",
          BooleanObjectMother.GetRandomBoolean() ? DbType.Double : DbType.Int32,
          BooleanObjectMother.GetRandomBoolean(),
          new Random().Next(),
          typeof (int),
          _typeConverterStub);

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

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.ConvertFromStorageType(originalValue)).Return(expectedValue);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, 4000);
      var actualValue = storageTypeInformation.ConvertFromStorageType(originalValue);
      Assert.That(actualValue, Is.SameAs(expectedValue));
    }

    [Test]
    public void ConvertToStorageType_DelegatesToInnerStorageTypeInformation ()
    {
      object originalValue = new object();
      object expectedValue = new object();

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.ConvertToStorageType(originalValue)).Return(expectedValue);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, 4000);
      var actualValue = storageTypeInformation.ConvertToStorageType(originalValue);
      Assert.That(actualValue, Is.SameAs(expectedValue));
    }

    [Test]
    public void Read_DelegatesToInnerStorageTypeInformation ()
    {
      object expectedValue = new object();
      var dataReaderStub = MockRepository.GenerateStub<IDataReader>();

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.Read(dataReaderStub, 1)).Return(expectedValue);

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, 4000);
      var actualValue = storageTypeInformation.Read(dataReaderStub, 1);
      Assert.That(actualValue, Is.SameAs(expectedValue));
    }

    [Test]
    public void CreateDataParameter_WithStringValue_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      var dbParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter>();
      bool isValueRead = false;

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.CreateDataParameter(dbCommandStub, originalValue)).Return(dbParameterMock);
      dbParameterMock.Stub(_ => _.Value).Return(new string('a', upperLimit)).WhenCalled(mi => isValueRead = true);
      dbParameterMock.Expect(_ => _.Size = upperLimit).WhenCalled(mi => Assert.That(isValueRead));

      var storageTypeInformation = 
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock));
      dbParameterMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateDataParameter_WithStringValueExceedingUpperLimitForFulltext_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      // This case cannot e handled gracefully. If the fulltext parameter value exceeds the upper limit, a DB-error cannot be avoided.

      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      var dbParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter>();

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.CreateDataParameter(dbCommandStub, originalValue)).Return(dbParameterMock);
      dbParameterMock.Stub(_ => _.Value).Return(new string('a', upperLimit + 1));

      var storageTypeInformation = 
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock));
    }

    [Test]
    public void CreateDataParameter_WithCharArrayValue_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      var dbParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter>();
      bool isValueRead = false;

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.CreateDataParameter(dbCommandStub, originalValue)).Return(dbParameterMock);
      dbParameterMock.Stub(_ => _.Value).Return(new char[upperLimit]).WhenCalled(mi => isValueRead = true);
      dbParameterMock.Expect(_ => _.Size = upperLimit).WhenCalled(mi => Assert.That(isValueRead));

      var storageTypeInformation = 
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock));
      dbParameterMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateDataParameter_WithCharArrayValueExceedingUpperLimitForFulltext_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      // This case cannot e handled gracefully. If the fulltext parameter value exceeds the upper limit, a DB-error cannot be avoided.

      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      var dbParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter>();

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.CreateDataParameter(dbCommandStub, originalValue)).Return(dbParameterMock);
      dbParameterMock.Stub(_ => _.Value).Return(new char[upperLimit + 1]);

      var storageTypeInformation = 
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock));
    }

    [Test]
    public void CreateDataParameter_WithValueIsNull_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      var dbParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter>();
      bool isValueRead = false;

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.CreateDataParameter(dbCommandStub, originalValue)).Return(dbParameterMock);
      dbParameterMock.Stub(_ => _.Value).Return(null).WhenCalled(mi => isValueRead = true);
      dbParameterMock.Expect(_ => _.Size = upperLimit).WhenCalled(mi => Assert.That(isValueRead));

      var storageTypeInformation = 
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock));
      dbParameterMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateDataParameter_WitValueIsDBNull_DelegatesToInnerStorageTypeInformation_AndSetsSizeToUpperLimitForFulltext ()
    {
      var upperLimit = 4242;
      object originalValue = "input";
      var dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      var dbParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter>();
      bool isValueRead = false;

      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStub.Stub(_ => _.CreateDataParameter(dbCommandStub, originalValue)).Return(dbParameterMock);
      dbParameterMock.Stub(_ => _.Value).Return(DBNull.Value).WhenCalled(mi => isValueRead = true);
      dbParameterMock.Expect(_ => _.Size = upperLimit).WhenCalled(mi => Assert.That(isValueRead));

      var storageTypeInformation = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, upperLimit);
      var actual = storageTypeInformation.CreateDataParameter(dbCommandStub, originalValue);

      Assert.That(actual, Is.SameAs(dbParameterMock));
      dbParameterMock.VerifyAllExpectations();
    }

    [Test]
    public void UnifyForEquivalentProperties_DelegatesToInnerStorageTypeInformation ()
    {
      var fulltextCompatibleMaxLength = 4242;
      var innerStorageTypeInformationStub1 = MockRepository.GenerateStub<IStorageTypeInformation>();
      var innerStorageTypeInformationStub2 = MockRepository.GenerateStub<IStorageTypeInformation>();
      var innerStorageTypeInformationStubUnified = MockRepository.GenerateStub<IStorageTypeInformation>();

      var innerStorageTypeInformationStubSource = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStubSource
          .Stub(
              _ => _.UnifyForEquivalentProperties(
                  Arg<IEnumerable<IStorageTypeInformation>>.List.Equal(new[] { innerStorageTypeInformationStub1, innerStorageTypeInformationStub2 })))
          .Return(innerStorageTypeInformationStubUnified);

      IStorageTypeInformation[] equivalentStorageTypes =
      {
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub1, fulltextCompatibleMaxLength),
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub2, fulltextCompatibleMaxLength)
      };

      var source = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(
          innerStorageTypeInformationStubSource,
          fulltextCompatibleMaxLength);

      var unifiedResult = source.UnifyForEquivalentProperties(equivalentStorageTypes);

      Assert.That(unifiedResult, Is.Not.SameAs(source));
      Assert.That(unifiedResult, Is.InstanceOf<SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator>());
      Assert.That(
          ((SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator) unifiedResult).InnerStorageTypeInformation,
          Is.SameAs(innerStorageTypeInformationStubUnified));
      Assert.That(
          ((SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator) unifiedResult).FulltextCompatibleMaxLength,
          Is.EqualTo(fulltextCompatibleMaxLength));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentFulltextCompatibleMaxLength ()
    {
      var innerStorageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      var innerStorageTypeInformationStubUnified = MockRepository.GenerateStub<IStorageTypeInformation>();

      var innerStorageTypeInformationStubSource = MockRepository.GenerateStub<IStorageTypeInformation>();
      innerStorageTypeInformationStubSource
          .Stub(
              _ => _.UnifyForEquivalentProperties(Arg<IEnumerable<IStorageTypeInformation>>.List.Equal(new[] { innerStorageTypeInformationStub })))
          .Return(innerStorageTypeInformationStubUnified);

      IStorageTypeInformation[] equivalentStorageTypes =
      {
          new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStub, 42),
      };

      var source = new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator(innerStorageTypeInformationStubSource, 13);

      Assert.That(
          () => source.UnifyForEquivalentProperties(equivalentStorageTypes),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has fulltext compatible max-length '13', and "
              + "the given property has fulltext compatible max-length '42'.", "equivalentStorageTypes"));
    }
  }
}