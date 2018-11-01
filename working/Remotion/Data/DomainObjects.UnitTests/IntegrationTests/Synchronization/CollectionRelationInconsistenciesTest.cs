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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  [TestFixture]
  public class CollectionRelationInconsistenciesTest : RelationInconsistenciesTestBase
  {
    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_ObjectLoadedFirst ()
    {
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.OrderItems.EnsureDataComplete();

      Assert.That (orderItem1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderItems, Has.Member(orderItem1));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (TestableClientTransaction, RelationEndPointID.Resolve (orderItem1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (TestableClientTransaction, RelationEndPointID.Resolve (orderItem1.Order, o => o.OrderItems));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_CollectionLoadedFirst ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.OrderItems.EnsureDataComplete ();
      var orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();

      Assert.That (orderItem1.Order, Is.SameAs (order1));
      Assert.That (order1.OrderItems, Has.Member(orderItem1));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

      // these do nothing
      BidirectionalRelationSyncService.Synchronize (TestableClientTransaction, RelationEndPointID.Resolve (orderItem1, oi => oi.Order));
      BidirectionalRelationSyncService.Synchronize (TestableClientTransaction, RelationEndPointID.Resolve (orderItem1.Order, o => o.OrderItems));

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse ()
    {
      SetDatabaseModifyable ();

      var company = CreateCompanyInDatabaseAndLoad ();
      Assert.That (company.IndustrialSector, Is.Null);

      var industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> (); // virtual end point not yet resolved

      SetIndustrialSectorInOtherTransaction (company.ID, industrialSector.ID);

      // Resolve virtual end point - the database says that company points to industrialSector, but the transaction says it points to null!
      var companiesOfIndustrialSector = industrialSector.Companies;
      companiesOfIndustrialSector.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (companiesOfIndustrialSector, Has.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      var otherCompany = companiesOfIndustrialSector.FirstOrDefault (c => c != company);
      CheckSyncState (otherCompany, c => c.IndustrialSector, true);

      CheckActionWorks (company.Delete);
      TestableClientTransaction.Rollback (); // required so that the remaining actions can be tried below

      // sync states not changed by Rollback
      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      CheckActionWorks (() => industrialSector.Companies.Remove (otherCompany));
      CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject ()));

      var companyIndex = industrialSector.Companies.IndexOf (company);
      CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Remove (company), "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies[companyIndex] = Company.NewObject (), "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies = new ObjectList<Company> (), "out of sync with the opposite object property");
      CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the opposite object property");

      CheckActionWorks (() => company.IndustrialSector = IndustrialSector.NewObject ());

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (industrialSector, s => s.Companies));

      CheckSyncState (industrialSector, s => s.Companies, true);
      Assert.That (companiesOfIndustrialSector, Has.No.Member (company));
      
      CheckActionWorks (() => industrialSector.Companies.Add (company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse_SolvableViaReload ()
    {
      SetDatabaseModifyable ();

      var company = CreateCompanyInDatabaseAndLoad ();
      Assert.That (company.IndustrialSector, Is.Null);

      var industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> (); // virtual end point not yet resolved

      SetIndustrialSectorInOtherTransaction (company.ID, industrialSector.ID);

      // Resolve virtual end point - the database says that company points to industrialSector, but the transaction says it points to null!
      var companiesOfIndustrialSector = industrialSector.Companies;
      companiesOfIndustrialSector.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (companiesOfIndustrialSector, Has.Member(company));
      CheckSyncState (industrialSector, s => s.Companies, false);

      UnloadService.UnloadData (TestableClientTransaction, company.ID);
      company.EnsureDataAvailable();

      CheckSyncState (industrialSector, s => s.Companies, true);
      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (companiesOfIndustrialSector, Has.Member(company));

      CheckActionWorks (() => industrialSector.Companies.Remove (company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectNotIncluded_ThatLocallyPointsToHere ()
    {
      SetDatabaseModifyable ();

      var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
      var company = companyID.GetObject<Company> ();

      Assert.That (company.Properties[typeof (Company), "IndustrialSector"].GetRelatedObjectID(), Is.EqualTo (DomainObjectIDs.IndustrialSector1));

      SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

      var industrialSector = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> ();
      // Resolve virtual end point - the database says that company does not point to IndustrialSector1, but the transaction says it does!
      industrialSector.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.No.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
      CheckSyncState (industrialSector.Companies[0], c => c.IndustrialSector, true);

      CheckActionThrows<InvalidOperationException> (company.Delete, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the collection property");

      CheckActionWorks (() => industrialSector.Companies.RemoveAt (0));
      CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject ()));

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync with the collection property");
      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync with the opposite property ");

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (company, c => c.IndustrialSector));

      CheckSyncState (company, c => c.IndustrialSector, true);
      Assert.That (industrialSector.Companies, Has.Member(company));
      CheckActionWorks (() => company.IndustrialSector = null);
      CheckActionWorks (() => industrialSector.Companies.Add (company));
    }
   
    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncludedInTwoCollections ()
    {
      SetDatabaseModifyable ();

      var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
      var company = companyID.GetObject<Company> ();

      var industrialSector1 = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> ();
      industrialSector1.Companies.EnsureDataComplete();

      SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

      var industrialSector2 = DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector> ();
      industrialSector2.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
      Assert.That (industrialSector1.Companies, Has.Member(company));
      Assert.That (industrialSector2.Companies, Has.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector1, s => s.Companies, true);
      CheckSyncState (industrialSector2, s => s.Companies, false);

      BidirectionalRelationSyncService.Synchronize (TestableClientTransaction, RelationEndPointID.Resolve (industrialSector2, s => s.Companies));

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
      Assert.That (industrialSector1.Companies, Has.Member(company));
      Assert.That (industrialSector2.Companies, Has.No.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector1, s => s.Companies, true);
      CheckSyncState (industrialSector2, s => s.Companies, true);
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneMany ()
    {
      SetDatabaseModifyable();

      // set up new IndustrialSector object in database with one company
      var industrialSector = CreateIndustrialSectorInDatabaseAndLoad ();
      industrialSector.Companies.EnsureDataComplete ();

      // in parallel transaction, add a second Company to the IndustrialSector
      var newCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (industrialSector.ID);

      Assert.That (industrialSector.Companies.Count, Is.EqualTo (1));

      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      var newCompany = newCompanyID.GetObject<Company> ();

      Assert.That (newCompany.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.No.Member (newCompany));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (1));

      CheckSyncState (industrialSector, s => s.Companies, true);
      CheckSyncState (newCompany, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector.Companies[0], c => c.IndustrialSector, true);

      CheckActionThrows<InvalidOperationException> (newCompany.Delete, "out of sync with the opposite property");
      CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the collection property");

      CheckActionWorks (() => industrialSector.Companies.RemoveAt (0));
      CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject ()));

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (newCompany), "out of sync with the collection property");
      CheckActionThrows<InvalidOperationException> (() => newCompany.IndustrialSector = null, "out of sync with the opposite property ");

      BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (newCompany, c => c.IndustrialSector));

      CheckSyncState (newCompany, c => c.IndustrialSector, true);
      Assert.That (industrialSector.Companies, Has.Member(newCompany));
      CheckActionWorks (() => newCompany.IndustrialSector = null);
      CheckActionWorks (() => industrialSector.Companies.Add (newCompany));
    }

    [Test]
    public void ObjectLoaded_WithInconsistentForeignKey_OneMany_UnloadedCorrectsIssue ()
    {
      SetDatabaseModifyable ();

      // set up new IndustrialSector object in database with one company
      var industrialSector = CreateIndustrialSectorInDatabaseAndLoad ();
      industrialSector.Companies.EnsureDataComplete ();

      // in parallel transaction, add a second Company to the IndustrialSector
      var newCompanyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (industrialSector.ID);

      Assert.That (industrialSector.Companies.Count, Is.EqualTo (1));

      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      var newCompany = newCompanyID.GetObject<Company> ();

      Assert.That (newCompany.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.No.Member(newCompany));

      CheckSyncState (industrialSector, s => s.Companies, true);
      CheckSyncState (newCompany, c => c.IndustrialSector, false);

      UnloadService.UnloadData (TestableClientTransaction, newCompany.ID);

      Assert.That (industrialSector.Companies.IsDataComplete, Is.True);

      SetIndustrialSectorInOtherTransaction (newCompanyID, null);
      newCompany.EnsureDataAvailable();

      Assert.That (newCompany.IndustrialSector, Is.Not.SameAs (industrialSector));

      CheckSyncState (industrialSector, s => s.Companies, true);
      CheckSyncState (newCompany, c => c.IndustrialSector, true);
    }

    [Test]
    public void Commit_DoesNotChangeInconsistentState_OneMany_ObjectIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, Has.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = industrialSector, "out of sync");
      industrialSector.Companies.Insert (0, DomainObjectIDs.Company2.GetObject<Company> ());
      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Remove (company), "out of sync");

      Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));

      ClientTransaction.Current.Commit ();

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));
    }

    [Test]
    public void Commit_DoesNotChangeInconsistentState_OneMany_ObjectNotIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectNotIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (5));
      Assert.That (industrialSector.Companies, Has.No.Member(company));

      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync");
      industrialSector.Companies.Add (DomainObjectIDs.Company1.GetObject<Company> ());
      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync");

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

      ClientTransaction.Current.Commit();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
    }
  }
}