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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions
{
  public class ReadOnlyTransactionsTestBase : StandardMappingTest
  {
    private ClientTransaction _readOnlyRootTransaction;
    private ClientTransaction _readOnlyMiddleTransaction;
    private ClientTransaction _writeableSubTransaction;
    private IClientTransactionListener _listenerDynamicMock;
    private IClientTransactionExtension _extensionStrictMock;

    protected ClientTransaction ReadOnlyRootTransaction
    {
      get { return _readOnlyRootTransaction; }
    }

    protected ClientTransaction ReadOnlyMiddleTransaction
    {
      get { return _readOnlyMiddleTransaction; }
    }

    protected ClientTransaction WriteableSubTransaction
    {
      get { return _writeableSubTransaction; }
    }

    protected IClientTransactionListener ListenerDynamicMock
    {
      get { return _listenerDynamicMock; }
    }

    protected IClientTransactionExtension ExtensionStrictMock
    {
      get { return _extensionStrictMock; }
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _listenerDynamicMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      _extensionStrictMock = MockRepository.GenerateStrictMock<IClientTransactionExtension> ();

      _readOnlyRootTransaction = ClientTransaction.CreateRootTransaction ();
      ExecuteInReadOnlyRootTransaction (InitializeReadOnlyRootTransaction);
      _readOnlyMiddleTransaction = ReadOnlyRootTransaction.CreateSubTransaction ();
      ExecuteInReadOnlyMiddleTransaction (InitializeReadOnlyMiddleTransaction);
      _writeableSubTransaction = ReadOnlyMiddleTransaction.CreateSubTransaction ();
      ExecuteInWriteableSubTransaction (InitializeWriteableSubTransaction);
    }

    protected virtual void InitializeReadOnlyRootTransaction ()
    {
    }

    protected virtual void InitializeReadOnlyMiddleTransaction ()
    {
    }

    protected virtual void InitializeWriteableSubTransaction ()
    {
    }

    protected void InstallExtensionMock ()
    {
      ExtensionStrictMock.Stub (stub => stub.Key).Return ("test");
      ReadOnlyRootTransaction.Extensions.Add (ExtensionStrictMock);
      ReadOnlyMiddleTransaction.Extensions.Add (ExtensionStrictMock);
      WriteableSubTransaction.Extensions.Add (ExtensionStrictMock);
    }

    protected void InstallListenerMock ()
    {
      ClientTransactionTestHelper.AddListener (ReadOnlyRootTransaction, ListenerDynamicMock);
      ClientTransactionTestHelper.AddListener (ReadOnlyMiddleTransaction, ListenerDynamicMock);
      ClientTransactionTestHelper.AddListener (WriteableSubTransaction, ListenerDynamicMock);
    }


    protected void CheckDataLoaded (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      Assert.That (ClientTransactionTestHelper.GetIDataManager (clientTransaction).DataContainers[domainObject.ID], Is.Not.Null);
      CheckState (clientTransaction, domainObject, StateType.Unchanged);
    }

    protected void CheckDataNotLoaded (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      CheckDataNotLoaded(clientTransaction, domainObject.ID);
      CheckState(clientTransaction, domainObject, StateType.NotLoadedYet);
    }

    protected void CheckState (ClientTransaction clientTransaction, DomainObject domainObject, StateType expectedState)
    {
      Assert.That (domainObject.TransactionContext[clientTransaction].State, Is.EqualTo (expectedState));
    }

    protected void CheckDataNotLoaded (ClientTransaction clientTransaction, ObjectID objectID)
    {
      Assert.That (ClientTransactionTestHelper.GetIDataManager (clientTransaction).DataContainers[objectID], Is.Null);
    }

    protected void CheckEndPointNull (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      Assert.That (ClientTransactionTestHelper.GetIDataManager (clientTransaction).RelationEndPoints[relationEndPointID], Is.Null);
    }

    protected void CheckEndPointComplete (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      var relationEndPoint = ClientTransactionTestHelper.GetIDataManager (clientTransaction).RelationEndPoints[relationEndPointID];
      Assert.That (relationEndPoint, Is.Not.Null);
      Assert.That (relationEndPoint.IsDataComplete, Is.True);
    }

    protected void CheckEndPointIncomplete (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      var relationEndPoint = ClientTransactionTestHelper.GetIDataManager (clientTransaction).RelationEndPoints[relationEndPointID];
      Assert.That (relationEndPoint, Is.Not.Null);
      Assert.That (relationEndPoint.IsDataComplete, Is.False);
    }

    protected void CheckForbidden (Action func, string operation)
    {
      var expectedMessage = string.Format (
          "The operation cannot be executed because the ClientTransaction is read-only, probably because it has an open subtransaction. "
          + "Offending transaction modification: {0}.", 
          operation);
      Assert.That (() => func (), Throws.TypeOf<ClientTransactionReadOnlyException> ().With.Message.EqualTo (expectedMessage));
    }

    protected void CheckProperty<TDomainObject, TValue> (
        ClientTransaction clientTransaction,
        TDomainObject domainObject,
        Expression<Func<TDomainObject, TValue>> propertyExpression,
        TValue expectedCurrentValue,
        TValue expectedOriginalValue)
        where TDomainObject : DomainObject
    {
      var propertyAccessor = GetPropertyAccessor (domainObject, propertyExpression, clientTransaction);
      Assert.That (propertyAccessor.GetValueWithoutTypeCheck(), Is.EqualTo (expectedCurrentValue));
      Assert.That (propertyAccessor.GetOriginalValueWithoutTypeCheck(), Is.EqualTo (expectedOriginalValue));
    }

    protected void SetProperty<TDomainObject, TValue> (
        ClientTransaction clientTransaction, TDomainObject domainObject, Expression<Func<TDomainObject, TValue>> propertyExpression, TValue newValue)
        where TDomainObject : DomainObject
    {
      var propertyAccessor = GetPropertyAccessor(domainObject, propertyExpression, clientTransaction);
      propertyAccessor.SetValue (newValue);
    }

    protected void CheckPropertyEquivalent<TDomainObject, TValue> (
        ClientTransaction clientTransaction,
        TDomainObject domainObject,
        Expression<Func<TDomainObject, IEnumerable<TValue>>> propertyExpression,
        IEnumerable<TValue> expectedCurrentValue,
        IEnumerable<TValue> expectedOriginalValue)
        where TDomainObject : IDomainObject
    {
      var isReadOnlyTransaction = !clientTransaction.IsWriteable;
      if (isReadOnlyTransaction)
      {
        ClientTransactionTestHelper.SetIsWriteable (clientTransaction, true);
      }

      try
      {
        var propertyAccessor = GetPropertyAccessor (domainObject, propertyExpression, clientTransaction);
        Assert.That (propertyAccessor.GetValueWithoutTypeCheck (), Is.EquivalentTo (expectedCurrentValue));
        Assert.That (propertyAccessor.GetOriginalValueWithoutTypeCheck (), Is.EquivalentTo (expectedOriginalValue));
      }
      finally
      {
        if (isReadOnlyTransaction)
        {
          ClientTransactionTestHelper.SetIsWriteable (clientTransaction, false);
        }
      }
    }

    protected void ExecuteInReadOnlyMiddleTransaction (Action action)
    {
      ReadOnlyMiddleTransaction.ExecuteInScope (action);
    }

    protected T ExecuteInReadOnlyMiddleTransaction<T> (Func<T> func)
    {
      return ReadOnlyMiddleTransaction.ExecuteInScope (func);
    }

    protected void ExecuteInReadOnlyRootTransaction (Action action)
    {
      ReadOnlyRootTransaction.ExecuteInScope (action);
    }

    protected T ExecuteInReadOnlyRootTransaction<T> (Func<T> func)
    {
      return ReadOnlyRootTransaction.ExecuteInScope (func);
    }

    protected void ExecuteInWriteableSubTransaction (Action action)
    {
      WriteableSubTransaction.ExecuteInScope (action);
    }

    protected T ExecuteInWriteableSubTransaction<T> (Func<T> func)
    {
      return WriteableSubTransaction.ExecuteInScope (func);
    }
  }
}