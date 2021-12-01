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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class RollbackEventsWithExistingObjectTest : ClientTransactionBaseTest
  {

    private Mock<ClientTransactionMockEventReceiver> _clientTransactionMockEventReceiver;
    private Mock<IClientTransactionExtension> _clientTransactionExtensionMock;

    private Order _order1;
    private Mock<DomainObjectMockEventReceiver> _order1MockEventReceiver;

    private Customer _customer1;
    private Mock<DomainObjectMockEventReceiver> _customer1MockEventReceiver;
    private string _orginalCustomerName;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _customer1 = _order1.Customer;
      _orginalCustomerName = _customer1.Name;

      _clientTransactionMockEventReceiver = new Mock<ClientTransactionMockEventReceiver> (MockBehavior.Strict, TestableClientTransaction);
      _clientTransactionExtensionMock = new Mock<IClientTransactionExtension> (MockBehavior.Strict);
      _clientTransactionExtensionMock.Setup (stub => stub.Key).Returns ("MockExtension");
      TestableClientTransaction.Extensions.Add(_clientTransactionExtensionMock.Object);
      _clientTransactionExtensionMock.BackToRecord();

      _order1MockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _order1);
      _customer1MockEventReceiver = new Mock<DomainObjectMockEventReceiver> (MockBehavior.Strict, _customer1);
    }

    public override void TearDown ()
    {
      TestableClientTransaction.Extensions.Remove("MockExtension");
      base.TearDown();
    }

    [Test]
    public void RollbackWithoutChanges ()
    {
      var sequence = new MockSequence();
      _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 0))));
      _clientTransactionMockEventReceiver.Object.RollingBack();
      _clientTransactionMockEventReceiver.Object.RolledBack();
      _clientTransactionExtensionMock.Expect (_ => _.RolledBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 0))));

      TestableClientTransaction.Rollback();

      _clientTransactionMockEventReceiver.Verify();
      _clientTransactionExtensionMock.Verify();
      _order1MockEventReceiver.Verify();
      _customer1MockEventReceiver.Verify();
    }

    [Test]
    public void RollbackWithDomainObject ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord(_order1MockEventReceiver.Object);
      _mockRepository.BackToRecord(_clientTransactionExtensionMock.Object);

      var sequence = new MockSequence();

      _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals(_.Count, 1) && _.Contains(_order1))));

      _clientTransactionMockEventReceiver.Object.RollingBack(_order1);
      _order1MockEventReceiver.InSequence (sequence).Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
      _order1MockEventReceiver.InSequence (sequence).Setup (_ => _.RolledBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();

      _clientTransactionMockEventReceiver.Object.RolledBack(_order1);

      _clientTransactionExtensionMock.Expect (_ => _.RolledBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals(_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals(_.Count, 1) && _.Contains(_order1))));

      _mockRepository.ReplayAll();

      TestableClientTransaction.Rollback();

      _clientTransactionMockEventReceiver.Verify();
      _clientTransactionExtensionMock.Verify();
      _order1MockEventReceiver.Verify();
      _customer1MockEventReceiver.Verify();
    }

    [Test]
    public void ModifyOtherObjectInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord(_order1MockEventReceiver.Object);
      _mockRepository.BackToRecord(_clientTransactionExtensionMock.Object);

      using (_mockRepository.Ordered())
      {
        _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1) &&new ContainsConstraint(_order1) )));

        _clientTransactionMockEventReceiver.Object.RollingBack(_order1);
        _order1MockEventReceiver.Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
        _order1MockEventReceiver.Setup (_ => _.RollingBack (null, null)).Callback (new EventHandler (ChangeCustomerNameCallback)).Verifiable();
        _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanging(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
        _customer1MockEventReceiver.Setup (_ => _.PropertyChanging (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
        _customer1MockEventReceiver.Setup (_ => _.PropertyChanged (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
        _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanged(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
        _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1) &&new ContainsConstraint(_customer1) )));

        _clientTransactionMockEventReceiver.Object.RollingBack(_customer1);
        _customer1MockEventReceiver.Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _customer1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
        _customer1MockEventReceiver.Setup (_ => _.RolledBack (It.Is<object> (_ => object.ReferenceEquals (_, _customer1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
        _order1MockEventReceiver.Setup (_ => _.RolledBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();

        _clientTransactionMockEventReceiver.Object.RolledBack(_order1, _customer1);
        _clientTransactionExtensionMock.Expect (_ => _.RolledBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2) &&new ContainsConstraint(_order1, _customer1) )));
      }

      TestableClientTransaction.Rollback();

      _clientTransactionMockEventReceiver.Verify();
      _clientTransactionExtensionMock.Verify();
      _order1MockEventReceiver.Verify();
      _customer1MockEventReceiver.Verify();
    }

    [Test]
    public void ModifyOtherObjectInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _mockRepository.BackToRecord(_order1MockEventReceiver.Object);
      _mockRepository.BackToRecord(_clientTransactionExtensionMock.Object);

      var sequence = new MockSequence();

      _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1) &&_.Contains (_order1) )));
      _clientTransactionMockEventReceiver.InSequence (sequence).Setup (_ => _.RollingBack (_order1)).Callback (new EventHandler<ClientTransactionEventArgs> (ChangeCustomerNameCallback)).Verifiable();

      _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanging(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
      _customer1MockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanging (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
      _customer1MockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanged (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();

      _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanged(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
      _order1MockEventReceiver.InSequence (sequence).Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();

      _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1) &&_.Contains (_customer1) )));

      _clientTransactionMockEventReceiver.Object.RollingBack(_customer1);
      _customer1MockEventReceiver.InSequence (sequence).Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _customer1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
      _customer1MockEventReceiver.InSequence (sequence).Setup (_ => _.RolledBack (It.Is<object> (_ => object.ReferenceEquals (_, _customer1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
      _order1MockEventReceiver.InSequence (sequence).Setup (_ => _.RolledBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();

      _clientTransactionMockEventReceiver.Object.RolledBack(_order1, _customer1);

      _clientTransactionExtensionMock.Expect (_ => _.RolledBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2) &&new ContainsConstraint(_order1, _customer1) )));

      TestableClientTransaction.Rollback();

      _clientTransactionMockEventReceiver.Verify();
      _clientTransactionExtensionMock.Verify();
      _order1MockEventReceiver.Verify();
      _customer1MockEventReceiver.Verify();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";
      _mockRepository.BackToRecord(_order1MockEventReceiver.Object);
      _mockRepository.BackToRecord(_customer1MockEventReceiver.Object);
      _mockRepository.BackToRecord(_clientTransactionExtensionMock.Object);

      var sequence = new MockSequence();

      _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),Arg<IReadOnlyList<DomainObject>>.Matches(_ => _ != null && object.Equals (_.Count, 2) &&new DomainObject[] { _order1, _customer1 }.All (_.Contains) )));

      _clientTransactionMockEventReceiver.Object.RollingBack(_order1, _customer1);
      _order1MockEventReceiver.InSequence (sequence).Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
      _order1MockEventReceiver.InSequence (sequence).Setup (_ => _.RollingBack (null, null)).Callback (new EventHandler (ChangeCustomerNameBackToOriginalCallback)).Verifiable();

      _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanging(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
      _customer1MockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanging (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
      _customer1MockEventReceiver.InSequence (sequence).Setup (_ => _.PropertyChanged (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();

      _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanged(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
      _customer1MockEventReceiver.InSequence (sequence).Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _customer1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
      _order1MockEventReceiver.InSequence (sequence).Setup (_ => _.RolledBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();

      _clientTransactionMockEventReceiver.Object.RolledBack(_order1);

      _clientTransactionExtensionMock.Expect (_ => _.RolledBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1) &&_.Contains (_order1) )));

      TestableClientTransaction.Rollback();

      _clientTransactionMockEventReceiver.Verify();
      _clientTransactionExtensionMock.Verify();
      _order1MockEventReceiver.Verify();
      _customer1MockEventReceiver.Verify();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";
      _mockRepository.BackToRecord(_order1MockEventReceiver.Object);
      _mockRepository.BackToRecord(_customer1MockEventReceiver.Object);
      _mockRepository.BackToRecord(_clientTransactionExtensionMock.Object);

      using (_mockRepository.Ordered())
      {
        _clientTransactionExtensionMock.Expect (_ => _.RollingBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 2) &&new ContainsConstraint(_order1, _customer1) )));
        _clientTransactionMockEventReceiver.Setup (_ => _.RollingBack (_order1, _customer1)).Callback (new EventHandler<ClientTransactionEventArgs> (ChangeCustomerNameBackToOriginalCallback)).Verifiable();
        _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanging(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
        _customer1MockEventReceiver.Setup (_ => _.PropertyChanging (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
        _customer1MockEventReceiver.Setup (_ => _.PropertyChanged (It.IsAny<object>(), It.IsAny<PropertyChangeEventArgs>())).Verifiable();
        _clientTransactionExtensionMock.Expect (_ => _.PropertyValueChanged(It.IsAny<ClientTransaction>(),It.IsAny<DomainObject>(),It.IsAny<PropertyDefinition>(),It.IsAny<object>(),It.IsAny<object>()));
        _order1MockEventReceiver.Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
        _customer1MockEventReceiver.Setup (_ => _.RollingBack (It.Is<object> (_ => object.ReferenceEquals (_, _customer1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();
        _order1MockEventReceiver.Setup (_ => _.RolledBack (It.Is<object> (_ => object.ReferenceEquals (_, _order1)), It.Is<EventArgs> (_ => _ != null))).Verifiable();

        // Note: Customer1 must not raise a RolledBack event, because its Name has been set back to the OriginalValue.

        _clientTransactionMockEventReceiver.Object.RolledBack(_order1);
        _clientTransactionExtensionMock.Expect (_ => _.RolledBack(It.Is<ClientTransaction> (_ => object.ReferenceEquals (_, TestableClientTransaction)),It.Is<IReadOnlyList<DomainObject>> (_ => _ != null && object.Equals (_.Count, 1) &&_.Contains (_order1) )));
      }

      TestableClientTransaction.Rollback();

      _clientTransactionMockEventReceiver.Verify();
      _clientTransactionExtensionMock.Verify();
      _order1MockEventReceiver.Verify();
      _customer1MockEventReceiver.Verify();
    }

    private void ChangeCustomerNameCallback (object sender, EventArgs e)
    {
      ChangeCustomerName();
    }

    private void ChangeCustomerNameCallback (object sender, ClientTransactionEventArgs args)
    {
      ChangeCustomerName();
    }

    private void ChangeCustomerName ()
    {
      _customer1.Name = "New customer name";
    }

    private void ChangeCustomerNameBackToOriginalCallback (object sender, EventArgs e)
    {
      ChangeCustomerNameBackToOriginal();
    }

    private void ChangeCustomerNameBackToOriginalCallback (object sender, ClientTransactionEventArgs args)
    {
      ChangeCustomerNameBackToOriginal();
    }

    private void ChangeCustomerNameBackToOriginal ()
    {
      _customer1.Name = _orginalCustomerName;
    }
  }
}
