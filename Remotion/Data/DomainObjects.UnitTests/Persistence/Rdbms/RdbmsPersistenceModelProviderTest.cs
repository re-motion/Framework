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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsPersistenceModelProviderTest : StandardMappingTest
  {
    private RdbmsPersistenceModelProvider _provider;
    private TypeDefinition _typeDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = new RdbmsPersistenceModelProvider();
      _typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition("Order", typeof(Order));
    }

    [Test]
    public void GetEntityDefinition ()
    {
      var entityDefinition = new Mock<IRdbmsStorageEntityDefinition>();
      _typeDefinition.SetStorageEntity(entityDefinition.Object);

      var result = _provider.GetEntityDefinition(_typeDefinition);

      Assert.That(result, Is.SameAs(entityDefinition.Object));
    }

    [Test]
    public void GetEntityDefinition_EmptyViewDefinition ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition("Order", classType: typeof(Order), baseClass: null);
      Assert.That(typeDefinition.HasStorageEntityDefinitionBeenSet, Is.False);
      Assert.That(
          () => _provider.GetEntityDefinition(typeDefinition),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "The Rdbms provider classes require a storage definition object of type 'IRdbmsStorageEntityDefinition' for type definition "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order', "
                  + "but that type definition has no storage definition object."));
    }

    [Test]
    public void GetEntityDefinition_WrongEntityDefinition ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition("Order", typeof(Order));
      typeDefinition.SetStorageEntity(new FakeStorageEntityDefinition(TestDomainStorageProviderDefinition, "Test"));
      Assert.That(
          () => _provider.GetEntityDefinition(typeDefinition),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "The Rdbms provider classes require a storage definition object of type 'IRdbmsStorageEntityDefinition' for type definition "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order', "
                  + "but that type definition has a storage definition object of type 'FakeStorageEntityDefinition'."));
    }

    [Test]
    public void GetStoragePropertyDefinition ()
    {
      var storagePropertyDefinition = new Mock<IRdbmsStoragePropertyDefinition>();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      propertyDefinition.SetStorageProperty(storagePropertyDefinition.Object);

      var result = _provider.GetStoragePropertyDefinition(propertyDefinition);

      Assert.That(result, Is.SameAs(storagePropertyDefinition.Object));
    }

    [Test]
    public void GetStoragePropertyDefinition_NoDefinition ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(_typeDefinition, "OrderNumber");
      Assert.That(propertyDefinition.HasStoragePropertyDefinitionBeenSet, Is.False);
      Assert.That(
          () => _provider.GetStoragePropertyDefinition(propertyDefinition),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The Rdbms provider classes require a storage definition object of type 'IRdbmsStoragePropertyDefinition' for property 'OrderNumber' of "
                  + "class-definition 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order', but that property has no storage definition object."));
    }

    [Test]
    public void GetStoragePropertyDefinition_NoRdbmsDefinition ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(_typeDefinition, "OrderNumber");
      var storagePropertyDefinition = new FakeStoragePropertyDefinition("Test");
      propertyDefinition.SetStorageProperty(storagePropertyDefinition);
      Assert.That(
          () => _provider.GetStoragePropertyDefinition(propertyDefinition),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The Rdbms provider classes require a storage definition object of type 'IRdbmsStoragePropertyDefinition' for property 'OrderNumber' of "
                  + "class-definition 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order', "
                  + "but that property has a storage definition object of type 'FakeStoragePropertyDefinition'."));
    }
  }
}
