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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class ConcreteTableInheritanceIntegrationTest : IntegrationTestBase
  {
    private TableInheritanceDomainObjectIDs _concreteObjectIDs;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _concreteObjectIDs = new TableInheritanceDomainObjectIDs(Configuration);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass_SingleTableInheritance ()
    {
      var customer = (from c in QueryFactory.CreateLinqQuery<Customer>()
        where c.Name == "Kunde 3"
        select c);

      CheckQueryResult(customer, DomainObjectIDs.Customer3);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass_SingleTableInheritance ()
    {
      // ReSharper disable RedundantNameQualifier
      var customer = (from c in QueryFactory.CreateLinqQuery<Customer>()
        where c.Type == Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerType.Standard
        select c);
      // ReSharper restore RedundantNameQualifier

      CheckQueryResult(customer, DomainObjectIDs.Customer1);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInSameClass_SingleTableInheritance ()
    {
      var orders = (from c in QueryFactory.CreateLinqQuery<Customer>()
        from o in c.Orders
        where c.Name == "Kunde 3"
        select o);

      CheckQueryResult(orders, DomainObjectIDs.Order3);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInBaseClass_SingleTableInheritance ()
    {
      var customers = (from c in QueryFactory.CreateLinqQuery<Customer>()
        where c.IndustrialSector.ID == DomainObjectIDs.IndustrialSector2
        select c);

      CheckQueryResult(customers, DomainObjectIDs.Customer3, DomainObjectIDs.Customer2);
    }

    [Test]
    public void ConcreteObjects_OfType_SelectingBaseType_SingleTableInheritance ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Company>();

      CheckQueryResult(
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void ConcreteObjects_OfType_SelectingSameType_SingleTableInheritance ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Customer>();

      CheckQueryResult(
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void ConcreteObject_OfType_UnrelatedType_ThrowsInvalidOperation_SingleTableInheritance ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Order>();
      Assert.That(
          () => CheckQueryResult(query),
          Throws.InvalidOperationException);
    }

    [Test]
    public void ConcreteObjects_Is_SelectingBaseType_SingleTableInheritance ()
    {
      // ReSharper disable once IsExpressionAlwaysTrue
      var query = QueryFactory.CreateLinqQuery<Customer>().Where(c=> c is Company);

      CheckQueryResult(
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void ConcreteObjects_Is_SelectingSameType_SingleTableInheritance ()
    {
      // ReSharper disable once IsExpressionAlwaysTrue
      var query = QueryFactory.CreateLinqQuery<Customer>().Where(c=> c is Customer);

      CheckQueryResult(
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    [Test]
    public void BaseObjects_PropertyAccessInSameClass_SingleTableInheritance ()
    {
      var company = (from c in QueryFactory.CreateLinqQuery<Company>()
        where c.Name == "Firma 2"
        select c);

      CheckQueryResult(company, DomainObjectIDs.Company2);
    }

    [Test]
    public void BaseObjects_PropertyAccessInSameClassViaRelation_SingleTableInheritance ()
    {
      var order = (from o in QueryFactory.CreateLinqQuery<Order>()
        where o.Customer.Name == "Kunde 4"
        select o);

      CheckQueryResult(
          order,
          DomainObjectIDs.Order4,
          DomainObjectIDs.Order5);
    }

    [Test]
    public void BaseObjects_MemberAccessInSameClass_SingleTableInheritance ()
    {
      var company = (from c in QueryFactory.CreateLinqQuery<Company>()
        where c.IndustrialSector.ID == DomainObjectIDs.IndustrialSector2 && c.Name == "Firma 2"
        select c);

      CheckQueryResult(company, DomainObjectIDs.Company2);
    }

    [Test]
    public void BaseObjects_OfType_SelectingDerivedType_SingleTableInheritance ()
    {
      var partnerIDs = new[]
                       {
                           (Guid) DomainObjectIDs.Partner1.Value,
                           (Guid) DomainObjectIDs.Distributor1.Value,
                           (Guid) DomainObjectIDs.Supplier1.Value,
                           (Guid) DomainObjectIDs.Company1.Value,
                           (Guid) DomainObjectIDs.Customer1.Value
                       };
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Partner>().Where(p => partnerIDs.Contains((Guid) p.ID.Value));

      CheckQueryResult(
          query,
          DomainObjectIDs.Partner1,
          DomainObjectIDs.Distributor1,
          DomainObjectIDs.Supplier1);
    }

    [Test]
    public void BaseObjects_Is_SelectingDerivedType_SingleTableInheritance ()
    {
      var partnerIDs = new[]
                       {
                           (Guid) DomainObjectIDs.Partner1.Value,
                           (Guid) DomainObjectIDs.Distributor1.Value,
                           (Guid) DomainObjectIDs.Supplier1.Value,
                           (Guid) DomainObjectIDs.Company1.Value,
                           (Guid) DomainObjectIDs.Customer1.Value
                       };
      var query = QueryFactory.CreateLinqQuery<Company>().Where(c => c is Partner).Where(p => partnerIDs.Contains((Guid) p.ID.Value));

      CheckQueryResult(
          query,
          DomainObjectIDs.Partner1,
          DomainObjectIDs.Distributor1,
          DomainObjectIDs.Supplier1);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFile>()
        where f.Name == "Datei im Root"
        select f);

      CheckQueryResult(fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClassViaRelation ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFile>()
        where f.ParentFolder.Name == "Root"
        select f);

      CheckQueryResult(fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClassViaCollection ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<TIClient>()
        from domainBase in c.AssignedObjects
        where domainBase.CreatedAt == new DateTime(2006, 01, 03)
        select domainBase;

      CheckQueryResult(query, _concreteObjectIDs.Person);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFile>()
        where f.Size == 512
        select f);

      CheckQueryResult(fsi, _concreteObjectIDs.File1);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInBaseClass ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFile>()
        where f.ParentFolder.ID == _concreteObjectIDs.FolderRoot
        select f);

      CheckQueryResult(fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void ConcreteObjects_OfType_SelectingBaseType ()
    {
      var query = QueryFactory.CreateLinqQuery<TIPerson>().OfType<TIDomainBase>();

      CheckQueryResult(
          query,
          _concreteObjectIDs.Person,
          _concreteObjectIDs.Person2,
          _concreteObjectIDs.PersonForUnidirectionalRelationTest,
          _concreteObjectIDs.Customer,
          _concreteObjectIDs.Customer2);
    }

    [Test]
    public void ConcreteObjects_OfType_SelectingSameType ()
    {
      var query = QueryFactory.CreateLinqQuery<TICustomer>().OfType<TICustomer>();

      CheckQueryResult(
          query,
          _concreteObjectIDs.Customer,
          _concreteObjectIDs.Customer2);
    }

    [Test]
    public void ConcreteObject_OfType_UnrelatedType_ThrowsInvalidOperation ()
    {
      var query = QueryFactory.CreateLinqQuery<TICustomer>().OfType<TIFile>();
      Assert.That(
          () => CheckQueryResult(query),
          Throws.InvalidOperationException);
    }

    [Test]
    public void ConcreteObjects_Is_SelectingBaseType ()
    {
      // ReSharper disable once IsExpressionAlwaysTrue
      var query = QueryFactory.CreateLinqQuery<TIPerson>().Where(b=> b is TIDomainBase);

      CheckQueryResult(
          query,
          _concreteObjectIDs.Person,
          _concreteObjectIDs.Person2,
          _concreteObjectIDs.PersonForUnidirectionalRelationTest,
          _concreteObjectIDs.Customer,
          _concreteObjectIDs.Customer2);
    }

    [Test]
    public void ConcreteObjects_Is_SelectingSameType ()
    {
      // ReSharper disable once IsExpressionAlwaysTrue
      var query = QueryFactory.CreateLinqQuery<TICustomer>().Where(b=> b is TICustomer);

      CheckQueryResult(
          query,
          _concreteObjectIDs.Customer,
          _concreteObjectIDs.Customer2);
    }

    [Test]
    public void BaseObjects_PropertyAccessInSameClass ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFileSystemItem>()
        where f.Name == "Datei im Root"
        select f);

      CheckQueryResult(fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void BaseObjects_MemberAccessInSameClass ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFileSystemItem>()
        where f.ParentFolder.ID == _concreteObjectIDs.FolderRoot
        select f);

      CheckQueryResult(fsi, _concreteObjectIDs.FileRoot, _concreteObjectIDs.Folder1);
    }

    [Test]
    public void BaseObjects_OfType_SelectingDerivedType ()
    {
      var personIDs = new[]
                       {
                           (Guid) _concreteObjectIDs.Person.Value,
                           (Guid) _concreteObjectIDs.Customer.Value,
                           (Guid) _concreteObjectIDs.OrganizationalUnit.Value
                       };
      var query = QueryFactory.CreateLinqQuery<TIDomainBase>().OfType<TIPerson>().Where(p => personIDs.Contains((Guid) p.ID.Value));

      CheckQueryResult(
          query,
          _concreteObjectIDs.Person,
          _concreteObjectIDs.Customer);
    }

    [Test]
    public void BaseObjects_Is_SelectingDerivedType ()
    {
      var personIDs = new[]
                       {
                           (Guid) _concreteObjectIDs.Person.Value,
                           (Guid) _concreteObjectIDs.Customer.Value,
                           (Guid) _concreteObjectIDs.OrganizationalUnit.Value
                       };
      var query = QueryFactory.CreateLinqQuery<TIDomainBase>().Where(b => b is TIPerson).Where(p => personIDs.Contains((Guid) p.ID.Value));

      CheckQueryResult(
          query,
          _concreteObjectIDs.Person,
          _concreteObjectIDs.Customer);
    }
  }
}