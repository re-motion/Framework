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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements <see cref="IDomainObjectCollectionEndPointChangeDetectionStrategy"/> for sub-transactions. In sub-transactions, a collection end point
  /// is regarded to have changed when either the opposite collection reference has changed or the sequence of items has changed. The order of the 
  /// items is significant.
  /// </summary>
  public class SubDomainObjectCollectionEndPointChangeDetectionStrategy : IDomainObjectCollectionEndPointChangeDetectionStrategy
  {
    public bool HasDataChanged (IDomainObjectCollectionData currentData, IDomainObjectCollectionData originalData)
    {
      ArgumentUtility.CheckNotNull("currentData", currentData);
      ArgumentUtility.CheckNotNull("originalData", originalData);

      return !originalData.SequenceEqual(currentData);
    }
  }
}
