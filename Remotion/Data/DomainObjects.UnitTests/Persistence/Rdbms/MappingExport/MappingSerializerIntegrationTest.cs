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
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class MappingSerializerIntegrationTest : SchemaGenerationTestBase
  {
    [Test]
    public void Serialize ()
    {
      var sqlStorageObjectFactory = new SqlStorageObjectFactory(
          StorageSettings,
          ServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          ServiceLocator.Current.GetInstance<IDataContainerValidator>());
      var mappingSerializer =
          new MappingSerializer(
              d => sqlStorageObjectFactory.CreateEnumSerializer(),
              (d, enumSerializer) => sqlStorageObjectFactory.CreateStorageProviderSerializer(enumSerializer));

      var actual = mappingSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinitions());
      var expected = XDocument.Load(new MemoryStream(ResourceManager.GetMappingExportOutput()));

      Assert.That(actual.ToString(), Is.EqualTo(expected.ToString()));
    }

    [Test]
    public void Serialize_OutputIsValid ()
    {
      var sqlStorageObjectFactory = new SqlStorageObjectFactory(
          StorageSettings,
          ServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          ServiceLocator.Current.GetInstance<IDataContainerValidator>());
      var mappingSerializer =
          new MappingSerializer(
              d => sqlStorageObjectFactory.CreateEnumSerializer(),
              (d, enumSerializer) => sqlStorageObjectFactory.CreateStorageProviderSerializer(enumSerializer));

      var actual = mappingSerializer.Serialize(MappingConfiguration.Current.GetTypeDefinitions());

      var schemaSet = new XmlSchemaSet();
      schemaSet.Add(XmlSchema.Read(new MemoryStream(ResourceManager.GetRdbmsMappingSchema()), null));
      actual.Validate(schemaSet, null);
    }
  }
}
