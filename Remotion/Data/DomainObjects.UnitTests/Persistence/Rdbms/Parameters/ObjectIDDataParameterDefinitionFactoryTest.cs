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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class ObjectIDDataParameterDefinitionFactoryTest : StandardMappingTest
{
  private Mock<IDataParameterDefinitionFactory> _nextDataParameterDefinitionFactoryMock;

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp();

    _nextDataParameterDefinitionFactoryMock = new Mock<IDataParameterDefinitionFactory>();
  }

  [Test]
  public void Initialize ()
  {
    var storageProviderDefinition = new TestableStorageProviderDefinition("name", Mock.Of<IStorageObjectFactory>());
    var storageTypeInformationProvider = Mock.Of<IStorageTypeInformationProvider>();
    var storageSettings = Mock.Of<IStorageSettings>();

    var result = new ObjectIDDataParameterDefinitionFactory(storageProviderDefinition, storageTypeInformationProvider, storageSettings, _nextDataParameterDefinitionFactoryMock.Object);

    Assert.That(result.StorageProviderDefinition, Is.SameAs(storageProviderDefinition));
    Assert.That(result.StorageTypeInformationProvider, Is.SameAs(storageTypeInformationProvider));
    Assert.That(result.StorageSettings, Is.SameAs(storageSettings));
  }

  [Test]
  public void CreateDataParameterDefinition_WithObjectIDInSameProviderDefinition ()
  {
    var storageProviderDefinition = DomainObjectIDs.Customer1.StorageProviderDefinition;
    var otherObjectID = DomainObjectIDs.Customer2;

    var storageTypeInformationStub = new Mock<IStorageTypeInformation>();

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub
        .Setup(_ => _.GetStorageTypeForID(true))
        .Returns(storageTypeInformationStub.Object);

    var factory = new ObjectIDDataParameterDefinitionFactory(
        storageProviderDefinition,
        storageTypeInformationProviderStub.Object,
        StorageSettings,
        _nextDataParameterDefinitionFactoryMock.Object);

    var queryStub = new Mock<IQuery>();
    _nextDataParameterDefinitionFactoryMock
        .Setup(_ => _.CreateDataParameterDefinition(It.IsAny<QueryParameter>(), queryStub.Object))
        .Throws(new AssertionException("Should not be called."));

    var result = factory.CreateDataParameterDefinition(new QueryParameter("other", otherObjectID), queryStub.Object);

    Assert.That(result, Is.InstanceOf<ObjectIDDataParameterDefinition>());
    Assert.That(result.As<ObjectIDDataParameterDefinition>().ValueStorageTypeInformation, Is.SameAs(storageTypeInformationStub.Object));
  }

  [Test]
  public void CreateDataParameterDefinition_WithObjectIDInOtherProviderDefinition ()
  {
    var providerDefinition = new TestableStorageProviderDefinition("bla", Mock.Of<IStorageObjectFactory>());
    var otherObjectID = DomainObjectIDs.Order5;

    var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation(dotNetType: typeof(string));

    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub
        .Setup(_ => _.GetStorageTypeForSerializedObjectID(true))
        .Returns(storageTypeInformation);

    var factory = new ObjectIDDataParameterDefinitionFactory(
        providerDefinition,
        storageTypeInformationProviderStub.Object,
        StorageSettings,
        _nextDataParameterDefinitionFactoryMock.Object);

    var queryStub = new Mock<IQuery>();
    _nextDataParameterDefinitionFactoryMock
        .Setup(_ => _.CreateDataParameterDefinition(It.IsAny<QueryParameter>(), queryStub.Object))
        .Throws(new AssertionException("Should not be called."));

    var result = factory.CreateDataParameterDefinition(new QueryParameter("other", otherObjectID), queryStub.Object);

    Assert.That(result, Is.InstanceOf<SerializedObjectIDDataParameterDefinition>());
    Assert.That(result.As<SerializedObjectIDDataParameterDefinition>().StorageTypeInformation, Is.SameAs(storageTypeInformation));
  }

  [Test]
  public void CreateDataParameterDefinition_WithSimpleValue_HandsQueryParameterToNextFactory ()
  {
    var providerDefinition = new TestableStorageProviderDefinition("bla", Mock.Of<IStorageObjectFactory>());
    var dummy = "Dummy";
    var queryStub = new Mock<IQuery>();
    var queryParameter = new QueryParameter("dummy", dummy);

    var storageTypeInformation = Mock.Of<IStorageTypeInformation>();
    var storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
    storageTypeInformationProviderStub
        .Setup(_ => _.GetStorageType(dummy))
        .Returns(storageTypeInformation);

    var expectedDataParameterDefinition = Mock.Of<IDataParameterDefinition>();
    _nextDataParameterDefinitionFactoryMock
        .Setup(_ => _.CreateDataParameterDefinition(queryParameter, queryStub.Object))
        .Returns(expectedDataParameterDefinition);

    var dataParameterDefinitionFactory = new ObjectIDDataParameterDefinitionFactory(
        providerDefinition,
        storageTypeInformationProviderStub.Object,
        StorageSettings,
        _nextDataParameterDefinitionFactoryMock.Object);

    var result = dataParameterDefinitionFactory.CreateDataParameterDefinition(queryParameter, queryStub.Object);

    Assert.That(result, Is.SameAs(expectedDataParameterDefinition));
  }
}
