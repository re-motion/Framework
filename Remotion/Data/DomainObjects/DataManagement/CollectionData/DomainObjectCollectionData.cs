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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Provides an an encapsulation of the data stored inside a <see cref="DomainObjectCollection"/>, implementing the 
  /// <see cref="IDomainObjectCollectionData"/> interface. The data is stored by means of two collections, an ordered <see cref="List{T}"/> of 
  /// <see cref="ObjectID"/>s and a <see cref="Dictionary{TKey,TValue}"/> mapping the IDs to <see cref="DomainObject"/> instances.
  /// This class does not perform any fancy argument checking, use <see cref="ModificationCheckingDomainObjectCollectionDataDecorator"/> for that. It does, however,
  /// ensure that no inconsistent state can be created, even when calling its members with invalid arguments.
  /// </summary>
  public class DomainObjectCollectionData : IDomainObjectCollectionData
  {
    private readonly List<ObjectID> _orderedObjectIDs = new List<ObjectID>();
    private readonly Dictionary<ObjectID, DomainObject> _objectsByID = new Dictionary<ObjectID, DomainObject>();

    public DomainObjectCollectionData ()
    {
    }

    public DomainObjectCollectionData (IEnumerable<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);

      foreach (var domainObject in domainObjects)
        Insert(Count, domainObject);
    }

    public long Version { get; private set; }

    public int Count
    {
      get { return _orderedObjectIDs.Count; }
    }

    Type? IDomainObjectCollectionData.RequiredItemType
    {
      get { return null; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    RelationEndPointID? IDomainObjectCollectionData.AssociatedEndPointID
    {
      get { return null; }
    }

    public bool IsDataComplete
    {
      get { return true; }
    }

    void IDomainObjectCollectionData.EnsureDataComplete ()
    {
      Assertion.IsTrue(((IDomainObjectCollectionData)this).IsDataComplete);
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return _objectsByID.ContainsKey(objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _objectsByID[_orderedObjectIDs[index]];
    }

    public DomainObject? GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      _objectsByID.TryGetValue(objectID, out var result);
      return result;
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      return _orderedObjectIDs.IndexOf(objectID);
    }

    public void Clear ()
    {
      // the following two lines won't throw => corruption impossible
      _orderedObjectIDs.Clear();
      _objectsByID.Clear();

      IncrementVersion();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      if (index < 0 || index > Count)
        throw new ArgumentOutOfRangeException("index");

      // the first line can throw an ArgumentException, but the second cannot => corruption impossible
      _objectsByID.Add(domainObject.ID, domainObject);
      _orderedObjectIDs.Insert(index, domainObject.ID);

      IncrementVersion();
    }

    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      return Remove(domainObject.ID);
    }

    public bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      var index = IndexOf(objectID);
      if (index == -1)
        return false;

      // if we got in here, the following two lines must succeed => corruption impossible
      _orderedObjectIDs.RemoveAt(index);
      _objectsByID.Remove(objectID);

      IncrementVersion();
      return true;
    }

    public void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull("value", value);

      var oldDomainObject = GetObject(index);
      if (oldDomainObject != value)
      {
        Assertion.IsTrue(_orderedObjectIDs[index] == oldDomainObject.ID);
        Assertion.IsTrue(_objectsByID.ContainsKey(oldDomainObject.ID));

        // only the first line can fail => corruption impossible

        _objectsByID.Add(value.ID, value); // this can fail
        _orderedObjectIDs.Insert(index + 1, value.ID);  // this must succeed, see assertion above

        _orderedObjectIDs.RemoveAt(index); // this must succeed, see assertion above
        _objectsByID.Remove(oldDomainObject.ID); // this must succeed, see assertion above

        IncrementVersion();
      }
    }

    public void Sort (Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull("comparison", comparison);

      _orderedObjectIDs.Sort((one, two) =>
      {
        var domainObjectOne = GetObject(one);
        var domainObjectTwo = GetObject(two);
        Assertion.DebugIsNotNull(domainObjectOne, "DomainObject '{0}' is missing in the collection.", one);
        Assertion.DebugIsNotNull(domainObjectTwo, "DomainObject '{0}' is missing in the collection.", two);
        return comparison(domainObjectOne, domainObjectTwo);
      });
      IncrementVersion();
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      var enumeratedVersion = Version;
      for (int i = 0; i < Count; i++)
      {
        if (Version != enumeratedVersion)
          throw new InvalidOperationException("Collection was modified during enumeration.");

        yield return GetObject(i);
      }

      // Need to check again, in case Count was decreased while enumerating
      if (Version != enumeratedVersion)
        throw new InvalidOperationException("Collection was modified during enumeration.");
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    private void IncrementVersion ()
    {
      ++Version;
    }
  }
}
