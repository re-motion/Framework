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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class PropertyIndexerTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private IndustrialSector _industrialSector;
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _industrialSector = _transaction.ExecuteInScope(() => IndustrialSector.NewObject());
      _order = _transaction.ExecuteInScope(() => Order.NewObject());
    }

    [Test]
    public void Item ()
    {
      var indexer = new PropertyIndexer(_industrialSector);
      var accessor = indexer["Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"];
      Assert.That(accessor, Is.Not.Null);
      Assert.That(
          accessor.PropertyData.PropertyDefinition,
          Is.SameAs(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector))
                  .GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name")));
    }

    [Test]
    public void Item_UsesAssociatedActiveTransactionByDefault ()
    {
      var indexer = new PropertyIndexer(_industrialSector);
      var accessor1 = indexer["Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"];
      Assert.That(accessor1.ClientTransaction, Is.SameAs(_transaction));

      var subTransaction = _transaction.CreateSubTransaction();

      var accessor2 = indexer["Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"];
      Assert.That(accessor2.ClientTransaction, Is.SameAs(_transaction));

      using (subTransaction.EnterDiscardingScope())
      {
        var accessor3 = indexer["Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"];
        Assert.That(accessor3.ClientTransaction, Is.SameAs(subTransaction));
      }
  }

    [Test]
    public void Item_WithSpecificTransaction_AllowsPassingAnInactiveParentTransaction ()
    {
      var indexer = new PropertyIndexer(_industrialSector);
      var subTransaction = _transaction.CreateSubTransaction();
      var accessor = indexer["Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name", _transaction];
      Assert.That(accessor.ClientTransaction, Is.Not.SameAs(subTransaction));
      Assert.That(accessor.ClientTransaction, Is.SameAs(_transaction));
    }

    [Test]
    public void Item_WithShortNotation ()
    {
      var indexer = new PropertyIndexer(_industrialSector);
      var accessor = indexer[typeof(IndustrialSector), "Name"];
      Assert.That(accessor.ClientTransaction, Is.SameAs(_transaction));
      Assert.That(
          accessor.PropertyData.PropertyDefinition,
          Is.SameAs(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector))
                  .GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name")));
    }

    [Test]
    public void Item_WithShortNotation_WithSpecificTransaction ()
    {
      var transaction = _industrialSector.RootTransaction.CreateSubTransaction();

      var indexer = new PropertyIndexer(_industrialSector);
      var accessor1 = indexer[typeof(IndustrialSector), "Name"];
      Assert.That(accessor1.ClientTransaction, Is.Not.SameAs(transaction));

      var accessor2 = indexer[typeof(IndustrialSector), "Name", transaction];
      Assert.That(accessor2.ClientTransaction, Is.SameAs(transaction));
    }

    [Test]
    public void Item_ThrowsForNonExistingProperty ()
    {
      var indexer = new PropertyIndexer(_industrialSector);
      Assert.That(
          () => indexer["Bla"],
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector' does not have a mapping property named 'Bla'."));
    }

    [Test]
    public void Count ()
    {
      Assert.That(_order.Properties.GetPropertyCount(), Is.EqualTo(6));

      OrderItem orderItem = _transaction.ExecuteInScope(() => OrderItem.NewObject());
      Assert.That(orderItem.Properties.GetPropertyCount(), Is.EqualTo(3));

      ClassWithAllDataTypes cwadt = _transaction.ExecuteInScope(() =>ClassWithAllDataTypes.NewObject());
      Assert.That(cwadt.Properties.GetPropertyCount(), Is.EqualTo(48));
    }

    [Test]
    public void AsEnumerable_GetsAllProperties ()
    {
      var propertyNames = (from propertyAccessor in _order.Properties.AsEnumerable()
                           select propertyAccessor.PropertyData.PropertyIdentifier).ToArray();

      Assert.That(propertyNames, Is.EquivalentTo(new[] {
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
        "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"
      }));
    }

    [Test]
    public void AsEnumerable_UsesAssociatedActiveTransaction ()
    {
      var transactions1 = (from propertyAccessor in _order.Properties.AsEnumerable()
                          select propertyAccessor.ClientTransaction).Distinct().ToArray();
      Assert.That(transactions1, Is.EqualTo(new[] { _transaction }));

      var subTransaction = _transaction.CreateSubTransaction();

      var transactions2 = (from propertyAccessor in _order.Properties.AsEnumerable()
                           select propertyAccessor.ClientTransaction).Distinct().ToArray();
      Assert.That(transactions2, Is.EqualTo(new[] { _transaction }));

      using (subTransaction.EnterDiscardingScope())
      {
        var transactions3 = (from propertyAccessor in _order.Properties.AsEnumerable()
                             select propertyAccessor.ClientTransaction).Distinct().ToArray();
        Assert.That(transactions3, Is.EqualTo(new[] { subTransaction }));
      }
    }

    [Test]
    public void AsEnumerable_SpecificTransaction_AllowsPassingAnInactiveParentTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();

      var transactions = (from propertyAccessor in _order.Properties.AsEnumerable(_transaction)
                          select propertyAccessor.ClientTransaction).Distinct().ToArray();
      Assert.That(transactions, Is.Not.EqualTo(new[] { subTransaction }));
      Assert.That(transactions, Is.EqualTo(new[] { _transaction }));
    }

    [Test]
    public void AsEnumerable_InvalidTransaction ()
    {
      Assert.That(
          () => (from propertyAccessor in _order.Properties.AsEnumerable(ClientTransaction.CreateRootTransaction())
       select propertyAccessor.ClientTransaction).ToArray(),
          Throws.InstanceOf<ClientTransactionsDifferException>()
              .With.Message.Contains("cannot be used in the given transaction "));
    }

    [Test]
    public void Contains ()
    {
      Assert.That(_order.Properties.Contains("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"), Is.True);
      Assert.That(_order.Properties.Contains("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"), Is.True);
      Assert.That(_order.Properties.Contains("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), Is.True);
      Assert.That(_order.Properties.Contains("OrderTicket"), Is.False);
      Assert.That(_order.Properties.Contains("Bla"), Is.False);
    }

    [Test]
    public void ShortNameAndType ()
    {
      Assert.That(
          _order.Properties[typeof(Order), "OrderNumber"],
          Is.EqualTo(_order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void ShortNameAndTypeWithShadowedProperties ()
    {
      var classWithDifferentProperties =
          (DerivedClassWithDifferentProperties)LifetimeService.NewObject(_transaction, typeof(DerivedClassWithDifferentProperties), ParamList.Empty);

      var indexer = new PropertyIndexer(classWithDifferentProperties);
      Assert.That(
          indexer[typeof(DerivedClassWithDifferentProperties), "String"],
          Is.EqualTo(indexer[typeof(DerivedClassWithDifferentProperties).FullName + ".String"]));
      Assert.That(
          indexer[typeof(ClassWithDifferentProperties), "String"],
          Is.EqualTo(indexer[typeof(ClassWithDifferentProperties).FullName + ".String"]));
    }

    [Test]
    public void Find_Property ()
    {
      var result = _order.Properties.Find(typeof(Order), "OrderNumber");
      Assert.That(result, Is.EqualTo(_order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]));
    }

    [Test]
    public void Find_VirtualRelationEndPoint ()
    {
      var result = _order.Properties.Find(typeof(Order), "OrderItems");
      Assert.That(result, Is.EqualTo(_order.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"]));
    }

    [Test]
    public void Find_WithoutExplicitType ()
    {
      Distributor distributor = _transaction.ExecuteInScope(() => Distributor.NewObject());
      Assert.That(distributor.Properties.Contains(typeof(Distributor).FullName + ".ContactPerson"), Is.False);
      Assert.That(distributor.Properties.Find("ContactPerson"), Is.EqualTo(distributor.Properties[typeof(Partner), "ContactPerson"]));
    }

    [Test]
    public void Find_Generic_WithInferredType ()
    {
      var classWithDifferentProperties =
          (DerivedClassWithDifferentProperties)LifetimeService.NewObject(_transaction, typeof(DerivedClassWithDifferentProperties), ParamList.Empty);
      var indexer = new PropertyIndexer(classWithDifferentProperties);

      var resultOnDerived = indexer.Find(classWithDifferentProperties, "String");
      Assert.That(resultOnDerived, Is.EqualTo(indexer[typeof(DerivedClassWithDifferentProperties).FullName + ".String"]));

      var resultOnBase = indexer.Find((ClassWithDifferentProperties)classWithDifferentProperties, "String");
      Assert.That(resultOnBase, Is.EqualTo(indexer[typeof(ClassWithDifferentProperties).FullName + ".String"]));
    }

    [Test]
    public void Find_NonExistingProperty ()
    {
      Distributor distributor = _transaction.ExecuteInScope(() => Distributor.NewObject());
      Assert.That(
          () => distributor.Properties.Find(typeof(Distributor), "Frobbers"),
          Throws.ArgumentException
              .With.Message.Contains(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Distributor' does not have or inherit a mapping property with the "
                  + "short name 'Frobbers'."));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainRoot ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>(_transaction);
      var relatedObjects = new List<DomainObject>(order.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Has.No.Member(order));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainIndirectRelatedObjects ()
    {
      Ceo ceo = DomainObjectIDs.Ceo1.GetObject<Ceo>(_transaction);
      var relatedObjects = new List<DomainObject>(ceo.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Has.No.Member(ceo.Company.IndustrialSector));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainDuplicates ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>(_transaction);
      var relatedObjects = new List<DomainObject>(order.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Is.Unique);
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainNulls ()
    {
      var relatedObjects = new List<DomainObject>(_order.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Has.No.Member(null));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>(_transaction);
      var relatedObjects = new List<DomainObject>(order.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Has.Member(order.Official));
      Assert.That(relatedObjects, Has.Member(order.OrderTicket));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectBothSides ()
    {
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer>(_transaction);
      var relatedObjects = new List<DomainObject>(computer.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Has.Member(computer.Employee));

      Employee employee = DomainObjectIDs.Employee3.GetObject<Employee>(_transaction);
      relatedObjects = new List<DomainObject>(employee.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Has.Member(employee.Computer));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectUnidirectional ()
    {
      Client client = DomainObjectIDs.Client2.GetObject<Client>(_transaction);
      var relatedObjects = new List<DomainObject>(client.Properties.GetAllRelatedObjects());
      Assert.That(relatedObjects, Has.Member(client.ParentClient));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsRelatedObjectsFromDomainObjectCollection ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>(_transaction);
      var relatedObjects = new List<DomainObject>(order.Properties.GetAllRelatedObjects());
      Assert.That(order.OrderItems, Is.SubsetOf(relatedObjects));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsRelatedObjectsFromVirtualCollection ()
    {
      Product product = DomainObjectIDs.Product1.GetObject<Product>(_transaction);
      var relatedObjects = new List<DomainObject>(product.Properties.GetAllRelatedObjects());
      Assert.That(product.Reviews, Is.SubsetOf(relatedObjects));
    }

    [Test]
    public void PropertyIndexer_CachesPropertyData ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>(_transaction);
      var indexer = new PropertyIndexer(order);
      Assert.That(indexer[typeof(Order), "OrderNumber"].PropertyData, Is.SameAs(indexer[typeof(Order), "OrderNumber"].PropertyData));
    }
  }
}
