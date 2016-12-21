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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class ClientTransactionTestHelperWithMocks
  {
    public static void EnsureTransactionThrowsOnEvents (ClientTransaction clientTransaction)
    {
      IClientTransactionListener listenerMock = CreateAndAddListenerStrictMock (clientTransaction);
      listenerMock.Stub (stub => stub.TransactionDiscard (clientTransaction)); // allow TransactionDicarding to be called
      listenerMock.Replay(); // no events expected
    }

    public static void EnsureTransactionThrowsOnEvent (
        ClientTransaction clientTransaction,
        Action<IClientTransactionListener> forbiddenEventExpectation)
    {
      IClientTransactionListener listenerMock = CreateAndAddListenerMock (clientTransaction);
      listenerMock.Expect (forbiddenEventExpectation).WhenCalled (mi => { throw new InvalidOperationException ("Forbidden event raised."); });
      listenerMock.Replay();
    }

    public static IClientTransactionListener CreateAndAddListenerMock (ClientTransaction clientTransaction)
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      ClientTransactionTestHelper.AddListener (clientTransaction, listenerMock);
      return listenerMock;
    }

    public static IClientTransactionListener CreateAndAddListenerStrictMock (ClientTransaction clientTransaction)
    {
      var listenerMock = MockRepository.GenerateStrictMock<IClientTransactionListener>();
      ClientTransactionTestHelper.AddListener (clientTransaction, listenerMock);
      return listenerMock;
    }
  }
}