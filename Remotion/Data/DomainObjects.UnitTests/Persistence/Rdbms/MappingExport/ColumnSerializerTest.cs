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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class ColumnSerializerTest : SchemaGenerationTestBase
  {
    [Test]
    public void Serialize_CreatesColumnElement ()
    {
      var propertyDefinition = GetPropertyDefinition((ClassWithAllDataTypes _) => _.Int32Property);
      var typeDefinition = propertyDefinition.TypeDefinition;

      var columnSerializer = new ColumnSerializer();
      var actual = columnSerializer.Serialize(propertyDefinition, GetRdbmsPersistenceModelProvider(typeDefinition)).ToArray();

      Assert.That(actual.Length, Is.EqualTo(1));
      Assert.That(actual[0].Name.LocalName, Is.EqualTo("column"));
    }

    [Test]
    public void Serialize_AddsNameAttribute ()
    {
      var propertyDefinition = GetPropertyDefinition((ClassWithAllDataTypes _) => _.Int32Property);
      var typeDefinition = propertyDefinition.TypeDefinition;

      var columnSerializer = new ColumnSerializer();
      var actual = columnSerializer.Serialize(propertyDefinition, GetRdbmsPersistenceModelProvider(typeDefinition)).Single();

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("name"));
      Assert.That(actual.Attribute("name").Value, Is.EqualTo("Int32"));
    }

    [Test]
    public void Serialize_AddsTypeAttribute ()
    {
      var propertyDefinition = GetPropertyDefinition((ClassWithAllDataTypes _) => _.Int32Property);
      var typeDefinition = propertyDefinition.TypeDefinition;

      var columnSerializer = new ColumnSerializer();
      var actual = columnSerializer.Serialize(propertyDefinition, GetRdbmsPersistenceModelProvider(typeDefinition)).Single();

      Assert.That(actual.Attributes().Select(a => a.Name.LocalName), Contains.Item("dbType"));
      Assert.That(actual.Attribute("dbType").Value, Is.EqualTo("Int32"));
    }

    [Test]
    public void Serialize_RelationProperty ()
    {
      var propertyDefinition = GetPropertyDefinition((Ceo _) => _.Company);
      var typeDefinition = propertyDefinition.TypeDefinition;

      var columnSerializer = new ColumnSerializer();
      var actual = columnSerializer.Serialize(propertyDefinition, GetRdbmsPersistenceModelProvider(typeDefinition)).ToArray();

      Assert.That(actual.Length, Is.EqualTo(2));
      Assert.That(actual[0].Attribute("name").Value, Is.EqualTo("CompanyID"));
      Assert.That(actual[0].Attribute("dbType").Value, Is.EqualTo("Guid"));
      Assert.That(actual[1].Attribute("name").Value, Is.EqualTo("CompanyIDClassID"));
      Assert.That(actual[1].Attribute("dbType").Value, Is.EqualTo("AnsiString"));
    }

    private IRdbmsPersistenceModelProvider GetRdbmsPersistenceModelProvider (TypeDefinition typeDefinition)
    {
      var storageProviderDefinition = (RdbmsProviderDefinition)typeDefinition.StorageEntityDefinition.StorageProviderDefinition;
      var persistenceModelProvider = storageProviderDefinition.Factory.CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
      return persistenceModelProvider;
    }
  }
}
