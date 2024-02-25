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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderTableInheritanceTest : TableInheritanceMappingTest
  {
    private RdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = RdbmsProviderObjectMother.CreateForIntegrationTest(StorageSettings, TableInheritanceTestDomainStorageProviderDefinition);
    }

    public override void TearDown ()
    {
      _provider.Dispose();
      base.TearDown();
    }
    [Test]
    public void LoadConcreteSingle ()
    {
      DataContainer customerContainer = _provider.LoadDataContainer(DomainObjectIDs.Customer).LocatedObject;
      Assert.That(customerContainer, Is.Not.Null);
      Assert.That(customerContainer.ID, Is.EqualTo(DomainObjectIDs.Customer));
      Assert.That(customerContainer.GetValue(GetPropertyDefinition(typeof(TIDomainBase), "CreatedBy")), Is.EqualTo("UnitTests"));
      Assert.That(customerContainer.GetValue(GetPropertyDefinition(typeof(TIPerson), "FirstName")), Is.EqualTo("Zaphod"));
      Assert.That(customerContainer.GetValue(GetPropertyDefinition(typeof(TICustomer), "CustomerType")), Is.EqualTo(CustomerType.Premium));
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithAbstractBaseClass ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(TIDomainBase), "Client");
      var createdAtProperty = GetPropertyDefinition(typeof(TIDomainBase), "CreatedAt");
      var sortExpression = new SortExpressionDefinition(new[] { new SortedPropertySpecification(createdAtProperty, SortOrder.Ascending) });

      var loadedDataContainers = _provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          sortExpression,
          DomainObjectIDs.Client).ToList();

      Assert.That(loadedDataContainers, Is.Not.Null);
      Assert.That(loadedDataContainers.Count, Is.EqualTo(4));
      Assert.That(loadedDataContainers[0].ID, Is.EqualTo(DomainObjectIDs.OrganizationalUnit));
      Assert.That(loadedDataContainers[1].ID, Is.EqualTo(DomainObjectIDs.Person));
      Assert.That(loadedDataContainers[2].ID, Is.EqualTo(DomainObjectIDs.PersonForUnidirectionalRelationTest));
      Assert.That(loadedDataContainers[3].ID, Is.EqualTo(DomainObjectIDs.Customer));
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithAbstractClassWithoutDerivations ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(AbstractClassWithoutDerivations), "DomainBase");

      var result = _provider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)relationEndPointDefinition,
          null,
          DomainObjectIDs.Customer);
      Assert.That(result, Is.Not.Null);
      Assert.That(result.Count(), Is.EqualTo(0));
    }
  }
}
