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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class TableInheritanceStorageEntitiesIntegrationTest : TableInheritanceMappingTest
  {
    [Test]
    public void TableInheritanceBaseClass ()
    {
      var domainBaseClass = Configuration.GetTypeDefinition(typeof(TIDomainBase));
      Assert.That(domainBaseClass, Is.Not.Null);
      Assert.That(domainBaseClass.StorageEntityDefinition, Is.TypeOf<UnionViewDefinition>());

      var personClass = Configuration.GetTypeDefinition(typeof(TIPerson));
      Assert.That(personClass.StorageEntityDefinition, Is.TypeOf<TableDefinition>());
      Assert.That(((TableDefinition)personClass.StorageEntityDefinition).TableName.EntityName, Is.EqualTo("TableInheritance_Person"));

      var organizationalUnitClass = Configuration.GetTypeDefinition(typeof(TIOrganizationalUnit));
      Assert.That(organizationalUnitClass.StorageEntityDefinition, Is.TypeOf<TableDefinition>());
      Assert.That(((TableDefinition)organizationalUnitClass.StorageEntityDefinition).TableName.EntityName, Is.EqualTo("TableInheritance_OrganizationalUnit"));

      Assert.That(
          ((UnionViewDefinition)domainBaseClass.StorageEntityDefinition).UnionedEntities,
          Is.EquivalentTo(new[] { personClass.StorageEntityDefinition, organizationalUnitClass.StorageEntityDefinition }));

      var customerClass = Configuration.GetTypeDefinition(typeof(TICustomer));
      Assert.That(customerClass.StorageEntityDefinition, Is.TypeOf<FilterViewDefinition>());
      Assert.That(((FilterViewDefinition)customerClass.StorageEntityDefinition).BaseEntity, Is.SameAs(personClass.StorageEntityDefinition));
      Assert.That(((FilterViewDefinition)customerClass.StorageEntityDefinition).ClassIDs, Is.EqualTo(new[] { "TI_Customer" }));
    }

    [Test]
    public void ClassWithoutEntity_AndWithoutHierarchy ()
    {
      var classDefinition = Configuration.GetTypeDefinition(typeof(AbstractClassWithoutHierarchy));
      Assert.That(classDefinition.StorageEntityDefinition, Is.TypeOf<EmptyViewDefinition>());
    }
  }
}
