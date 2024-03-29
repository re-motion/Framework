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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class ObjectReaderFactoryTest : StandardMappingTest
  {
    private Mock<IRdbmsPersistenceModelProvider> _rdbmsPersistenceModelProviderStub;
    private ObjectReaderFactory _factory;
    private Mock<IRdbmsStorageEntityDefinition> _entityDefinitionStub;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ObjectIDStoragePropertyDefinition _objectIDProperty;
    private SimpleStoragePropertyDefinition _timestampProperty;
    private Mock<IInfrastructureStoragePropertyDefinitionProvider> _infrastructureStoragePropertyDefinitionProviderStub;
    private Mock<IStorageTypeInformationProvider> _storageTypeInformationProviderStub;
    private Mock<IDataContainerValidator> _dataContainerValidatorStub;

    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProviderStub = new Mock<IRdbmsPersistenceModelProvider>();
      _entityDefinitionStub = new Mock<IRdbmsStorageEntityDefinition>();
      _storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
      _dataContainerValidatorStub = new Mock<IDataContainerValidator>();

      _objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;
      _entityDefinitionStub.Setup(stub => stub.ObjectIDProperty).Returns(_objectIDProperty);
      _entityDefinitionStub.Setup(stub => stub.TimestampProperty).Returns(_timestampProperty);

      _column1 = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn("Column2");

      _infrastructureStoragePropertyDefinitionProviderStub = new Mock<IInfrastructureStoragePropertyDefinitionProvider>(MockBehavior.Strict);
      _factory = new ObjectReaderFactory(
          _rdbmsPersistenceModelProviderStub.Object,
          _infrastructureStoragePropertyDefinitionProviderStub.Object,
          _storageTypeInformationProviderStub.Object,
          _dataContainerValidatorStub.Object);
    }

    [Test]
    public void CreateDataContainerReader_OverloadWithNoParameters ()
    {
      _infrastructureStoragePropertyDefinitionProviderStub.Setup(stub => stub.GetObjectIDStoragePropertyDefinition()).Returns(_objectIDProperty);
      _infrastructureStoragePropertyDefinitionProviderStub.Setup(stub => stub.GetTimestampStoragePropertyDefinition()).Returns(_timestampProperty);

      var result = _factory.CreateDataContainerReader();

      Assert.That(result, Is.TypeOf(typeof(DataContainerReader)));
      var dataContainerReader = (DataContainerReader)result;
      Assert.That(dataContainerReader.OrdinalProvider, Is.TypeOf(typeof(NameBasedColumnOrdinalProvider)));
      Assert.That(dataContainerReader.IDProperty, Is.SameAs(_objectIDProperty));
      Assert.That(dataContainerReader.TimestampProperty, Is.SameAs(_timestampProperty));
      Assert.That(dataContainerReader.DataContainerValidator, Is.SameAs(_dataContainerValidatorStub.Object));
    }

    [Test]
    public void CreateDataContainerReader ()
    {
      var result = _factory.CreateDataContainerReader(_entityDefinitionStub.Object, new[] { _column1, _column2 });

      Assert.That(result, Is.TypeOf(typeof(DataContainerReader)));
      var dataContainerReader = (DataContainerReader)result;
      CheckOrdinalProvider(dataContainerReader.OrdinalProvider);
      Assert.That(dataContainerReader.IDProperty, Is.SameAs(_objectIDProperty));
      Assert.That(dataContainerReader.TimestampProperty, Is.SameAs(_timestampProperty));
      Assert.That(dataContainerReader.DataContainerValidator, Is.SameAs(_dataContainerValidatorStub.Object));
    }

    [Test]
    public void CreateObjectIDReader ()
    {
      var result = _factory.CreateObjectIDReader(_entityDefinitionStub.Object, new[] { _column1, _column2 });

      Assert.That(result, Is.TypeOf(typeof(ObjectIDReader)));
      var objectIDReader = (ObjectIDReader)result;
      CheckOrdinalProvider(objectIDReader.ColumnOrdinalProvider);
      Assert.That(objectIDReader.IDProperty, Is.SameAs(_objectIDProperty));
    }

    [Test]
    public void CreateTimestampReader ()
    {
      var result = _factory.CreateTimestampReader(_entityDefinitionStub.Object, new[] { _column1, _column2 });

      Assert.That(result, Is.TypeOf(typeof(TimestampReader)));
      var timestampReader = (TimestampReader)result;
      CheckOrdinalProvider(timestampReader.ColumnOrdinalProvider);
      Assert.That(timestampReader.IDProperty, Is.SameAs(_objectIDProperty));
      Assert.That(timestampReader.TimestampProperty, Is.SameAs(_timestampProperty));
    }

    [Test]
    public void CreateResultRowReader ()
    {
      var result = _factory.CreateResultRowReader();

      Assert.That(result, Is.TypeOf(typeof(QueryResultRowReader)));
      var queryResultRowReader = (QueryResultRowReader)result;
      Assert.That(queryResultRowReader.StorageTypeInformationProvider, Is.SameAs(_storageTypeInformationProviderStub.Object));
    }

    private void CheckOrdinalProvider (IColumnOrdinalProvider ordinalProvider)
    {
      Assert.That(ordinalProvider, Is.TypeOf(typeof(DictionaryBasedColumnOrdinalProvider)));
      Assert.That(((DictionaryBasedColumnOrdinalProvider)ordinalProvider).Ordinals.Count, Is.EqualTo(2));
      Assert.That(((DictionaryBasedColumnOrdinalProvider)ordinalProvider).Ordinals[_column1.Name], Is.EqualTo(0));
      Assert.That(((DictionaryBasedColumnOrdinalProvider)ordinalProvider).Ordinals[_column2.Name], Is.EqualTo(1));
    }
  }
}
