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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.ForbiddenOperations
{
  [TestFixture]
  public class LoadEventsTest : ReadOnlyTransactionsTestBase
  {
    private Order _order;
    private Location _location;
    private ILoadEventReceiver _loadEventReceiverMock;
    private Client _client1;
    private Client _client2;
    private Client _client3;
    private Client _client4;
    private OrderTicket _orderTicket1;

    public override void SetUp ()
    {
      base.SetUp();

      _order = (Order) LifetimeService.GetObjectReference (WriteableSubTransaction, DomainObjectIDs.Order1);
      _location = (Location) LifetimeService.GetObjectReference (WriteableSubTransaction, DomainObjectIDs.Location1);
      _client1 = (Client) LifetimeService.GetObject (WriteableSubTransaction, DomainObjectIDs.Client1, false);
      _client2 = (Client) LifetimeService.GetObject (WriteableSubTransaction, DomainObjectIDs.Client2, false);
      _client3 = (Client) LifetimeService.GetObject (WriteableSubTransaction, DomainObjectIDs.Client3, false);
      _client4 = (Client) LifetimeService.NewObject (WriteableSubTransaction, typeof (Client), ParamList.Empty);
      _orderTicket1 = (OrderTicket) LifetimeService.GetObject (WriteableSubTransaction, DomainObjectIDs.OrderTicket1, false);

      _loadEventReceiverMock = MockRepository.GenerateStrictMock<ILoadEventReceiver>();
      _order.SetLoadEventReceiver (_loadEventReceiverMock);
      _location.SetLoadEventReceiver (_loadEventReceiverMock);
    }

    [Test]
    public void OnLoaded_CannotModifyOtherObject_PropertyValues ()
    {
      // No load events for _order
      _order.SetLoadEventReceiver (null);

      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Load _location, but try to modify _order
        ExpectOnLoadedCalls (
            _location,
            mi => CheckForbiddenSetProperty (ReadOnlyRootTransaction, _location, _order, o => o.OrderNumber, newValue: 2),
            mi => CheckForbiddenSetProperty (ReadOnlyMiddleTransaction, _location, _order, o => o.OrderNumber, newValue: 3),
            mi => CheckForbiddenSetProperty (WriteableSubTransaction, _location, _order, o => o.OrderNumber, newValue: 4));

        WriteableSubTransaction.EnsureDataAvailable (_location.ID);

        _loadEventReceiverMock.VerifyAllExpectations();

        CheckProperty (ReadOnlyRootTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 1, expectedCurrentValue: 1);
        CheckProperty (ReadOnlyMiddleTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 1, expectedCurrentValue: 1);
        CheckProperty (WriteableSubTransaction, _order, o => o.OrderNumber, expectedOriginalValue: 1, expectedCurrentValue: 1);
      }
    }

    [Test]
    public void OnLoaded_CannotModifyOtherObject_UnidirectionalRelations ()
    {
      // No load events for _location
      _location.SetLoadEventReceiver (null);

      using (_loadEventReceiverMock.GetMockRepository().Ordered())
      {
        // Load _order, but try to modify _location
        ExpectOnLoadedCalls (
            _order,
            mi => CheckForbiddenSetProperty (ReadOnlyRootTransaction, _order, _location, l => l.Client, newValue: _client2),
            mi => CheckForbiddenSetProperty (ReadOnlyMiddleTransaction, _order, _location, l => l.Client, newValue: _client3),
            mi => CheckForbiddenSetProperty (WriteableSubTransaction, _order, _location, l => l.Client, newValue: _client4));

        WriteableSubTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations();

        CheckProperty (ReadOnlyRootTransaction, _location, l => l.Client, expectedOriginalValue: _client1, expectedCurrentValue: _client1);
        CheckProperty (ReadOnlyMiddleTransaction, _location, l => l.Client, expectedOriginalValue: _client1, expectedCurrentValue: _client1);
        CheckProperty (WriteableSubTransaction, _location, l => l.Client, expectedOriginalValue: _client1, expectedCurrentValue: _client1);
      }
    }

    [Test]
    public void OnLoaded_CannotModifyOtherObject_BidirectionalRelations ()
    {
      // No load events for _location
      _order.SetLoadEventReceiver (null);

      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Load _location, but try to modify _order
        ExpectOnLoadedCalls (
          _location,
          mi => CheckForbiddenSetProperty (ReadOnlyRootTransaction, _location, _order, o => o.OrderTicket, newValue: null),
          mi => CheckForbiddenSetProperty (ReadOnlyMiddleTransaction, _location, _order, o => o.OrderTicket, newValue: null),
          mi => CheckForbiddenSetProperty (WriteableSubTransaction, _location, _order, o => o.OrderTicket, newValue: null));

        WriteableSubTransaction.EnsureDataAvailable (_location.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckProperty (ReadOnlyRootTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (ReadOnlyMiddleTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (WriteableSubTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
      }
    }

    [Test]
    public void OnLoaded_CannotModifyThisObject_BidirectionalRelations ()
    {
      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        // Load _order, but trying to modify _order.OrderTicket would also modify _orderTicket1
        var offendingObject = _orderTicket1;
        var offendingProperty = GetPropertyIdentifier (typeof (OrderTicket), "Order");
        ExpectOnLoadedCalls (
            _order,
            mi => CheckForbiddenSetProperty (ReadOnlyRootTransaction, _order, _order, o => o.OrderTicket, null, offendingObject, offendingProperty),
            mi => CheckForbiddenSetProperty (ReadOnlyMiddleTransaction, _order, _order, o => o.OrderTicket, null, offendingObject, offendingProperty),
            mi => CheckForbiddenSetProperty (WriteableSubTransaction, _order, _order, o => o.OrderTicket, null, offendingObject, offendingProperty));

        WriteableSubTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckProperty (ReadOnlyRootTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (ReadOnlyMiddleTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
        CheckProperty (WriteableSubTransaction, _order, o => o.OrderTicket, expectedOriginalValue: _orderTicket1, expectedCurrentValue: _orderTicket1);
      }
    }

    [Test]
    public void OnLoaded_CannotCreateObject ()
    {
      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        var expectedSpecificMessage = "An object of type 'Partner' cannot be created.";
        ExpectOnLoadedCalls (
            _order,
            mi => CheckForbiddenOperation (ReadOnlyRootTransaction, () => Partner.NewObject (), _order, expectedSpecificMessage),
            mi => CheckForbiddenOperation (ReadOnlyMiddleTransaction, () => Partner.NewObject (), _order, expectedSpecificMessage),
            mi => CheckForbiddenOperation (WriteableSubTransaction, () => Partner.NewObject (), _order, expectedSpecificMessage));

        WriteableSubTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckNoDataLoadedForType(ReadOnlyRootTransaction);
        Assert.That (
            ClientTransactionTestHelper.GetIDataManager (ReadOnlyMiddleTransaction).DataContainers.All (
                dc => dc.DomainObjectType != typeof (ClassWithAllDataTypes)));
        Assert.That (
            ClientTransactionTestHelper.GetIDataManager (WriteableSubTransaction).DataContainers.All (
                dc => dc.DomainObjectType != typeof (ClassWithAllDataTypes)));
      }
    }

    [Test]
    public void OnLoaded_CannotDeleteObject ()
    {
      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        var expectedSpecificMessage = "Object 'Client|1627ade8-125f-4819-8e33-ce567c42b00c|System.Guid' cannot be deleted.";
        ExpectOnLoadedCalls (
            _order,
            mi => CheckForbiddenOperation (ReadOnlyRootTransaction, () => _client1.Delete (), _order, expectedSpecificMessage),
            mi => CheckForbiddenOperation (ReadOnlyMiddleTransaction, () => _client1.Delete (), _order, expectedSpecificMessage),
            mi => CheckForbiddenOperation (WriteableSubTransaction, () => _client1.Delete (), _order, expectedSpecificMessage));

        WriteableSubTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckState (ReadOnlyRootTransaction, _client1, StateType.Unchanged);
        CheckState (ReadOnlyMiddleTransaction, _client1, StateType.Unchanged);
        CheckState (WriteableSubTransaction, _client1, StateType.Unchanged);
      }
    }

    [Test]
    public void OnLoaded_CannotCauseSameObjectToBeLoadedInSubTx_WhileItIsLoadedIntoParent_InitiatedFromParent ()
    {
      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (
                mi =>
                {
                  Assert.That (ClientTransaction.Current, Is.SameAs (ReadOnlyRootTransaction));
                  Assert.That (
                      () => WriteableSubTransaction.EnsureDataAvailable (_order.ID),
                      Throws.InvalidOperationException.With.Message.EqualTo (
                          "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: "
                          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'."));
                });

        ReadOnlyRootTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckState (ReadOnlyRootTransaction, _order, StateType.Unchanged);
        CheckState (ReadOnlyMiddleTransaction, _order, StateType.NotLoadedYet);
        CheckState (WriteableSubTransaction, _order, StateType.NotLoadedYet);
      }
    }

    [Test]
    public void OnLoaded_CannotCauseSameObjectToBeLoadedInSubTx_WhileItIsLoadedIntoParent_InitiatedFromSub ()
    {
      using (_loadEventReceiverMock.GetMockRepository ().Ordered ())
      {
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (
                mi =>
                {
                  Assert.That (ClientTransaction.Current, Is.SameAs (ReadOnlyRootTransaction));
                  Assert.That (
                      () => ReadOnlyMiddleTransaction.EnsureDataAvailable (_order.ID), 
                      Throws.InvalidOperationException.With.Message.EqualTo (
                          "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: " 
                          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'."));
                  Assert.That (
                      () => WriteableSubTransaction.EnsureDataAvailable (_order.ID),
                      Throws.InvalidOperationException.With.Message.EqualTo (
                          "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: "
                          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'."));
                });
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (
                mi =>
                {
                  Assert.That (ClientTransaction.Current, Is.SameAs (ReadOnlyMiddleTransaction));
                  Assert.That (
                      () => WriteableSubTransaction.EnsureDataAvailable (_order.ID), 
                      Throws.InvalidOperationException.With.Message.EqualTo (
                          "It's not possible to load objects into a subtransaction while they are being loaded into a parent transaction: "
                          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'."));
                });
        _loadEventReceiverMock
            .Expect (mock => mock.OnLoaded (_order))
            .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (WriteableSubTransaction)));

        WriteableSubTransaction.EnsureDataAvailable (_order.ID);

        _loadEventReceiverMock.VerifyAllExpectations ();

        CheckState (ReadOnlyRootTransaction, _order, StateType.Unchanged);
        CheckState (ReadOnlyMiddleTransaction, _order, StateType.Unchanged);
        CheckState (WriteableSubTransaction, _order, StateType.Unchanged);
      }
    }

    private void CheckForbiddenSetProperty<TDomainObject, TValue> (
        ClientTransaction clientTransaction,
        DomainObject loadedObject,
        TDomainObject modifiedObject,
        Expression<Func<TDomainObject, TValue>> propertyExpression,
        TValue newValue,
        DomainObject offendingObject = null,
        string offendingProperty = null)
      where TDomainObject : DomainObject
    {
      offendingObject = offendingObject ?? modifiedObject;
      offendingProperty = offendingProperty ?? GetPropertyAccessorData (modifiedObject, propertyExpression).PropertyIdentifier;

      CheckForbiddenOperation (clientTransaction, 
          () => SetProperty (clientTransaction, modifiedObject, propertyExpression, newValue), 
          loadedObject, 
          string.Format ("The object '{0}' cannot be modified. (Modified property: '{1}'.)", offendingObject.ID, offendingProperty));
    }

    private void CheckForbiddenOperation (ClientTransaction clientTransaction, TestDelegate testDelegate, DomainObject loadedObject, string expectedSpecificMessage)
    {
      Assert.That (ClientTransaction.Current, Is.SameAs (clientTransaction));
      var expected = string.Format (
          "While the object '{0}' is being loaded, only this object can be modified. {1}",
          loadedObject.ID,
          expectedSpecificMessage);
      Assert.That (testDelegate, Throws.InvalidOperationException.With.Message.EqualTo (expected));
    }

    private void ExpectOnLoadedCalls (
        DomainObject loadedObject,
        Action<MethodInvocation> actionInRoot,
        Action<MethodInvocation> actionInMiddle,
        Action<MethodInvocation> actionInSub)
    {
      _loadEventReceiverMock
          .Expect (mock => mock.OnLoaded (loadedObject))
          .WhenCalled (actionInRoot);
      _loadEventReceiverMock
          .Expect (mock => mock.OnLoaded (loadedObject))
          .WhenCalled (actionInMiddle);
      _loadEventReceiverMock
          .Expect (mock => mock.OnLoaded (loadedObject))
          .WhenCalled (actionInSub);
    }

    private void CheckNoDataLoadedForType (ClientTransaction clientTransaction)
    {
      var dataManager = ClientTransactionTestHelper.GetIDataManager (clientTransaction);
      Assert.That (dataManager.DataContainers.All (dc => dc.DomainObjectType != typeof (ClassWithAllDataTypes)));
    }
  }
}