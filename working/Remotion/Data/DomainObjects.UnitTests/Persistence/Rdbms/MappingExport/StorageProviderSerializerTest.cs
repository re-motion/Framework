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
  public class StorageProviderSerializerTest : SchemaGenerationTestBase
  {

    [Test]
    public void Serialize_AddsNameAttribute ()
    {
      var classDefinitions = MappingConfiguration.Current.GetTypeDefinitions();
      var groupedByStorageProvider = classDefinitions.Where (c => c.StorageEntityDefinition.StorageProviderDefinition is RdbmsProviderDefinition)
          .GroupBy (c => (RdbmsProviderDefinition) c.StorageEntityDefinition.StorageProviderDefinition)
          .First();

      var storageProviderSerializer = new StorageProviderSerializer (MockRepository.GenerateStub<IClassSerializer>());
      var actual = storageProviderSerializer.Serialize (groupedByStorageProvider, groupedByStorageProvider.Key);

      Assert.That (actual.Attributes().Select (a => a.Name.LocalName), Contains.Item ("name"));
      Assert.That (actual.Attribute ("name").Value, Is.EqualTo ("SchemaGenerationFirstStorageProvider"));
    }
  
    [Test]
    public void Serialize_AddsClassElements ()
    {
      var classDefinitions = MappingConfiguration.Current.GetTypeDefinitions();
      var groupedByStorageProvider = classDefinitions.Where (c => c.StorageEntityDefinition.StorageProviderDefinition is RdbmsProviderDefinition)
          .GroupBy (c => (RdbmsProviderDefinition) c.StorageEntityDefinition.StorageProviderDefinition)
          .First();

      var classSerializerStub = MockRepository.GenerateStub<IClassSerializer>();
      var expectedElement = new XElement ("class");
      classSerializerStub.Stub (_ => _.Serialize (Arg<ClassDefinition>.Is.NotNull))
          .Return (expectedElement)
          .Repeat.Any();

      var storageProviderSerializer = new StorageProviderSerializer (classSerializerStub);
      var actual = storageProviderSerializer.Serialize (groupedByStorageProvider, groupedByStorageProvider.Key);

      Assert.That (actual.Elements(), Is.Not.Empty);
      Assert.That (actual.Elements().First(), Is.SameAs (expectedElement));
    }
  }
}