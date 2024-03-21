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
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class MappingSerializerTest : SchemaGenerationTestBase
  {
    private Mock<Func<RdbmsProviderDefinition, IEnumSerializer>> _enumSerializerFactoryStub;
    private Mock<Func<RdbmsProviderDefinition, IEnumSerializer, IStorageProviderSerializer>> _storageProviderSerializerFactoryStub;
    private MappingSerializer _mappingSerializer;

    public override void SetUp ()
    {
      base.SetUp();
      _enumSerializerFactoryStub = new Mock<Func<RdbmsProviderDefinition, IEnumSerializer>>();
      _storageProviderSerializerFactoryStub =
          new Mock<Func<RdbmsProviderDefinition, IEnumSerializer, IStorageProviderSerializer>>();
      _mappingSerializer = new MappingSerializer(_enumSerializerFactoryStub.Object, _storageProviderSerializerFactoryStub.Object);
    }

    [Test]
    public void Serialize_CreatesXDocument ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      _storageProviderSerializerFactoryStub
          .Setup(_ => _(It.IsAny<RdbmsProviderDefinition>(), It.IsAny<IEnumSerializer>()))
          .Returns(new Mock<IStorageProviderSerializer>().Object);
      _enumSerializerFactoryStub
          .Setup(_ => _(It.IsAny<RdbmsProviderDefinition>()))
          .Returns(enumSerializerStub.Object);
      enumSerializerStub
          .Setup(_ => _.Serialize())
          .Returns(new XElement[0]);

      var actual = _mappingSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That(actual.Root.Name.LocalName, Is.EqualTo("mapping"));
    }

    [Test]
    public void Serialize_UsesCorrectNamespace ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      _storageProviderSerializerFactoryStub
          .Setup(_ => _(It.IsAny<RdbmsProviderDefinition>(), It.IsAny<IEnumSerializer>()))
          .Returns(new Mock<IStorageProviderSerializer>().Object);
      _enumSerializerFactoryStub
          .Setup(_ => _(It.IsAny<RdbmsProviderDefinition>()))
          .Returns(enumSerializerStub.Object);
      enumSerializerStub
          .Setup(_ => _.Serialize())
          .Returns(new XElement[0]);

      var actual = _mappingSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That(actual.Root.Name.Namespace.NamespaceName, Is.EqualTo("http://www.re-motion.org/Data/DomainObjects/Rdbms/Mapping/1.0"));
    }

    [Test]
    public void Serialize_AddsStorageProviderElements ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var storageProviderSerializerStub = new Mock<IStorageProviderSerializer>();
      _storageProviderSerializerFactoryStub.Setup(_ => _(It.IsAny<RdbmsProviderDefinition>(), It.IsAny<IEnumSerializer>())).Returns(storageProviderSerializerStub.Object);
      _enumSerializerFactoryStub.Setup(_ => _(It.IsAny<RdbmsProviderDefinition>())).Returns(enumSerializerStub.Object);
      enumSerializerStub.Setup(_ => _.Serialize()).Returns(new XElement[0]);

      var expected = new XElement("storageProvider");
      storageProviderSerializerStub.Setup(_ => _.Serialize(It.IsAny<IEnumerable<TypeDefinition>>(), It.IsAny<RdbmsProviderDefinition>())).Returns(expected);

      var actual = _mappingSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That(actual.Root.Elements().First(), Is.SameAs(expected));
    }

    [Test]
    public void Serialize_AddsEnumTypes ()
    {
      var enumSerializerStub = new Mock<IEnumSerializer>();
      var storageProviderSerializerStub = new Mock<IStorageProviderSerializer>();
      _storageProviderSerializerFactoryStub.Setup(_ => _(It.IsAny<RdbmsProviderDefinition>(), It.IsAny<IEnumSerializer>())).Returns(storageProviderSerializerStub.Object);
      _enumSerializerFactoryStub.Setup(_ => _(It.IsAny<RdbmsProviderDefinition>())).Returns(enumSerializerStub.Object);

      var storageProviderElement = new XElement("storageProvider");
      var enumTypeElement = new XElement("enumType");

      storageProviderSerializerStub.Setup(_ => _.Serialize(It.IsAny<IEnumerable<TypeDefinition>>(), It.IsAny<RdbmsProviderDefinition>())).Returns(storageProviderElement);
      enumSerializerStub.Setup(_ => _.Serialize()).Returns(new[] { enumTypeElement });

      var actual = _mappingSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That(actual.Root.Elements("storageProvider").Count(), Is.GreaterThanOrEqualTo(1));
      Assert.That(actual.Root.Elements("enumType").Count(), Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public void Serialize_GroupsTypeDefinitionsByStorageProvider ()
    {
      var typeDefinitions = MappingConfiguration.Current.GetTypeDefinitions();
      var storageProviders = typeDefinitions
          .Select(c => c.StorageEntityDefinition.StorageProviderDefinition).Distinct().OfType<RdbmsProviderDefinition>().ToArray();

      var enumSerializerStub = new Mock<IEnumSerializer>();
      _enumSerializerFactoryStub.Setup(_ => _(It.IsAny<RdbmsProviderDefinition>())).Returns(enumSerializerStub.Object);

      var enumTypeElement = new XElement("enumType");
      enumSerializerStub.Setup(_ => _.Serialize()).Returns(new[] { enumTypeElement });

      foreach (var rdbmsProviderDefinition in storageProviders)
      {
        var storageProviderSerializerMock = new Mock<IStorageProviderSerializer>(MockBehavior.Strict);
        _storageProviderSerializerFactoryStub.Setup(_ => _(rdbmsProviderDefinition, enumSerializerStub.Object)).Returns(storageProviderSerializerMock.Object);

        storageProviderSerializerMock
            .Setup(_ => _.Serialize(It.IsAny<IEnumerable<TypeDefinition>>(), rdbmsProviderDefinition))
            .Returns(new XElement("storageProvider"))
            .Verifiable();
      }

      _mappingSerializer.Serialize(typeDefinitions);
    }
  }
}
