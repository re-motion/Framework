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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class RollbackEventsWithNewObjectTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private Order _order1;
    private Customer _newCustomer;

    // construction and disposing

    public RollbackEventsWithNewObjectTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _newCustomer = Customer.NewObject ();
    }

    [Test]
    public void DiscardOtherObjectInDomainObjectRollingBack ()
    {
      _order1.Customer = _newCustomer;
      _order1.RollingBack += Order1_RollingBack;

      TestableClientTransaction.Rollback ();

      // expectation: no ObjectInvalidException
    }

    [Test]
    public void ObjectDiscardedInDomainObjectRollingBackDoesNotRaiseRollingBackEvent ()
    {
      _order1.Customer = _newCustomer;
      _order1.RollingBack += Order1_RollingBack;
      _newCustomer.RollingBack += NewCustomer_RollingBack_MustNotBeCalled;

      TestableClientTransaction.Rollback ();

      // expectation: NewCustomer_RollingBack_MustNotBeCalled must not throw an AssertionException
    }

    [Test]
    public void DiscardObjectInClientTransactionRollingBack ()
    {
      _order1.Customer = _newCustomer;
      TestableClientTransaction.RollingBack += ClientTransaction_RollingBack;

      TestableClientTransaction.Rollback ();

      // expectation: no ObjectInvalidException
    }

    [Test]
    public void RolledBackEventWithNewObject ()
    {
      MockRepository mockRepository = new MockRepository ();

      ClientTransactionMockEventReceiver clientTransactionMockEventReceiver = 
          mockRepository.StrictMock<ClientTransactionMockEventReceiver> (TestableClientTransaction);

      using (mockRepository.Ordered ())
      {
        clientTransactionMockEventReceiver.RollingBack (_newCustomer);
        clientTransactionMockEventReceiver.RolledBack ();
      }

      mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      mockRepository.VerifyAll ();
    }

    private void NewCustomer_RollingBack_MustNotBeCalled (object sender, EventArgs e)
    {
      throw new AssertionException ("New customer must not throw a RollingBack event, because it has been made invalid.");
    }

    private void Order1_RollingBack (object sender, EventArgs e)
    {
      DeleteNewCustomer ();
    }

    private void ClientTransaction_RollingBack (object sender, ClientTransactionEventArgs args)
    {
      DeleteNewCustomer ();
    }

    private void DeleteNewCustomer ()
    {
      _newCustomer.Delete ();
    }
  }
}
