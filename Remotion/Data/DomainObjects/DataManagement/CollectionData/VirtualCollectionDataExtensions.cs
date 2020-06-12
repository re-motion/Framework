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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Provides extension methods for working with <see cref="IVirtualCollectionData"/> instances.
  /// </summary>
  public static class VirtualCollectionDataExtensions
  {
    public static bool SetEquals (this IVirtualCollectionData collection, IEnumerable<DomainObject> comparedSet)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);
      ArgumentUtility.CheckNotNull ("comparedSet", comparedSet);

      var setOfComparedObjects = new HashSet<DomainObject>(); // this is used to get rid of all duplicates to get a correct result
      foreach (var domainObject in comparedSet)
      {
        if (collection.GetObject (domainObject.ID) != domainObject)
          return false;

        setOfComparedObjects.Add (domainObject);
      }

      // the collection must contain exactly the same number of items as the comparedSet (duplicates ignored)
      return collection.Count == setOfComparedObjects.Count;
    }
  }
}