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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class CommitEventsTest : ClientTransactionBaseTest
  {
    private Customer _customer;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _customer = DomainObjectIDs.Customer1.GetObject<Customer> ();
    }

    [Test]
    public void CommitEvents ()
    {
      _customer.Name = "New name";

      var domainObjectEventReceiver = new DomainObjectEventReceiver (_customer);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.That (domainObjectEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (domainObjectEventReceiver.HasCommittedEventBeenCalled, Is.True);

      Assert.That (clientTransactionEventReceiver.CommittingDomainObjectLists.Count, Is.EqualTo (1));
      Assert.That (clientTransactionEventReceiver.CommittedDomainObjectLists.Count, Is.EqualTo (1));

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjects.Count, Is.EqualTo (1));
      Assert.That (committedDomainObjects.Count, Is.EqualTo (1));

      Assert.That (committingDomainObjects[0], Is.SameAs (_customer));
      Assert.That (committedDomainObjects[0], Is.SameAs (_customer));
    }

    [Test]
    public void ModifyOtherObjectInDomainObjectCommitting ()
    {
      var ceo = _customer.Ceo;

      _customer.Name = "New name";
      _customer.Committing += (sender, e) => ceo.Name = "New CEO name";

      var ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.That (ceoEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (ceoEventReceiver.HasCommittedEventBeenCalled, Is.True);

      Assert.That (clientTransactionEventReceiver.CommittingDomainObjectLists.Count, Is.EqualTo (2));
      Assert.That (clientTransactionEventReceiver.CommittedDomainObjectLists.Count, Is.EqualTo (1));

      var committingDomainObjects1 = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committingDomainObjects2 = clientTransactionEventReceiver.CommittingDomainObjectLists[1];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjects1, Is.EqualTo (new[] { _customer }));
      Assert.That (committingDomainObjects2, Is.EqualTo (new[] { ceo }));
      Assert.That (committedDomainObjects, Is.EquivalentTo (new DomainObject[] { _customer, ceo }));
    }

    [Test]
    public void ModifyOtherObjectInClientTransactionCommitting ()
    {
      _customer.Name = "New name";
      TestableClientTransaction.Committing += ClientTransaction_CommittingForModifyOtherObjectInClientTransactionCommitting;

      Ceo ceo = _customer.Ceo;

      var ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.That (ceoEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (ceoEventReceiver.HasCommittedEventBeenCalled, Is.True);

      Assert.That (clientTransactionEventReceiver.CommittingDomainObjectLists.Count, Is.EqualTo (2));
      Assert.That (clientTransactionEventReceiver.CommittedDomainObjectLists.Count, Is.EqualTo (1));

      var committingDomainObjectsForFirstCommitEvent = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committingDomainObjectsForSecondCommit = clientTransactionEventReceiver.CommittingDomainObjectLists[1];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjectsForFirstCommitEvent.Count, Is.EqualTo (1));
      Assert.That (committingDomainObjectsForSecondCommit.Count, Is.EqualTo (1));
      Assert.That (committedDomainObjects.Count, Is.EqualTo (2));

      Assert.That (committingDomainObjectsForFirstCommitEvent.Contains (_customer), Is.True);
      Assert.That (committingDomainObjectsForFirstCommitEvent.Contains (ceo), Is.False);

      Assert.That (committingDomainObjectsForSecondCommit.Contains (_customer), Is.False);
      Assert.That (committingDomainObjectsForSecondCommit.Contains (ceo), Is.True);

      Assert.That (committedDomainObjects.Contains (_customer), Is.True);
      Assert.That (committedDomainObjects.Contains (ceo), Is.True);
    }

    [Test]
    public void ModifyOtherObjects ()
    {
      _customer.Name = "New name";

      Ceo ceo = _customer.Ceo;
      ceo.Name = "New CEO name";

      Order order = _customer.Orders[DomainObjectIDs.Order1];
      IndustrialSector industrialSector = _customer.IndustrialSector;

      var ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      var customerEventReceiver = new DomainObjectEventReceiver (_customer);
      var orderEventReceiver = new DomainObjectEventReceiver (order);
      var industrialSectorEventReceiver = new DomainObjectEventReceiver (industrialSector);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      _customer.Committing += (sender, e) => order.OrderNumber = 1000;
      TestableClientTransaction.Committing += (sender1, args) =>
      {
        var customer = (Customer) args.DomainObjects.SingleOrDefault (obj => obj.ID == DomainObjectIDs.Customer1);
        if (customer != null)
          customer.IndustrialSector.Name = "New industrial sector name";
      };

      TestableClientTransaction.Commit();

      Assert.That (ceoEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (ceoEventReceiver.HasCommittedEventBeenCalled, Is.True);

      Assert.That (customerEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (customerEventReceiver.HasCommittedEventBeenCalled, Is.True);

      Assert.That (orderEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (orderEventReceiver.HasCommittedEventBeenCalled, Is.True);

      Assert.That (industrialSectorEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (industrialSectorEventReceiver.HasCommittedEventBeenCalled, Is.True);

      Assert.That (clientTransactionEventReceiver.CommittingDomainObjectLists.Count, Is.EqualTo (2));
      Assert.That (clientTransactionEventReceiver.CommittedDomainObjectLists.Count, Is.EqualTo (1));

      var committingDomainObjectsForFirstCommitEvent = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committingDomainObjectsForSecondCommitEvent = clientTransactionEventReceiver.CommittingDomainObjectLists[1];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjectsForFirstCommitEvent, Is.EquivalentTo (new DomainObject[] { _customer, ceo }));
      Assert.That (committingDomainObjectsForSecondCommitEvent, Is.EquivalentTo (new DomainObject[] { order, industrialSector }));
      Assert.That (committedDomainObjects, Is.EquivalentTo (new DomainObject[] { _customer, ceo, order, industrialSector }));
    }

    [Test]
    public void CommitWithoutChanges ()
    {
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.That (clientTransactionEventReceiver.CommittingDomainObjectLists.Count, Is.EqualTo (1));
      Assert.That (clientTransactionEventReceiver.CommittedDomainObjectLists.Count, Is.EqualTo (1));

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjects.Count, Is.EqualTo (0));
      Assert.That (committedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void CommitWithExistingObjectDeleted ()
    {
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      ClassWithAllDataTypes classWithAllDataTypes = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
      ObjectID classWithAllDataTypesID = classWithAllDataTypes.ID;

      classWithAllDataTypes.Delete();

      TestableClientTransaction.Commit();

      Assert.That (clientTransactionEventReceiver.CommittingDomainObjectLists.Count, Is.EqualTo (1));
      Assert.That (clientTransactionEventReceiver.CommittedDomainObjectLists.Count, Is.EqualTo (1));

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjects.Count, Is.EqualTo (1));
      Assert.That (committedDomainObjects.Count, Is.EqualTo (0));

      Assert.That (committingDomainObjects.Any (obj => obj.ID == classWithAllDataTypesID), Is.True);
    }

    [Test]
    public void CommittedEventForObjectChangedBackToOriginal ()
    {
      _customer.Name = "New name";

      var customerEventReceiver = new DomainObjectEventReceiver (_customer);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);
      _customer.Committing += (sender, e) => { _customer.Name = _customer.Properties[typeof (Company), "Name"].GetOriginalValue<string>(); };

      TestableClientTransaction.Commit();

      Assert.That (customerEventReceiver.HasCommittingEventBeenCalled, Is.True);
      Assert.That (customerEventReceiver.HasCommittedEventBeenCalled, Is.False);

      Assert.That (clientTransactionEventReceiver.CommittingDomainObjectLists.Count, Is.EqualTo (1));
      Assert.That (clientTransactionEventReceiver.CommittedDomainObjectLists.Count, Is.EqualTo (1));

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjects.Count, Is.EqualTo (1));
      Assert.That (committedDomainObjects.Count, Is.EqualTo (0));
    }

    private void ClientTransaction_CommittingForModifyOtherObjectInClientTransactionCommitting (object sender, ClientTransactionEventArgs args)
    {
      var customer = args.DomainObjects[0] as Customer;
      if (customer != null)
        customer.Ceo.Name = "New CEO name";
    }
  }
}
