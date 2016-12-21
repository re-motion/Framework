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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects
{
  public static class TransactionHierarchyManagerTestHelper
  {
    public static void SetIsWriteable (TransactionHierarchyManager transactionHierarchyManager, bool value)
    {
      ArgumentUtility.CheckNotNull ("transactionHierarchyManager", transactionHierarchyManager);
      PrivateInvoke.SetNonPublicField (transactionHierarchyManager, "_isWriteable", value);
    }

    public static void SetSubtransaction (TransactionHierarchyManager hierarchyManager, ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("hierarchyManager", hierarchyManager);
      PrivateInvoke.SetNonPublicField (hierarchyManager, "_subTransaction", subTransaction);
    }
  }
}