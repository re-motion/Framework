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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class ModificationsInLoadEventTest : ReadOnlyTransactionsTestBase
  {
    private Order _order;
    private Location _location;
    private ILoadEventReceiver _loadEventReceiverMock;
    private Client _client1;
    private Client _client2;
    private Client _client3;
    private Client _client4;

    public override void SetUp ()
    {
      base.SetUp();

      _order = (Order) LifetimeService.GetObjectReference (WriteableSubTransaction, DomainObjectIDs.Order1);
      _location = (Location) LifetimeService.GetObjectReference (WriteableSubTransaction, DomainObjectIDs.Location1);
      _client1 = (Client) LifetimeService.GetObject (WriteableSubTransaction, DomainObjectIDs.Client1, false);
      _client2 = (Client) LifetimeService.GetObject (WriteableSubTransaction, DomainObjectIDs.Client2, false);
      _client3 = (Client) LifetimeService.GetObject (WriteableSubTransaction, DomainObjectIDs.Client3, false);
      _client4 = (Client) LifetimeService.NewObject (WriteableSubTransaction, typeof (Client), ParamList.Empty);

      _loadEventReceiverMock = MockRepository.GenerateStrictMock<ILoadEventReceiver>();
      _order.SetLoadEventReceiver (_loadEventReceiverMock);
      _location.SetLoadEventReceiver (_loadEventReceiverMock);
    }

    [Test]
    public void OnLoaded_CanModifyThisObject_PropertyValues ()
    {
      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckTransactionAndSetProperty (ReadOnlyRootTransaction, _order, o => o.OrderNumber, expectedValue: 1, newValue: 2));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckTransactionAndSetProperty (ReadOnlyMiddleTransaction, _order, o => o.OrderNumber, expectedValue: 2, newValue: 3));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => CheckTransactionAndSetProperty (WriteableSubTransaction, _order, o => o.OrderNumber, expectedValue: 3, newValue: 4));
      }
      WriteableSubTransaction.EnsureDataAvailable (_order.ID);

      _loadEventReceiverMock.VerifyAllExpectations();

      CheckProperty (ReadOnlyRootTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 1, expectedCurrentValue: 2);
      CheckProperty (ReadOnlyMiddleTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 2, expectedCurrentValue: 3);
      CheckProperty (WriteableSubTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 3, expectedCurrentValue: 4);
    }

    [Test]
    public void OnLoaded_CanModifyThisObject_UnidirectionalRelations ()
    {
      using (_loadEventReceiverMock.GetMockRepository().Ordered())
      {
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckTransactionAndSetProperty (ReadOnlyRootTransaction, _location, l => l.Client, expectedValue: _client1, newValue: _client2));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckTransactionAndSetProperty (ReadOnlyMiddleTransaction, _location, l => l.Client, expectedValue: _client2, newValue: _client3));
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_location))
            .WhenCalled (mi => CheckTransactionAndSetProperty (WriteableSubTransaction, _location, l => l.Client, expectedValue: _client3, newValue: _client4));
      }

      WriteableSubTransaction.EnsureDataAvailable (_location.ID);

      _loadEventReceiverMock.VerifyAllExpectations();

      CheckProperty (ReadOnlyRootTransaction, _location, l => l.Client, expectedOriginalValue: _client1, expectedCurrentValue: _client2);
      CheckProperty (ReadOnlyMiddleTransaction, _location, l => l.Client, expectedOriginalValue: _client2, expectedCurrentValue: _client3);
      CheckProperty (WriteableSubTransaction, _location, l => l.Client, expectedOriginalValue: _client3, expectedCurrentValue: _client4);
    }

    private void CheckTransactionAndSetProperty<TDomainObject, TValue> (
        ClientTransaction clientTransaction,
        TDomainObject domainObject,
        Expression<Func<TDomainObject, TValue>> propertyExpression,
        TValue expectedValue,
        TValue newValue)
      where TDomainObject : DomainObject
    {
      Assert.That (ClientTransaction.Current, Is.SameAs (clientTransaction));
      CheckProperty (clientTransaction, domainObject, propertyExpression, expectedValue, expectedValue);
      SetProperty (clientTransaction, domainObject, propertyExpression, newValue);
    }
  }
}