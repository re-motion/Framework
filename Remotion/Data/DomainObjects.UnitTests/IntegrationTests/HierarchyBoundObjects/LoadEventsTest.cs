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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.HierarchyBoundObjects
{
  [TestFixture]
  public class LoadEventsTest : HierarchyBoundObjectsTestBase
  {
    private ClientTransaction _rootTransaction;
    private ClientTransaction _subTransaction;
    private Order _orderReference;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction ();
      _subTransaction = _rootTransaction.CreateSubTransaction();
      _subTransaction.EnterDiscardingScope();

      _orderReference = DomainObjectIDs.Order1.GetObjectReference<Order> (_rootTransaction);
    }

    [Test]
    public void OnLoaded_IsExecutedWithRightTransactionActivated ()
    {
      var eventReceiverMock = MockRepository.GenerateStrictMock<ILoadEventReceiver>();
      _orderReference.SetLoadEventReceiver (eventReceiverMock);

      eventReceiverMock
          .Expect (mock => mock.OnLoaded (_orderReference))
          .WhenCalled (
              mi =>
              {
                Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction));
                Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (_rootTransaction));
              });
      eventReceiverMock
          .Expect (mock => mock.OnLoaded (_orderReference))
          .WhenCalled (
              mi =>
              {
                Assert.That (ClientTransaction.Current, Is.SameAs (_subTransaction));
                Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (_subTransaction));
              });

      _orderReference.EnsureDataAvailable();

      eventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void TransactionOnLoaded_IsExecutedWithRightTransactionActivated ()
    {
      var rootTransactionEventReceiverMock = MockRepository.GenerateStrictMock<ClientTransactionMockEventReceiver> (_rootTransaction);
      var subTransactionEventReceiverMock = MockRepository.GenerateStrictMock<ClientTransactionMockEventReceiver> (_subTransaction);
      
      rootTransactionEventReceiverMock
        .Expect (mock => mock.Loaded (new[] { _orderReference }))
        .WhenCalled (
          mi =>
          {
            Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction));
            Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (_rootTransaction));
          });
      subTransactionEventReceiverMock
          .Expect (mock => mock.Loaded (new[] { _orderReference })).WhenCalled (
          mi =>
          {
            Assert.That (ClientTransaction.Current, Is.SameAs (_subTransaction));
            Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (_subTransaction));
          });

      _orderReference.EnsureDataAvailable ();

      rootTransactionEventReceiverMock.VerifyAllExpectations ();
      subTransactionEventReceiverMock.VerifyAllExpectations ();
    }
  }
}