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
  /// Implements <see cref="IDomainObjectCollectionData"/> as an abstract class by delegating to another instance of 
  /// <see cref="IDomainObjectCollectionData"/>, calling the abstract <see cref="OnDataChanging"/> and <see cref="OnDataChanged"/>
  /// methods each for each change to the collection.
  /// </summary>
  [Serializable]
  public abstract class ObservableCollectionDataDecoratorBase : DomainObjectCollectionDataDecoratorBase
  {
    public enum OperationKind
    {
      Insert,
      Remove,
      Sort
    };

    protected ObservableCollectionDataDecoratorBase (IDomainObjectCollectionData wrappedData)
        : base (wrappedData)
    {
    }

    protected abstract void OnDataChanging (OperationKind operation, DomainObject affectedObject, int index);
    protected abstract void OnDataChanged (OperationKind operation, DomainObject affectedObject, int index);

    public override void Clear ()
    {
      var removedObjects = new Stack<DomainObject>(); // holds the removed objects in order to raise 

      int index = 0;
      foreach (var domainObject in this)
      {
        OnDataChanging (OperationKind.Remove, domainObject, index);
        removedObjects.Push (domainObject);
        ++index;
      }

      Assertion.IsTrue (index == Count);

      WrappedData.Clear();

      foreach (var domainObject in removedObjects)
      {
        --index;
        OnDataChanged (OperationKind.Remove, domainObject, index);
      }

      Assertion.IsTrue (index == 0);
    }

    public override void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      OnDataChanging (OperationKind.Insert, domainObject, index);
      WrappedData.Insert (index, domainObject);
      OnDataChanged (OperationKind.Insert, domainObject, index);
    }

    public override bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      int index = IndexOf (domainObject.ID);
      if (index == -1)
        return false;

      OnDataChanging (OperationKind.Remove, domainObject, index);
      WrappedData.Remove (domainObject);
      OnDataChanged (OperationKind.Remove, domainObject, index);

      return true;
    }

    public override bool Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      int index = IndexOf (objectID);
      if (index == -1)
        return false;

      var domainObject = GetObject (objectID);
      OnDataChanging (OperationKind.Remove, domainObject, index);
      WrappedData.Remove (objectID);
      OnDataChanged (OperationKind.Remove, domainObject, index);

      return true;
    }

    public override void Replace (int index, DomainObject value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      var oldDomainObject = GetObject (index);
      if (oldDomainObject != value)
      {
        OnDataChanging (OperationKind.Remove, oldDomainObject, index);
        OnDataChanging (OperationKind.Insert, value, index);
        WrappedData.Replace (index, value);
        OnDataChanged (OperationKind.Remove, oldDomainObject, index);
        OnDataChanged (OperationKind.Insert, value, index);
      }
    }

    public override void Sort (Comparison<DomainObject> comparison)
    {
      OnDataChanging (OperationKind.Sort, null, -1);
      try
      {
        base.Sort (comparison);
      }
      finally
      {
        OnDataChanged (OperationKind.Sort, null, -1);
      }
    }
  }
}