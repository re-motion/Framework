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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.TableInheritance
{
  [TestFixture]
  public class TableInheritanceDomainObjectTest : TableInheritanceMappingTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void OneToManyRelationToAbstractClass ()
    {
      TIClient client = DomainObjectIDs.Client.GetObject<TIClient> ();

      DomainObjectCollection assignedObjects = client.AssignedObjects;

      Assert.That (assignedObjects.Count, Is.EqualTo (4));
      Assert.That (assignedObjects[0].ID, Is.EqualTo (DomainObjectIDs.OrganizationalUnit));
      Assert.That (assignedObjects[1].ID, Is.EqualTo (DomainObjectIDs.Person));
      Assert.That (assignedObjects[2].ID, Is.EqualTo (DomainObjectIDs.PersonForUnidirectionalRelationTest));
      Assert.That (assignedObjects[3].ID, Is.EqualTo (DomainObjectIDs.Customer));
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance.TIHistoryEntry.Owner' of the loaded DataContainer "
        + "'TI_HistoryEntry|2c7fb7b3-eb16-43f9-bdde-b8b3f23a93d2|System.Guid' refers to ClassID 'TI_OrganizationalUnit', "
        + "but the actual ClassID is 'TI_Person'.")]
    public void SameIDInDifferentConcreteTables ()
    {
      TIPerson person = new ObjectID(typeof (TIPerson), new Guid ("{B969AFCB-2CDA-45ff-8490-EB52A86D5464}")).GetObject<TIPerson> ();
      person.HistoryEntries.EnsureDataComplete();
    }

    [Test]
    public void RelationsFromConcreteSingle ()
    {
      TICustomer customer = DomainObjectIDs.Customer.GetObject<TICustomer> ();
      Assert.That (customer.CreatedBy, Is.EqualTo ("UnitTests"));
      Assert.That (customer.FirstName, Is.EqualTo ("Zaphod"));
      Assert.That (customer.CustomerType, Is.EqualTo (CustomerType.Premium));

      TIRegion region = customer.Region;
      Assert.That (region, Is.Not.Null);
      Assert.That (region.ID, Is.EqualTo (DomainObjectIDs.Region));

      DomainObjectCollection orders = customer.Orders;
      Assert.That (orders.Count, Is.EqualTo (1));
      Assert.That (orders[0].ID, Is.EqualTo (DomainObjectIDs.Order));

      DomainObjectCollection historyEntries = customer.HistoryEntries;
      Assert.That (historyEntries.Count, Is.EqualTo (2));
      Assert.That (historyEntries[0].ID, Is.EqualTo (DomainObjectIDs.HistoryEntry2));
      Assert.That (historyEntries[1].ID, Is.EqualTo (DomainObjectIDs.HistoryEntry1));

      TIClient client = customer.Client;
      Assert.That (client.ID, Is.EqualTo (DomainObjectIDs.Client));

      Assert.That (customer.AbstractClassesWithoutDerivations, Is.Empty);
    }

    [Test]
    public void RelationsFromConcrete ()
    {
      TIPerson person = DomainObjectIDs.Person.GetObject<TIPerson> ();
      Assert.That (person.Client.ID, Is.EqualTo (DomainObjectIDs.Client));
      Assert.That (person.HistoryEntries.Count, Is.EqualTo (1));
    }


    [Test]
    public void OneToManyRelationToConcreteSingle ()
    {
      TIRegion region = DomainObjectIDs.Region.GetObject<TIRegion> ();
      Assert.That (region.Customers.Count, Is.EqualTo (1));
      Assert.That (region.Customers[0].ID, Is.EqualTo (DomainObjectIDs.Customer));
    }

    [Test]
    public void ManyToOneRelationToConcreteSingle ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        TIOrder order = DomainObjectIDs.Order.GetObject<TIOrder>();
        Assert.That (order.Customer.ID, Is.EqualTo (DomainObjectIDs.Customer));
      }
    }

    [Test]
    public void ManyToOneRelationToAbstractClass ()
    {
      TIHistoryEntry historyEntry = DomainObjectIDs.HistoryEntry1.GetObject<TIHistoryEntry> ();
      Assert.That (historyEntry.Owner.ID, Is.EqualTo (DomainObjectIDs.Customer));
    }

    [Test]
    public void UpdateConcreteSingle ()
    {
      TIRegion expectedNewRegion = TIRegion.NewObject ();
      expectedNewRegion.Name = "Wachau";

      TICustomer expectedCustomer = DomainObjectIDs.Customer.GetObject<TICustomer> ();
      expectedCustomer.LastName = "NewLastName";
      expectedCustomer.Region = expectedNewRegion;

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        TICustomer actualCustomer = expectedCustomer.ID.GetObject<TICustomer> ();
        Assert.That (actualCustomer.LastName, Is.EqualTo ("NewLastName"));
        Assert.That (actualCustomer.Region.ID, Is.EqualTo (expectedNewRegion.ID));
      }
    }

    [Test]
    public void InsertConcreteSingle ()
    {
      TICustomer expectedCustomer = TICustomer.NewObject ();
      expectedCustomer.FirstName = "Franz";
      expectedCustomer.LastName = "Kameramann";
      expectedCustomer.DateOfBirth = new DateTime (1950, 1, 3);
      expectedCustomer.CustomerType = CustomerType.Premium;
      expectedCustomer.CustomerSince = DateTime.Now;

      TIAddress expectedAddress = TIAddress.NewObject();
      expectedAddress.Street = "Linzer Straße 1";
      expectedAddress.Zip = "3100";
      expectedAddress.City = "St. Pölten";
      expectedAddress.Country = "Österreich";
      expectedAddress.Person = expectedCustomer;

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        TICustomer actualCustomer = expectedCustomer.ID.GetObject<TICustomer> ();
        Assert.That (actualCustomer, Is.Not.Null);
        Assert.That (actualCustomer.Address.ID, Is.EqualTo (expectedAddress.ID));
      }
    }

    [Test]
    public void DeleteConcreteSingle ()
    {
      var customer = DomainObjectIDs.Customer.GetObject<TICustomer> ();
      
      
      foreach (var historyEntry in customer.HistoryEntries.Clone ())
        historyEntry.Delete ();

      customer.Delete ();
      

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        try
        {
          DomainObjectIDs.Customer.GetObject<TICustomer> ();
          Assert.Fail ("ObjectsNotFoundException was expected.");
        }
        catch (ObjectsNotFoundException)
        {
        }
      }
    }
  }
}
