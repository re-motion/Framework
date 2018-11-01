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
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is = Rhino.Mocks.Constraints.Is;
using List = Rhino.Mocks.Constraints.List;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class RollbackEventsWithExistingObjectTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;

    private ClientTransactionMockEventReceiver _clientTransactionMockEventReceiver;
    private IClientTransactionExtension _clientTransactionExtensionMock;

    private Order _order1;
    private DomainObjectMockEventReceiver _order1MockEventReceiver;

    private Customer _customer1;
    private DomainObjectMockEventReceiver _customer1MockEventReceiver;
    private string _orginalCustomerName;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _customer1 = _order1.Customer;
      _orginalCustomerName = _customer1.Name;

      _mockRepository = new MockRepository ();

      _clientTransactionMockEventReceiver = _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (TestableClientTransaction);
      _clientTransactionExtensionMock = _mockRepository.StrictMock<IClientTransactionExtension> ();
      _clientTransactionExtensionMock.Stub (stub => stub.Key).Return ("MockExtension");
      _clientTransactionExtensionMock.Replay();
      TestableClientTransaction.Extensions.Add (_clientTransactionExtensionMock);
      _clientTransactionExtensionMock.BackToRecord();

      _order1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      _customer1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_customer1);
    }

    public override void TearDown ()
    {
      TestableClientTransaction.Extensions.Remove ("MockExtension");
      base.TearDown ();
    }

    [Test]
    public void RollbackWithoutChanges ()
    {
      using (_mockRepository.Ordered ())
      {
        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 0));

        _clientTransactionMockEventReceiver.RollingBack ();
        _clientTransactionMockEventReceiver.RolledBack ();

        _clientTransactionExtensionMock.RolledBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 0));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RollbackWithDomainObject ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & List.IsIn (_order1));

        _clientTransactionMockEventReceiver.RollingBack (_order1);

        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _clientTransactionMockEventReceiver.RolledBack (_order1);

        _clientTransactionExtensionMock.RolledBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & List.IsIn (_order1));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ModifyOtherObjectInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & new ContainsConstraint (_order1));

        _clientTransactionMockEventReceiver.RollingBack (_order1);

        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());
        LastCall.Do (new EventHandler (ChangeCustomerNameCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();

        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & new ContainsConstraint (_customer1));

        _clientTransactionMockEventReceiver.RollingBack (_customer1);

        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _customer1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull());

        _clientTransactionMockEventReceiver.RolledBack (_order1, _customer1);

        _clientTransactionExtensionMock.RolledBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 2) & new ContainsConstraint (_order1, _customer1));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ModifyOtherObjectInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & List.IsIn (_order1));

        _clientTransactionMockEventReceiver.RollingBack (_order1);
        LastCall.Do (new EventHandler<ClientTransactionEventArgs> (ChangeCustomerNameCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();

        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & List.IsIn (_customer1));

        _clientTransactionMockEventReceiver.RollingBack (_customer1);

        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _customer1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull());

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _clientTransactionMockEventReceiver.RolledBack (_order1, _customer1);

        _clientTransactionExtensionMock.RolledBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 2) & new ContainsConstraint (_order1, _customer1));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_customer1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 2) & List.ContainsAll (new DomainObject[] { _order1, _customer1 }));

        _clientTransactionMockEventReceiver.RollingBack (_order1, _customer1);

        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());
        LastCall.Do (new EventHandler (ChangeCustomerNameBackToOriginalCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();
        
        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        // Note: Customer1 must not raise a RolledBack event, because its Name has been set back to the OriginalValue.

        _clientTransactionMockEventReceiver.RolledBack (_order1);

        _clientTransactionExtensionMock.RolledBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & List.IsIn (_order1));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";
      _mockRepository.BackToRecord (_order1MockEventReceiver);
      _mockRepository.BackToRecord (_customer1MockEventReceiver);
      _mockRepository.BackToRecord (_clientTransactionExtensionMock);

      using (_mockRepository.Ordered ())
      {
        _clientTransactionExtensionMock.RollingBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 2) & new ContainsConstraint (_order1, _customer1));

        _clientTransactionMockEventReceiver.RollingBack (_order1, _customer1);
        LastCall.Do (new EventHandler<ClientTransactionEventArgs> (ChangeCustomerNameBackToOriginalCallback));

        _clientTransactionExtensionMock.PropertyValueChanging (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments ();
        _customer1MockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments ();
        _clientTransactionExtensionMock.PropertyValueChanged (TestableClientTransaction, null, null, null, null);
        LastCall.IgnoreArguments ();

        _order1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        _customer1MockEventReceiver.RollingBack (null, null);
        LastCall.Constraints (Is.Same (_customer1), Is.NotNull ());

        _order1MockEventReceiver.RolledBack (null, null);
        LastCall.Constraints (Is.Same (_order1), Is.NotNull ());

        // Note: Customer1 must not raise a RolledBack event, because its Name has been set back to the OriginalValue.

        _clientTransactionMockEventReceiver.RolledBack (_order1);

        _clientTransactionExtensionMock.RolledBack (null, null);
        LastCall.Constraints (Is.Same (TestableClientTransaction), Property.Value ("Count", 1) & List.IsIn (_order1));
      }

      _mockRepository.ReplayAll ();

      TestableClientTransaction.Rollback ();

      _mockRepository.VerifyAll ();
    }

    private void ChangeCustomerNameCallback (object sender, EventArgs e)
    {
      ChangeCustomerName ();
    }

    private void ChangeCustomerNameCallback (object sender, ClientTransactionEventArgs args)
    {
      ChangeCustomerName ();
    }

    private void ChangeCustomerName ()
    {
      _customer1.Name = "New customer name";
    }

    private void ChangeCustomerNameBackToOriginalCallback (object sender, EventArgs e)
    {
      ChangeCustomerNameBackToOriginal ();
    }

    private void ChangeCustomerNameBackToOriginalCallback (object sender, ClientTransactionEventArgs args)
    {
      ChangeCustomerNameBackToOriginal ();
    }

    private void ChangeCustomerNameBackToOriginal ()
    {
      _customer1.Name = _orginalCustomerName;
    }
  }
}
