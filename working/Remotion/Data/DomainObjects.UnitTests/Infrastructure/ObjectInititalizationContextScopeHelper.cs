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
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  public static class ObjectInititalizationContextScopeHelper
  {
    public static T CallWithNewObjectInitializationContext<T> (ClientTransaction clientTransaction, ObjectID objectID, Func<T> func)
    {
      var initializationContext = CreateNewObjectInitializationContext (clientTransaction, objectID);

      using (new ObjectInititalizationContextScope (initializationContext))
      {
        return func();
      }
    }

    public static void CallWithNewObjectInitializationContext (ClientTransaction clientTransaction, ObjectID objectID, Action action)
    {
      var initializationContext = CreateNewObjectInitializationContext (clientTransaction, objectID);
      using (new ObjectInititalizationContextScope (initializationContext))
      {
        action();
      }
    }

    private static NewObjectInitializationContext CreateNewObjectInitializationContext (ClientTransaction rootTransaction, ObjectID objectID)
    {
      var initializationContext = new NewObjectInitializationContext (
          objectID,
          rootTransaction,
          ClientTransactionTestHelper.GetEnlistedDomainObjectManager (rootTransaction),
          ClientTransactionTestHelper.GetIDataManager (rootTransaction));
      return initializationContext;
    }
  }
}