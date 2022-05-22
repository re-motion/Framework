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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class RollbackEventsWithExistingObjectTest : ClientTransactionBaseTest
  {
    private Order _order1;

    private Customer _customer1;
    private string _orginalCustomerName;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _customer1 = _order1.Customer;
      _orginalCustomerName = _customer1.Name;
    }

    public override void TearDown ()
    {
      TestableClientTransaction.Extensions.Remove("MockExtension");
      base.TearDown();
    }

    [Test]
    public void RollbackWithoutChanges ()
    {
      var order1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var customer1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _customer1);
      var clientTransactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, TestableClientTransaction);
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(TestableClientTransaction, It.Is<IReadOnlyList<IDomainObject>>(c => c.Count == 0)))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, Array.Empty<DomainObject>())
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRolledBack(TestableClientTransaction, Array.Empty<DomainObject>())
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(TestableClientTransaction, It.Is<IReadOnlyList<IDomainObject>>(c => c.Count == 0)))
          .Verifiable();

      TestableClientTransaction.Rollback();

      clientTransactionMockEventReceiver.Verify();
      extensionMock.Verify();
      order1MockEventReceiver.Verify();
      customer1MockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void RollbackWithDomainObject ()
    {
      _order1.DeliveryDate = DateTime.Now;

      var order1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var customer1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _customer1);
      var clientTransactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, TestableClientTransaction);
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(TestableClientTransaction, new DomainObject[] { _order1 }))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, _order1)
          .Verifiable();
      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();

      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRolledBack(TestableClientTransaction, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(TestableClientTransaction, new DomainObject[] { _order1 }))
          .Verifiable();

      TestableClientTransaction.Rollback();

      clientTransactionMockEventReceiver.Verify();
      extensionMock.Verify();
      order1MockEventReceiver.Verify();
      customer1MockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ModifyOtherObjectInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;

      var order1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var customer1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _customer1);
      var clientTransactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, TestableClientTransaction);
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(TestableClientTransaction, new DomainObject[] { _order1 }))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, _order1)
          .Verifiable();
      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_order1, It.IsNotNull<EventArgs>()))
          .Callback((object sender, EventArgs args) => ChangeCustomerNameCallback(sender, args))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanging(
                  TestableClientTransaction,
                  _customer1,
                  It.IsAny<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>()))
          .Verifiable();
      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanging(_customer1, It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();

      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanged(_customer1, It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanged(
                  TestableClientTransaction,
                  _customer1,
                  It.IsAny<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>()))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(TestableClientTransaction, new DomainObject[] { _customer1 }))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, _customer1)
          .Verifiable();
      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_customer1, It.IsNotNull<EventArgs>()))
          .Verifiable();

      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(_customer1, It.IsNotNull<EventArgs>()))
          .Verifiable();
      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRolledBack(TestableClientTransaction, _order1, _customer1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RolledBack(
                  TestableClientTransaction,
                  It.Is<ReadOnlyCollection<IDomainObject>>(_ => _.SetEquals(new DomainObject[] { _order1, _customer1 }))))
          .Verifiable();

      TestableClientTransaction.Rollback();

      clientTransactionMockEventReceiver.Verify();
      extensionMock.Verify();
      order1MockEventReceiver.Verify();
      customer1MockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ModifyOtherObjectInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;

      var order1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var customer1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _customer1);
      var clientTransactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, TestableClientTransaction);
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(TestableClientTransaction, new DomainObject[] { _order1 }))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, _order1)
          .Callback((object sender, ClientTransactionEventArgs args) => ChangeCustomerNameCallback(sender, args))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanging(
                  TestableClientTransaction,
                  _customer1,
                  It.IsAny<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>()))
          .Verifiable();
      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanging(_customer1, It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();

      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanged(_customer1, It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanged(
                  TestableClientTransaction,
                  _customer1,
                  It.IsAny<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>()))
          .Verifiable();

      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(TestableClientTransaction, new DomainObject[] { _customer1 }))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, _customer1)
          .Verifiable();
      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_customer1, It.IsNotNull<EventArgs>()))
          .Verifiable();

      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(_customer1, It.IsNotNull<EventArgs>()))
          .Verifiable();

      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRolledBack(TestableClientTransaction, _order1, _customer1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RolledBack(
                  TestableClientTransaction,
                  It.Is<ReadOnlyCollection<IDomainObject>>(objs => objs.SetEquals(new DomainObject[] { _order1, _customer1 }))))
          .Verifiable();

      TestableClientTransaction.Rollback();

      clientTransactionMockEventReceiver.Verify();
      extensionMock.Verify();
      order1MockEventReceiver.Verify();
      customer1MockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInDomainObjectRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";

      var order1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var customer1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _customer1);
      var clientTransactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, TestableClientTransaction);
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RollingBack(
                  TestableClientTransaction,
                  It.Is<IReadOnlyList<IDomainObject>>(objs => objs.SetEquals(new DomainObject[] { _order1, _customer1 }))))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, _order1, _customer1)
          .Verifiable();
      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_order1, It.IsNotNull<EventArgs>()))
          .Callback((object sender, EventArgs args) => ChangeCustomerNameBackToOriginalCallback(sender, args))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanging(
                  TestableClientTransaction,
                  _customer1,
                  It.IsAny<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>())).Verifiable();
      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanging(_customer1, It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();

      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanged(_customer1, It.IsAny<PropertyChangeEventArgs>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanged(
                  TestableClientTransaction,
                  _customer1,
                  It.IsAny<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>()))
          .Verifiable();

      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_customer1, It.IsNotNull<EventArgs>()))
          .Verifiable();

      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRolledBack(TestableClientTransaction, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(TestableClientTransaction, new DomainObject[] { _order1 }))
          .Verifiable();

      TestableClientTransaction.Rollback();

      clientTransactionMockEventReceiver.Verify();
      extensionMock.Verify();
      order1MockEventReceiver.Verify();
      customer1MockEventReceiver.Verify();
      sequence.Verify();
    }

    [Test]
    public void ChangeOtherObjectBackToOriginalInClientTransactionRollingBack ()
    {
      _order1.DeliveryDate = DateTime.Now;
      _customer1.Name = "New customer name";

      var order1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _order1);
      var customer1MockEventReceiver = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _customer1);
      var clientTransactionMockEventReceiver = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, TestableClientTransaction);
      var extensionMock = AddExtensionToClientTransaction(TestableClientTransaction);
      var sequence = new VerifiableSequence();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.RollingBack(
                  TestableClientTransaction,
                  It.Is<IReadOnlyList<IDomainObject>>(objs => objs.SetEquals(new DomainObject[] { _order1, _customer1 }))))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRollingBack(TestableClientTransaction, _order1, _customer1)
          .Callback((object sender, ClientTransactionEventArgs args) => ChangeCustomerNameBackToOriginalCallback(sender, args))
          .Verifiable();

      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanging(
                  TestableClientTransaction,
                  _customer1,
                  It.IsNotNull<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>()))
          .Verifiable();
      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanging(_customer1, It.IsNotNull<PropertyChangeEventArgs>()))
          .Verifiable();

      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.PropertyChanged(_customer1, It.IsNotNull<PropertyChangeEventArgs>()))
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.PropertyValueChanged(
                  TestableClientTransaction,
                  _customer1,
                  It.IsNotNull<PropertyDefinition>(),
                  It.IsAny<object>(),
                  It.IsAny<object>()))
          .Verifiable();

      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();
      customer1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RollingBack(_customer1, It.IsNotNull<EventArgs>()))
          .Verifiable();

      order1MockEventReceiver
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(_order1, It.IsNotNull<EventArgs>()))
          .Verifiable();
      clientTransactionMockEventReceiver
          .InVerifiableSequence(sequence)
          .SetupRolledBack(TestableClientTransaction, _order1)
          .Verifiable();
      extensionMock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.RolledBack(TestableClientTransaction, new DomainObject[] { _order1 }))
          .Verifiable();

      TestableClientTransaction.Rollback();

      clientTransactionMockEventReceiver.Verify();
      extensionMock.Verify();
      order1MockEventReceiver.Verify();
      customer1MockEventReceiver.Verify();
      sequence.Verify();
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

    private static Mock<IClientTransactionExtension> AddExtensionToClientTransaction (TestableClientTransaction transaction)
    {
      var extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
      extensionMock.Setup(stub => stub.Key).Returns("TestExtension");
      transaction.Extensions.Add(extensionMock.Object);
      extensionMock.Reset();

      return extensionMock;
    }
  }
}
