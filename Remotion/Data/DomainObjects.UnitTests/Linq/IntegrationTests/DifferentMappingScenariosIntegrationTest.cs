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
using Remotion.Data.DomainObjects.UnitTests.TestDomain.InheritanceRootSample;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class DifferentMappingScenariosIntegrationTest : IntegrationTestBase
  {
    private TableInheritanceDomainObjectIDs _concreteObjectIDs;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      
      _concreteObjectIDs = new TableInheritanceDomainObjectIDs (Configuration);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass_SingleTableInheritance ()
    {
      var customer = (from c in QueryFactory.CreateLinqQuery<Customer>()
                   where c.Name == "Kunde 3"
                   select c);

      CheckQueryResult (customer, DomainObjectIDs.Customer3);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass_SingleTableInheritance ()
    {
// ReSharper disable RedundantNameQualifier
      var customer = (from c in QueryFactory.CreateLinqQuery<Customer> ()
                      where c.Type == Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerType.Standard
                      select c);
// ReSharper restore RedundantNameQualifier

      CheckQueryResult (customer, DomainObjectIDs.Customer1);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInSameClass_SingleTableInheritance ()
    {
      var orders = (from c in QueryFactory.CreateLinqQuery<Customer> ()
                      from o in c.Orders
                      where c.Name=="Kunde 3"
                      select o);
      
      CheckQueryResult (orders, DomainObjectIDs.Order3);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInBaseClass_SingleTableInheritance ()
    {
      var customers = (from c in QueryFactory.CreateLinqQuery<Customer> ()
                    where c.IndustrialSector.ID == DomainObjectIDs.IndustrialSector2
                    select c);

      CheckQueryResult (customers, DomainObjectIDs.Customer3, DomainObjectIDs.Customer2);
    }

    [Test]
    public void BaseObjects_PropertyAccessInSameClass_SingleTableInheritance ()
    {
      var company = (from c in QueryFactory.CreateLinqQuery<Company> ()
                     where c.Name == "Firma 2"
                     select c);

      CheckQueryResult (company, DomainObjectIDs.Company2);
    }

    [Test]
    public void BaseObjects_MemberAccessInSameClass_SingleTableInheritance ()
    {
      var company = (from c in QueryFactory.CreateLinqQuery<Company> ()
                     where c.IndustrialSector.ID == DomainObjectIDs.IndustrialSector2 && c.Name=="Firma 2"
                     select c);

      CheckQueryResult (company, DomainObjectIDs.Company2);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFile> ()
                     where f.Name == "Datei im Root"
                     select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFile> ()
                 where f.Size == 512
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.File1);
    }

    [Test]
    public void ConcreteObjects_MemberAccessInBaseClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFile> ()
                 where f.ParentFolder.ID == _concreteObjectIDs.FolderRoot
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void BaseObjects_PropertyAccessInSameClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFileSystemItem> ()
                 where f.Name == "Datei im Root"
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot);
    }

    [Test]
    public void BaseObjects_MemberAccessInSameClass_ConcreteTableInheritance ()
    {
      var fsi = (from f in QueryFactory.CreateLinqQuery<TIFileSystemItem> ()
                 where f.ParentFolder.ID == _concreteObjectIDs.FolderRoot
                 select f);

      CheckQueryResult (fsi, _concreteObjectIDs.FileRoot, _concreteObjectIDs.Folder1);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInSameClass_ClassAboveInheritanceHierarchy ()
    {
      var storageClass = (from f in QueryFactory.CreateLinqQuery<StorageGroupClass> ()
                 where f.StorageGroupClassIdentifier == "StorageGroupName1"
                 select f);

      CheckQueryResult (storageClass, DomainObjectIDs.StorageGroupClass1);
    }

    [Test]
    public void ConcreteObjects_PropertyAccessInBaseClass_ClassAboveInheritanceHierarchy ()
    {
      var storageClass = (from f in QueryFactory.CreateLinqQuery<StorageGroupClass> ()
                          where f.AboveInheritanceIdentifier == "AboveInheritanceName1"
                          select f);

      CheckQueryResult (storageClass, DomainObjectIDs.StorageGroupClass1);
    }

    [Test]
    public void AccessingEntity_WithoutAnyTables ()
    {
      var query1 = QueryFactory.CreateLinqQuery<AbstractClassWithoutDerivations>();
      CheckQueryResult (query1);

      var query2 = QueryFactory.CreateLinqQuery<AbstractClassWithoutDerivations>().Where (x => x.DomainBase != null);
      CheckQueryResult (query2);

      var countWithPropertyAccess = (from db in QueryFactory.CreateLinqQuery<TIDomainBase>()
                                     where db.AbstractClassesWithoutDerivations.Count() == 0
                                     select db).Count();
      var countWithoutPropertyAccess = QueryFactory.CreateLinqQuery<TIDomainBase>().Count();
      Assert.That (countWithPropertyAccess, Is.EqualTo (countWithoutPropertyAccess).And.GreaterThan (0));
    }
  }
}