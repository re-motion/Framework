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
  /// Provides extension methods for working with <see cref="IDomainObjectCollectionData"/> instances.
  /// </summary>
  public static class DomainObjectCollectionDataExtensions
  {
    public static void Add (this IDomainObjectCollectionData data, IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("data", data);
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      data.Insert(data.Count, domainObject);
    }

    public static void AddRange (this IDomainObjectCollectionData data, IEnumerable<IDomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull("data", data);
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      foreach (var domainObject in domainObjects)
        Add(data, domainObject);
    }

    public static void AddRangeAndCheckItems (this IDomainObjectCollectionData data, IEnumerable<IDomainObject> domainObjects, Type? requiredItemType)
    {
      ArgumentUtility.CheckNotNull("data", data);
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      var index = 0;
      foreach (var domainObject in domainObjects)
      {
        if (domainObject == null)
          throw ArgumentUtility.CreateArgumentItemNullException("domainObjects", index);
        if (requiredItemType != null && !requiredItemType.IsInstanceOfType(domainObject))
          throw ArgumentUtility.CreateArgumentItemTypeException("domainObjects", index, requiredItemType, domainObject.ID.ClassDefinition.Type);
        if (data.ContainsObjectID(domainObject.ID))
        {
          throw new ArgumentException(
              string.Format("Item {1} of parameter '{0}' is a duplicate ('{2}').", "domainObjects", index, domainObject.ID),
              "domainObjects");
        }

        data.Add(domainObject);

        ++index;
      }
    }

    public static void ReplaceContents (this IDomainObjectCollectionData data, IEnumerable<IDomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull("data", data);
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      data.Clear();
      data.AddRange(domainObjects);
    }

    public static bool SetEquals (this IDomainObjectCollectionData collection, IEnumerable<IDomainObject> comparedSet)
    {
      ArgumentUtility.CheckNotNull("collection", collection);
      ArgumentUtility.CheckNotNull("comparedSet", comparedSet);

      var setOfComparedObjects = new HashSet<IDomainObject>(); // this is used to get rid of all duplicates to get a correct result
      foreach (var domainObject in comparedSet)
      {
        if (collection.GetObject(domainObject.ID) != domainObject)
          return false;

        setOfComparedObjects.Add(domainObject);
      }

      // the collection must contain exactly the same number of items as the comparedSet (duplicates ignored)
      return collection.Count == setOfComparedObjects.Count;
    }
  }
}
