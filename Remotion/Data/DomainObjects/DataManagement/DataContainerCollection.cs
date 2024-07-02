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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class DataContainerCollection : CommonCollection, IList<DataContainer>, IReadOnlyCollection<DataContainer>
  {
    // types

    // static members and constants

    public static DataContainerCollection Join (DataContainerCollection firstCollection, DataContainerCollection secondCollection)
    {
      ArgumentUtility.CheckNotNull("firstCollection", firstCollection);
      ArgumentUtility.CheckNotNull("secondCollection", secondCollection);

      DataContainerCollection joinedCollection = new DataContainerCollection(firstCollection, false);
      foreach (DataContainer dataContainer in secondCollection)
      {
        if (!joinedCollection.Contains(dataContainer.ID))
          joinedCollection.Add(dataContainer);
      }

      return joinedCollection;
    }

    // member fields

    // construction and disposing

    public DataContainerCollection ()
    {
    }

    // standard constructor for collections
    public DataContainerCollection (IEnumerable collection, bool makeCollectionReadOnly)
    {
      ArgumentUtility.CheckNotNull("collection", collection);

      foreach (DataContainer dataContainer in collection)
        Add(dataContainer);

      this.SetIsReadOnly(makeCollectionReadOnly);
    }

    // methods and properties

    public DataContainerCollection GetDifference (DataContainerCollection dataContainers)
    {
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      DataContainerCollection difference = new DataContainerCollection();

      foreach (DataContainer dataContainer in this)
      {
        if (!dataContainers.Contains(dataContainer.ID))
          difference.Add(dataContainer);
      }

      return difference;
    }

    public DataContainerCollection Merge (DataContainerCollection dataContainers)
    {
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      DataContainerCollection mergedCollection = new DataContainerCollection();

      foreach (DataContainer dataContainer in this)
      {
        var otherDataContainer = dataContainers[dataContainer.ID];
        if (otherDataContainer != null)
          mergedCollection.Add(otherDataContainer);
        else
          mergedCollection.Add(dataContainer);
      }

      return mergedCollection;
    }

    public new IEnumerator<DataContainer> GetEnumerator ()
    {
      // Use non-generic base implementation
// ReSharper disable LoopCanBeConvertedToQuery
      foreach (DataContainer dataContainer in (IEnumerable)this)
// ReSharper restore LoopCanBeConvertedToQuery
        yield return dataContainer;
    }

    #region Standard implementation for collections

    public bool Contains (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull("dataContainer", dataContainer);

      return BaseContains(dataContainer.ID, dataContainer);
    }

    public void CopyTo (DataContainer[] array, int arrayIndex)
    {
      base.CopyTo(array, arrayIndex);
    }

    public bool Contains (ObjectID id)
    {
      return BaseContainsKey(id);
    }

    public int IndexOf (DataContainer item)
    {
      ArgumentUtility.CheckNotNull("item", item);
      return BaseIndexOfKey(item.ID);
    }

    public void Insert (int index, DataContainer item)
    {
      ArgumentUtility.CheckNotNull("item", item);
      BaseInsert(index, item.ID, item);
    }

    public DataContainer this [int index]
    {
      get { return (DataContainer)BaseGetObject(index); }
    }

    DataContainer IList<DataContainer>.this[int index]
    {
      get { return this[index]; }
      set { throw new NotSupportedException("It is not supported to set a DataContainer based by index."); }
    }

    public DataContainer? this [ObjectID id]
    {
      get { return (DataContainer?)BaseGetObject(id); }
    }

    public int Add (DataContainer value)
    {
      ArgumentUtility.CheckNotNull("value", value);

      return BaseAdd(value.ID, value);
    }

    public void RemoveAt (int index)
    {
      Remove(this[index]);
    }

    public void Remove (ObjectID id)
    {
      var dataContainer = (DataContainer?)BaseGetObject(id);
      if (dataContainer == null)
        return;

      Remove(dataContainer);
    }

    public bool Remove (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull("dataContainer", dataContainer);

      var countBefore = Count;
      BaseRemove(dataContainer.ID);
      return Count < countBefore;
    }

    void ICollection<DataContainer>.Add (DataContainer item)
    {
      Add(item);
    }

    public void Clear ()
    {
      BaseClear();
    }

    #endregion
  }
}
