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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides functionality for accessing the data management facilities of a <see cref="ClientTransaction"/>.
  /// </summary>
  public static class DataManagementService
  {
    /// <summary>
    /// Returns the <see cref="IDataManager"/> of the given <see cref="ClientTransaction"/>. Use wisely.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to be returned.</param>
    /// <returns>
    /// The <see cref="IDataManager"/> is the entry point to the <paramref name="clientTransaction"/>'s data management facilities (e.g., 
    /// <see cref="DataContainer"/> instances, <see cref="IRelationEndPoint"/> objects, etc.). When operating at the data management level, it is 
    /// possible to render the internal data structures of the <paramref name="clientTransaction"/> inconsistent. Therefore, use with care and make
    /// sure you know what you are doing.
    /// </returns>
    public static IDataManager GetDataManager (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      return clientTransaction.DataManager;
    }
  }
}