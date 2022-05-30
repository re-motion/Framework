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
using Moq;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class ClientTransactionTestHelperWithMocks
  {
    public static void EnsureTransactionThrowsOnEvents (ClientTransaction clientTransaction)
    {
      var listenerMock = CreateAndAddListenerStrictMock(clientTransaction);
      listenerMock.Setup(stub => stub.TransactionDiscard(clientTransaction)); // allow TransactionDiscarding to be called
      // no events expected
    }

    public static void EnsureTransactionThrowsOnEvent (
        ClientTransaction clientTransaction,
        Expression<Action<IClientTransactionListener>> forbiddenEventExpectation)
    {
      var listenerMock = CreateAndAddListenerMock(clientTransaction);
      listenerMock.Setup(forbiddenEventExpectation).Throws(new InvalidOperationException("Forbidden event raised."));
    }

    public static Mock<IClientTransactionListener> CreateAndAddListenerMock (ClientTransaction clientTransaction)
    {
      var listenerMock = new Mock<IClientTransactionListener>();
      ClientTransactionTestHelper.AddListener(clientTransaction, listenerMock.Object);
      return listenerMock;
    }

    public static Mock<IClientTransactionListener> CreateAndAddListenerStrictMock (ClientTransaction clientTransaction)
    {
      var listenerMock = new Mock<IClientTransactionListener>(MockBehavior.Strict);
      ClientTransactionTestHelper.AddListener(clientTransaction, listenerMock.Object);
      return listenerMock;
    }
  }
}
