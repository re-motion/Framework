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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
public class StorageProviderCollection : CommonCollection, IDisposable
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderCollection ()
  {
  }

  // standard constructor for collections
  public StorageProviderCollection (StorageProviderCollection collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (StorageProvider provider in collection)
    {
      Add (provider);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  #region IDisposable Members

  public virtual void Dispose ()
  {
    for (int i = Count - 1; i>= 0; i--)
      this[i].Dispose ();      

    BaseClear ();
    GC.SuppressFinalize (this);
  }

  #endregion

  // methods and properties

  #region Standard implementation for "add-only" collections

  public bool Contains (StorageProvider provider)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);

    return BaseContains (provider.StorageProviderDefinition.Name, provider);
  }

  public bool Contains (string storageProviderID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    return BaseContainsKey (storageProviderID);
  }

  public StorageProvider this [int index]  
  {
    get { return (StorageProvider) BaseGetObject (index); }
  }

  public StorageProvider this [string storageProviderID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
      return (StorageProvider) BaseGetObject (storageProviderID); 
    }
  }

  public int Add (StorageProvider value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    
    return BaseAdd (value.StorageProviderDefinition.Name, value);
  }

  #endregion
}
}
