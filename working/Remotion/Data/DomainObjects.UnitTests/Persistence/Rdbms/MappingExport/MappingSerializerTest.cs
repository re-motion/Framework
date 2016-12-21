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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class MappingSerializerTest : SchemaGenerationTestBase
  {
    private Func<RdbmsProviderDefinition, IEnumSerializer> _enumSerializerFactoryStub;
    private Func<RdbmsProviderDefinition, IEnumSerializer, IStorageProviderSerializer> _storageProviderSerializerFactoryStub;
    private MappingSerializer _mappingSerializer;

    public override void SetUp ()
    {
      base.SetUp();
      _enumSerializerFactoryStub = MockRepository.GenerateStub<Func<RdbmsProviderDefinition, IEnumSerializer>>();
      _storageProviderSerializerFactoryStub =
          MockRepository.GenerateStub<Func<RdbmsProviderDefinition, IEnumSerializer, IStorageProviderSerializer>>();
      _mappingSerializer = new MappingSerializer (_enumSerializerFactoryStub, _storageProviderSerializerFactoryStub);
    }

    [Test]
    public void Serialize_CreatesXDocument ()
    {
      var enumSerializerStub = MockRepository.GenerateStub<IEnumSerializer>();
      _storageProviderSerializerFactoryStub.Stub (_ => _ (null, null))
          .IgnoreArguments()
          .Return (MockRepository.GenerateStub<IStorageProviderSerializer>());
      _enumSerializerFactoryStub.Stub (_ => _ (null)).IgnoreArguments().Return (enumSerializerStub);
      enumSerializerStub.Stub (_ => _.Serialize()).Return (new XElement[0]);

      var actual = _mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That (actual.Root.Name.LocalName, Is.EqualTo ("mapping"));
    }

    [Test]
    public void Serialize_UsesCorrectNamespace ()
    {
      var enumSerializerStub = MockRepository.GenerateStub<IEnumSerializer>();
      _storageProviderSerializerFactoryStub.Stub (_ => _ (null, null))
          .IgnoreArguments()
          .Return (MockRepository.GenerateStub<IStorageProviderSerializer>());
      _enumSerializerFactoryStub.Stub (_ => _ (null)).IgnoreArguments().Return (enumSerializerStub);
      enumSerializerStub.Stub (_ => _.Serialize()).Return (new XElement[0]);

      var actual = _mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That (actual.Root.Name.Namespace.NamespaceName, Is.EqualTo ("http://www.re-motion.org/Data/DomainObjects/Rdbms/Mapping/1.0"));
    }

    [Test]
    public void Serialize_AddsStorageProviderElements ()
    {
      var enumSerializerStub = MockRepository.GenerateStub<IEnumSerializer>();
      var storageProviderSerializerStub = MockRepository.GenerateStub<IStorageProviderSerializer>();
      _storageProviderSerializerFactoryStub.Stub (_ => _ (null, null)).IgnoreArguments().Return (storageProviderSerializerStub);
      _enumSerializerFactoryStub.Stub (_ => _ (null)).IgnoreArguments().Return (enumSerializerStub);
      enumSerializerStub.Stub (_ => _.Serialize()).Return (new XElement[0]);

      var expected = new XElement ("storageProvider");
      storageProviderSerializerStub.Stub (_ => _.Serialize (null, null)).IgnoreArguments().Return (expected);

      var actual = _mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That (actual.Root.Elements().First(), Is.SameAs (expected));
    }

    [Test]
    public void Serialize_AddsEnumTypes ()
    {
      var enumSerializerStub = MockRepository.GenerateStub<IEnumSerializer>();
      var storageProviderSerializerStub = MockRepository.GenerateStub<IStorageProviderSerializer>();
      _storageProviderSerializerFactoryStub.Stub (_ => _ (null, null)).IgnoreArguments().Return (storageProviderSerializerStub);
      _enumSerializerFactoryStub.Stub (_ => _ (null)).IgnoreArguments().Return (enumSerializerStub);

      var storageProviderElement = new XElement ("storageProvider");
      var enumTypeElement = new XElement ("enumType");

      storageProviderSerializerStub.Stub (_ => _.Serialize (null, null)).IgnoreArguments().Return (storageProviderElement);
      enumSerializerStub.Stub (_ => _.Serialize()).Return (new[] { enumTypeElement });

      var actual = _mappingSerializer.Serialize (MappingConfiguration.Current.GetTypeDefinitions());

      Assert.That (actual.Root.Elements ("storageProvider").Count(), Is.GreaterThanOrEqualTo (1));
      Assert.That (actual.Root.Elements ("enumType").Count(), Is.GreaterThanOrEqualTo (1));
    }

    [Test]
    public void Serialize_GroupsClassDefinitionsByStorageProvider ()
    {
      var classDefinitions = MappingConfiguration.Current.GetTypeDefinitions();
      var storageProviders = classDefinitions
          .Select (c => c.StorageEntityDefinition.StorageProviderDefinition).Distinct().OfType<RdbmsProviderDefinition>().ToArray();

      var enumSerializerStub = MockRepository.GenerateStub<IEnumSerializer>();
      _enumSerializerFactoryStub.Stub (_ => _ (null)).IgnoreArguments().Return (enumSerializerStub);

      var enumTypeElement = new XElement ("enumType");
      enumSerializerStub.Stub (_ => _.Serialize()).Return (new[] { enumTypeElement });

      foreach (var rdbmsProviderDefinition in storageProviders)
      {
        var storageProviderSerializerMock = MockRepository.GenerateStrictMock<IStorageProviderSerializer>();
        _storageProviderSerializerFactoryStub.Stub (_ => _ (rdbmsProviderDefinition, enumSerializerStub)).Return (storageProviderSerializerMock);

        storageProviderSerializerMock.Expect (_ => _.Serialize (Arg<IEnumerable<ClassDefinition>>.Is.Anything, Arg.Is (rdbmsProviderDefinition)))
            .Return (new XElement ("storageProvider"));

        storageProviderSerializerMock.Replay();
      }

      _mappingSerializer.Serialize (classDefinitions);
    }
  }
}